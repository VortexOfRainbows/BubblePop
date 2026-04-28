using UnityEngine;
public class BonusFizz : Shotgun
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        base.InitializeDescription(ref description);
        description.WithName("Bottle Burst");
    }
}
public class BottleBlast : BubbleBlast
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        base.InitializeDescription(ref description);
        description.WithName("Bottle Blast");
        description.WithDescription("Y:[Secondary attack] releases Y:4 G:(+4 per stack) additional bubbles on impact");
        description.WithShortDescription("Secondary attack releases extra bubbles");
    }
}
public class BottleFlip : PowerUp
{
    public override void Init()
    {
        Weighting = Epic;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bottle Flip");
        description.WithDescription("Y:[Secondary attack] bounces and Y:explodes Y:1 G:(+1 per stack) additional times");
        description.WithShortDescription("Secondary attack bounces and explodes an additional time");
    }
    public override void HeldEffect(Player p)
    {
        p.BottleFlip += Stack;
    }
}
public class FancyFootwork : CloudWalker
{
    public override void Init()
    {
        Weighting = Epic;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        base.InitializeDescription(ref description);
        description.WithName("Fancy Footwork");
        description.WithDescription($"Gain a Y:10% G:(+10% per stack) chance to Y:[dodge damage] \nChance to Y:dodge reduced by Y:50% for each Y:[consecutive dodge]");
        description.WithShortDescription("Chance to dodge attacks");
    }
    public override void HeldEffect(Player p)
    {
        p.DodgeStat += 0.1f * Stack;
    }
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<FizzyThinkImJustGonnaStandThereAndTakeIt>();
}
public class CarbonForce : ShotSpeed
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Carbon Force");
        description.WithDescription("Small bubbles gain Y:+1 G:(+1 per stack) Y:pierce and travel Y:10% G:(+10% per stack) further and faster"); 
        description.WithShortDescription("Small bubbles get additional pierce and travel further");
    }
    public override void HeldEffect(Player p)
    {
        base.HeldEffect(p);
        p.BonusBubblePierce += Stack;
    }
}
public class FlavorExplosion : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Flavor Explosion");
        description.WithDescription("Small bubbles Y:splash on hit or upon expiring for Y:20% G:(+5% per stack) Y:damage \nY:[Splash size] scales with Y:damage");
        description.WithShortDescription("Small bubbles splash on hit or upon expiring");
    }
    public override void HeldEffect(Player p)
    {
        p.ExplodingBubbles += Stack;
    }
}
public class ExplosionRadius : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Splash Radius");
        description.WithDescription("Increases Y:[cola splash radius] by Y:10% G:(+10% per stack)");
        description.WithShortDescription("Increases cola splash radius");
    }
    public override void HeldEffect(Player p)
    {
        p.ExplodeRadiusMult += 0.1f * Stack;
    }
}
public class BonusBoards : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bonus Boards");
        description.WithDescription("Release Y:1 G:(+1 per stack) additional Y:Skateboards on Y:dismount");
        description.WithShortDescription("Release additional Skateboards on dismount");
    }
    public override void HeldEffect(Player p)
    {
        p.BonusBoards += Stack;
    }
}
public class Kickflip : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Kickflip");
        description.WithDescription("Increases Y:[movement speed] by Y:15% G:(+15% per stack) while Y:mounting or Y:dismounting your Y:Skateboard");
        description.WithShortDescription("Faster movement while mounting or dismounting Skateboard");
    }
    public override void HeldEffect(Player p)
    {
        p.Kickflip += Stack;
    }
}
public class RetaliatoryBlast : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Retaliatory Blast");
        description.WithDescription("Drop a Y:10 G:(+10 per stack) Y:[damage bath bomb when hurt] \nY:[Explosion size] scales with Y:damage");
        description.WithShortDescription("Drop a high-damage bath bomb when hurt");
    }
    public override void HeldEffect(Player p)
    {
        p.RetaliatoryBomb += Stack;
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/InitiativeAddOn");
    }
}
public class BombasticBrew : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bombastic Brew");
        description.WithDescription("Secondary attack drop a Y:2 G:(+2 per stack) Y:[damage bath bomb] \nY:[Explosion size] scales with Y:damage");
        description.WithShortDescription("Secondary attack drops a bath bomb");
    }
    public override void HeldEffect(Player p)
    {
        p.BombBrew += Stack;
    }
}
public class CollateralDamage : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Collateral Damage");
        description.WithDescription("Y:[Skull enemies] drop a Y:5 G:(+5 per stack) Y:[damage bath bomb] \nY:[Explosion size] scales with Y:damage");
        description.WithShortDescription("Skull enemies drop a bath bomb when killed");
    }
    public override void HeldEffect(Player p)
    {
        p.SkullBomb += Stack;
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/InitiativeAddOn");
    }
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<FizzyCoolGuys>();
}
public class ClusterBombs : PowerUp
{
    public override void Init()
    {
        Weighting = Legendary;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Cluster Bombs");
        description.WithDescription("Y:[Bath bombs] drop Y:smaller Y:50% G:(+50% per stack) Y:[damage bath bombs] \nY:[Explosion size] scales with Y:damage");
        description.WithShortDescription("Bath bombs drop smaller bath bombs");
    }
    public override void HeldEffect(Player p)
    {
        p.ClusterBomb += Stack;
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/InitiativeAddOn");
    }
}
public class SharpBombs : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Sharp Bombs");
        description.WithDescription("Y:[Bath bombs] release Y:50% G:(+50% per stack) more Y:shrapnel \nIncreases lifespan of Y:shrapnel by Y:10% G:(+10% per stack)");
        description.WithShortDescription("Bath bombs release extra shrapnel");
    }
    public override void HeldEffect(Player p)
    {
        p.BonusShrapnel += 0.5f * Stack;
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/InitiativeAddOn");
    }
}
public class PrizeBombs : PowerUp
{
    public override void Init()
    {
        Weighting = Epic;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Prize Bombs");
        description.WithDescription("Enemies drop Y:10 G:(+10 per stack) additional Y:coins when killed by Y:[bath bombs] or Y:shrapnel");
        description.WithShortDescription("Enemies drop coins when killed by bath bombs");
    }
    public override void HeldEffect(Player p)
    {
        p.PrizeBombCoins += 10 * Stack;
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/InsightAddOn");
    }
}
public class Restock : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"All Y:shops gain Y:+2 G:(+2 per stack) Y:stock \nUsing Y:gems to Y:restock has a Y:50% G:(+50% per stack) chance to Y:stock extra Y:powers");
        description.WithShortDescription("All shops gain additional stock and a chance for bonus stock on restock");
    }
    public override void OnPickup(int count)
    {
        if (GachaponShop.AllShops.Count > 0)
            foreach (GachaponShop shop in GachaponShop.AllShops)
            {
                shop.RestockRemaining += 2;
                shop.RestockMachine.UpdateUI(shop);
            }
    }
    public override void HeldEffect(Player p)
    {
        p.BonusStocks += Stack * 2;
        p.BonusRestockChance += 0.5f * Stack;
    }
}