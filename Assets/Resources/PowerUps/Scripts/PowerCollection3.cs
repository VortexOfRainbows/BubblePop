using UnityEngine;
public class BonusFizz : Shotgun
{
    public override void Init() =>  Weighting = Common;
}
public class BottleBlast : BubbleBlast
{
    public override void Init() => Weighting = Uncommon;
}
public class BottleFlip : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
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
        Weighting = SuperRare;
    }
    public override void HeldEffect(Player p)
    {
        p.DodgeStat += 0.1f * Stack + 0.1f;
    }
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<FizzyThinkImJustGonnaStandThereAndTakeIt>();
}
public class CarbonForce : ShotSpeed
{
    public override void Init()
    {
        Weighting = Uncommon;
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
    public override void HeldEffect(Player p)
    {
        p.ExplodingBubbles += Stack;
    }
}
public class SplashRadius : PowerUp
{
    public override void Init()
    {
        Weighting = Common;
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
    public override void HeldEffect(Player p)
    {
        p.Kickflip += Stack;
        p.SkateboardBonusDamage += Stack;
    }
}
public class RetaliatoryBlast : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void HeldEffect(Player p)
    {
        p.RetaliatoryBomb += Stack;
    }
}
public class BombasticBrew : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
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
    public override void HeldEffect(Player p)
    {
        p.SkullBomb += Stack;
    }
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<FizzyCoolGuys>();
}
public class ClusterBombs : PowerUp
{
    public override void Init()
    {
        Weighting = Legendary;
    }
    public override void HeldEffect(Player p)
    {
        p.ClusterBomb += Stack;
    }
}
public class SharpBombs : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void HeldEffect(Player p)
    {
        p.BonusShrapnel += 0.5f * Stack;
    }
}
public class PrizeBombs : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void HeldEffect(Player p)
    {
        p.PrizeBombCoins += 10 * Stack;
    }
}
public class Restock : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void OnPickup(int count)
    {
        if (GachaponShop.AllShops.Count > 0)
            foreach (GachaponShop shop in GachaponShop.AllShops)
            {
                shop.RestockRemaining += 1 * count;
                shop.RestockMachine.UpdateUI(shop);
            }
    }
    public override void HeldEffect(Player p)
    {
        p.BonusStocks += Stack * 1;
        p.BonusRestockChance += 1f * Stack + 0.01f;
    }
}
public class CrystalSerum : PowerUp
{
    public override void Init()
    {
        Weighting = Legendary;
    }
    public override void HeldEffect(Player p)
    {
        p.GemDropChance += 0.2f;
        p.BonusGems += Stack;
    }
}
public class LightningInABottle : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void HeldEffect(Player p)
    {
        p.InstakillsOnWaveStart += 3 * Stack;
    }
}
public class SnowLeopardTale : PowerUp
{
    public override void Init()
    {
        Weighting = SuperRare;
    }
    public override void HeldEffect(Player p)
    {
        p.ChillDuration += 3 * Stack;
    }
    public override bool IsBlackMarket()
    {
        return true;
    }
    public override UnlockCondition BlackMarketVariantUnlockCondition => UnlockCondition.Get<SlowThingsDownALittle>();
}
public class Gladiator : PowerUp
{
    public override void Init()
    {
        Weighting = Uncommon;
    }
    public override void OnPickup(int count)
    {
        foreach (Player player in Player.AllPlayers)
            player.SetShield(player.GetShield() + count);
    }
    public override void HeldEffect(Player p)
    {
        p.GladiatorDuration = 8 + Stack * 2;
    }
    public override bool IsBlackMarket()
    {
        return true;
    }
    public override Sprite GetTexture()
    {
        return Resources.Load<Sprite>("PowerUps/KurtPower");
    }
}