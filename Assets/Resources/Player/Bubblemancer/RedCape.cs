using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCape : BubblemancerCape
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubbleBirbUnlock10>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Red Bubblemancy Cape").WithDescription("Increases movement speed by 20%");
    }
    public override void EquipUpdate()
    {
        player.TrueMoveModifier += 0.2f;// + 0.1f * player.SpentBonusLives;
    }
    public override int GetRarity()
    {
        return 4;
    }
}
