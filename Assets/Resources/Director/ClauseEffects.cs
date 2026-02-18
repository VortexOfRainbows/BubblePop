using System;
using UnityEngine;
public abstract class ClauseEffect
{
    protected static string RedText(string s)
    {
        return DetailedDescription.TextBoundedByRarityColor(1, s, true);
    }
    /// <summary>
    /// Called when the wave starts
    /// </summary>
    public virtual void Apply()
    {

    }
    protected virtual float Cost()
    {
        return 0;
    }
    /// <summary>
    /// Called when the wave ends
    /// </summary>
    public virtual void Resolve()
    {

    }
    public virtual string Description()
    {
        return "";
    }
    public bool Free = false;
    public float GetCost()
    {
        return Free ? 0 : Cost();
    }
}
public class EnemyCard : ClauseEffect
{
    public WaveDirector.WaveModifiers MyModifier => IsPermanent ? WaveDirector.PermanentModifiers : WaveDirector.TemporaryModifiers;
    public float PermanentMultiplier => 1.75f;
    public bool IsPermanent { get; set; } = false;
    public Enemy EnemyToAdd;
    public EnemyCard(Enemy prefabToAdd)
    {
        EnemyToAdd = prefabToAdd;
    }
    public EnemyCard(GameObject prefabToAdd)
    {
        EnemyToAdd = prefabToAdd.GetComponent<Enemy>();
    }
    public override void Apply()
    {
        MyModifier.WaveSpecialBonusEnemy = EnemyToAdd.gameObject;
    }
    protected override float Cost()
    {
        return EnemyToAdd.CostMultiplier * 8 * (IsPermanent ? PermanentMultiplier : 1);
    }
    public override string Description()
    {
        return $"{"New enemy:".WithSizeAndColor(30, DetailedDescription.LesserGray)} {RedText(EnemyToAdd.Name())}";
    }
}
public abstract class DirectorModifier : ClauseEffect
{
    public virtual bool CanBeApplied => true;
    public WaveDirector.WaveModifiers MyModifier => IsPermanent ? WaveDirector.PermanentModifiers : WaveDirector.TemporaryModifiers;
    public float ApplicationStrength { get; set; }
    public virtual float PointToPercentRatio => 100f;
    public bool IsPermanent { get; set; } = false;
    public virtual float PermanentMultiplier => 0.1f;
    public float Percent => ApplicationStrength / PointToPercentRatio * (IsPermanent ? PermanentMultiplier : 1);
    public string PercentAsText => $"+{Percent * 100:#.#}%";
    public string NumberText => $"+{(int)Percent}";
    protected override float Cost()
    {
        return ApplicationStrength;
    }
    public override string Description()
    {
        return $"Increases director attribute by {RedText(PercentAsText)}";
    }
}
public class EnemyStrengthModifier : DirectorModifier
{
    public override float PointToPercentRatio => 200;
    public override float PermanentMultiplier => 0.2f;
    public override void Apply()
    {
        MyModifier.EnemyScaling += Percent;
    }
    public override string Description()
    {
        return $"{"Enemy Health:".WithSizeAndColor(30, DetailedDescription.LesserGray)} {RedText(PercentAsText)}";
    }
}
public class DirectorCreditPowerModifier : DirectorModifier
{
    public override void Apply()
    {
        MyModifier.CreditGatherScaling += Percent;
    }
    public override string Description()
    {
        return $"{"Wave Power:".WithSizeAndColor(30, DetailedDescription.LesserGray)} {RedText(PercentAsText)}";
    }
}
public class DirectorSwarmModifier : DirectorModifier
{

}
public class DirectorMultiPortalModifier : DirectorModifier
{

}
public class DirectorCardCooldownModifier : DirectorModifier
{

}
public class DirectorAmbushBonusModifier : DirectorModifier
{
    public override bool CanBeApplied => Percent >= 1;
    public override float PointToPercentRatio => 2f;
    public override void Apply()
    {
        MyModifier.InitialAmbush += Percent;
    }
    public override string Description()
    {
        return $"{"Initial Ambush:".WithSizeAndColor(30, DetailedDescription.LesserGray)} {RedText(NumberText)}";
    }
}
public class DirectorSwarmSpeedModifier : DirectorModifier
{

}
public class DirectorMultiPortalSpeedModifier : DirectorModifier
{

}
public class DirectorSkullWaveModifier : DirectorModifier
{
    public override float PermanentMultiplier => 1f;
    public override bool CanBeApplied => Percent >= 1;
    public override float PointToPercentRatio => 50;
    public override void Apply()
    {
        MyModifier.BonusSkullWaves += (int)Percent;
    }
    public override string Description()
    {
        return $"{"Skull Waves:".WithSizeAndColor(30, DetailedDescription.LesserGray)} {RedText(NumberText)}";
    }
}
public class DirectorSkullSwarmModifier : DirectorModifier
{
    public override float PermanentMultiplier => 1f;
    public EnemyCard Parent { get; private set; }
    public override float PointToPercentRatio => 1;
    public DirectorSkullSwarmModifier(EnemyCard parent)
    {
        Parent = parent;
    }
    protected override float Cost()
    {
        return 0;
    }
    public override void Apply()
    {
        Type enemyType = Parent.EnemyToAdd.GetType();
        if (MyModifier.BonusSkullSwarm.ContainsKey(enemyType))
            MyModifier.BonusSkullSwarm[enemyType] += (int)Percent;
        else
            MyModifier.BonusSkullSwarm[enemyType] = (int)Percent;
    }
    public override string Description()
    {
        if(IsPermanent)
            return $"{$"Skull Swarm (".WithSizeAndColor(28, DetailedDescription.LesserGray)}" +
            $"{Parent.EnemyToAdd.Name().WithSizeAndColor(28, DetailedDescription.Rares[5])}" +
            $"{"):".WithSizeAndColor(28, DetailedDescription.LesserGray)} {RedText(NumberText)}";
        else
            return $"{$"Skull Swarm".WithSizeAndColor(30, DetailedDescription.LesserGray)} {RedText(NumberText)}";

    }
}
public abstract class Reward : ClauseEffect
{
    public static Vector2 RewardPosition() => Main.PylonPositon + new Vector2(0, 1).RotatedBy(Utils.RandFloat(Mathf.PI / 4f, Mathf.PI * 7f / 4f)) * 5;
    public static Vector2 RewardPositionChest() => Main.PylonPositon + new Vector2(0, -1).RotatedBy(Utils.RandFloat(Mathf.PI / 4f, Mathf.PI * 7f / 4f)) * 7;
    public bool BeforeWaveEndReward = false;
    public sealed override void Apply()
    {
        if (BeforeWaveEndReward)
            GrantReward();
    }
    public sealed override void Resolve()
    {
        if (!BeforeWaveEndReward)
            GrantReward();
    }
    public virtual void GrantReward()
    {

    }
}
public class PowerReward : Reward
{
    public PowerReward(int powerType)
    {
        PowerType = powerType;
    }
    public int PowerType;
    public int Amt { get; set; } = 1;
    protected override float Cost()
    {
        return PowerUp.Get(PowerType).Cost * (BeforeWaveEndReward ? 1.5f : 1);
    }
    public override void GrantReward()
    {
        for(int i = 0; i < Amt; ++i)
            PowerUp.Spawn(PowerType, RewardPosition());
    }
    public override string Description()
    {
        return $"{PowerUp.Get(PowerType).UnlockedName} x{Amt}";
    }
}
public class ChestReward : Reward
{
    public int ChestQuantity = 1;
    public int ChestType = 1;
    public ChestReward(int value, int chestType = 0)
    {
        this.ChestType = chestType;
        coins = value;
    }
    public int coins;
    protected override float Cost()
    {
        return coins;
    }
    public override void GrantReward()
    {
        for(int i = 0; i < ChestQuantity; i++)
            CoinManager.SpawnChest(RewardPositionChest, ChestType);
    }
    public override string Description()
    {
        if(ChestType == 2)
            return $"{DetailedDescription.TextBoundedByColor(DetailedDescription.Rares[5], ChestQuantity > 1 ? $"{ChestQuantity} Gem Chests" : $"{ChestQuantity} Gem Chest")}";
        if (ChestType == 1)
            return $"{DetailedDescription.TextBoundedByColor(DetailedDescription.Rares[2], ChestQuantity > 1 ? $"{ChestQuantity} Blue Chests" : $"{ChestQuantity} Blue Chest")}";
        return $"{DetailedDescription.TextBoundedByColor(DetailedDescription.Rares[0], ChestQuantity > 1 ? $"{ChestQuantity} Chests" : $"{ChestQuantity} Chest")}";
    }
}
public class CoinReward : Reward
{
    public CoinReward(int value)
    {
        coins = value;
    }
    public int coins;
    protected override float Cost()
    {
        return coins * (BeforeWaveEndReward ? 1.5f : 1);
    }
    public override void GrantReward()
    {
        CoinManager.SpawnCoin(RewardPosition, coins, 0.5f);
    }
    public override string Description()
    {
        return $"{DetailedDescription.TextBoundedByColor(DetailedDescription.Yellow, coins > 1 ? $"{coins} coins" : $"{coins} coin")}";
    }
}
public class HealReward : Reward
{
    public HealReward(int value, int heals = 1)
    {
        this.heals = heals;
        coins = value;
    }
    public int coins;
    public int heals = 1;
    protected override float Cost()
    {
        return coins;
    }
    public override void GrantReward()
    {
        for(int i = 0; i < heals; ++i)
        {
            CoinManager.SpawnHeart(RewardPosition, 0.5f);
        }
    }
    public override string Description()
    {
        return $"{DetailedDescription.TextBoundedByColor(DetailedDescription.Rares[5], heals > 1 ? $"{heals} hearts" : $"{heals} heart")}";
    }
}
public class KeyReward : Reward
{
    public KeyReward(int value, int keys = 1)
    {
        this.keys = keys;
        coins = value;
    }
    public int coins;
    public int keys = 1;
    protected override float Cost()
    {
        return coins;
    }
    public override void GrantReward()
    {
        for (int i = 0; i < keys; ++i)
            CoinManager.SpawnKey(RewardPosition, 0.5f);
    }
    public override string Description()
    {
        return $"{DetailedDescription.TextBoundedByColor(DetailedDescription.LesserGray, keys > 1 ? $"{keys} keys" : $"{keys} key")}";
    }
}
public class TokenReward : Reward
{
    public TokenReward(int tokens = 1)
    {
        this.tokens = tokens;
    }
    public int tokens = 1;
    protected override float Cost()
    {
        return 0;
    }
    public override void GrantReward()
    {
        for (int i = 0; i < tokens; ++i)
            CoinManager.SpawnToken(RewardPosition, 0.5f);
    }
    public override string Description()
    {
        return $"{DetailedDescription.TextBoundedByColor(ColorHelper.TokenColor.ToHexString(), tokens > 1 ? $"{tokens} Tokens" : $"{tokens} Token")}";
    }
}
public class GemReward : Reward
{
    public GemReward(int value, int gems = 1)
    {
        this.gems = gems; 
        coins = value;
    }
    public int coins;
    public int gems = 1;
    protected override float Cost()
    {
        return coins;
    }
    public override void GrantReward()
    {
        for (int i = 0; i < gems; ++i)
            CoinManager.SpawnGem(RewardPosition, 0.5f);
    }
    public override string Description()
    {
        return $"{DetailedDescription.TextBoundedByColor(DetailedDescription.Rares[1], gems > 1 ? $"{gems} Gems" : $"{gems} Gem")}";
    }
}
