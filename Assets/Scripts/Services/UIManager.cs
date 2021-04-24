using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    }
    public void SwitchCursor(cursor_type type)
    {
        cursors.ForEach(c => c.gameObject.SetActive(false));
        GameCursor cursor = null;
        switch (type)
        {
            
            case cursor_type.ground:
                cursor = cursors.Find(c => c is GroundLevelCursor);
                break;
            case cursor_type.star:
                cursor = cursors.Find(c => c is StarLevelCursor);
                break;
        }
        cursor?.gameObject.SetActive(true);
    }

    public enum cursor_type
    {
        ground,
        star
    }
}

