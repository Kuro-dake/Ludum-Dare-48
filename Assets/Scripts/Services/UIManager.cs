using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[CreateAssetMenu(fileName = "UIManager", menuName = "Service/UIManager", order = 0)]

public class UIManager : Service
{
    [SerializeField]
    List<GameCursor> cursor_prefabs;
    [System.NonSerialized]
    List<GameCursor> cursors = new List<GameCursor>();


    public Canvas canvas => service_transform.GetComponent<Canvas>();

    public Image curtain_image => service_transform.Find("Curtain").GetComponent<Image>();
    float curtain_t = 1f;
    public bool curtain;
    public bool curtain_visible => curtain_t == 1f && curtain;
    public bool curtain_clear => curtain_t == 0f && !curtain;
    public void DevClearCurtain()
    {
        curtain_image.color = Color.clear;
        curtain = false;
        curtain_t = 0f;
    }
    Transform dialogue_transform => service_transform.Find("Dialogue");
    TextMeshProUGUI tmpro => dialogue_transform.Find("Text").GetComponent<TextMeshProUGUI>();
    string text_dialogue { get => tmpro.text; set => tmpro.text = value; }
    [SerializeField]
    float dialogue_t = 1f;
    [SerializeField]
    bool dialogue_on = false;
    public void ShowDialogueLine(string text)
    {
        StartCoroutine(ShowDialogueLineStep(text), "ui_dialogue");
    }
    [SerializeField]
    float dialogue_transition_speed = 2f;
    float dts_multiplier = 1f;
    IEnumerator ShowDialogueLineStep(string text)
    {
        if (dialogue_t < 1f) {
            dialogue_on = false;
            dts_multiplier = 2f;
            while (dialogue_t < 1f)
            {
                yield return null;
            }
        }
        dialogue_transform.GetComponent<UIBlockAnimations>().away_rotation = Random.insideUnitSphere * 90f;
        text_dialogue = text;
        dialogue_on = true;
        while (dialogue_t > 0f)
        {
            yield return null;
        }
        dts_multiplier = 1f;

    }

    public void RunDialogue(List<string> lines)
    {
        StartCoroutine(RunDialogueStep(lines));
    }


    public bool running_dialogue { get; protected set; } = false;
    IEnumerator RunDialogueStep(List<string> lines)
    {
        running_dialogue = true;
        Queue<string> dialogue_queue = new Queue<string>(lines);

        while (dialogue_queue.Count > 0)
        {
            ShowDialogueLine(dialogue_queue.Dequeue());
            yield return SC.controls.WaitForSpacebar();
            yield return null;
        }
        dialogue_on = false;
        running_dialogue = false;
    }

    public override void GameStartInitialize()
    {
        base.GameStartInitialize();
        DontDestroyOnLoad(service_transform);
        cursor_prefabs.ForEach(cp => cursors.Add(Instantiate(cp, service_transform)));
        curtain = false;

    }
    public override void Update()
    {
        base.Update();
        curtain_t = Mathf.MoveTowards(curtain_t, curtain ? 1f : 0f, Time.deltaTime);
        curtain_image.color = Color.Lerp(Color.clear, Color.black, curtain_t);
        dialogue_t = Mathf.MoveTowards(dialogue_t, dialogue_on ? 0f : 1f, Time.deltaTime * dialogue_transition_speed * dts_multiplier);
        dialogue_transform.GetComponent<UIBlockAnimations>().t = dialogue_t;
    }
    public GameCursor cursor { get; protected set; } = null;
    IEnumerator DelaySwitchCursor(cursor_type type) {
        yield return null;
        cursors.ForEach(c => c.gameObject.SetActive(false));
        
        switch (type)
        {

            case cursor_type.ground:
                cursor = cursors.Find(c => c is GroundLevelCursor);
                break;
            case cursor_type.star:
                cursor = cursors.Find(c => c is StarLevelCursor);
                break;
            case cursor_type.ability_target:
                cursor = cursors.Find(c => c is AbilityTargetCursor);
                break;
            case cursor_type.ui:
                cursor = cursors.Find(c => c is UICursor);
                break;
        }
        cursor?.gameObject.SetActive(true);
    }
    public void SwitchCursor(cursor_type type)
    {
        StartCoroutine(DelaySwitchCursor(type));
        
    }

    public Promise<Vector2> ability_target_promise { get; protected set; }

    public Promise<Vector2> GetAbilityTarget()
    {
        SwitchCursor(cursor_type.ability_target);
        return ability_target_promise = new Promise<Vector2>();
    }

    public enum cursor_type
    {
        ground,
        star,
        ability_target,
        ui
    }
    IEnumerator FIOCStep(System.Action callback)
    {
        SC.ui.curtain = true;
        while (!SC.ui.curtain_visible)
        {
            yield return null;
        }
        callback?.Invoke();

        SC.ui.curtain = false;
    }
    public const string ui_fioc_routine_name = "ui_fioc_routine";
    public void FadeInOutCallback(System.Action callback)
    {
        StartCoroutine(FIOCStep(callback), ui_fioc_routine_name);
    }
}

