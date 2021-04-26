using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAbility : Ability
{
    public float needle_delay = .15f;
    public int needle_number = 10;
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

        Vector2 target_vector = target;
        for (int i = 0; i < needle_number; i++)
        {
            Vector2 origin = GM.player.aim_transform.position;
            Carrier.Create(origin + Random.insideUnitCircle * .5f, AttackData.Create(3, "hit", GM.player), target_vector, "fireball");
            yield return new WaitForSeconds(needle_delay);
            target_vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
