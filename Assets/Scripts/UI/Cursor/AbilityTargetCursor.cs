using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTargetCursor : GameCursor
{
    protected override void LeftClick()
    {
        base.LeftClick();
        SC.ui.ability_target_promise.val = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SC.ui.SwitchCursor(UIManager.cursor_type.ground);
    }

    protected override void RightClick()
    {
        base.RightClick();
        SC.ui.ability_target_promise.Break();
        SC.ui.SwitchCursor(UIManager.cursor_type.ground);
    }
}
