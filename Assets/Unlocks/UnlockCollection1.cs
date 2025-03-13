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
        return "Reach a Highscore of 1000\n" +
            $"Current best: {UIManager.highscore}";
    }
    protected override bool IsUnlocked => UIManager.highscore >= 1000;
}

public class ChargeShot10 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<ChargeShot>();
    public override string LockedText()
    {
        return $"Possess 10 {Power.UnlockedName()} in a single run\n" +
            $"Current best: {Power.PickedUpBestAllRuns}";
    }
    protected override bool IsUnlocked => Power.PickedUpBestAllRuns >= 10;
}

public class ShotSpeed10 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<ShotSpeed>();
    public override string LockedText()
    {
        return $"Possess 10 {Power.UnlockedName()} in a single run\n" +
            $"Current best: {Power.PickedUpBestAllRuns}";
    }
    protected override bool IsUnlocked => Power.PickedUpBestAllRuns >= 10;
}

public class StarbarbUnlock5 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    public override string LockedText()
    {
        return $"Possess 5 {Power.UnlockedName()} in a single run or pick up 50 {Power.UnlockedName()} across multiple runs\n" +
            $"Current best: {Power.PickedUpBestAllRuns} / {Power.PickedUpCountAllRuns}";
    }
    protected override bool IsUnlocked => Power.PickedUpCountAllRuns >= 50 || Power.PickedUpBestAllRuns >= 5;
}
public class ChoiceUnlock200 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Choice>();
    public override string LockedText()
    {
        return $"Pick up 200 {Power.UnlockedName()} across multiple runs\n" +
            $"Current best: {Power.PickedUpCountAllRuns}";
    }
    protected override bool IsUnlocked => Power.PickedUpCountAllRuns >= 200;
}
public class PlayerDeathUnlock100 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    public override string LockedText()
    {
        return $"Die 100 times\n" +
            $"Current best: {PlayerData.PlayerDeaths}";
    }
    protected override bool IsUnlocked => PlayerData.PlayerDeaths >= 100;
}