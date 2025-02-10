using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice : PowerUp
{
    public override string Name() => "???";
    public override string Description() => "Pick which power you want from a given selection";
    public override void HeldEffect(Player p)
    {
        if (Stack > 0 && !PowerUp.PickingPowerUps)
        {
            p.RemovePower(Type);
            PowerUp.TurnOnPowerUpSelectors();
        }
    }
}
public class ChargeShot : PowerUp
{
    public override string Name() => "Charge Shot";
    public override string Description() => "Increases the size and damage of your charge attacks";
    public override void HeldEffect(Player p)
    {
        p.ChargeShotDamage += Stack;
    }
}
public class Shotgun : PowerUp
{
    public override string Name() => "Shotgun";
    public override string Description() => "Increases the amount of bubbles shot by your primary weapon";
    public override void HeldEffect(Player p)
    {
        p.ShotgunPower += Stack;
    }
}
public class Dash : PowerUp
{
    public override void Init()
    {
        Weighting = 0.8f;
    }
    public override string Name() => "Sparkle Sparkle";
    public override string Description() => "Scatter stars around you while dashing";
    public override void HeldEffect(Player p)
    {
        p.DashSparkle += Stack;
    }
}
public class ShotSpeed : PowerUp
{
    public override string Name() => "Bubble Propulsion";
    public override string Description() => "Blown bubbles travel further and faster";
    public override void HeldEffect(Player p)
    {
        p.FasterBulletSpeed += Stack;
    }
}
public class Starbarbs : PowerUp
{
    public override void Init()
    {
        Weighting = 0.3f;
    }
    public override string Name() => "Starbarbs";
    public override string Description() => "Enemies killed by stars explode into stars";
    public override void HeldEffect(Player p)
    {
        p.Starbarbs += Stack;
        if (Stack > StarbarbUnlock5.StarbarbBestCount)
            StarbarbUnlock5.StarbarbBestCount = Stack;
    }
}
public class SoapySoap : PowerUp
{
    public override void Init()
    {
        Weighting = 0.3f;
    }
    public override string Name() => "Soapy Soap";
    public override string Description() => "Charge attacks leave behind a trail of bubbles";
    public override void HeldEffect(Player p)
    {
        p.SoapySoap += Stack;
    }
}
public class BubbleBlast : PowerUp
{
    public override void Init()
    {
        Weighting = 0.75f;
    }
    public override string Name() => "Bubble Blast";
    public override string Description() => "Charge attacks release bubbles upon expiring";
    public override void HeldEffect(Player p)
    {
        p.BubbleBlast += Stack;
    }
}
public class Starshot : PowerUp
{
    public override void Init()
    {
        Weighting = 0.075f;
    }
    public override string Name() => "Starshot";
    public override string Description() => "Chance for stars to be fired alongside shotgun bubbles" +
        "\nIncreases the amount of bubbles shot by your primary weapon";
    public override void HeldEffect(Player p)
    {
        p.Starshot += Stack;
        p.ShotgunPower += Stack;
    }
}