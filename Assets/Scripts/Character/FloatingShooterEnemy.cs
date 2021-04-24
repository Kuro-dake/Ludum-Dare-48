using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingShooterEnemy : Enemy
{
    [SerializeField]
    int projectile_number = 1;
    [SerializeField]
    float projectile_delay = .3f;
    string projectile_type = "bullet";
    float spread = .3f;
    FloatRange angle_range = new FloatRange(-0.15f, 0.15f);
    FloatRange radius_range = new FloatRange(5f, 7f);
    FloatRange move_shoot_delay = new FloatRange(.3f, .6f);
    protected override IEnumerator Pursue()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(pursuit_delay);
            
            MoveTo(FindPositionOnCircle(GM.player.transform.position, radius_range, angle_range));
            while (SC.routines.IsRunning(char_movement_routine_name))
            {
                yield return null;
            }
            yield return new WaitForSeconds(move_shoot_delay);
            
            for(int i = 0;i<projectile_number; i++)
            {
                Carrier.Create(transform.position, AttackData.Create(attack, "hit", this), target.transform.position + (Random.insideUnitCircle * .3f).Vector3());
                yield return new WaitForSeconds(projectile_delay);
            }
        }
    }

    public static Vector3 FindPositionOnCircle(Vector3 center, float radius, float angle_modifier = 0f)
    {
        
        float ang = (angle_modifier) * 360;
        Vector3 pos = new Vector3(center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad),
                                   center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad),
                                   center.z);
        return pos;

    }
}
