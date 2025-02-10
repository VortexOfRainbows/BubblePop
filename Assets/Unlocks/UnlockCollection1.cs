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

public class ScoreUnlock1000 : UnlockCondition
{
    public override string LockedText()
    {
        return "Reach a Bubble Best of 1000 to unlock\n" +
            $"Current best: {UIManager.highscore}";
    }
    public override bool IsUnlocked => UIManager.highscore >= 1000;
}

public class ScoreUnlock3000 : UnlockCondition
{
    public override string LockedText()
    {
        return "Reach a Bubble Best of 3000 to unlock\n" +
            $"Current best: {UIManager.highscore}";
    }
    public override bool IsUnlocked => UIManager.highscore >= 3000;
}

public class StarbarbUnlock5 : UnlockCondition
{
    public static int StarbarbBestCount = 0;
    public override void SaveData()
    {
        SaveInt("StarbarbBest", StarbarbBestCount);
    }
    public override void LoadData()
    {
        StarbarbBestCount = LoadInt("StarbarbBest");
    }
    public override string LockedText()
    {
        return $"Possess 5 starbarb power ups in a single run to unlock\n" +
            $"Current best: {StarbarbBestCount}";
    }
    public override bool IsUnlocked => StarbarbBestCount >= 5;
}