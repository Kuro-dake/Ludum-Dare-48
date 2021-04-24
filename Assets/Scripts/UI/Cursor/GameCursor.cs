using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameCursor : MonoBehaviour
{

    Dictionary<KeyCode, System.Action> click_actions;
    Dictionary<KeyCode, System.Action> hold_actions;

    protected virtual void LeftClick()
    {

    }

    protected virtual void RightClick()
    {

    }

    protected virtual void LeftHold()
    {

    }

    protected virtual void RightHold()
    {

    }

    private void Start()
    {
       click_actions = new Dictionary<KeyCode, System.Action>()
        {
            {KeyCode.Mouse0, LeftClick },
            {KeyCode.Mouse1, RightClick }
        };
        hold_actions = new Dictionary<KeyCode, System.Action>()
        {
            {KeyCode.Mouse0, LeftHold },
            {KeyCode.Mouse1, RightHold }
        };
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        foreach(KeyValuePair<KeyCode,System.Action> kv in click_actions)
        {
            if (Input.GetKeyDown(kv.Key))
            {
                kv.Value();
            }
        }
        foreach (KeyValuePair<KeyCode, System.Action> kv in hold_actions)
        {
            if (Input.GetKey(kv.Key))
            {
                kv.Value();
            }
        }
    }
    protected Vector2 pos;
    Vector2 canvas_pos;
    private void LateUpdate()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(SC.ui.canvas.GetComponent<RectTransform>(), Input.mousePosition, SC.ui.canvas.worldCamera, out canvas_pos);

        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = SC.ui.service_transform.TransformPoint(canvas_pos);
    }
}
