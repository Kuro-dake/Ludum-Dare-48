using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Controls", menuName = "Service/Controls", order = 0)]

public class Controls : Service
{
    Dictionary<KeyCode, Vector2> directions = new Dictionary<KeyCode, Vector2>()
    {
        {KeyCode.W, Vector2.up },
        {KeyCode.A, Vector2.left },
        {KeyCode.S, Vector2.down },
        {KeyCode.D, Vector2.right }
    };
    public override void Update()
    {
        base.Update();
        foreach(KeyValuePair<KeyCode, Vector2> kv in directions)
        {
            if (Input.GetKey(kv.Key))
            {
                //GM.player.Movement(kv.Value);
            }
        }
        
    }
}
