using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleStaff : BubblemancerWand
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ChargeShot10>();
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
        return "Start with Coalescence and Soapy Soap";
    }
    public override void OnStartWith()
    {
        PowerUp.Spawn<Coalescence>(Player.Position, 0);
        PowerUp.Spawn<SoapySoap>(Player.Position, 0);
    }
    public override int GetPrice() => 5;
}
