using System.Collections;
using System.Collections.Generic;
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
            p.RemovePower(Type, Stack);
        }
    }
}