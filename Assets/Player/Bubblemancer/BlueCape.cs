using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCape : BubblemancerCape
{
    public override UnlockCondition UnlockCondition => UnlockCondition.Get<StarbarbUnlock5>();
    protected override string Description()
    {
        return "A fashionable robe styled with wool from Bubblemancer's elite blue sheep" +
            "\n" +
            "Decreases dash cooldown and dash distance";
    }
    public override void OnStartWith()
    {
        Player.Instance.DashMult = 0.5f;
    }
}
