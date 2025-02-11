using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartsUnlocked : UnlockCondition
{
    public override string LockedText()
    {
        return "Starts unlocked by default";
    }
    protected override bool IsUnlocked => true;
}

public class ScoreUnlock1000 : UnlockCondition
{
    public override string LockedText()
    {
        return "Reach a Bubble Best of 1000 to unlock\n" +
            $"Current best: {UIManager.highscore}";
    }
    protected override bool IsUnlocked => UIManager.highscore >= 1000;
}

public class ChargeShot10 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<ChargeShot>();
    public override string LockedText()
    {
        return $"Possess 10 {Power.Name()} in a single run\n" +
            $"Current best: {PowerUp.Get<ChargeShot>().PickedUpBestAllRuns}";
    }
    protected override bool IsUnlocked => Power.PickedUpBestAllRuns >= 10;
}

public class StarbarbUnlock5 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    public override string LockedText()
    {
        return $"Possess 5 {Power.Name()} in a single run or pick up 50 {Power.Name()} accross multiple runs to unlock\n" +
            $"Current best: {Power.PickedUpBestAllRuns} / {Power.PickedUpCountAllRuns}";
    }
    protected override bool IsUnlocked => Power.PickedUpCountAllRuns >= 50 || Power.PickedUpBestAllRuns >= 5;
}

public class PlayerDeathUnlock100 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    public override string LockedText()
    {
        return $"Die 100 times to unlock\n" +
            $"Current best: {PlayerData.PlayerDeaths}";
    }
    protected override bool IsUnlocked => PlayerData.PlayerDeaths >= 100;
}