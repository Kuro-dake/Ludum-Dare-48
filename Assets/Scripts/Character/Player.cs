using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public void Movement(Vector2 dir)
    {
        transform.position += dir.Vector3() * Time.deltaTime * speed;
    }
}
