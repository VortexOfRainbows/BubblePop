using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartsUnlocked : UnlockCondition
{
    public override string LockedText()
    {
        return "Starts unlocked by default";
    }
    public override bool IsUnlocked => true;
}

public class ScoreUnlock2000 : UnlockCondition
{
    public override string LockedText()
    {
        return "Reach a Bubble Best of 2000 to unlock";
    }
    public override bool IsUnlocked => UIManager.highscore >= 2000;
}

public class ScoreUnlock5000 : UnlockCondition
{
    public override string LockedText()
    {
        return "Reach a Bubble Best of 5000 to unlock";
    }
    public override bool IsUnlocked => UIManager.highscore >= 5000;
}