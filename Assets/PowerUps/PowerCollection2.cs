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
public class RollForCharisma : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Roll For Persuasion");
        description.WithDescription("Y:20% G:(+1% per stack) chance to Y:heal after Y:[purchasing a power] or gain Y:10 G:(+5 per stack) Y:coins if uninjured");
        description.WithShortDescription("Chance to heal or refund a portion of money spent when purchasing power");
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
}
public class RollForDexterity : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Y:20% G:(+1% per stack) chance to gain Y:15% increased movement speed after using Y:[your ability,] stacking up to Y:2 G:(+1 per stack) times and lasting Y:[5 seconds]");
        description.WithShortDescription("Chance to gain increased speed after using your ability");
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
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Y:20% G:(+1% per stack) chance to deal 150% G:(+5% per stack) to 300% G:(+20% per stack) Y:[bonus damage] on Y:[first strike]");
        description.WithShortDescription("Chance for first hit to deal additional damage");
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
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases weights of Y:[rare powers] in the Y:[power pool] by Y:20% G:(+20% per stack)");
        description.WithShortDescription("Chance of seeing rare powers increased");
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
}
public class BubbleShield : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Gain Y:[1 shield slot] and Y:[recover 1 shield] on pickup or at the start of Y:[even-numbered waves] " +
            "\nWhen a Y:[shield is broken,] extend Y:[immunity frames] by Y:40% G:(+20% per stack) and release Y:24 G:(+8 per stack) bubbles");
        description.WithShortDescription("Gain a shield on even-numbered waves, extend immunity frames and release bubbles when shields are broken");
    }
    public override void OnPickup(int count)
    {
        Player.Instance.SetShield(Player.Instance.GetShield() + 1);
    }
    public override void HeldEffect(Player p)
    {
        p.BubbleShields += Stack;
        p.TotalMaxShield += 1;
        //p.ImmunityFrameMultiplier += 0.05f * Stack;
        p.ShieldImmunityFrameMultiplier += 0.2f + 0.2f * Stack;
    }
}