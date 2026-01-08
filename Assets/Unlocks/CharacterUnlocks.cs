using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblemancerUnlock : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Unlikely Hero");
        description.WithDescription("Starts unlocked");
    }
    protected override bool TryUnlockCondition => true;
}
public class ThoughtBubbleUnlock : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gatekeeper");
        description.WithDescription("Reach and complete wave 20");
    }
    protected override bool TryUnlockCondition => WaveDirector.HighScoreWaveNum > 20;
}
public class GachaponUnlock : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("In For A Penny");
        description.WithDescription("Spend Y:[5000 coins] across multiple runs");
    }
}