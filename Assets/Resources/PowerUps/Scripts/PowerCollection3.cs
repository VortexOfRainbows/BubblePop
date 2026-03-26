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
        description.WithDescription("Small bubbles Y:explode on hit or upon expiring for Y:20% G:(+5% per stack) Y:damage \nY:[Explosion size] scales with Y:damage");
        description.WithShortDescription("Small bubbles explode on hit or upon expiring");
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
        description.WithName("Explosion Radius");
        description.WithDescription("Increases Y:[cola explosion radius] by Y:10% G:(+10% per stack)");
        description.WithShortDescription("Increases cola explosion radius");
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
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/DexBoot");
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
        description.WithDescription("Secondary attack drop a Y:3 G:(+3 per stack) Y:[damage bath bomb] \nY:[Explosion size] scales with Y:damage");
        description.WithShortDescription("Secondary attack drops a bath bomb");
    }
    public override void HeldEffect(Player p)
    {
        p.BombBrew += Stack;
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/Dice/InitiativeAddOn");
    }
}