using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashAbility : Ability
{
    protected override IEnumerator CastStep()
    {
        Promise<Vector2> target = SC.ui.GetAbilityTarget();

        while (!target.fulfilled && !target.broken)
        {
            yield return null;
        }
        if (target.broken)
        {
            yield break;
        }
        Vector2 target_val = target.val;
        target_val.y = SC.env.ground_y;
        GM.player.transform.position = target_val;
    }
}
