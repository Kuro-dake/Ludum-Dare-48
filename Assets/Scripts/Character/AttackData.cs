using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackData
{
    public int damage;
    public string effect;
    public Character origin;
    public Vector3 impact_point;
    public static AttackData Create(int damage, string effect, Character origin, Vector3 impact = default)
    {
        return new AttackData(damage, effect, origin, impact);
    }

    AttackData(int d, string e, Character o, Vector3 i)
    {
        damage = d;
        effect = e;
        origin = o;
        impact_point = i;
    }
}
