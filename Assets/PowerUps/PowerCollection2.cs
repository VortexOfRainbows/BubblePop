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
        description.WithName("Roll For Charisma");
        description.WithDescription("Y:20% G:(+1% per stack) Y:chance to Y:heal after Y:[purchasing a power] or gain Y:25 G:(+25 per stack) Y:coins if uninjured");
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
        description.WithDescription("Y:20% G:(+1% per stack) Y:chance to gain Y:15% increased movement speed after using Y:[your ability,] stacking up to Y:2 G:(+1 per stack) times and lasting Y:[5 seconds]");
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
        description.WithDescription("Y:20% G:(+1% per stack) Y:chance to deal Y:200% G:(+20% per stack) Y:[bonus damage] on Y:[first strike]");
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
        description.WithShortDescription("Gain a shield on even-numbered waves\nWhen shields are broken, extend immunity frames and release bubbles");
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
public class ZapRadius : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases Y:[thunder bubble radius] by Y:10% G:(+10% per stack)");
        description.WithShortDescription("Increases thunder bubble radius");
    }
    public override void HeldEffect(Player p)
    {
        p.ZapRadiusMult += 0.10f * Stack;
    }
}
public class Electroluminescence : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Y:[Thunder bubbles] fire Y:1 G:(+1 per stack) Y:[beams of light] at enemies within Y:3 G:(+0.5 per stack) Y:units of the Y:[thunder bubble radius] for Y:[2 damage]");
        description.WithShortDescription("Thunder bubbles fire beams of light at nearby enemies");
    }
    public override void HeldEffect(Player p)
    {
        p.Electroluminescence += Stack;
    }
}
public class Burger : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases Y:[attack speed, movement speed, and damage] by Y:10% G:(+10% per stack)");
        description.WithShortDescription("Burger!?");
    }
    public override void HeldEffect(Player p)
    {
        p.AttackSpeedModifier += Stack * 0.1f;
        p.MoveSpeedMod += Mathf.Sqrt(Stack) * 0.1f;
        p.DamageMultiplier += Stack * 0.1f;
    }
    public override bool IsBlackMarket()
    {
        return true;
    }
    public override int Cost => 30;
}
public class BonusBatteries : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription("Increases the number of Y:[thunder bubbles] you can cast by Y:1 G:(+1 per stack)");
        description.WithShortDescription("Increases the number of thunder bubbles you can cast");
    }
    public override void HeldEffect(Player p)
    {
        p.AllowedThunderBalls += Stack;
    }
}
public class ResearchNotes : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Y:[After killing 3 Skull enemies,] become a <color={DetailedDescription.Rares[0]}>Choice</color> with 5 options G:(consumed on use)");
        description.WithShortDescription("After killing 3 Skull enemies, become a Choice with 5 options");
    }
    public override void HeldEffect(Player p)
    {
        p.HasResearchNotes = true;
        while (p.ResearchNoteKillCounter >= 3)
        {
            p.ResearchNoteBonuses++;
            if (Stack > 0 && !PowerUp.PickingPowerUps)
            {
                p.RemovePower(Type);
                PowerUp.Get<Choice>().PickUp(); //Choice is ID 0
            }
            p.ResearchNoteKillCounter -= 3;
        }
    }
    public override int Cost => 25;
}
public class ResearchGrants : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Y:[Skull enemies] drop Y:3 G:(+2 per stack) additional Y:coins");
        description.WithShortDescription("Skull enemies drop additional coins");
    }
    public override void HeldEffect(Player p)
    {
        p.FlatSkullCoinBonus += 1 + 2 * Stack;
    }
}
public class Boomerang : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Electric Boomerang");
        description.WithDescription($"Increases Y:[thunder bubble] recall damage by Y:50% G:(+50% per stack)");
        description.WithShortDescription("Increases thunder bubble recall damage");
    }
    public override void HeldEffect(Player p)
    {
        p.ThunderBubbleReturnDamageBonus += Stack * 1f;
    }
}
public class ThunderBubbles : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Echo Bubbles");
        description.WithDescription($"When recalled, Y:[thunder bubbles] leave behind a Y:[latent charge] that dissipates into Y:3 G:(+1 per stack) bubbles after Y:[thunder bubbles] are fully recalled");
        description.WithShortDescription("Thunder bubbles release bubbles when recalled");
    }
    public override void HeldEffect(Player p)
    {
        p.EchoBubbles += 2 + Stack;
    }
}
public class Supernova : PowerUp
{
    public override void Init()
    {
        Weighting = Legendary;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Stars have a Y:[20% chance] to Y:detonate on hit for Y:10 G:(+5 per stack) Y:damage " +
            $"\nActivates <color=#BAE3FE>Starbarbs</color> and <color=#BAE3FE>Lucky Star</color>");
        description.WithShortDescription("Stars have a chance to explode");
    }
    public override void HeldEffect(Player p)
    {
        p.Supernova += Stack;
    }
}
public class ResonanceRuby : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Increases the Y:quality of Y:[wave rewards] by Y:20% G:(+20% per stack) \n" +
            $"<color={DetailedDescription.Yellow}>Wave end</color> gains an additional Y:10% G:(+10% per stack) Y:chance to contain a Y:[bonus power reward]");
        description.WithShortDescription("Improves wave rewards");
    }
    public override void HeldEffect(Player p)
    {
        p.Ruby += Stack;
    }
}
public class DoubleDown : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Enemies Y:[killed by chips] drop additional Y:coins equal to the Y:[overkill damage dealt] up to a Y:[max of 5 coins] G:(+3 per stack) \nIncreases Y:[chip damage] by Y:1 G:(+1 per stack)");
        description.WithShortDescription("Enemies killed by chips drop additional coins and increases chip damage");
    }
    public override void HeldEffect(Player p)
    {
        p.DoubleDownChip += Stack;
    }
}
public class FocusFizz : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Increases Y:[critical strike chance] by Y:5% G:(+5% per stack)");
        description.WithShortDescription("Increases critical strike chance");
    }
    public override void HeldEffect(Player p)
    {
        p.CriticalStrikeChance += 0.05f * Stack;
    }
}
public class Coupons : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithDescription($"Reduces the cost of Y:powers in the Y:shop by Y:10% G:(+10% per stack)");
        description.WithShortDescription("Reduces the cost of powers in the shop");
    }
    public override void OnPickup(int count)
    {
        if(GachaponShop.Instance != null && GachaponShop.Instance.Stock != null)
        {
            foreach (PowerUpObject p in GachaponShop.Instance.Stock)
            {
                p.Cost = (int)(p.Cost - p.MyPower.Cost * 0.1f);
            }
        }
    }
    public override void HeldEffect(Player p)
    {
        p.ShopDiscount += 0.1f * Stack;
    }
}