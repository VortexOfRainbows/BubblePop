using System.Collections;
using System.Collections.Generic;
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
    public virtual float GetCost()
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
}
public class EnemyPoolAddition : ClauseEffect
{
    public float PermanentMultiplier => 0.5f;
    public bool IsPermanent { get; set; } = false;
    public Enemy EnemyToAdd;
    public EnemyPoolAddition(Enemy prefabToAdd)
    {
        EnemyToAdd = prefabToAdd;
    }
    public override void Apply()
    {
        //Unimplemented right now
    }
    public override float GetCost()
    {
        return EnemyToAdd.CostMultiplier * 10 * (IsPermanent ? PermanentMultiplier : 1);
    }
    public override string Description()
    {
        return $"{"New enemy:".WithSizeAndColor(30, DetailedDescription.LesserGray)} {RedText(EnemyToAdd.Name())}";
    }
}
public abstract class DirectorModifier : ClauseEffect
{
    public float ApplicationStrength { get; set; }
    public virtual float PointToPercentRatio => 100f;
    public bool IsPermanent { get; set; } = false;
    public virtual float PermanentMultiplier => 0.1f;
    public float Percent => ApplicationStrength / PointToPercentRatio * (IsPermanent ? PermanentMultiplier : 1);
    public string PercentAsText => $"+{Percent * 100:#.#}%";
    public string NumberText => $"+{(int)(Percent * 100)}";
    public override float GetCost()
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
    public override string Description()
    {
        return $"{"Enemy health:".WithSizeAndColor(30, DetailedDescription.LesserGray)} {RedText(PercentAsText)}";
    }
}
public class DirectorCreditModifier : DirectorModifier
{
    public override string Description()
    {
        return $"{"Enemy spawns:".WithSizeAndColor(30, DetailedDescription.LesserGray)} {RedText(PercentAsText)}";
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
    public override string Description()
    {
        return $"Enemy spawns: {RedText(PercentAsText)}";
    }
}
public class DirectorInitialWaveBonusModifier : DirectorModifier
{
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
public abstract class Reward : ClauseEffect
{
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
    public override float GetCost()
    {
        return PowerUp.Get(PowerType).Cost * (BeforeWaveEndReward ? 2 : 1);
    }
    public override void GrantReward()
    {
        PowerUp.Get(PowerType).PickUp(Amt);
    }
    public override string Description()
    {
        return $"{PowerUp.Get(PowerType).UnlockedName} x{Amt}";
    }
}
public class ChestReward : Reward
{
    //Currently unimplemented
}
public class CoinReward : Reward
{
    public CoinReward(int value)
    {
        coins = value;
    }
    public int coins;
    public override float GetCost()
    {
        return coins * (BeforeWaveEndReward ? 2 : 1);
    }
    public override void GrantReward()
    {
        CoinManager.SpawnCoin(Player.Instance.transform.position, coins, 0.5f);
    }
    public override string Description()
    {
        return $"{DetailedDescription.TextBoundedByColor(DetailedDescription.Yellow, $"{coins} coins")}";
    }
}
