using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCape : BubblemancerCape
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<WaveUnlock10>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bluebblemancy Cape").WithDescription("Start with a random 3-star power");
    }
    public override void OnStartWith()
    {
        int i = Utils.RandInt(PowerUp.AvailablePowers.Count);
        for(int j = 0; j < 50; ++j)
        {
            if (PowerUp.Get(PowerUp.AvailablePowers[i]).GetRarity() == 3)
                break;
            i = Utils.RandInt(PowerUp.AvailablePowers.Count);
        }
        PowerUp.Spawn(PowerUp.AvailablePowers[i], Player.Position);  
    }
    public override int GetRarity()
    {
        return 3;
    }
}
