using System;

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
        description.WithDescription("Reach and complete wave 10");
        //description.WithShortDescription("");
    }
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
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
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
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
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
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
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
    }
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    protected override bool TryUnlockCondition => Power.PickedUpBestAllRuns >= 5;
}
public class PlayerDeathUnlock10 : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubblemancer: First Steps");
        description.WithDescription("Die 10 times");
    }
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
    }
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    protected override bool TryUnlockCondition => PlayerData.PlayerDeaths >= 10;
}
public class BubbleBirbUnlock10 : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubblemancer: From the Dead");
        description.WithDescription($"Pick up 10 {Power.UnlockedName} across multiple runs");
    }
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
    }
    public override PowerUp Power => PowerUp.Get<BubbleBirb>();
    protected override bool TryUnlockCondition => Power.PickedUpCountAllRuns >= 10;
}
public class ThoughtBubbleWave15NoAttack : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Thought Bubble: Calculated");
        description.WithDescription("As Y:[Thought Bubble,] reach and complete wave 15 without using your weapon");
    }
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Lab;
        category = Challenge;
    }
}
public class GachaponWave15AllSkullWaves : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gachapon: Triple Down");
        description.WithDescription("Reach and complete wave 15 as Y:[Gachapon] while choosing only max-difficulty Y:[Wave Cards]");
    }
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Challenge;
    }
}
public class GachaponBurger : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gachapon: Yummy!");
        description.WithDescription($"Possess 3 {Power.UnlockedName} in a single run");
    }
    public override PowerUp Power => PowerUp.Get<Burger>();
    protected override bool TryUnlockCondition => Power.PickedUpBestAllRuns >= 3;
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Completionist;
    }
}
public class GachaponBubblebirb : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gachapon: Kamikaze");
        description.WithDescription($"As Y:Gachapon, kill {EnemyID.OldLeonard.GetComponent<Enemy>().Name()} with {Power.UnlockedName} resurrection flames");
    }
    public override PowerUp Power => PowerUp.Get<BubbleBirb>();
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Challenge;
    }
}
public class BubblemancerPerfection : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Choice>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubblemancer: Bath Bomb");
        description.WithDescription($"As Y:Bubblemancer, reach and complete wave 15 without taking damage");
    }
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Challenge;
    }
    public override int Rarity => 4;
}
public class GachaponAddicted : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Choice>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gachapon: Addicted");
        description.WithDescription($"As Y:Gachapon, reach and complete wave 40");
    }
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Completionist;
    }
    public override int Rarity => 4;
}
public class ThoughtBubbleShortForCalc : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Calculator>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Thought Bubble: Short for Calc");
        description.WithDescription($"Assimilate 5 or more {PowerUp.Get<BubbleMitosis>().UnlockedName} into a single {Power.UnlockedName}");
    }
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Lab;
        category = Challenge;
    }
    public override int Rarity => 4;
}
public class ThoughtBubbleDecisionsDecisions : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Choice>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Thought Bubble: Decisions Decisions");
        description.WithDescription($"Reroll a single {Power.UnlockedName} 10 times");
    }
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Lab;
        category = Completionist;
    }
    public override int Rarity => 2;
}
public class GachaponClover : UnlockCondition
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gachapon: Clover");
        description.WithDescription($"Using Y:[Gacha Slots,] roll Y:Jackpot 3 times in a row");
    }
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Completionist;
    }
    public override int Rarity => 5;
}