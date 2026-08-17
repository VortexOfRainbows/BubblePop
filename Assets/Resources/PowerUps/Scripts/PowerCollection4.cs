using UnityEngine;
public class Corrode : PowerUp
{
    public override void Init() => Weighting = Common;
    public override void HeldEffect(Player p)
    {
        p.CorrodeDamage += 0.2f * Stack;
    }
}
public class Contaminate : PowerUp
{
    public override void Init() => Weighting = Common;
    public override void HeldEffect(Player p)
    {
        p.TarShots += 1; //Grant tar to the player if they do not already have it for this power... idk why im doing this but I feel like I should
        p.TarBonusDuration += Stack;
    }
}
public class Combust : PowerUp
{
    public override void Init() => Weighting = Uncommon;
    public override void HeldEffect(Player p)
    {
        p.CombustBonusDamage += Stack;
    }
}
public class Concoct : PowerUp
{
    public override void Init() => Weighting = Uncommon;
    public override void HeldEffect(Player p)
    {
        p.TarConcoct += Stack;
        //p.BonusTarSlow += 0.1f * Stack;
    }
}
public class OilSpill : PowerUp
{
    public override void Init() =>  Weighting = Rare;
    public override void HeldEffect(Player p)
    {
        p.OilSpill += 4 * Stack;
    }
}
public class Gasoline : PowerUp
{
    public override void Init() => Weighting = Rare;
    public override void HeldEffect(Player p)
    {
        p.Gasoline += 1 + Stack;
    }
}
public class BlackDiamond : PowerUp
{
    public override void Init() => Weighting = SuperRare;
    public override void HeldEffect(Player p)
    {
        p.BonusBlackDiamond += Stack;
        p.SecondaryAttackSpeedModifier += Stack * 0.3f;
    }
}
public class Smokestack : PowerUp
{
    public override void Init() => Weighting = Uncommon;
    public override void HeldEffect(Player p)
    {
        p.SmokeStack += Stack;
    }
}
public class GoldenGun : PowerUp
{
    public override void Init() => Weighting = Rare;
    public override void HeldEffect(Player p)
    {
        p.GoldenGun += Stack;
    }
}
public class DiversifiedPortfolio : PowerUp
{
    public override void Init() => Weighting = Rare;
    public override void HeldEffect(Player p)
    {
        if (Stack > 0 && !PowerUp.PickingPowerUps)
        {
            p.InvestmentChoices++;
            p.RemovePower(Type);
            PowerUp.TurnOnPowerUpSelectors();
        }
    }
    public override int CrucibleGems(bool dissolve = false)
    {
        return dissolve ? 10 : 25;
    }
}
public class CompoundInterest : PowerUp
{
    public override void Init() => Weighting = Legendary;
    public override void HeldEffect(Player p)
    {
        p.CompoundInterest += Stack;
    }
}
public class Pumpjack : PowerUp
{
    public override void Init() => Weighting = SuperRare;
    public override void HeldEffect(Player p)
    {
        p.Pumpjack += Stack;
    }
}
public class Futures : PowerUp
{
    public override bool IsInvestmentPower() => true;
    public override void Init() => Weighting = Common;
    public override void HeldEffect(Player p)
    {
        p.TotalInvestments += Stack;
        p.HasFutures = true;
    }
    public override bool EffectedBySoup() => false;
}
public class Commodities : PowerUp
{
    public override bool IsInvestmentPower() => true;
    public override void Init() => Weighting = Uncommon;
    public override void HeldEffect(Player p)
    {
        p.TotalInvestments += Stack;
        p.HasCommodities = true;
    }
}
public class Options : PowerUp
{
    public override bool IsInvestmentPower() => true;
    public override void Init() => Weighting = Rare;
    public override void HeldEffect(Player p)
    {
        p.TotalInvestments += Stack;
        p.HasOptions = true;
    }
}
public class Securities : PowerUp
{
    public override bool IsInvestmentPower() => true;
    public override void Init() => Weighting = SuperRare;
    public override void HeldEffect(Player p)
    {
        p.TotalInvestments += Stack;
        p.HasSecurities = true;
    }
}
public class Windfall : PowerUp
{
    public override bool IsInvestmentPower() => true;
    public override void Init() => Weighting = Legendary;
    public override void HeldEffect(Player p)
    {
        p.TotalInvestments += Stack;
        p.HasWindfall = true;
    }
}
public class ReactorCore : PowerUp
{
    public override void Init() => Weighting = Uncommon;
    public override void HeldEffect(Player p)
    {
        p.OilBarrelSize += Stack;
    }
}
public class FlintAndSteel : PowerUp
{
    public override void Init() => Weighting = Rare;
    public override void HeldEffect(Player p)
    {
        p.FlintAndSteel += 1 + Stack;
    }
}
public class Thunderbolt : PowerUp
{
    public override void Init() => Weighting = Legendary;
    public override void HeldEffect(Player p)
    {
        p.BonusAerialBarrels += 1 + Stack;
    }
}
public class DefenseContract : PowerUp
{
    public override void Init() => Weighting = SuperRare;
    public override void HeldEffect(Player p)
    {
        p.HelicopterStacks += Stack;
    }
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<OilKingTooBigToFail>();
}
public class Soup : PowerUp
{
    public override void Init() => Weighting = Uncommon;
    public override void HeldEffect(Player p)
    {
        p.Bonus1StarStacksFromSoup = Stack; //Purposely does not use +=, as this is NOT reset every frame, but only if you have 0 soup left
    }
    public override bool IsBlackMarket() => true;
}
public class SpilledSoup : PowerUp
{
    public override void Init() => Weighting = -1;
    public override void HeldEffect(Player p)
    {
        //DOES NOTHING
    }
    public override bool IsBlackMarket() => true;
    public override int CrucibleGems(bool dissolve = false)
    {
        return dissolve ? 1 : base.CrucibleGems(dissolve);
    }
    public override int CalculateRarity() => 2;
}
public class TachyonAccelerator : PowerUp
{
    public override void Init() => Weighting = SuperRare;
    public override void HeldEffect(Player p)
    {
        p.TachyonStacks += Stack;
    }
    public override bool IsBlackMarket() => true;
}