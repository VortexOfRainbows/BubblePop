using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblemancerUnlock : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubblemancer: Awakening");
        description.WithDescription("Starts unlocked");
    }
    protected override bool TryUnlockCondition => true;
}
public class ThoughtBubbleUnlock : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Thought Bubble: Awakening");
        description.WithDescription("Starts unlocked G:(for now)");
    }
    protected override bool TryUnlockCondition => true; // UIManager.highscore >= 3000;
}
public class GachaponUnlock : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gachapon: Awakening");
        description.WithDescription("Starts unlocked G:(for now)");
    }
    protected override bool TryUnlockCondition => true; // purchase 10 items from shops
}