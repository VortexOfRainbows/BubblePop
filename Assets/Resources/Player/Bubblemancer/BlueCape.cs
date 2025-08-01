using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCape : BubblemancerCape
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<StarbarbUnlock5>();
    protected override string Description()
    {
        return "A fashionable robe styled with wool from Bubblemancer's elite blue sheep" +
            "\n" +
            "Greatly reduces ability cooldown and slightly increases attack speed";
    }
    public override void EquipUpdate()
    {
        player.AbilityRecoverySpeed += 0.6f;
        player.AttackSpeedModifier += 0.1f;
    }
}
