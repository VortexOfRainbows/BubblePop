using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public class Choice : PowerUp
{
    public override string Name() => "???";
    public override string Description() => "Pick which power you want from a given selection";
    public override void HeldEffect(Player p)
    {
        if (Stack > 0)
            p.RemovePower(Type);
    }
}
public class Dash : PowerUp
{
    public override string Name() => "Dash Trail";
    public override string Description() => "Leave a trail of bubbles behind as you dash";
    public override void HeldEffect(Player p)
    {
        //Unimplemented;
    }
}
public class Planet : PowerUp
{
    public override string Name() => "Bubble System";
    public override string Description() => "Periodically spawn bubbles around you";
    public override void HeldEffect(Player p)
    {
        //Unimplemented;
    }
}