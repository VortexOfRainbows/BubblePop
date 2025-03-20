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
    public override int GetPrice() => 10;
    public override void EquipUpdate()
    {
        p.AbilityRecoverySpeed += 0.6f;
        p.AttackSpeedModifier += 0.1f;
    }
}
