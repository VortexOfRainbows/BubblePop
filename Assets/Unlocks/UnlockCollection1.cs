public class StartsUnlocked : UnlockCondition
{
    public override string LockedText()
    {
        return "Starts unlocked by default";
    }
    protected override bool TryUnlockCondition => true;
}
public class WaveUnlock10 : UnlockCondition
{
    public override string LockedText()
    {
        return "Complete wave 10\n" +
            $"Current best: {WaveDirector.HighScoreWaveNum}";
    }
    protected override bool TryUnlockCondition => WaveDirector.HighScoreWaveNum > 10;
}
public class ChargeShot10 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<ChargeShot>();
    public override string LockedText()
    {
        return $"Possess 10 {Power.UnlockedName} in a single run\n" +
            $"Current best: {Power.PickedUpBestAllRuns}";
    }
    protected override bool TryUnlockCondition => Power.PickedUpBestAllRuns >= 10;
}

public class ShotSpeed10 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<ShotSpeed>();
    public override string LockedText()
    {
        return $"Possess 10 {Power.UnlockedName} in a single run\n" +
            $"Current best: {Power.PickedUpBestAllRuns}";
    }
    protected override bool TryUnlockCondition => Power.PickedUpBestAllRuns >= 10;
}

public class StarbarbUnlock5 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    public override string LockedText()
    {
        return $"Possess 5 {Power.UnlockedName} in a single run or pick up 50 {Power.UnlockedName} across multiple runs\n" +
            $"Current best: {Power.PickedUpBestAllRuns} / {Power.PickedUpCountAllRuns}";
    }
    protected override bool TryUnlockCondition => Power.PickedUpCountAllRuns >= 50 || Power.PickedUpBestAllRuns >= 5;
}
public class ChoiceUnlock200 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Choice>();
    public override string LockedText()
    {
        return $"Pick up 200 {Power.UnlockedName} across multiple runs\n" +
            $"Current best: {Power.PickedUpCountAllRuns}";
    }
    protected override bool TryUnlockCondition => Power.PickedUpCountAllRuns >= 200;
}
public class PlayerDeathUnlock20 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    public override string LockedText()
    {
        return $"Die 20 times\n" +
            $"Current best: {PlayerData.PlayerDeaths}";
    }
    protected override bool TryUnlockCondition => PlayerData.PlayerDeaths >= 20;
}
public class BubbleBirbUnlock10 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<BubbleBirb>();
    public override string LockedText()
    {
        return $"Pick up 10 {Power.UnlockedName} across multiple runs";
    }
    protected override bool TryUnlockCondition => Power.PickedUpCountAllRuns >= 10;
}
public class ThoughtBubbleWave15NoAttack : UnlockCondition
{
    public override string LockedText()
    {
        return $"Reach and complete wave 15 as Thought Bubble without using your weapon";
    }
}
