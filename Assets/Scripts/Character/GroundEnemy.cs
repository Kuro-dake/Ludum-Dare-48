using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : Enemy
{
    
    protected override IEnumerator Pursue()
    {
        while (true)
        {
            while (airborne)
            {
                yield return null;
            }
            yield return new WaitForSeconds(pursuit_delay);
            MoveTo(Vector2.MoveTowards(GM.player.transform.position, transform.position, 1f));
            while (SC.routines.IsRunning(char_movement_routine_name))
            {
                yield return null;
            }
        }
    }

    



}
