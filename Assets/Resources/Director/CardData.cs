using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    public CardData(ModifierCard owner)
    {
        Owner = owner;
    }
    /* 
     * CardData ought to contain
     * 1. Information on the enemy that it adds to the pool
     * 2. Information on other effects if the enemy is already added
     * 3. Information on the rewards the card provides
     */
    public ModifierCard Owner;
    public float AvailablePoints;
    public float SpentPoints = 0;
    public EnemyClause EnemyClause;
    public ModifierClause ModifierClause;
    public RewardClause RewardClause;
    public void GetPointsAllowed()
    {
        AvailablePoints = 20 * Owner.DifficultyMultiplier; //This should be tied to the wave number in some way
    }
    public void Generate()
    {
        GetPointsAllowed();
        RegisterClause(new EnemyClause(AvailablePoints));
        RegisterClause(new ModifierClause(AvailablePoints));
        RegisterClause(new RewardClause(SpentPoints));
    }
    public void RegisterClause(CardClause c)
    {
        if (c is EnemyClause e)
            EnemyClause = e;
        else if (c is ModifierClause m)
            ModifierClause = m;
        if (c is RewardClause r)
        {
            RewardClause = r;
            SpentPoints = c.Points;
        }
        else
        {
            SpentPoints += AvailablePoints - c.Points;
            AvailablePoints = c.Points;
        }
    }
    public string CardName()
    {
        return EnemyClause.Enemy.EnemyToAdd.Name() + " Wave";
    }
}
public abstract class CardClause
{
    /// <summary>
    /// Enemy cards cost points to spawn
    /// Modifier cards cost points to add
    /// Based on the amount of points spent, reward cards are added
    /// </summary>
    public CardClause(float points)
    {
        Points = points;
        GenerateData();
    }
    public float Points { get; set; }
    public override string ToString()
    {
        return "";
    }
    public abstract void GenerateData();
}
public class EnemyClause : CardClause
{
    public EnemyPoolAddition Enemy;
    public EnemyClause(float points) : base(points)
    {

    }
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
        EnemyPoolAddition newEnemy = null;
        while(newEnemy == null)
        {
            Enemy e = EnemyID.AllEnemyList[Utils.RandInt(EnemyID.Max)].GetComponent<Enemy>();
            newEnemy = new(e);
            if(newEnemy.GetCost() > Points)
                newEnemy = null;
        }
        if(Points > newEnemy.GetCost() / newEnemy.PermanentMultiplier) //If I can afford it at the current price, make it permanent
        {
            newEnemy.IsPermanent = true;
        }
        return newEnemy;
    }
}
public class ModifierClause : CardClause
{
    private List<DirectorModifier> PossibleModifiers = new();
    public List<DirectorModifier> PermanentModifiers = new();
    public List<DirectorModifier> Modifiers = new();
    public int ModifiersToAllow = 1;
    public ModifierClause(float points) : base(points)
    {

    }
    protected void InitializePossibleModifiers()
    {
        if (Points > 10)
        {
            PossibleModifiers.Add(new EnemyStrengthModifier());
            PossibleModifiers.Add(new DirectorCreditModifier());
        }
        if (Points > 20)
        {
            PossibleModifiers.Add(new DirectorInitialWaveBonusModifier());
            //PossibleModifiers.Add(new DirectorCardCooldownModifier());
        }
    }
    public override void GenerateData()
    {
        InitializePossibleModifiers();
        ModifiersToAllow = 1 + (int)(Points / 50f);
        if (ModifiersToAllow > 5)
            ModifiersToAllow = 5;
        float percent = 1f / ModifiersToAllow;
        float originalPoints = Points;
        while (Points >= 1 && PossibleModifiers.Count > 0)
        {
            float spendingRange = (Mathf.Min(Utils.RandInt(ModifiersToAllow), Utils.RandInt(ModifiersToAllow)) + 1) * percent;
            DirectorModifier m = GenRandomModifier(spendingRange * originalPoints);
            Points -= m.GetCost();
            if (m.IsPermanent)
                PermanentModifiers.Add(m);
            else
                Modifiers.Add(m);
        }
    }
    public DirectorModifier GenRandomModifier(float pointsSpent)
    {
        int i = Utils.RandInt(PossibleModifiers.Count);
        var chosen = PossibleModifiers[i];
        chosen.ApplicationStrength = pointsSpent;
        PossibleModifiers.RemoveAt(i);
        if(pointsSpent > 10)
        {
            float chanceForPermanent = 0.8f * Mathf.Clamp(pointsSpent / 100f, 0, 1);
            if(chanceForPermanent > Utils.RandFloat())
            {
                chosen.IsPermanent = true;
            }
        }
        return chosen;
    }
}
public class RewardClause : CardClause
{
    public List<Reward> PreRewards = new();
    public List<Reward> PostRewards = new();
    public RewardClause(float points) : base(points)
    {

    }
    public void AddPowerReward(PowerReward r, List<Reward> list)
    {
        bool cloneExists = false;
        Reward SameType = list.Find(g => g is PowerReward g2 && g2.PowerType == r.PowerType);
        if (SameType != null && SameType is PowerReward r3)
        {
            r3.Amt++;
            cloneExists = true;
        }
        if (!cloneExists)
            list.Add(r);
    }
    public void AddCashReward(CoinReward r, List<Reward> list)
    {
        bool cloneExists = false;
        Reward SameType = list.Find(g => g is CoinReward g2);
        if (SameType != null && SameType is CoinReward g2)
        {
            g2.coins += r.coins;
            cloneExists = true;
        }
        if (!cloneExists)
            list.Add(r);
    }
    public override void GenerateData()
    {
        while (Points >= 1)
        {
            Reward r = GenRandomReward();
            Points -= r.GetCost();
            if (r.BeforeWaveEndReward)
            {
                if (r is PowerReward p)
                    AddPowerReward(p, PreRewards);
                else if (r is CoinReward c)
                    AddCashReward(c, PostRewards);
            }
            else
            {
                if (r is PowerReward p)
                    AddPowerReward(p, PostRewards);
                else if(r is CoinReward c)
                    AddCashReward(c, PostRewards);
            }
        }
    }
    private Reward GenRandomReward()
    {
        float PointsAvailable = Points;
        bool beforeWaveReward = Utils.RandFloat(1) > 0.5f;
        Reward reward = null;
        if(Utils.RandFloat(1) < 0.5f || Points < 10)
        {
            reward = new CoinReward(beforeWaveReward ? (int)(PointsAvailable / 2f + 0.4f) : (int)(PointsAvailable + 0.4f));
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
        reward.BeforeWaveEndReward = beforeWaveReward;
        return reward;
    }
}