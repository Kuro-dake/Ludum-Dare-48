using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public virtual bool is_playing { get; set; }
    [SerializeField]
    public bool reusable;
    public virtual void Play(Vector2 position)
    {
        transform.position = position;
    }

    public virtual void Stop() { }
}
