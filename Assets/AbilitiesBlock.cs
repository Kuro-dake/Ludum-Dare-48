using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class AbilitiesBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool over;
    public void OnPointerEnter(PointerEventData data)
    {
        SC.ui.SwitchCursor(UIManager.cursor_type.ui);
        over = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if(SC.ui.cursor is UICursor)
        {
            SC.ui.SwitchCursor(UIManager.cursor_type.ground);
        }
        over = false;
    }

    [SerializeField]
    UIButton ability_button_prefab;

    [SerializeField]
    List<UIButtonAction> actions = new List<UIButtonAction>();
    public Transform buttons_parent => transform.Find("Buttons");
    public Transform shortcuts_parent => transform.Find("Shortcuts");
    [System.Serializable]
    public class UIButtonAction
    {
        public KeyCode shortcut;
        public UnityEngine.Events.UnityEvent action;
        public Sprite icon;
        public string text;
        public bool holdable = false;
        public Ability ability;
    }
    static AbilitiesBlock inst;
    private void Start()
    {
        inst = this;
        Initialize();
    }

    public void Initialize()
    {
        foreach(UIButtonAction a in actions)
        {
            UIButton ngo = Instantiate(ability_button_prefab, buttons_parent);
            ngo.GetComponent<Button>().onClick.AddListener(ngo.Trigger);
            
            ngo.action = a;
            ngo.Initialize(this);
        }
    }

}
