using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.Rendering;
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
    public EnemyClause EnemyClause;
    public ModifierClause ModifierClause;
    public RewardClause RewardClause;
    public float DifficultyMult => Owner.DifficultyMultiplier;
    //private CardClause[] Clauses => new CardClause[] { EnemyClause, ModifierClause, RewardClause };
    public void GetPointsAllowed()
    {
        AvailablePoints = (18 + WaveDirector.WaveNum * 2) * DifficultyMult; //This should be tied to the wave number in some way
    }
    public void Generate()
    {
        GetPointsAllowed();
        float originalAvailablePoints = AvailablePoints;
        AddClauses(out EnemyClause e, out ModifierClause m, out RewardClause r);
        e ??= new EnemyClause(AvailablePoints);
        RegisterClause(e);
        m ??= new ModifierClause(AvailablePoints);
        RegisterClause(m);
        r ??= new RewardClause(originalAvailablePoints);
        RegisterClause(r);
    }
    public void AddClauses(out EnemyClause e, out ModifierClause m, out RewardClause r)
    {
        e = null;
        m = null;
        r = null;
        int waveNum = WaveDirector.WaveNum;
        if(waveNum == 1)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.OldDuck) { IsPermanent = true });
            if(DifficultyMult == 1)
                m = new ModifierClause(AvailablePoints, 0);
        }
    }
    public void RegisterClause(CardClause c)
    {
        if (c is EnemyClause e)
            EnemyClause = e;
        else if (c is ModifierClause m)
            ModifierClause = m;
        if (c is RewardClause r)
            RewardClause = r;
        AvailablePoints = c.Points;
    }
    public string CardName()
    {
        return EnemyClause.Enemy.EnemyToAdd.Name() + " Wave";
    }
    public void ApplyPermanentModifiers()
    {
        if (EnemyClause.Enemy.IsPermanent)
            EnemyClause.Apply();
        ModifierClause.ApplyPermanent();
    }
    public void ApplyTemporaryModifiers()
    {
        if (!EnemyClause.Enemy.IsPermanent)
            EnemyClause.Apply();
        ModifierClause.ApplyTemporary();
    }
    public void GrantImmediateRewards()
    {
        RewardClause.Apply();
    }
    public void GrantCompletionRewards()
    {
        RewardClause.Resolve();
    }
    public void ResolveModifiers()
    {
        EnemyClause.Resolve();
        ModifierClause.ResolvePermanent();
        ModifierClause.ResolveTemporary();
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
    public EnemyCard Enemy;
    public bool AlreadyInPool = false;
    public EnemyClause(float points, EnemyCard newCard = null) : base(points)
    {
        Enemy = newCard;
        GenerateData();
    }
    private bool EnemyAlreadyInPool(EnemyCard p)
    {
        return WaveDirector.PossibleEnemies().Find(g => g.GetComponent<Enemy>().GetType() == p.EnemyToAdd.GetType()) != null;
    }
    public override void GenerateData()
    {
        Enemy ??= GenRandomEnemyPoolAddition();
        if (!EnemyAlreadyInPool(Enemy))
            Points -= Enemy.GetCost();
        else
            AlreadyInPool = true;
    }
    public EnemyCard GenRandomEnemyPoolAddition()
    {
        EnemyCard newEnemy = null;
        while(newEnemy == null)
        {
            Enemy e = EnemyID.AllEnemyList[Utils.RandInt(EnemyID.Max)].GetComponent<Enemy>();
            newEnemy = new(e);
            if(newEnemy.GetCost() > Points)
                newEnemy = null;
        }
        if(Points > newEnemy.GetCost() * newEnemy.PermanentMultiplier) //If I can afford it at the current price, make it permanent
        {
            newEnemy.IsPermanent = true;
        }
        return newEnemy;
    }
    public void Apply() => Enemy.Apply();
    public void Resolve() => Enemy.Resolve();
}
public class ModifierClause : CardClause
{
    private readonly List<DirectorModifier> PossibleModifiers = new();
    public List<DirectorModifier> PermanentModifiers = new();
    public List<DirectorModifier> TemporaryModifiers = new();
    public int ModifiersToAllow = 1;
    public ModifierClause(float points, int modifiersAllowed = 1, params DirectorModifier[] mods) : base(points)
    {
        InitializePossibleModifiers(); //Need to change this to not allow repeats with the manually inputted modifiers
        ModifiersToAllow = modifiersAllowed;
        foreach(DirectorModifier modifier in mods)
        {
            if (modifier.IsPermanent)
                PermanentModifiers.Add(modifier);
            else
                TemporaryModifiers.Add(modifier);
            Points -= modifier.GetCost();
        }
        GenerateData();
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
        if (ModifiersToAllow == 0)
            return;
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
                TemporaryModifiers.Add(m);
        }
    }
    public DirectorModifier GenRandomModifier(float pointsSpent)
    {
        int i = Utils.RandInt(PossibleModifiers.Count);
        var chosen = PossibleModifiers[i];
        chosen.ApplicationStrength = pointsSpent;
        PossibleModifiers.RemoveAt(i);
        //if(pointsSpent > 10)
        //{
        //    float chanceForPermanent = 0.8f * Mathf.Clamp(pointsSpent / 100f, 0, 1);
        //    if(chanceForPermanent > Utils.RandFloat())
        //    {
        //        chosen.IsPermanent = true;
        //    }
        //}
        return chosen;
    }
    public void ApplyPermanent()
    {
        foreach (DirectorModifier r in PermanentModifiers)
            r.Apply();
    }
    public void ApplyTemporary()
    {
        foreach (DirectorModifier r in TemporaryModifiers)
            r.Apply();
    }
    public void ResolvePermanent()
    {
        foreach (DirectorModifier r in PermanentModifiers)
            r.Resolve();
    }
    public void ResolveTemporary()
    {
        foreach (DirectorModifier r in TemporaryModifiers)
            r.Resolve();
    }
}
public class RewardClause : CardClause
{
    public List<Reward> PreRewards = new();
    public List<Reward> PostRewards = new();
    public int RewardsAllowed = -1;
    public int RewardsAdded = 0;
    public RewardClause(float points, int rewardsAllowed = -1, params Reward[] rewards) : base(points)
    {
        RewardsAllowed = rewardsAllowed;
        foreach(Reward r in rewards)
        {
            Points -= r.GetCost();
            var listType = r.BeforeWaveEndReward ? PreRewards : PostRewards;
            if (r is PowerReward p)
                AddPowerReward(p, listType);
            else if (r is CoinReward c)
                AddCashReward(c, listType);
        }
        GenerateData();
    }
    public void AddPowerReward(PowerReward r, List<Reward> list)
    {
        Reward SameType = list.Find(g => g is PowerReward g2 && g2.PowerType == r.PowerType);
        if (SameType != null && SameType is PowerReward r3)
        {
            r3.Amt++;
            ++RewardsAdded;
        }
        else
        {
            list.Add(r);
            ++RewardsAdded;
        }
    }
    public void AddCashReward(CoinReward r, List<Reward> list)
    {
        Reward SameType = list.Find(g => g is CoinReward g2);
        if (SameType != null && SameType is CoinReward g2)
        {
            g2.coins += r.coins;
        }
        else //clone does not exists
        {
            list.Add(r);
            ++RewardsAdded;
        }
    }
    public override void GenerateData()
    {
        while (Points >= 1 && (RewardsAdded < RewardsAllowed || RewardsAllowed == -1))
        {
            Reward r = GenRandomReward();
            Points -= r.GetCost();
            var listType = r.BeforeWaveEndReward ? PreRewards : PostRewards;
            if (r is PowerReward p)
                AddPowerReward(p, listType);
            else if (r is CoinReward c)
                AddCashReward(c, listType);
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
    public void Apply()
    {
        foreach(Reward r in PreRewards)
            r.Apply();
    }
    public void Resolve()
    {
        foreach (Reward r in PostRewards)
            r.Resolve();
    }
}