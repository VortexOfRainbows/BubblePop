using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueblemancerHat : BubblemancerHat
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<WaveUnlock10>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bluebblemancy Hat").WithDescription("Start with Choice");
    }
    public override void OnStartWith()
    {
        PowerUp.Spawn<Choice>(Player.Position, 0);
    }
    public override int GetRarity()
    {
        return 3;
    }
}
