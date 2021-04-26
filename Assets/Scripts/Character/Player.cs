using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    public Transform aim_transform => transform.Find("aim");
    public Transform aim_center => transform.Find("aim_center");

    [SerializeField]
    float aim_distance;

    PlayParticleSystemEffect _shield;
    PlayParticleSystemEffect shield_ppse
    {
        get
        {
            if(_shield == null)
            {
                _shield = SC.effects["shield"] as PlayParticleSystemEffect;
                _shield.GetComponent<FollowTransform>().to_follow = aim_center;
            }
            return _shield;
        }
    }

    protected override void Update()
    {
        base.Update();
        aim_transform.position = aim_center.position.Vector2() + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - aim_center.position).Vector2().normalized * aim_distance; //Vector3.MoveTowards(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), 2f);
        orientation_left = aim_transform.position.x < aim_center.position.x;
        if (!SC.env.active)
        {
            return;
        }
        if(shield > 0 && !shield_ppse.ps.isPlaying)
        {
            shield_ppse.ps.Play();
        }
        if(shield <= 0 && shield_ppse.ps.isPlaying)
        {
            shield_ppse.Stop();
        }
    }

    public void Movement(Vector2 dir)
    {
        Vector3 npos = transform.position + dir.Vector3() * Time.deltaTime * speed;
        npos.x = Mathf.Clamp(npos.x, LevelGM.cam_bounds.min.x + 1f, LevelGM.cam_bounds.max.x - 1f);
        transform.position = npos;
        walking = true;
        
        
    }

    protected override void Die()
    {
        SC.controls.active = false;
        SC.env.StopProgenv();
        SC.game.LoadScene("Level");
    }
}
