using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCape : BubblemancerCape
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<StarbarbUnlock5>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bluebblemancy Cape").WithDescription("Reduces ability cooldown by 60% and increases attack speed by 10%");
    }
    public override void EquipUpdate()
    {
        player.AbilityRecoverySpeed += 0.6f;
        player.AttackSpeedModifier += 0.1f;
    }
}
