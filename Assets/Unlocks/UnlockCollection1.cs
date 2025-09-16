public class StartsUnlocked : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Into the Bubblebath");
        description.WithDescription("Launch the game for the first time");
        //description.WithShortDescription("");
    }
    protected override bool TryUnlockCondition => true;
}
public class WaveUnlock10 : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("You birds don't scare me!");
        description.WithDescription("Complete wave 10");
        //description.WithShortDescription("");
    }
    protected override bool TryUnlockCondition => WaveDirector.HighScoreWaveNum > 10;
}
public class ChargeShot10 : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubblemancer: Kamehameha");
        description.WithDescription($"Possess 10 {Power.UnlockedName} in a single run");
    }
    public override PowerUp Power => PowerUp.Get<ChargeShot>();
    protected override bool TryUnlockCondition => Power.PickedUpBestAllRuns >= 10;
}
public class ShotSpeed10 : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubblemancer: Gas-Operated");
        description.WithDescription($"Possess 10 {Power.UnlockedName} in a single run");
    }
    public override PowerUp Power => PowerUp.Get<ShotSpeed>();
    protected override bool TryUnlockCondition => Power.PickedUpBestAllRuns >= 10;
}

public class StarbarbUnlock5 : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubblemancer: Superstar");
        description.WithDescription($"Possess 5 {Power.UnlockedName} in a single run");
    }
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    protected override bool TryUnlockCondition => Power.PickedUpBestAllRuns >= 5;
}
public class ChoiceUnlock200 : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Choice>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Thought Bubble: Decisions, Decisions");
        description.WithDescription($"Pick up 200 {Power.UnlockedName} across multiple runs");
    }
    protected override bool TryUnlockCondition => Power.PickedUpCountAllRuns >= 200;
}
public class PlayerDeathUnlock20 : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubblemancer: First Steps");
        description.WithDescription("Die 20 times");
    }
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    protected override bool TryUnlockCondition => PlayerData.PlayerDeaths >= 20;
}
public class BubbleBirbUnlock10 : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubblemancer: From the Dead");
        description.WithDescription($"Pick up 10 {Power.UnlockedName} across multiple runs");
    }
    public override PowerUp Power => PowerUp.Get<BubbleBirb>();
    protected override bool TryUnlockCondition => Power.PickedUpCountAllRuns >= 10;
}
public class ThoughtBubbleWave15NoAttack : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Thought Bubble: Calculated");
        description.WithDescription("Reach and complete wave 15 as Y:[Thought Bubble] without using your weapon");
    }
}
