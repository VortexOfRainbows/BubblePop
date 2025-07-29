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
    protected override string Name()
    {
        return "Bubble Staff";
    }
    protected override string Description()
    {
        return "An expertly crafted bubble-tech weapon";
    }
}
