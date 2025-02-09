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
        return "Start with 150 points and 2 soapy soap power ups";
    }
    public override void OnStartWith()
    {
        EventManager.Point += 150;
        PowerUp.Spawn<SoapySoap>(Player.Position, 150);
        PowerUp.Spawn<SoapySoap>(Player.Position, 0);
    }
}
