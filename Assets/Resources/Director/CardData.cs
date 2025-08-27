using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEditor.Build;
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
    public static int UpcomingWave => WaveDirector.WaveNum + 1;
    //private CardClause[] Clauses => new CardClause[] { EnemyClause, ModifierClause, RewardClause };
    public void GetPointsAllowed()
    {
        AvailablePoints = 10 + (5 + UpcomingWave * 5) * DifficultyMult; //This should be tied to the wave number in some way
    }
    public void Generate()
    {
        GetPointsAllowed();
        float originalAvailablePoints = AvailablePoints;
        AddClauses(out EnemyClause e, out ModifierClause m, out RewardClause r);
        e ??= new EnemyClause(AvailablePoints);
        m ??= new ModifierClause(AvailablePoints - 20);
        r ??= new RewardClause(originalAvailablePoints);
        RegisterClause(e);
        RegisterClause(m);
        RegisterClause(r);
    }
    public void AddClauses(out EnemyClause e, out ModifierClause m, out RewardClause r)
    {
        int difficultyNum = (int)DifficultyMult;
        bool MinDifficultyCard = difficultyNum == 1;
        bool MidDifficulty = difficultyNum == 2;
        bool MaxDifficultyCard = difficultyNum == 3;
        e = null;
        m = null;
        r = null;
        int waveNum = UpcomingWave;
        if(waveNum == 1)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.OldDuck) { IsPermanent = true });
        }
        if(waveNum == 3)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.OldSoap));
        }
        if (waveNum == 4)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(MinDifficultyCard ? EnemyID.Chicken : MidDifficulty ? EnemyID.Crow : EnemyID.OldFlamingo) { IsPermanent = true });
        }
        if (waveNum == 5)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.Crow));
        }
        if(waveNum == 7)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(MinDifficultyCard ? EnemyID.Crow : EnemyID.OldFlamingo));
        }
        if (waveNum == 8)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(MaxDifficultyCard ? EnemyID.Gatligator : EnemyID.OldFlamingo));
        }
        if (waveNum == 9)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(!MaxDifficultyCard ? EnemyID.Gatligator : EnemyID.OldLeonard));
        }
        if (waveNum == 10)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.OldLeonard) { IsPermanent = !MinDifficultyCard });
        }
        if (waveNum >= 15 && waveNum % 5 == 0)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.OldLeonard) { IsPermanent = true });
            float strength = waveNum >= 25 ? 200 : 50;
            m = new ModifierClause(AvailablePoints, 1, new DirectorAmbushBonusModifier() { ApplicationStrength = strength, IsPermanent = waveNum >= 25 }, new DirectorSkullWaveModifier() { ApplicationStrength = 50 * difficultyNum});
        }
        if(waveNum >= 5 && m == null)
        {
            if (waveNum % 3 == 0) //Previously, there was a 10% power boost with each wave number, so this should mimic the old system
                m = new ModifierClause(AvailablePoints + 300, 1, new DirectorCreditPowerModifier() { ApplicationStrength = 300, IsPermanent = true });
            else if (waveNum % 3 == 1) //Previously, there was a 5% health boost with each wave number, so this should mimic the old system (with some changed scaling)
            {
                float strength = 10 + 5 * (int)(waveNum / 5);
                m = new ModifierClause(AvailablePoints + strength * 10, 1, new EnemyStrengthModifier() { ApplicationStrength = strength * 10, IsPermanent = true });
            }
            else if (waveNum % 3 == 2 && waveNum >= 17)
            {
                m = new ModifierClause(AvailablePoints + 50, 1, new DirectorSkullWaveModifier() { ApplicationStrength = 50, IsPermanent = true});
            }
        }
    }
    public void RegisterClause(CardClause c)
    {
        //AvailablePoints = c.Points;
        c.Owner = this;
        if (c is EnemyClause e)
        {
            EnemyClause = e;
        }
        else if (c is ModifierClause m)
            ModifierClause = m;
        if (c is RewardClause r)
            RewardClause = r;
    }
    public string CardName()
    {
        return EnemyClause.Enemy.EnemyToAdd.Name() + " Wave";
    }
    public void ApplyPermanentModifiers()
    {
        ModifierClause.ApplyPermanent();
    }
    public void ApplyTemporaryModifiers()
    {
        ModifierClause.ApplyTemporary();
    }
    public void ApplyEnemies()
    {
        EnemyClause.Apply();
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
    public CardData Owner { get; set; }
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
    public readonly List<WaveCard> AssociatedWaveCards = new();
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
        if (!Enemy.IsPermanent && Points > Enemy.GetCost() * Enemy.PermanentMultiplier) //If I can afford it at the current price, make it permanent
        {
            Enemy.IsPermanent = true;
        }
        if (!EnemyAlreadyInPool(Enemy))
            Points -= Enemy.GetCost();
        else
        {
            AlreadyInPool = true;
            //Increase difficulty if already in pool instead
        }
    }
    public EnemyCard GenRandomEnemyPoolAddition()
    {
        EnemyCard newEnemy = null;
        while (newEnemy == null)
        {
            Enemy e = EnemyID.AllEnemyList[Utils.RandInt(EnemyID.Max)].GetComponent<Enemy>();
            newEnemy = new(e);
            if (newEnemy.GetCost() > Points)
                newEnemy = null;
        }
        return newEnemy;
    }
    public void Apply()
    {
        PrepareWaveCards();
        WaveDirector.AssociatedWaveCards = AssociatedWaveCards;
        WaveMeter.Instance.SetTicks(AssociatedWaveCards.Count);
        Enemy.Apply();
    }
    public void Resolve() => Enemy.Resolve();
    public void PrepareWaveCards()
    {
        AssociatedWaveCards.Clear();
        int maxSwarmDifficulty = 4;
        float difficultMult = 1 + Owner.DifficultyMult + WaveDirector.TemporaryModifiers.BonusSkullWaves; //2 mid-waves by default
        if (Enemy.EnemyToAdd is EnemyBossDuck || Enemy.EnemyToAdd is Gatligator) //1 mid-wave by default for bosses, 3 at max card difficulty
        {
            difficultMult -= 1;
            if (Enemy.EnemyToAdd is EnemyBossDuck)
                maxSwarmDifficulty -= 3;
            else
                maxSwarmDifficulty -= 1;
        }
        for (int i = 0; i < difficultMult; ++i)
        {
            var card = WaveDeck.DrawSingleSpawn(WaveDeck.RandomPositionOnPlayerEdge(), Enemy.EnemyToAdd.gameObject, 0, 0.5f, 0);
            card.Patterns[0].Skull = true;
            float chance = i * (0.05f * difficultMult * WaveDirector.WaveNum);
            if (i > 1 && chance > Utils.RandFloat()) {
                card.ToSwarmCircle(Mathf.Min(maxSwarmDifficulty, 1 + i), 10, 0, 0.5f);
            }
            AssociatedWaveCards.Add(card);
        }
    }
}
public class ModifierClause : CardClause
{
    private readonly List<DirectorModifier> PossibleModifiers = new();
    public List<DirectorModifier> PermanentModifiers = new();
    public List<DirectorModifier> TemporaryModifiers = new();
    public int ModifiersToAllow = 1;
    public ModifierClause(float points, int modifiersAllowed = 1, params DirectorModifier[] mods) : base(points)
    {
        InitializePossibleModifiers();
        ModifiersToAllow = modifiersAllowed;
        foreach (DirectorModifier modifier in mods)
        {
            if (modifier.IsPermanent)
                PermanentModifiers.Add(modifier);
            else
                TemporaryModifiers.Add(modifier);
            for(int i = PossibleModifiers.Count - 1; i >= 0; --i)
                if (PossibleModifiers[i].GetType() == modifier.GetType())
                    PossibleModifiers.RemoveAt(i);
            Points -= modifier.GetCost();
        }
        GenerateData();
    }
    protected void InitializePossibleModifiers()
    {
        if (Points >= 10)
        {
            PossibleModifiers.Add(new EnemyStrengthModifier());
            PossibleModifiers.Add(new DirectorCreditPowerModifier());
        }
        if (Points >= 20)
            PossibleModifiers.Add(new DirectorAmbushBonusModifier());
        if (Points >= 50)
            PossibleModifiers.Add(new DirectorSkullWaveModifier());
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
            if (m == null)
                return;
            Points -= m.GetCost();
            if (m.IsPermanent)
                PermanentModifiers.Add(m);
            else
                TemporaryModifiers.Add(m);
        }
    }
    public DirectorModifier GenRandomModifier(float pointsSpent)
    {
        if(PossibleModifiers.Count <= 0)
        {
            return null;
        }
        int i = Utils.RandInt(PossibleModifiers.Count);
        var chosen = PossibleModifiers[i];
        chosen.ApplicationStrength = pointsSpent;
        PossibleModifiers.RemoveAt(i);
        if(!chosen.CanBeApplied)
            return GenRandomModifier(pointsSpent);
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