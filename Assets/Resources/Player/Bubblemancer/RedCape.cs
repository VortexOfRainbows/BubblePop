using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCape : BubblemancerCape
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubbleBirbUnlock10>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Red Bubblemancy Cape").WithDescription($"Gain a {PowerUp.Get<Contract>().UnlockedName} everytime you resurrect");
    }
    public override int GetRarity()
    {
        return 4;
    }
}
