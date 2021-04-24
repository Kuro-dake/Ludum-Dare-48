using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackData
{
    public int damage;
    public string effect;

    public static AttackData Create(int damage, string effect)
    {
        return new AttackData(damage, effect);
    }

    AttackData(int d, string e)
    {
        damage = d;
        effect = e;
    }
}
