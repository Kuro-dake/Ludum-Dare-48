using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackData
{
    public int damage;
    public string effect;
    public Character origin;
    public static AttackData Create(int damage, string effect, Character origin)
    {
        return new AttackData(damage, effect, origin);
    }

    AttackData(int d, string e, Character o)
    {
        damage = d;
        effect = e;
        origin = o;
    }
}
