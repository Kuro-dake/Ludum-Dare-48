using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AbilitiesBlock : MonoBehaviour
{

    [SerializeField]
    UIButton ability_button_prefab;

    [SerializeField]
    List<UIButtonAction> actions = new List<UIButtonAction>();

    [System.Serializable]
    public class UIButtonAction
    {
        public KeyCode shortcut;
        public UnityEngine.Events.UnityEvent action;
        public Sprite icon;
        public string text;
        public bool holdable = false;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        foreach(UIButtonAction a in actions)
        {
            UIButton ngo = Instantiate(ability_button_prefab, transform);
            ngo.GetComponent<Button>().onClick.AddListener(a.action.Invoke);
            ngo.action = a;
        }
    }

}
