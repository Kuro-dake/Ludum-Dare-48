using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class LevelGM : GM
{
    protected override void Start()
    {
        base.Start();
        _player = Instantiate(Resources.Load<Player>("Player"));
        player.gameObject.SetActive(true);
        player.Initialize();
        SC.game.cine_cam.Follow = player.transform;

        SC.game.Test();
        SC.ui.SwitchCursor(UIManager.cursor_type.ground);
        SC.controls.active = true;

        UpdateCameraConfines();

        SC.env.RestartLevel();

    }

    static Bounds _cam_bounds;
    public static Bounds cam_bounds => _cam_bounds;
    
    const float cam_bounds_width_half = 40f;
    public static void UpdateCameraConfines(bool combat = false)
    {

        PolygonCollider2D pc2d = inst.GetComponent<PolygonCollider2D>();
        if (pc2d != null)
        {
            Destroy(pc2d);
        }
        Vector2 center = Camera.main.transform.position + Vector3.right * (combat ? 0f : cam_bounds_width_half * .5f);

        pc2d = inst.gameObject.AddComponent<PolygonCollider2D>();

        pc2d.points = new Vector2[] {

            center + new Vector2(cam_bounds_width_half, 20f),
            center + new Vector2(cam_bounds_width_half, SC.env.ground_y - 10f),

            center + new Vector2(-cam_bounds_width_half, SC.env.ground_y - 10f),
            center + new Vector2(-cam_bounds_width_half, 20f),



        };

        _cam_bounds = pc2d.bounds;

        CamScript.vcam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = pc2d;
    }


}
