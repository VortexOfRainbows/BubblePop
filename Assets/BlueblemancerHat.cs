using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueblemancerHat : BubblemancerHat
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ScoreUnlock1000>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bluebblemancy Cape").WithDescription("Start with Choice");
    }
    public override void OnStartWith()
    {
        PowerUp.Spawn<Choice>(Player.Position, 0);
    }
}
