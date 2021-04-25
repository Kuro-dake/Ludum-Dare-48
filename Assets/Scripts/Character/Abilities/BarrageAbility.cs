using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageAbility : Ability
{
    public float needle_delay = .15f;
    public int needle_number = 10;
    [SerializeField]
    Color color_1, color_2;
    protected override IEnumerator CastStep()
    {

        Explosion e = SC.effects["explosion"] as Explosion;
        e.color_1 = color_1;
        e.color_2 = color_2;
        
        e.speed = 10f;
        

        Promise<Vector2> target = SC.ui.GetAbilityTarget();

        while (!target.fulfilled && !target.broken)
        {
            yield return null;
        }
        if (target.broken)
        {
            yield break;
        }

        Vector2 origin = GM.player.aim_transform.position;

        e.Play(origin);
        e.speed = 5f;
        e.transform.localScale = Vector3.one * .8f;
        for (int i = 0; i < needle_number; i++)
        {
            Carrier.Create(origin + Random.insideUnitCircle * .5f, AttackData.Create(1, "hit_needle", GM.player), target, "laser");
            yield return new WaitForSeconds(needle_delay);
        }
        e.Stop();
    }
}
