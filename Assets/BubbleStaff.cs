using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleStaff : BubblemancerWand
{
    public override UnlockCondition UnlockCondition => UnlockCondition.Get<ScoreUnlock5000>();
    protected override string Name()
    {
        return "Bubble Staff";
    }
    protected override string Description()
    {
        return "Start with 200 points, charge shot, bubble blast, and soapy soap";
    }
    public override void OnStartWith()
    {
        EventManager.Point += 200;
        PowerUp.Spawn<ChargeShot>(Player.Position, 0);
        PowerUp.Spawn<BubbleBlast>(Player.Position, 0);
        PowerUp.Spawn<SoapySoap>(Player.Position, 200);
    }
}
