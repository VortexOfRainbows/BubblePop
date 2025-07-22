using UnityEngine;

public class Choice : PowerUp
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Pick which power you want from a given selection");
    }
    public override void HeldEffect(Player p)
    {
        if (Stack > 0 && !PowerUp.PickingPowerUps)
        {
            p.RemovePower(Type);
            PowerUp.TurnOnPowerUpSelectors();
        }
    }
    public override int Cost => 25;
}
public class WeaponUpgrade : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Haste");
        description.WithDescription("Increases Y:[attack speed] by Y:10% G:(+10% per stack)");
        description.WithShortDescription("Increases attack speed");
    }
    public override void HeldEffect(Player p)
    {
        p.AttackSpeedModifier += Stack * 0.1f;
    }
    public override Sprite GetTexture()
    {
        return Resources.Load<Sprite>("PowerUps/Haste");
    }
}
public class Overclock : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Reduces Y:[ability cooldown] by Y:20% G:(+20% per stack)");
        description.WithShortDescription("Reduces ability cooldown");
    }
    public override void HeldEffect(Player p)
    {
        p.AbilityRecoverySpeed += 0.2f * Stack;
    }
}
public class ChargeShot : PowerUp
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases the Y:damage of your Y:[secondary attack] by Y:50% G:(+50% per stack) and the Y:size of your Y:[secondary attacks] by 40% G:(+40% per stack)");
        description.WithShortDescription("Increases the size and damage of your secondary attack");
    }
    public override void HeldEffect(Player p)
    {
        p.ChargeShotDamage += Stack;
    }
}
public class Shotgun : PowerUp
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases the amount of bubbles shot by your Y:[primary attack] by Y:20% G:(+20% per stack)");
        description.WithShortDescription("Increases the amount of bubbles shot by your primary attack");
    }
    public override void HeldEffect(Player p)
    {
        p.ShotgunPower += Stack;
    }
}
public class Dash : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Sparkle Sparkle");
        description.WithDescription("Y:Dashing releases Y:2 G:(+1 per stack) damaging stars");
        description.WithDescriptionVariant<ThoughtBubble>("Y:[Channeling recall] has a Y:33% G:(+17% per stack) Y:chance to release damaging stars");
        description.WithDescriptionVariant<Gachapon>("Y:Chips have a Y:50% G:(+25% per stack) Y:chance to release damaging stars");
        description.WithShortDescription("Adds damaging stars to your ability");
    }
    public override void HeldEffect(Player p)
    {
        p.DashSparkle += Stack;
    }
}
public class ShotSpeed : PowerUp
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubble Propulsion");
        description.WithDescription("Blown bubbles travel Y:10% G:(+10% per stack) further and Y:10% G:(+10% per stack) faster"); //These numbers are not quite correct but do give a good estimate
        description.WithShortDescription("Blown bubbles travel further and faster");
    }
    //protected override string Name() => "Bubble Propulsion";
    //protected override string Description() => "Blown bubbles travel further and faster";
    public override void HeldEffect(Player p)
    {
        p.FasterBulletSpeed += Stack;
    }
}
public class Starbarbs : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Enemies Y:[killed by stars] explode into Y:3 G:(+1 per stack) stars");
        description.WithShortDescription("Enemies killed by stars explode into stars");
    }
    public override void HeldEffect(Player p)
    {
        p.Starbarbs += Stack;
    }
}
public class SoapySoap : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Your Y:[secondary attack] releases Y:5 G:(+3 per stack) bubbles while traveling, plus Y:[2 more bubbles for each charge level]");
        description.WithShortDescription("Your secondary attack leaves behind a trail of bubbles");
    }
    public override void HeldEffect(Player p)
    {
        p.SoapySoap += Stack;
    }
}
public class BubbleBlast : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Your Y:[secondary attack] releases Y:4 G:(+3 per stack) bubbles upon expiring, plus Y:1 G:(+1 per stack) Y:[more bubbles for each charge level]");
        description.WithShortDescription("Your secondary attack releases bubbles upon expiring");
    }
    public override void HeldEffect(Player p)
    {
        p.BubbleBlast += Stack;
    }
}
public class Starshot : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Y:15% G:(+5% per stack) Y:chance to launch stars with your Y:[primary attack] " + //This isn't quite accurate, but is a good approximation
            "\nIncreases the amount of bubbles shot by your Y:[primary attack] by Y:20% G:(+20% per stack)");
        description.WithShortDescription("Launch stars with your primary attack");
    }
    public override void HeldEffect(Player p)
    {
        p.Starshot += Stack;
        p.ShotgunPower += Stack;
    }
}
public class BinaryStars : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Release Y:2 G:(+1 per stack) stars every Y:[1.5 seconds]");
        description.WithShortDescription("Periodically release stars");
    }
    public override void HeldEffect(Player p)
    {
        p.BinaryStars += Stack;
    }
}
public class EternalBubbles : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases lifespan of small bubbles by Y:[0.8 seconds] G:(+0.4 seconds per stack)");
        description.WithShortDescription("Increases lifespan of small bubbles");
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/DurationUpgrade");
    }
    public override void HeldEffect(Player p)
    {
        p.EternalBubbles += Stack;
    }
}
public class BubbleBirb : PowerUp
{
    public override void Init()
    {
        Weighting = Legendary;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Resurrect in a dance of flames after death G:(consumed on use)");
        description.WithShortDescription("Resurrect in a dance of flames after death");
    }
    public override void HeldEffect(Player p)
    {
        p.BonusPhoenixLives += Stack; 
    }
    public override void OnPickup(int count)
    {
        Player.Instance.PickedUpPhoenixLivesThisRound += count;
    }
}
public class BubbleTrail : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Release a bubble every Y:[0.67 seconds] G:(-33% per stack)");
        description.WithShortDescription("Periodically release bubbles");
    }
    public override void HeldEffect(Player p)
    {
        p.BubbleTrail += Stack;
    }
}
public class Coalescence : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Your Y:[secondary attack] may be charged Y:1 additional times " +
            "\nIncreases Y:[secondary attack speed] by Y:5% G:(+5% per stack)");
        description.WithShortDescription("Your secondary attack may be charged an additional time");
    }
    public override void HeldEffect(Player p)
    {
        p.Coalescence += Stack;
        p.SecondaryAttackSpeedModifier += 0.05f * Stack;
    }
}
public class LuckyStar : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Enemies Y:[killed by stars] have an additional Y:2% G:(+2% per stack) Y:chance to Y:[drop powers]");
        description.WithShortDescription("Enemies killed by stars have an additional chance to drop powers");
    }
    public override void HeldEffect(Player p)
    {
        p.LuckyStar += Stack;
    }
}
public class TrailOfThoughts : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Trail of Thoughts");
        description.WithDescription("Increases the maximum length of your Y:[thought trail] by Y:3 G:(+3 per stacK)");
        description.WithShortDescription("Increases the maximum length of your thought trail");
    }
    public override void HeldEffect(Player p)
    {
        p.TrailOfThoughts += Stack;
    }
}
public class Magnet : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Extends the distance coins are collected from by Y:75% G:(+75% per stack)");
        description.WithShortDescription("Collect coins from farther away");
    }
    public override void HeldEffect(Player p)
    {
        p.Magnet += Stack;
    }
}
public class SpearOfLight : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Spear of Light");
        description.WithDescription("Fire beams of light at enemies within Y:9 G:(+2.25 per stack) Y:units for Y:[2.5 damage] G:(+0.5 per stack) every Y:[2.2 seconds]");
        description.WithShortDescription("Periodically fire beams of light at nearby enemies");
    }
    public override void HeldEffect(Player p)
    {
        p.LightSpear += Stack;
    }
}
public class NeuronActivation : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Reflection");
        description.WithDescription("Enemies Y:[struck by beams of light] fire beams of light at nearby enemies up to Y:1 G:(+1 per stack) additional times");
        description.WithShortDescription("Enemies struck by beams of light fire beams of light at nearby enemies");
    }
    public override void HeldEffect(Player p)
    {
        p.LightChainReact += Stack;
    }
}
public class BrainBlast : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Release Y:20% G:(+10% per stack) more bubbles when Y:[channeling recall] and explode into Y:1 G:(+0.75 per stack) additional bubbles per segment Y:[after recall]");
        description.WithShortDescription("Release additional bubbles when recalling");
    }
    public override void HeldEffect(Player p)
    {
        p.BrainBlast += Stack;
    }
}
public class Raise : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases Y:[maximum chip stack height] by Y:1 G:(+1 per stack) and reduces Y:[ability cooldown] by Y:10% G:(+10% per stack)");
        description.WithShortDescription("Increases maximum chip stack height and slightly reduces ability cooldown");
    }
    public override void HeldEffect(Player p)
    {
        p.ChipHeight += Stack;
        p.AbilityRecoverySpeed += 0.1f * Stack;
    }
}
public class AllIn : PowerUp
{
    public override void Init()
    {
        Weighting = Legendary;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("All-In");
        description.WithDescription("Increases Y:[total chip stacks] by Y:2 G:(+2 per stack) and grants a Y:5% G:(+5% per stack) Y:chance for chips to be enhanced " +
            "\nIncreases Y:[maximum chip stack height] by Y:2 G:(+2 per stack) and reduces Y:[ability cooldown] Y:multiplicatively by Y:60% G:(+60% per stack)");
        description.WithShortDescription("Greatly increases the potency of your chips");
    }
    public override void HeldEffect(Player p)
    {
        p.ChipHeight += Stack;
        p.ChipStacks += Stack * 2;
        p.AbilityRecoverySpeedMult += 0.6f * Stack;
        p.BlueChipChance += Mathf.Min(10, Stack) * 0.05f;
    }
}
public class SnakeEyes : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Y:8.3% G:(+2.8% per stack) Y:chance to Y:[recursively strike] enemies with green lightning");
        description.WithShortDescription("Chance to deal additional damage");
    }
    public override void HeldEffect(Player p)
    {
        p.SnakeEyes += Stack;
    }
}
public class Refraction : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Beams of light target up to Y:1 G:(+1 per stack) additional enemies within Y:9 G:(+2 per stack) Y:units");
        description.WithShortDescription("Beams of light can target an additional enemy");
    }
    public override void HeldEffect(Player p)
    {
        p.Refraction += Stack;
    }
}
public class Calculator : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Friendly projectiles Y:[seek out] enemies within Y:4 G:(+2 per stack) Y:units");
        description.WithShortDescription("Grants aim assist");
    }
    public override void HeldEffect(Player p)
    {
        p.HomingRange += Stack * 2f + 2f;
    }
}