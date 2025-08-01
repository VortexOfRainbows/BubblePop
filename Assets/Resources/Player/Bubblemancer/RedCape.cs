using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCape : BubblemancerCape
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubbleBirbUnlock10>();
    protected override string Description()
    {
        return "Increases movement speed by 20%\nAdditionally increases movement speed by 10% each time you resurrect";
    }
    public override void EquipUpdate()
    {
        player.MoveSpeedMod += 0.2f + 0.1f * player.SpentBonusLives;
    }
}
