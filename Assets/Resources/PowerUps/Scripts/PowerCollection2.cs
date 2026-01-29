using UnityEngine;
public class BubbleMitosis : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Mitosis");
        description.WithDescription("Y:[Upon obtaining your next power,] gain Y:[additional stacks] equal to the amount of <color=#BAE3FE>Mitosis</color> stacks G:(consumed on use)");
        description.WithShortDescription("Assimilated into next power you obtain");
    }
    public override void HeldEffect(Player p)
    {
        if (p.MostRecentPower != null && p.MostRecentPower.Type != Type)
        {
            p.MostRecentPower.PickUp(Stack);
            if(Stack >= 5 && p.MostRecentPower is Calculator)
            {
                UnlockCondition.Get<ThoughtBubbleShortForCalc>().SetComplete();
            }
            p.RemovePower(Type, Stack);
        }
    }
}
public class RollForCharisma : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Roll For Charisma");
        description.WithDescription("Y:20% G:(+1% per stack) Y:chance to Y:heal after Y:[purchasing a power] or gain Y:25 G:(+25 per stack) Y:coins if uninjured");
        description.WithShortDescription("Chance to heal or refund a portion of money spent when purchasing power");
    } 
    public override void HeldEffect(Player p)
    {
        p.RollChar += Stack;
    }
    public override Sprite GetTexture()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/RollForCharisma");
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/CharismaSunglasses");
    }
}
public class RollForDexterity : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Y:20% G:(+1% per stack) Y:chance to gain Y:15% increased movement speed after using Y:[your ability,] stacking up to Y:2 G:(+1 per stack) times and lasting Y:[5 seconds]");
        description.WithShortDescription("Chance to gain increased speed after using your ability");
    }
    public override void HeldEffect(Player p)
    {
        p.RollDex += Stack;
    }
    public override Sprite GetTexture()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/RollForDexterity");
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/DexBoot");
    }
}
public class RollForInitiative : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Y:20% G:(+1% per stack) Y:chance to deal Y:200% G:(+20% per stack) Y:[bonus damage] on Y:[first strike]");
        description.WithShortDescription("Chance for first hit to deal additional damage");
    }
    public override void HeldEffect(Player p)
    {
        p.RollInit += Stack;
    }
    public override Sprite GetTexture()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/RollForInitiative");
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/InitiativeAddOn");
    }
}
public class RollForPerception : PowerUp
{
    public override void Init()
    {
        Weighting = Legendary;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases weights of Y:[rare powers] in the Y:[power pool] by Y:20% G:(+20% per stack)");
        description.WithShortDescription("Chance of seeing rare powers increased");
        description.WithDescriptionVariant<SlotMachineWeapon>("Increases weights of Y:[rare powers] in the Y:[power pool] by Y:20% G:(+20% per stack) \nIncreases the likelihood of Y:[high-rarity spins] by Y:20% G:(+20% per stack)");
        description.WithShortDescriptionVariant<SlotMachineWeapon>("Chance of seeing rare powers and rare spins increased");
    }
    public override void HeldEffect(Player p)
    {
        p.RollPerc += Stack;
    }
    public override Sprite GetTexture()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/RollForPerception");
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/InsightAddOn");
    }
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<GachaponClover>();
}
public class BubbleShield : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Gain Y:[1 shield slot] and Y:[recover 1 shield] on pickup or at the start of Y:[even-numbered waves] " +
            "\nWhen a Y:[shield is broken,] extend Y:[immunity frames] by Y:40% G:(+20% per stack) and release Y:24 G:(+8 per stack) bubbles");
        description.WithShortDescription("Gain a shield on even-numbered waves\nWhen shields are broken, extend immunity frames and release bubbles");
    }
    public override void OnPickup(int count)
    {
        Player.Instance.SetShield(Player.Instance.GetShield() + count);
    }
    public override void HeldEffect(Player p)
    {
        p.BubbleShields += Stack;
        p.TotalMaxShield += 1;
        //p.ImmunityFrameMultiplier += 0.05f * Stack;
        p.ShieldImmunityFrameMultiplier += 0.2f + 0.2f * Stack;
    }
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<BubblemancerPerfection>();
}
public class ZapRadius : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases Y:[thunder bubble radius] by Y:10% G:(+10% per stack)");
        description.WithShortDescription("Increases thunder bubble radius");
    }
    public override void HeldEffect(Player p)
    {
        p.ZapRadiusMult += 0.10f * Stack;
    }
}
public class Electroluminescence : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Y:[Thunder bubbles] fire Y:1 G:(+1 per stack) Y:[beams of light] at enemies within Y:3 G:(+0.5 per stack) Y:units of the Y:[thunder bubble radius] for Y:[2 damage]");
        description.WithShortDescription("Thunder bubbles fire beams of light at nearby enemies");
    }
    public override void HeldEffect(Player p)
    {
        p.Electroluminescence += Stack;
    }
}
public class Burger : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases Y:[attack speed] and Y:damage by Y:10% G:(+10% per stack) \nReduces Y:[movement speed] by Y:10% G:(+10% per stack)");
        description.WithShortDescription("Burger!?");
    }
    public override void HeldEffect(Player p)
    {
        p.AttackSpeedModifier += Stack * 0.1f;
        p.TrueMoveModifier -= Stack * 0.1f; 
        p.DamageMultiplier += Stack * 0.1f;
    }
    public override bool IsBlackMarket()
    {
        return true;
    }
}
public class BonusBatteries : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases the number of Y:[thunder bubbles] you can cast by Y:1 G:(+1 per stack)");
        description.WithShortDescription("Increases the number of thunder bubbles you can cast");
    }
    public override void HeldEffect(Player p)
    {
        p.AllowedThunderBalls += Stack;
    }
}
public class ResearchNotes : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Y:[After killing 3 Skull enemies,] become a <color={DetailedDescription.Rares[0]}>Choice</color> with 5 options G:(consumed on use)");
        description.WithShortDescription("After killing 3 Skull enemies, become a Choice with 5 options");
    }
    public override void HeldEffect(Player p)
    {
        p.HasResearchNotes = true;
        while (p.ResearchNoteKillCounter >= 3)
        {
            p.ResearchNoteBonuses++;
            if (Stack > 0 && !PowerUp.PickingPowerUps)
            {
                p.RemovePower(Type);
                PowerUp.Get<Choice>().PickUp(); //Choice is ID 0
            }
            p.ResearchNoteKillCounter -= 3;
        }
    }
    public override int Cost => 50;
    public override int CrucibleGems(bool dissolve = false)
    {
        return dissolve ? 10 : 25;
    }
}
public class ResearchGrants : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Y:[Skull enemies] drop Y:3 G:(+2 per stack) additional Y:coins");
        description.WithShortDescription("Skull enemies drop additional coins");
    }
    public override void HeldEffect(Player p)
    {
        p.FlatSkullCoinBonus += 1 + 2 * Stack;
    }
}
public class Boomerang : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Electric Boomerang");
        description.WithDescription($"Increases Y:[thunder bubble] recall damage by Y:50% G:(+50% per stack)");
        description.WithShortDescription("Increases thunder bubble recall damage");
    }
    public override void HeldEffect(Player p)
    {
        p.ThunderBubbleReturnDamageBonus += Stack * 1f;
    }
}
public class ThunderBubbles : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Echo Bubbles");
        description.WithDescription($"When recalled, Y:[thunder bubbles] leave behind a Y:[latent charge] that dissipates into Y:3 G:(+1 per stack) bubbles after Y:[thunder bubbles] are fully recalled");
        description.WithShortDescription("Thunder bubbles release bubbles when recalled");
    }
    public override void HeldEffect(Player p)
    {
        p.EchoBubbles += 2 + Stack;
    }
}
public class Supernova : PowerUp
{
    public override void Init()
    {
        Weighting = Legendary;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Stars have a Y:[20% chance] to Y:detonate on hit for Y:10 G:(+5 per stack) Y:damage " +
            $"\nActivates <color=#BAE3FE>Starbarbs</color> and <color=#BAE3FE>Lucky Star</color>");
        description.WithShortDescription("Stars have a chance to explode");
    }
    public override void HeldEffect(Player p)
    {
        p.Supernova += Stack;
    }
}
public class ResonanceRuby : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Increases the Y:quality of Y:[wave rewards] by Y:20% G:(+20% per stack) \n" +
            $"<color={DetailedDescription.Yellow}>Wave end</color> gains an additional Y:10% G:(+10% per stack) Y:chance to contain a Y:[bonus power reward]");
        description.WithShortDescription("Improves wave rewards");
    }
    public override void HeldEffect(Player p)
    {
        p.Ruby += Stack;
    }
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<GachaponAddicted>();
}
public class DoubleDown : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Enemies Y:[killed by chips] drop additional Y:coins equal to the Y:[overkill damage dealt] up to a Y:[maximum of 5 coins] G:(+3 per stack) \nIncreases Y:[chip damage] by Y:1 G:(+1 per stack)");
        description.WithShortDescription("Enemies killed by chips drop additional coins and increases chip damage");
        //description.WithDescriptionVariant<SlotMachineWeapon>($"Enemies Y:[killed by chips] drop Y:1 G:(+1 per stack) token and additional Y:coins equal to the Y:[overkill damage dealt] up to a Y:[maximum of 5 coins] G:(+3 per stack) \nIncreases Y:[chip damage] by Y:1 G:(+1 per stack)");
        //description.WithShortDescriptionVariant<SlotMachineWeapon>("Enemies killed by chips drop additional coins and a token, plus increases chip damage");
    }
    public override void HeldEffect(Player p)
    {
        p.DoubleDownChip += Stack;
    }
}
public class FocusFizz : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Increases Y:[critical strike chance] by Y:5% G:(+5% per stack)");
        description.WithShortDescription("Increases critical strike chance");
    }
    public override void HeldEffect(Player p)
    {
        p.CriticalStrikeChance += 0.05f * Stack;
    }
}
public class Coupons : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Reduces the cost of Y:powers in the Y:shop by Y:12% G:(+12% per stack)");
        description.WithShortDescription("Reduces the cost of powers in the shop");
    }
    public override void OnPickup(int count)
    {
        if(GachaponShop.AllShops.Count > 0)
        {
            foreach (GachaponShop shop in GachaponShop.AllShops)
            {
                if (shop.Stock != null)
                    foreach (PowerUpObject p in shop.Stock)
                        for (int i = 0; i < count; ++i)
                            p.Cost = (int)(p.Cost - p.MyPower.Cost * 0.12f);
            }
        }
    }
    public override void HeldEffect(Player p)
    {
        p.ShopDiscount += 0.12f * Stack;
    }
}
public class CloudWalker : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Cloud Walkers");
        description.WithDescription($"Increases Y:[movement speed] by Y:10% G:(+10% per stack)");
        description.WithShortDescription("Increases movement speed");
    }
    public override void HeldEffect(Player p)
    {
        p.TrueMoveModifier += Stack * 0.1f;
    }
}
public class PerpetualBubbleMachine : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Adds Y:1 G:(+1 per stack) <color={DetailedDescription.Rares[0]}>Choice</color> to Y:[wave start]");
        description.WithShortDescription("The key to infinite bubble forever?");
    }
    public override void HeldEffect(Player p)
    {
        p.PerpetualBubble += Stack;
    }
    public override bool IsBlackMarket()
    {
        return true;
    }
}
public class ConsolationPrize : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases Y:[non-winning spin damage] and Y:[secondary attack damage] by Y:20% G:(+20% per stack) \nY:7.77% chance to gain Y:2 G:(+2 per stack) Y:coins on Y:[non-winning spins] " +
            "\nR:[Increases spin price by 0.25] G:(+0.25 per stack) R:coins");
        description.WithShortDescription("Increases non-winning spin damage, secondary attack damage, and gives a chance for consolation coins");
    }
    public override void HeldEffect(Player p)
    {
        p.ConsolationPrize += Stack;
        p.SpinPriceIncrease += 0.25f * Stack;
    }
}
public class Pity : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Pity Charm");
        description.WithDescription("Each consecutive Y:[non-Jackpot spin] increases Y:[Jackpot chance] by Y:4% G:(+2% per stack) " +
            "\nR:[Increases spin price by 0.25] G:(+0.25 per stack) R:coins");
        description.WithShortDescription("Increases Jackpot chance after consecutive spins without a Jackpot");
    }
    public override void HeldEffect(Player p)
    {
        p.PityGrowthAmount = 0.02f + 0.02f * Stack;
        p.SpinPriceIncrease += 0.25f * Stack;
    }
}
public class TokenPouch : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Token Pouch");
        //description.WithDescription("Increases the number of Y:Tokens you can hold by Y:2 G:(+2 per stack) and adds Y:1 G:(+1 per stack) Y:Tokens to Y:[wave start] " +
        //    "\nR:[Increases spin price by 0.5] G:(+0.5 per stack) R:coins");
        //description.WithShortDescription("Hold more Tokens and get Tokens at the start of every wave");
        description.WithDescription("Increases the number of Y:Tokens you can hold by Y:2 G:(+2 per stack) " +
            "\nR:[Increases spin price by 0.25] G:(+0.25 per stack) R:coins");
        description.WithShortDescription("Hold more Tokens");
    }
    public override void HeldEffect(Player p)
    {
        p.MaxTokens += Stack * 2;
        //p.TokensPerWave += Stack * 1;
        p.SpinPriceIncrease += 0.25f * Stack;
    }
    //Token pouch dissolves for more gems cause I think it's funny
    public override int CrucibleGems(bool dissolve = false) => 5;
}
public class BOGOSpin : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Buy One Get One");
        description.WithDescription("Get a Y:[Bonus spin] on Y:10% G:(+10% per stack) of Y:spins \nEach Y:[Bonus spin] has Y:77.7% increased Y:[attack speed] for Y:[every spin that came before it] " +
            "\nR:[Increases spin price by] R:1 G:(+1 per stack) R:coins");
        description.WithShortDescription("Sometimes get a Bonus spin for free");
    }
    public override void HeldEffect(Player p)
    {
        p.BuyOneGetOneMult += 0.1f * Stack;
        p.SpinPriceIncrease += 1 * Stack;
    }
}
public class PhilosophersStone : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Philosopher's Stone");
        description.WithDescription("Increases Y:damage of Y:[high-rarity spins] by Y:10% G:(+10% per stack) and Y:[high-rarity spins] drop Y:50% G:(+50% per stack) more Y:coins on hit " +
            "\nR:[Increases spin price by] R:2 G:(+2 per stack) R:coins");
        description.WithShortDescription("Increases the damage dealt and coins dropped by high-rarity spins");
    }
    public override void HeldEffect(Player p)
    {
        p.PhilosophersStone += Stack;
        p.SpinPriceIncrease += 2 * Stack;
    }
}
public class RouletteWheel : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Roulette Wheel");
        description.WithDescription("Increases the Y:[burst count] of your Y:[primary attack] by Y:1 G:(+1 per stack)" +
            " \nEach Y:[burst] has Y:7.77% increased Y:[attack speed] for Y:[every burst in the same spin that came before it] " +
            "\nR:[Increases spin price by] R:4 G:(+4 per stack) R:coins");
        description.WithShortDescription("Keep that ball rolling!");
    }
    public override void HeldEffect(Player p)
    {
        p.ExtraGachaBurst += Stack;
        p.SpinPriceIncrease += 4 * Stack;
    }
}
public class BatterUp : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Batter Up");
        description.WithDescription("Y:[Secondary attack] launches Y:1 G:(+1 per stack) Y:[Curveball Tokens] for Y:[50% secondary attack damage] that drop Y:1 Y:Token on kill");
        description.WithShortDescription("Let's hit it out of the park!");
    }
    public override void HeldEffect(Player p)
    {
        p.BatterUp += Stack;
    }
}
public class PiratesBooty : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Pirate's Booty");
        description.WithDescription("Y:[Skull enemies] have a Y:10% G:(+10% per stack) chance to drop a Y:[Pirate chest] or Y:key G:(consumed on use)");
        description.WithShortDescription("Next killed Skull enemy has a chance to drop a chest or key");
    }
    public override void HeldEffect(Player p)
    {
        p.PiratesBooty += Stack;
    }
}
public class Eureka : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Eureka!");
        description.WithDescription($"Reduces <color={DetailedDescription.Rares[0]}>Choice</color> Y:Reroll cost by Y:1 G:(+1 per stack) Y:gems and increases Y:[Reroll] count by Y:1 G:(+1 per stack)");
        description.WithShortDescription("Reduces Choice Reroll cost and increases Reroll count");
    }
    public override void OnPickup(int count)
    {
        ChoicePowerMenu.Instance.CostScaling -= count;
        ChoicePowerMenu.Instance.RemainingRerolls += count;
    }
    public override void HeldEffect(Player p)
    {
        p.Eureka += Stack;
    }
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<ThoughtBubbleDecisionsDecisions>();
}
public class BlackMarketDelivery : PowerUp
{
    public override void Init()
    {
        Weighting = Epic;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Special Delivery");
        description.WithDescription("Each Y:[Skull Wave] has a Y:10% G:(+10% per stack) chance to drop a Y:[Black Market crate] G:(consumed on use)");
        description.WithShortDescription("The next Skull Wave has a chance to deliver a Black Market crate");
    }
    public override void HeldEffect(Player p)
    {
        p.BlackMarketDelivery += Stack;
    }
}
public class ShardsOfPower : PowerUp
{
    public override void Init()
    {
        Weighting = Legendary;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Shards of Power");
        description.WithDescription("Drops Y:[3 Rainbow Shards] when Y:dissolved in a Y:Crucible \nY:[Rainbow Shards] can be used to Y:duplicate any Y:power you have");
        description.WithShortDescription("Drops Rainbow Shards when dissolved");
    }
    public override int CrucibleGems(bool dissolve)
    {
        return -3;
    }
}
public class Contract : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Y:Trade for a Y:power from a given selection");
        description.WithShortDescription("!TRADE OFFER!");
    }
    public override void HeldEffect(Player p)
    {
        p.HasContract = true;
        if (Stack > 0 && !PowerUp.PickingPowerUps)
        {
            p.ChoiceContract++;
            p.RemovePower(Type);
            PowerUp.TurnOnPowerUpSelectors();
        }
    }
    public override bool IsBlackMarket()
    {
        return true;
    }
    public override int Cost => 150;
    public override int CrucibleGems(bool dissolve = false)
    {
        return dissolve ? 10 : 25;
    }
}