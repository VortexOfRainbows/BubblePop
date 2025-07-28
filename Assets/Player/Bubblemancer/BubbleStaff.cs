using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleStaff : BubblemancerWand
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ChargeShot10>();
    protected override string Name()
    {
        return "Bubble Staff";
    }
    protected override string Description()
    {
        return "Start with Coalescence";
    }
    public override void OnStartWith()
    {
        PowerUp.Spawn<Coalescence>(Player.Position, 0);
    }
}
