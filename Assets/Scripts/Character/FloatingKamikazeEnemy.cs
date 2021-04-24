using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingKamikazeEnemy : Enemy
{


    FloatRange angle_range = new FloatRange(-0.15f, 0.15f);
    FloatRange radius_range = new FloatRange(5f, 7f);

    protected override IEnumerator Pursue()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(pursuit_delay);
            speed *= 4f;
            MoveTo(Vector2.MoveTowards(GM.player.transform.position, transform.position, 1f));

            while (SC.routines.IsRunning(char_movement_routine_name))
            {
                yield return null;
            }
            speed /= 4f;
            yield return new WaitForSeconds(.2f);
            MoveTo(FindPositionOnCircle(GM.player.transform.position, radius_range, angle_range));
            while (SC.routines.IsRunning(char_movement_routine_name))
            {
                yield return null;
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
