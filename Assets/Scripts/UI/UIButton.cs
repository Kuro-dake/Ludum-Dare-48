using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    AbilitiesBlock.UIButtonAction _action;
    public AbilitiesBlock.UIButtonAction action
    {
        get => _action;
        set
        {
            _action = value;
            GetComponentInChildren<TextMeshProUGUI>().text = action.shortcut.KeyShortcut();
        }
    }
    public void Initialize(AbilitiesBlock parent)
    {
        StartCoroutine(DelayedShortcutSwap(parent));
    }
    IEnumerator DelayedShortcutSwap(AbilitiesBlock parent)
    {
        yield return null;
        transform.Find("Shortcut").SetParent(parent.shortcuts_parent);
    }
    private void Update()
    {
        if (!SC.controls.active)
        {
            return;
        }
        if ((Input.GetKey(KeyCode.Mouse0) && over || Input.GetKey(action.shortcut)) && action.holdable || Input.GetKeyDown(action.shortcut))
        {
            Trigger();
        }
    }

    public void Trigger()
    {
        if (!SC.controls.active)
        {
            return;
        }
        action.action?.Invoke();
        action.ability?.Cast();
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
