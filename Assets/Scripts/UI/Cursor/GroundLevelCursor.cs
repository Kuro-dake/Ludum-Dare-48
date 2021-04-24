using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLevelCursor : GameCursor
{
    protected override void LeftClick()
    {
        base.LeftClick();
        

        Vector2 groundpos = pos;
        groundpos.y = GM.player.transform.position.y;

        GM.player.MoveTo(groundpos);

    }

    protected override void RightClick()
    {
        base.RightClick();
        SC.ui.SwitchCursor(UIManager.cursor_type.star);
    }
}
