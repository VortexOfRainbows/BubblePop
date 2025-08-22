using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    /* 
     * CardData ought to contain
     * 1. Information on the enemy that it adds to the pool
     * 2. Information on other effects if the enemy is already added
     * 3. Information on the rewards the card provides
     */
    public float AvailablePoints;
    public float SpentPoints = 0;
    public EnemyClause Enemies;
    public ModifierClause Modifiers;
    public RewardClause Rewards;
    public void GetPointsAllowed()
    {
        AvailablePoints = 100; //This should be tied to the wave number in some way
    }
    public void Generate()
    {
        GetPointsAllowed();
        Generate<EnemyClause>(AvailablePoints);
        Generate<ModifierClause>(AvailablePoints);
        Generate<RewardClause>(SpentPoints);
    }
    public T Generate<T>(float points) where T : CardClause, new()
    {
        T c = CardClause.Generate<T>(points);
        if (c is EnemyClause e)
            Enemies = e;
        else if (c is ModifierClause m)
            Modifiers = m;
        if (c is RewardClause r)
        {
            Rewards = r;
            SpentPoints = c.Points;
        }
        else
        {
            SpentPoints += AvailablePoints - c.Points;
            AvailablePoints = c.Points;
        }
        return c;
    }
}
public abstract class CardClause
{
    /// <summary>
    /// Enemy cards cost points to spawn
    /// Modifier cards cost points to add
    /// Based on the amount of points spent, reward cards are added
    /// </summary>
    public float Points { get; set; }
    public static T Generate<T>(float AvailablePoints) where T : CardClause, new()
    {
        T clause = new();
        clause.Points = AvailablePoints;
        clause.GenerateData();
        return clause;
    }
    public override string ToString()
    {
        return "";
    }
    public abstract void GenerateData();
}
public class EnemyClause : CardClause
{
    public EnemyPoolAddition Enemy;
    public bool EnemyAlreadyInPool(EnemyPoolAddition p)
    {
        return false; //temp
    }
    public override void GenerateData()
    {
        Enemy = GenRandomEnemyPoolAddition();
        if (!EnemyAlreadyInPool(Enemy))
            Points -= Enemy.GetCost();
    }
    public EnemyPoolAddition GenRandomEnemyPoolAddition()
    {
        return new EnemyPoolAddition(EnemyID.OldDuck.GetComponent<Enemy>());
    }
}
public class ModifierClause : CardClause
{
    public List<DirectorModifier> Modifiers = new();
    public int ModifiersToAllow = 1;
    public override void GenerateData()
    {
        ModifiersToAllow = 1 + (int)(Points / 20f);
        if (ModifiersToAllow > 5)
            ModifiersToAllow = 5;
        float percent = 1f / ModifiersToAllow;
        float originalPoints = Points;
        while (Points > 0)
        {
            float spendingRange = Mathf.Min(Utils.RandFloat(percent, 1), Utils.RandFloat(percent, 1));
            DirectorModifier m = GenRandomModifier(spendingRange * originalPoints);
            Points -= m.GetCost();
            Modifiers.Add(m);
        }
    }
    public DirectorModifier GenRandomModifier(float pointsSpent)
    {
        List<DirectorModifier> PossibleModifiers = new()
        {
            new EnemyStrengthModifier(),
            new DirectorCreditModifier(),
        };
        if (Points > 10)
        {
            PossibleModifiers.Add(new DirectorInitialWaveBonusModifier());
            PossibleModifiers.Add(new DirectorCardCooldownModifier());
        }
        var chosen = PossibleModifiers[Utils.RandInt(PossibleModifiers.Count)];
        chosen.ApplicationStrength = pointsSpent;
        return chosen;
    }
}
public class RewardClause : CardClause
{
    public List<Reward> Rewards = new();
    public override void GenerateData()
    {
        while (Points > 0)
        {
            Reward r = GenRandomReward();
            Points -= r.GetCost();
            Rewards.Add(r);
        }
    }
    private Reward GenRandomReward()
    {
        float PointsAvailable = Points;
        Reward reward = null;
        if(Utils.RandFloat(1) < 0.5f)
        {
            reward = new CoinReward((int)PointsAvailable);
        }
        else
        {
            reward = new PowerReward(PowerUp.RandomFromPool(0, -1));
            while(reward.GetCost() > PointsAvailable)
            {
                reward = new PowerReward(PowerUp.RandomFromPool(0, -1));
                PointsAvailable += 5;
            }
        }
        return reward;
    }
}