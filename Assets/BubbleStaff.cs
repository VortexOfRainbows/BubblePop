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
        return "Start with 100 points, a charge shot power up, and a bubble propulsion power up";
    }
    public override void OnStartWith()
    {
        EventManager.Point += 100;
        PowerUp.Spawn<ChargeShot>(Player.Position, 100);
        PowerUp.Spawn<ShotSpeed>(Player.Position, 0);
    }
}
