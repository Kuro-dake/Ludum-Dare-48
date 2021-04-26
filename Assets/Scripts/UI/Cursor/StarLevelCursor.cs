using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLevelCursor : GameCursor
{
    protected override void LeftClick()
    {
        base.LeftClick();
        //GM.player.MoveTo(pos);
    }

    protected override void RightClick()
    {
        base.RightClick();
        //SC.ui.SwitchCursor(UIManager.cursor_type.ground);
    }
}
