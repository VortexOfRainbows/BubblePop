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
        p.DamagePower += Stack;
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
    public override string Name() => "Sparkle-Sparkle Dash";
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