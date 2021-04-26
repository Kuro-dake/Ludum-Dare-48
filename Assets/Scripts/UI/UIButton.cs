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
            transform.Find("icon").GetComponent<Image>().sprite = action.icon;
        }
    }
    public void Initialize(AbilitiesBlock parent)
    {
        StartCoroutine(DelayedShortcutSwap(parent));
        DisplayCooldown();
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
        if (/*(Input.GetKey(KeyCode.Mouse0) && over ||*/ Input.GetKey(action.shortcut)/*)*/ && action.holdable || Input.GetKeyDown(action.shortcut))
        {
            Trigger();
        }
        if(action.ability != null)
        {
            DisplayCooldown();
        }
    }
    Image screen => transform.Find("Screen").GetComponent<Image>();
    TextMeshProUGUI cooldown => transform.Find("Cooldown").GetComponent<TextMeshProUGUI>();
    void DisplayCooldown()
    {
        

        if(action.ability == null)
        {
            screen.gameObject.SetActive(false);
            cooldown.gameObject.SetActive(false);
            return;
        }

        if (!action.ability.active)
        {
            screen.gameObject.SetActive(true);
            cooldown.gameObject.SetActive(false);
            return;
        }

        if (action.ability.current_cooldown <= 0f)
        {
            screen.gameObject.SetActive(false);
            cooldown.gameObject.SetActive(false);
            return;
        }



        screen.gameObject.SetActive(true);
        cooldown.gameObject.SetActive(true);
        float cc = action.ability.current_cooldown;
        string ctext = Mathf.CeilToInt(cc).ToString();
        if (cc < 1f)
        {
            cc *= 10;
            ctext = "0." + Mathf.CeilToInt(cc).ToString();
        }
        cooldown.text = ctext;

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
            //GetComponent<Image>().color = value ? Color.blue : Color.black;
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
