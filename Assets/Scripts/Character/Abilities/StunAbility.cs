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
        for(int i = 0; i < 20; i++)
        {
            
            Carrier.Create(origin + Random.insideUnitCircle * .5f, AttackData.Create(0, "stun", GM.player), Camera.main.ScreenToWorldPoint(Input.mousePosition), "stun");
            SC.sounds.PlayResource("player_shoot", .3f, new FloatRange(.9f, 1.1f));
            yield return new WaitForSeconds(.05f);
        }
        
        
        
    }
}
