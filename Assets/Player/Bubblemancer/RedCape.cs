using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCape : BubblemancerCape
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<StartsUnlocked>();
    protected override string Description()
    {
        return "Increases movement speed\nAdditionally increases movement speed each time you resurrect";
    }
    public override int GetPrice() => 15;
    public override void EquipUpdate()
    {
        player.MoveSpeedMod += 0.2f + 0.1f * player.SpentBonusLives;
    }
}
