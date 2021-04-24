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
        GM.player.transform.position = target.val;
    }
}
