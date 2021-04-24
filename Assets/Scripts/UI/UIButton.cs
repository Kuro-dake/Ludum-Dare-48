using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    AbilitiesBlock.UIButtonAction _action;
    public AbilitiesBlock.UIButtonAction action
    {
        get => _action;
        set
        {
            _action = value;
            GetComponentInChildren<Text>().text = action.shortcut.ToString();
        }
    }

    private void Update()
    {
        if(action.shortcut == KeyCode.D)
        {
            Debug.Log("mouse 0 " + Input.GetKey(KeyCode.Mouse0));
            Debug.Log("over " + over);
            Debug.Log("holdable " + action.holdable);
        }
        if ((Input.GetKey(KeyCode.Mouse0) && over || Input.GetKey(action.shortcut)) && action.holdable || Input.GetKeyDown(action.shortcut))
        {
            action.action?.Invoke();
        }
    }
    bool _over = false;
    bool over {
        get => _over;
        set
        {
            _over = value;
            GetComponent<Image>().color = value ? Color.blue : Color.black;
        }
    }
    public void OnPointerEnter(PointerEventData data)
    {
        over = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        over = false;
    }
}
