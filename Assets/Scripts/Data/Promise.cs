using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Promise<T>
{
    T _value;
    public bool fulfilled { get; protected set; } = false;
    public bool broken { get; protected set; } = false;
    public T val { get => _value; set { fulfilled = true; _value = value; } }

    public static implicit operator T(Promise<T> p) => p.val;
    public void Break()
    {
        broken = true;
    }
    public void Reset()
    {
        fulfilled = false;
        broken = false;
        _value = default(T);
    }

}
