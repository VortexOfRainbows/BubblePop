using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleStaff : BubblemancerWand
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ChargeShot10>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        base.ModifyPowerPool(powerPool);
        powerPool.Add<Coalescence>();
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubble Staff").WithDescription("An expertly crafted bubble-tech weapon");
    }
}
