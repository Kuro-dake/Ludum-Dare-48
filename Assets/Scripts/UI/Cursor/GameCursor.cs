using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameCursor : MonoBehaviour
{

    Dictionary<KeyCode, System.Action> actions;

    protected virtual void LeftClick()
    {
        Debug.Log("left");
    }

    protected virtual void RightClick()
    {
        Debug.Log("right");
    }

    private void Start()
    {
       actions = new Dictionary<KeyCode, System.Action>()
        {
            {KeyCode.Mouse0, LeftClick },
            {KeyCode.Mouse1, RightClick }
        };
    }

    // Update is called once per frame
    void Update()
    {
        foreach(KeyValuePair<KeyCode,System.Action> kv in actions)
        {
            if (Input.GetKeyDown(kv.Key))
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
