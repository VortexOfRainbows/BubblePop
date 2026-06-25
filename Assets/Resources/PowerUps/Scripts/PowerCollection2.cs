using UnityEngine;
public class BubbleMitosis : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    //public override void InitializeDescription(ref DetailedDescription description)
    //{
    //    description.WithName("Mitosis");
    //    description.WithDescription("Y:[Upon obtaining your next power,] gain Y:[additional stacks] equal to the amount of <color=#BAE3FE>Mitosis</color> stacks G:(consumed on use)");
    //    description.WithShortDescription("Assimilated into next power you obtain");
    //}
    public override void HeldEffect(Player p)
    {
        if (p.MostRecentPower != null && p.MostRecentPower.Type != Type)
        {
            int originalStack = Stack;
            p.RemovePower(Type, originalStack);
            p.MostRecentPower.PickUp(p, originalStack);
            if (originalStack >= 5 && p.MostRecentPower is Calculator)
            {
                UnlockCondition.Get<ThoughtBubbleShortForCalc>().SetComplete();
            }
        }
    }
    public override int CrucibleGems(bool dissolve = false)
    {
        return dissolve ? base.CrucibleGems(dissolve) : 25;
    }
}
public class RollForCharisma : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
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
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<GachaponHealer>();
}
public class RollForDexterity : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
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
    public override void ModifyDescription(ref PowerDescription description)
    {
        description.WithAlt<SlotMachineWeapon>(true, true);
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
    public override void OnPickup(int count)
    {
        foreach(Player player in Player.AllPlayers)
            player.SetShield(player.GetShield() + count);
    }
    public override void HeldEffect(Player p)
    {
        p.BubbleShields += Stack;
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
    //public override void InitializeDescription(ref DetailedDescription description)
    //{
    //    description.WithDescription($"Y:[After killing 3 Skull enemies,] become a <color={ColorHelper.RarityColorHex[0]}>Choice</color> with 5 options G:(consumed on use)");
    //    description.WithShortDescription("After killing 3 Skull enemies, become a Choice with 5 options");
    //}
    public override void HeldEffect(Player p)
    {
        p.HasResearchNotes = true;
        if (p.ResearchNoteKillCounter >= 3)
        {
            if (Stack > 0 && !PowerUp.PickingPowerUps)
            {
                p.ResearchNoteBonuses++;
                p.RemovePower(Type);
                PowerUp.Get<Choice>().PickUp(p); //Choice is ID 0
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
    //public override void InitializeDescription(ref DetailedDescription description)
    //{
    //    description.WithDescription($"Stars have a Y:[20% chance] to Y:detonate on hit for Y:10 G:(+5 per stack) Y:damage " +
    //        $"\nActivates <color=#BAE3FE>Starbarbs</color> and <color=#BAE3FE>Lucky Star</color>");
    //    description.WithShortDescription("Stars have a chance to explode");
    //}
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
    public override int ShardReplicationCost(int stackSize = 1)
    {
        return stackSize * 1;
    }
}
public class CloudWalker : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
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
    //public override void InitializeDescription(ref DetailedDescription description)
    //{
    //    description.WithDescription($"Adds Y:1 G:(+1 per stack) <color={ColorHelper.RarityColorHex[0]}>Choice</color> to Y:[wave end] \nR:[Rainbow Shard replication cost increases with stack size]");
    //    description.WithShortDescription("The key to infinite bubble forever?");
    //}
    public override void HeldEffect(Player p)
    {
        p.PerpetualBubble += Stack;
    }
    public override bool IsBlackMarket()
    {
        return true;
    }
    public override int ShardReplicationCost(int stackSize = 1)
    {
        int currentStack = Stack;
        return (currentStack * stackSize) + (stackSize * stackSize + stackSize) / 2;
    }
}
public class ConsolationPrize : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
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
    public override void ModifyDescription(ref PowerDescription description)
    {
        description.WithBlackMarketVariant();
    }
    public override void OnPickup(int count)
    {
        if(IsBlackMarket())
            ChoicePowerMenu.Instance.CostScaling -= count * 5;
        else
            ChoicePowerMenu.Instance.CostScaling -= count * 2;
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
        Weighting = SuperRare;
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
    public override int CrucibleGems(bool dissolve)
    {
        return dissolve ? -3 : 25;
    }
    public override int ShardReplicationCost(int stackSize = 1)
    {
        return stackSize * 5;
    }
}
public class Contract : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
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
public class RainbowFlower : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void HeldEffect(Player p)
    {
        //p.RainbowFlowers += Stack;
    }
    public override bool IsBlackMarket() => true;
    public override int CrucibleGems(bool dissolve = false)
    {
        if(dissolve)
        {
            int count = 1;
            while (Utils.RollWithLuck(0.5f))
                ++count;
            return -count;
        }
        return 15;
    }
    public override int ShardReplicationCost(int stackSize = 1)
    {
        return stackSize * 3;
    }
}
public class QuantumCake : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    //public override void InitializeDescription(ref DetailedDescription description)
    //{
    //    description.WithName("Quantum Cake");
    //    description.WithDescription($"Drops a Y:[Heart] and becomes a {"Schrodinger's Cake".WithColor(ColorHelper.RarityColorHex[5])} when Y:dissolved in a Y:Crucible");
    //    description.WithShortDescription("And you can eat it too!");
    //}
    public override void HeldEffect(Player p)
    {

    }
    public override bool IsBlackMarket() => true;
}
public class EatenCake : PowerUp
{
    public override void Init()
    {
        Weighting = -1;
    }
    //public override void InitializeDescription(ref DetailedDescription description)
    //{
    //    description.WithName("Schrodinger's Cake");
    //    description.WithDescription($"Becomes a {PowerUp.Get<QuantumCake>().UnlockedName} at the start of a Y:wave G:(consumed on use)");
    //    description.WithShortDescription("And you can have it too!");
    //}
    public override void HeldEffect(Player p)
    {

    }
    public override int CalculateRarity() => 3;
    public override bool IsBlackMarket() => true;
}
public class GlassShard : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void HeldEffect(Player p)
    {
        p.GlassShards += Stack;
    }
    public override bool IsBlackMarket() => true;
}
public class BountyHunter : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void HeldEffect(Player p)
    {
        p.BountyHunter += Stack;
    }
    public override bool IsBlackMarket() => true;
}