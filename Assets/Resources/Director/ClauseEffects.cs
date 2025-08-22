using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseEffect
{
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
}
public class EnemyPoolAddition : ClauseEffect
{
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
        return EnemyToAdd.CostMultiplier * 10;
    }
}
public abstract class DirectorModifier : ClauseEffect
{
    public float ApplicationStrength { get; set; }
    public override float GetCost()
    {
        return ApplicationStrength;
    }
}
public class EnemyStrengthModifier : DirectorModifier
{
    
}
public class DirectorCreditModifier : DirectorModifier
{

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
public class DirectorInitialWaveBonusModifier : DirectorModifier
{

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
    public override float GetCost()
    {
        return PowerUp.Get(PowerType).Cost * (BeforeWaveEndReward ? 2 : 1);
    }
    public override void GrantReward()
    {
        PowerUp.Get(PowerType).PickUp(1);
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
}
