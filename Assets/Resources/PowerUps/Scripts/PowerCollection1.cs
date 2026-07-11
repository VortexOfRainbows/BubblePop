using UnityEngine;

public class Choice : PowerUp
{
    public override void HeldEffect(Player p)
    {
        if (Stack > 0 && !PowerUp.PickingPowerUps)
        {
            p.RemovePower(Type);
            PowerUp.TurnOnPowerUpSelectors();
        }
    }
    public override int Cost => 50;
    public override int CrucibleGems(bool dissolve = false)
    {
        return dissolve ? 10 : 25;
    }
    public override int ShardReplicationCost(int stackSize = 1)
    {
        return stackSize * 2;
    }
    public override bool BriefDescIsSameAsLong => true;
}
public class ChargeShot : PowerUp
{
    public override void HeldEffect(Player p)
    {
        p.ChargeShotDamage += Stack;
    }
}
public class Shotgun : PowerUp
{
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
    public override void ModifyDescription(ref PowerDescription description)
    {
        description.WithAlt<ThoughtBubble>().WithAlt<Gachapon>().WithAlt<Fizzy>();
    }
    public override void HeldEffect(Player p)
    {
        p.DashSparkle += Stack;
    }
}
public class ShotSpeed : PowerUp
{
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
    public override void HeldEffect(Player p)
    {
        p.Starshot += Stack;
        p.ShotgunPower += Stack;
    }
}
public class WeaponUpgrade : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void HeldEffect(Player p)
    {
        p.AttackSpeedModifier += Stack * 0.07f;
    }
    public override Sprite GetTexture()
    {
        return Resources.Load<Sprite>("PowerUps/Haste");
    }
}
public class BinaryStars : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
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
    public override void HeldEffect(Player p)
    {
        p.BubbleTrail += Stack;
    }
}
public class Coalescence : PowerUp
{
    public override void Init()
    {
        Weighting = Legendary;
    }
    public override void HeldEffect(Player p)
    {
        p.Coalescence += Stack;
        //p.SecondaryAttackSpeedModifier += 0.05f * Stack;
    }
}
public class LuckyStar : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void HeldEffect(Player p)
    {
        p.LuckyStar += Stack;
        p.LuckyStarItemsAllowedPerWave += 2 + Stack;
    }
}
public class TrailOfThoughts : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
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
    public override void ModifyDescription(ref PowerDescription description)
    {
        description.WithAlt<SlotMachineWeapon>(true, false);
    }
    public override void HeldEffect(Player p)
    {
        p.Magnet += Stack;
        p.BonusCoinFromWaveRewards += Stack * 5;
    }
}
public class SpearOfLight : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void HeldEffect(Player p)
    {
        p.LightSpear += Stack;
    }
}
public class Overclock : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void HeldEffect(Player p)
    {
        p.AbilityRecoverySpeed += 0.2f * Stack;
    }
}
public class NeuronActivation : PowerUp
{
    public override void Init()
    {
        Weighting = Legendary;
    }
    public override void HeldEffect(Player p)
    {
        p.LightChainReact = 1;
        p.LightMultiplierBonusDamage += 0.6f * Stack - 0.4f;
        //Stack;
    }
}
public class BrainBlast : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
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
    public override void HeldEffect(Player p)
    {
        p.HomingRange += Stack * 2f + 2f;
    }
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<ThoughtBubbleShortForCalc>();
}