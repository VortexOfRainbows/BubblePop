public class StartsUnlocked : UnlockCondition
{
    protected override bool TryUnlockCondition => true;
}
public class WaveUnlock10 : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
    }
    protected override bool TryUnlockCondition => WaveDirector.HighScoreWaveNum > 10;
    public override UnlockCondition PreReqUnlock => Get<BubblemancerUnlock>();
}
public class ChargeShot10 : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
    }
    public override PowerUp Power => PowerUp.Get<ChargeShot>();
    protected override bool TryUnlockCondition => Power.PickedUpBestAllRuns >= 10;
    public override UnlockCondition PreReqUnlock => Get<BubblemancerUnlock>();
}
public class ShotSpeed10 : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
    }
    public override PowerUp Power => PowerUp.Get<ShotSpeed>();
    protected override bool TryUnlockCondition => Power.PickedUpBestAllRuns >= 10;
    public override UnlockCondition PreReqUnlock => Get<BubblemancerUnlock>();
}

public class StarbarbUnlock5 : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
    }
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    protected override bool TryUnlockCondition => Power.PickedUpBestAllRuns >= 5;
    public override UnlockCondition PreReqUnlock => Get<BubblemancerUnlock>();
}
public class PlayerDeathUnlock10 : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
    }
    public override PowerUp Power => PowerUp.Get<Starbarbs>();
    protected override bool TryUnlockCondition => PlayerData.PlayerDeaths >= 10;
    public override UnlockCondition PreReqUnlock => Get<BubblemancerUnlock>();
}
public class BubbleBirbUnlock10 : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
    }
    public override PowerUp Power => PowerUp.Get<BubbleBirb>();
    protected override bool TryUnlockCondition => Power.PickedUpCountAllRuns >= 10;
    public override UnlockCondition PreReqUnlock => Get<BubblemancerUnlock>();
}
public class ThoughtBubbleWave15NoAttack : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Lab;
        category = Challenge;
    }
    public override UnlockCondition PreReqUnlock => Get<ThoughtBubbleUnlock>();
}
public class GachaponWave15AllSkullWaves : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Challenge;
    }
    public override UnlockCondition PreReqUnlock => Get<GachaponUnlock>();
}
public class GachaponBurger : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Burger>();
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Completionist;
    }
    public override UnlockCondition PreReqUnlock => Get<GachaponUnlock>();
}
public class GachaponBubblebirb : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<BubbleBirb>();
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Challenge;
    }
    public override UnlockCondition PreReqUnlock => Get<GachaponUnlock>();
}
public class BubblemancerPerfection : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Choice>();
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Challenge;
    }
    public override int Rarity => 4;
    public override UnlockCondition PreReqUnlock => Get<BubblemancerUnlock>();
}
public class GachaponAddicted : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Choice>();
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Completionist;
    }
    public override int Rarity => 4;
    public override UnlockCondition PreReqUnlock => Get<GachaponUnlock>();
}
public class ThoughtBubbleShortForCalc : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Calculator>();
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Lab;
        category = Challenge;
    }
    public override int Rarity => 4;
    public override UnlockCondition PreReqUnlock => Get<ThoughtBubbleUnlock>();
}
public class ThoughtBubbleDecisionsDecisions : UnlockCondition
{
    public override PowerUp Power => PowerUp.Get<Choice>();
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Lab;
        category = Completionist;
    }
    public override int Rarity => 2;
    public override UnlockCondition PreReqUnlock => Get<ThoughtBubbleUnlock>();
}
public class GachaponClover : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Completionist;
    }
    public override int Rarity => 5;
    public override UnlockCondition PreReqUnlock => Get<GachaponUnlock>();
}
public class GachaponHealer : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Completionist;
    }
    public override int Rarity => 4;
    public override UnlockCondition PreReqUnlock => Get<GachaponUnlock>();
}
public class ThoughtBubbleArsenal : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Lab;
        category = Completionist;
    }
    public override int Rarity => 2;
    public override UnlockCondition PreReqUnlock => Get<ThoughtBubbleUnlock>();
}
public class ThoughtBubbleIndistinguishable : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Lab;
        category = Completionist;
    }
    public override int Rarity => 2;
    public override UnlockCondition PreReqUnlock => Get<ThoughtBubbleUnlock>();
}
public class FizzyThinkImJustGonnaStandThereAndTakeIt : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Completionist;
    }
    public override int Rarity => 4;
    public override UnlockCondition PreReqUnlock => Get<FizzyUnlock>();
}
public class GachaponBlackjack : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Challenge;
    }
    public override int Rarity => 4;
    public override UnlockCondition PreReqUnlock => Get<GachaponUnlock>();
}
public class FizzyCoolGuys : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Challenge;
    }
    public override int Rarity => 3;
    public override UnlockCondition PreReqUnlock => Get<FizzyUnlock>();
}
public class SlowThingsDownALittle : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Lab;
        category = Secret;
    }
    public override int Rarity => 4;
}
public class ThoughtBubbleCatchThis: UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Lab;
        category = Challenge;
    }
    public override int Rarity => 3;
    public override UnlockCondition PreReqUnlock => Get<ThoughtBubbleUnlock>();
}
public class BubblemancerCatalyst : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = Meadows;
        category = Completionist;
    }
    public override int Rarity => 5;
}
public class FizzyFocus : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Completionist;
    }
    public override int Rarity => 3;
    public override UnlockCondition PreReqUnlock => Get<FizzyUnlock>();
}
public class FizzyFakeDoctor : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Completionist;
    }
    public override int Rarity => 2;
    public override UnlockCondition PreReqUnlock => Get<FizzyUnlock>();
}
public class FizzyStuffed : UnlockCondition
{
    public override void SetAchievementCategories(ref int zone, ref int category)
    {
        zone = City;
        category = Challenge;
    }
    public override int Rarity => 4;
    public override UnlockCondition PreReqUnlock => Get<FizzyUnlock>();
}
//Achievement idea for (2-star) Fizzy: Speedrun: As Fizzy, win in under 20 minutes (does not count paused time)
//Achievement idea for (3-star) Fizzy: Touch Grass: As Fizzy, win while holding Rainbow Flower, Pity Charm, and Lucky Star
//Achievement idea for (5-star) Fizzy: Third Day: As Fizzy, reach the third loop on Ascension 3 without dying, then resurrect
//Achievement idea for (2-star) Oil King: Rock Feller: as Oil King, kill all segments of a Rock Golem with a single drone strike
//Achievement idea for (3-star) Oil King: Quagmire: afflict an enemy with tar, chill, and poison, then detonate them for over 1000 total damage from debuffs
//Achievement idea for (4-star) Oil King: Environmentalist: win without afflicting tar on any enemy
//Achievement idea for (5-star) Oil King: Golden Slaughterer: win on Ascension 3 without taking damage, picking up red items, or taking non-investments from choices
