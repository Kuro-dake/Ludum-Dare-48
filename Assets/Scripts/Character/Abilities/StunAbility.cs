using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunAbility : Ability
{
    
    protected override IEnumerator CastStep()
    {
        Vector2 origin = GM.player.aim_transform.position;

        Promise<Vector2> target = SC.ui.GetAbilityTarget();

        while (!target.fulfilled && !target.broken)
        {
            yield return null;
        }
        if (target.broken)
        {
            yield break;
        }
        Carrier.Create(origin + Random.insideUnitCircle * .5f, AttackData.Create(0, "stun", GM.player), target, "stun");
        
        
    }
}
