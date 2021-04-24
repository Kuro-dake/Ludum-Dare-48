using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        

    }

}
