using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblemancerUnlock : UnlockCondition
{
    public override string LockedText()
    {
        return "Unlocked by default!";
    }
    protected override bool TryUnlockCondition => true;
}
public class ThoughtBubbleUnlock : UnlockCondition
{
    public override string LockedText()
    {
        return "Reach a Highscore of 3000\n";
    }
    protected override bool TryUnlockCondition => true; // UIManager.highscore >= 3000;
}
public class GachaponUnlock : UnlockCondition
{
    public override string LockedText()
    {
        return "Reach a total coin count of $500\n";
    }
    protected override bool TryUnlockCondition => true; // purchase 10 items from shops
}