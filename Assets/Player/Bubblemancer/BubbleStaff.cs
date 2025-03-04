using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleStaff : BubblemancerWand
{
    public override UnlockCondition UnlockCondition => UnlockCondition.Get<ChargeShot10>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        base.ModifyPowerPool(powerPool);
        powerPool.Add<SoapySoap>();
        powerPool.Add<Coalescence>();
    }
    protected override string Name()
    {
        return "Bubble Staff";
    }
    protected override string Description()
    {
        return "Start with 100 points, Coalescence, and Soapy Soap";
    }
    public override void OnStartWith()
    {
        EventManager.Point += 100;
        PowerUp.Spawn<Coalescence>(Player.Position, 0);
        PowerUp.Spawn<SoapySoap>(Player.Position, 100);
    }
}
