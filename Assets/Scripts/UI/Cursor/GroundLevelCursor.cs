using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLevelCursor : GameCursor
{

    float attack_delay = 0f;

    protected override void LeftHold()
    {
        base.LeftHold();
        if (!SC.controls.active)
        {
            return;
        }
        if(attack_delay <= 0f)
        {
            Vector3 mousePosition = Input.mousePosition;

            mousePosition.z = mousePosition.z - (Camera.main.transform.position.z - 45);

            Carrier.Create(GM.player.aim_transform.position, AttackData.Create(1, "hit", GM.player), Camera.main.ScreenToWorldPoint(mousePosition), "exploder");
            attack_delay = SC.player_stats.attack_delay;
        }
        

    }

    protected override void RightClick()
    {
        base.RightClick();


        /*Vector2 groundpos = pos;
        groundpos.y = GM.player.transform.position.y;

        GM.player.MoveTo(groundpos);*/


    }

    protected override void Update()
    {
        base.Update();
        attack_delay -= Time.deltaTime;
    }
}
