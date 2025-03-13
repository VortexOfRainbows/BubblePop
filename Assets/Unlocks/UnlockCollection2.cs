using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblemancerUnlock : UnlockCondition
{
    public override string LockedText()
    {
        return "Reach a Highscore of 5000\n" +
            $"Current best: {UIManager.highscore}";
    }
    protected override bool IsUnlocked => UIManager.highscore >= 5000;
}
public class ThoughtBubbleUnlock : UnlockCondition
{
    public override string LockedText()
    {
        return "Reach a Highscore of 3000\n" +
            $"Current best: {UIManager.highscore}";
    }
    protected override bool IsUnlocked => UIManager.highscore >= 3000;
}