using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashAbility : Ability
{
    [SerializeField]
    Color color_1, color_2;
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

        Explosion e = SC.effects["explosion"] as Explosion;
        e.color_1 = color_1;
        e.color_2 = color_2;
        e.Play(GM.player.aim_center.position);
        e.lifetime = .1f;
        e.transform.localScale = Vector3.one * 1.5f;
        e.speed = 3f;

        GM.player.transform.position = target_val;

        e = SC.effects["explosion"] as Explosion;
        e.color_1 = color_1;
        e.color_2 = color_2;
        e.Play(GM.player.aim_center.position);
        e.lifetime = .1f;
        e.speed = 3f;
        e.transform.localScale = Vector3.one * 1.3f;
    }

}
