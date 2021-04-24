using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAbility : Ability
{
    public int shield_value = 10;
    protected override IEnumerator CastStep()
    {
        GM.player.GainShield(shield_value);
        yield return null;
    }
}
