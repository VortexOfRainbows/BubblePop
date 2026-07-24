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
        p.OilSpill += 2 + 2 * Stack;
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

    }
}
public class GoldenGun : PowerUp
{
    public override void Init() => Weighting = Rare;
    public override void HeldEffect(Player p)
    {

    }
}
public class DiversifiedPortfolio : PowerUp
{
    public override void Init() => Weighting = Rare;
    public override void HeldEffect(Player p)
    {

    }
}
public class CompoundInterest : PowerUp
{
    public override void Init() => Weighting = SuperRare;
    public override void HeldEffect(Player p)
    {

    }
}
public class Pumpjack : PowerUp
{
    public override void Init() => Weighting = Legendary;
    public override void HeldEffect(Player p)
    {

    }
}
public class Futures : PowerUp
{
    public override bool IsInvestmentPower() => true;
    public override void Init() => Weighting = Common;
    public override void HeldEffect(Player p)
    {

    }
}
public class Commodities : PowerUp
{
    public override bool IsInvestmentPower() => true;
    public override void Init() => Weighting = Uncommon;
    public override void HeldEffect(Player p)
    {

    }
}
public class Options : PowerUp
{
    public override bool IsInvestmentPower() => true;
    public override void Init() => Weighting = Rare;
    public override void HeldEffect(Player p)
    {

    }
}
public class Securities : PowerUp
{
    public override bool IsInvestmentPower() => true;
    public override void Init() => Weighting = SuperRare;
    public override void HeldEffect(Player p)
    {

    }
}
public class Windfall : PowerUp
{
    public override bool IsInvestmentPower() => true;
    public override void Init() => Weighting = Legendary;
    public override void HeldEffect(Player p)
    {

    }
}