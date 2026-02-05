using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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
    public float DifficultyMult => Owner.DifficultyMultiplier + Player.Instance.PersonalWaveCardBonus;
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
        float bonusMult = Player.Instance.Ruby * 0.2f + 1f;
        float pointsAllocatedToPowers = originalAvailablePoints * bonusMult;
        r ??= new RewardClause(pointsAllocatedToPowers, -1, DifficultyMult);
        if(e.AlternativeModifier != null)
        {
            if (e.AlternativeModifier.IsPermanent)
                m.PermanentModifiers.Insert(0, e.AlternativeModifier);
            else
                m.TemporaryModifiers.Insert(0, e.AlternativeModifier);
        }
        RegisterClause(e);
        RegisterClause(m);
        RegisterClause(r);
    }
    public void AddClauses(out EnemyClause e, out ModifierClause m, out RewardClause r)
    {
        int difficultyNum = (int)Owner.DifficultyMultiplier;
        bool MinDifficultyCard = difficultyNum == 1;
        bool MidDifficulty = difficultyNum == 2;
        bool MaxDifficultyCard = difficultyNum == 3;
        e = null;
        m = null;
        r = null;
        int waveNum = UpcomingWave;
        if ((waveNum == 1 || waveNum == 2) && !WaveDirector.EnemyPool.Contains(EnemyID.RockSpider))
        {
            if (Utils.RandFloat(1) < 0.12f * waveNum * difficultyNum)
                e ??= new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.RockSpider) { IsPermanent = true });
        }
        if (waveNum == 1)
        {
            e ??= new EnemyClause(AvailablePoints, new EnemyCard(Utils.RandFloat(1) < 0.3f ? EnemyID.OldDuck : EnemyID.Chicken) { IsPermanent = true });
        }
        if(waveNum == 3)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.OldSoap));
        }
        if (waveNum == 4 || waveNum == 5)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(MinDifficultyCard ? (Utils.RandFloat(1) < 0.3f ? EnemyID.Chicken : EnemyID.OldDuck) : MidDifficulty ? EnemyID.Crow : EnemyID.OldFlamingo));
        }
        if (waveNum == 6)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(MaxDifficultyCard ? EnemyID.Crow : (Utils.RandFloat(1) < 0.7f ? EnemyID.Chicken : EnemyID.OldDuck)));
            m = new(AvailablePoints, 1, new DirectorSkullSwarmModifier(e.Enemy) { ApplicationStrength = MaxDifficultyCard ? 1 : 2, IsPermanent = false });
        }
        if (waveNum == 7)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(MinDifficultyCard ? EnemyID.Crow : EnemyID.OldFlamingo) { IsPermanent = MaxDifficultyCard });
        }
        if (waveNum == 8)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(MinDifficultyCard ? EnemyID.Ent : MaxDifficultyCard ? EnemyID.Gatligator : EnemyID.OldFlamingo));
        }
        if (waveNum == 9)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(Utils.RandFloat(1) < 0.15f ? EnemyID.Infector : MinDifficultyCard ? EnemyID.Ent : MaxDifficultyCard ? EnemyID.OldLeonard : EnemyID.Gatligator));
        }
        if (waveNum == 10)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.OldLeonard) { IsPermanent = false });
        }
        if(waveNum == 11)
        {
            if(Utils.RandFloat(1) < 0.5f)
                e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.Infector) { IsPermanent = false });
        }
        if (waveNum == 12)
        {
            e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.Ent) { IsPermanent = true });
            m = new(AvailablePoints, 1, new DirectorSkullSwarmModifier(e.Enemy) { ApplicationStrength = 2, IsPermanent = false });
        }
        if (waveNum == 13 || waveNum == 14)
        {
            if (Utils.RandFloat(1) < (waveNum == 13 ? 0.5f : 0.66f))
                e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.Infector) { IsPermanent = true });
        }
        if (waveNum >= 15 && waveNum % 5 == 0)
        {
            if(waveNum % 10 == 0 && Utils.RandFloat(1) < 0.5f)
                e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.Infector) { IsPermanent = true });
            else
                e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.OldLeonard) { IsPermanent = true });
            float strength = waveNum >= 25 ? 200 : 50;
            if(waveNum % 10 == 0)
                m = new ModifierClause(AvailablePoints, 1, new DirectorAmbushBonusModifier() { 
                    ApplicationStrength = strength, IsPermanent = waveNum >= 25 }, 
                    new DirectorSkullWaveModifier() { ApplicationStrength = 50 * difficultyNum },
                    new EnemyStrengthModifier() { ApplicationStrength = 1000, IsPermanent = true, Free = true });
            else
                m = new ModifierClause(AvailablePoints, 1, new DirectorAmbushBonusModifier() { ApplicationStrength = strength, IsPermanent = waveNum >= 25 }, new DirectorSkullWaveModifier() { ApplicationStrength = 50 * difficultyNum });
        }
        if (waveNum == 16 || waveNum == 17)
        {
            if (Utils.RandFloat(1) < (waveNum == 16 ? 0.25f : 0.75f))
                e = new EnemyClause(AvailablePoints, new EnemyCard(EnemyID.Sentinel) { IsPermanent = true });
        }
        if (waveNum >= 5 && m == null)
        {
            if (waveNum % 3 == 0) //Previously, there was a 10% power boost with each wave number, so this should mimic the old system
            {
                float strength = 240 + 20 * (int)(waveNum / 3);
                m = new ModifierClause(AvailablePoints, 1, new DirectorCreditPowerModifier() { ApplicationStrength = strength, IsPermanent = true, Free = true });
            }
            else if (waveNum % 3 == 1) //Previously, there was a 5% health boost with each wave number, so this should mimic the old system (with some changed scaling)
            {
                float strength = 10 + 5 * (int)(waveNum / 5);
                m = new ModifierClause(AvailablePoints, 1, new EnemyStrengthModifier() { ApplicationStrength = strength * 10, IsPermanent = true, Free = true });
            }
            else if (waveNum % 3 == 2 && waveNum >= 17)
            {
                m = new ModifierClause(AvailablePoints, 1, new DirectorSkullWaveModifier() { ApplicationStrength = 50, IsPermanent = true, Free = true });
            }
        }
    }
    public void RegisterClause(CardClause c)
    {
        //AvailablePoints = c.Points;
        c.Owner = this;
        if (c is EnemyClause e)
            EnemyClause = e;
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
    public DirectorSkullSwarmModifier AlternativeModifier = null;
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
            AlternativeModifier = new(Enemy)
            {
                ApplicationStrength = 1,
                IsPermanent = Enemy.IsPermanent && Points > Enemy.GetCost() * Enemy.PermanentMultiplier //In order for the alternative modifier to be permanent, it needs to pass the permanency cost check twice
            };
        }
    }
    public EnemyCard GenRandomEnemyPoolAddition()
    {
        EnemyCard newEnemy = null;
        while (newEnemy == null)
        {
            Enemy e = EnemyID.SpawnableEnemiesList[Utils.RandInt(EnemyID.MaxRandom)];
            if (WaveDirector.WaveNum < 5) //Temporary. Replace with special tiered getter later which does different stuff depending on wave number
            {
                while(e is Infector)
                    e = EnemyID.SpawnableEnemiesList[Utils.RandInt(EnemyID.MaxRandom)];
            }
            else if(WaveDirector.WaveNum > 10)
            {
                Enemy secondChoice = EnemyID.SpawnableEnemiesList[Utils.RandInt(EnemyID.MaxRandom)];
                if(WaveDirector.EnemyPool.Contains(e.gameObject))
                    e = secondChoice;
                else if (secondChoice.CostMultiplier > e.CostMultiplier)
                    e = secondChoice;
            }
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
        int maxSwarmDifficulty = 6;
        float difficultMult = 1 + Owner.DifficultyMult + WaveDirector.TemporaryModifiers.BonusSkullWaves; //2 mid-waves by default
        if (Enemy.EnemyToAdd is EnemyBossDuck || Enemy.EnemyToAdd is Gatligator 
            || Enemy.EnemyToAdd is Sentinel || Enemy.EnemyToAdd is RockGolem) //1 mid-wave by default for bosses, 3 at max card difficulty
        {
            difficultMult -= 1;
            if (Enemy.EnemyToAdd is EnemyBossDuck || Enemy.EnemyToAdd is RockGolem)
                maxSwarmDifficulty -= 3;
            else
                maxSwarmDifficulty -= 1;
        }
        if (Enemy.EnemyToAdd is Infector)
            maxSwarmDifficulty = 1;
        int wavesWithoutSwarm = 0;
        int max = (int)difficultMult;
        for (int i = 0; i < max; ++i)
        {
            GameObject enemyType = Enemy.EnemyToAdd.gameObject;
            int TotalDudes = 1;
            if (WaveDirector.TemporaryModifiers.BonusSkullSwarm.TryGetValue(Enemy.EnemyToAdd.GetType(), out int bonus))
                TotalDudes += bonus;
            GameObject[] enemies = new GameObject[TotalDudes];
            for (int j = 0; j < TotalDudes; ++j)
                enemies[j] = enemyType;
            var card = WaveDeck.DrawMultiSpawn(WaveDeck.RandomPositionOnPlayerEdge(), 0, 0.5f, 0, 1.75f, enemies);
            card.Patterns[0].Skull = true;
            float chance = wavesWithoutSwarm * (0.05f * difficultMult * WaveDirector.WaveNum);
            if ((wavesWithoutSwarm >= 1 && chance > Utils.RandFloat()) || i == max)
            {
                wavesWithoutSwarm = 0;
                card.ToSwarmCircle(Mathf.Min(maxSwarmDifficulty, 1 + wavesWithoutSwarm), 10, 0, 0.5f);
            }
            else
            {
                ++wavesWithoutSwarm;
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
    public float HealingChance = 1f;
    public RewardClause(float points, int rewardsAllowed = -1, float healingChance = 1f, params Reward[] rewards) : base(points)
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
            else if (r is ChestReward ch)
                AddChestReward(ch, listType);
            else if (r is KeyReward k)
                AddKeyReward(k, listType);
            else if (r is GemReward g)
                AddGemReward(g, listType);
        }
        HealingChance = healingChance;
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
        r.coins = ((r.coins + 4) / 5) * 5;
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
    public void AddKeyReward(KeyReward r, List<Reward> list)
    {
        Reward SameType = list.Find(g => g is KeyReward g2);
        if (SameType != null && SameType is KeyReward g2)
        {
            g2.keys += r.keys;
        }
        else
        {
            list.Add(r);
            ++RewardsAdded;
        }
    }
    public void AddGemReward(GemReward r, List<Reward> list)
    {
        Reward SameType = list.Find(g => g is GemReward g2);
        if (SameType != null && SameType is GemReward g2)
        {
            g2.gems += r.gems;
        }
        else
        {
            list.Add(r);
            ++RewardsAdded;
        }
    }
    public void AddChestReward(ChestReward r, List<Reward> list)
    {
        Reward SameType = list.Find(g => g is ChestReward g2 && g2.ChestType == r.ChestType);
        if (SameType != null && SameType is ChestReward g2)
        {
            g2.ChestQuantity += r.ChestQuantity;
        }
        else //clone does not exists
        {
            list.Add(r);
            ++RewardsAdded;
        }
    }
    public override void GenerateData()
    {
        int tokens = Player.Instance.TokensPerWave;
        if (tokens > 0)
        {
            TokenReward reward = new(tokens)
            {
                BeforeWaveEndReward = true
            };
            PreRewards.Add(reward);
        }
        int choices = Player.Instance.PerpetualBubble;
        if (choices > 0)
        {
            PowerReward reward = new(PowerUp.Get<Choice>().MyID)
            {
                Free = true,
                BeforeWaveEndReward = true,
                Amt = choices
            };
            AddPowerReward(reward, PreRewards);
        }
        float Rubies = Player.Instance.Ruby * 0.1f * Mathf.Sqrt(HealingChance);
        while (Utils.RandFloat(1) < Rubies)
        {
            float FreePowerPoint = 100;
            PowerReward reward = GeneratePower(ref FreePowerPoint, 10);
            reward.Free = true;
            reward.BeforeWaveEndReward = false;
            Rubies -= 1;
            AddPowerReward(reward, PostRewards);
        }
        if (CardData.UpcomingWave == 1)
            AddKeyReward(new KeyReward(0, 1) { BeforeWaveEndReward = true }, PreRewards);
        while (Points >= 1 && (RewardsAdded < RewardsAllowed || RewardsAllowed == -1))
        {
            Reward r = GenRandomReward();
            Points -= r.GetCost();
            var listType = r.BeforeWaveEndReward ? PreRewards : PostRewards;
            if (r is PowerReward p)
                AddPowerReward(p, listType);
            else if (r is CoinReward c)
                AddCashReward(c, listType);
            else if (r is ChestReward ch)
                AddChestReward(ch, listType);
            else if (r is KeyReward k)
                AddKeyReward(k, listType);
            else
                listType.Add(r);
        }
    }
    private PowerReward GeneratePower(ref float PointsAvailable, float BonusOnReroll = 5)
    {
        PowerReward reward = new(PowerUp.RandomFromPool(0, -1));
        while (reward.GetCost() > PointsAvailable)
        {
            reward = new PowerReward(PowerUp.RandomFromPool(0, -1));
            PointsAvailable += BonusOnReroll;
        }
        return reward;
    }
    private ChestReward GenerateChest(ref float PointsAvailable)
    {
        int chestQuantity = 1;
        int chestQuality = 0;
        if (Utils.RandFloat(PointsAvailable) > 45 && Utils.RandFloat() < 0.6f)
        {
            chestQuality++;
            if (Utils.RandFloat(PointsAvailable) > 95 && Utils.RandFloat() < 0.6f)
                chestQuality++;
        }
        int chestSpawnCost = 25;
        if (chestQuality == 1)
            chestSpawnCost = 50;
        if (chestQuality == 2)
            chestSpawnCost = 100;

        float spendablePoints = PointsAvailable - chestSpawnCost;
        while (Utils.RandFloat(spendablePoints) > chestSpawnCost)
        {
            if (Utils.RandFloat() < 0.33f)
            {
                chestQuantity++;
                spendablePoints -= chestSpawnCost;
            }
            else break;
        }

        ChestReward reward = new(chestSpawnCost * chestQuantity, chestQuality);
        reward.ChestQuantity = chestQuantity;
        return reward;
    }
    public int RewardType = -1;
    private Reward GenRandomReward()
    {
        float PointsAvailable = Points;
        bool beforeWaveReward = Utils.RandFloat(1) > 0.5f;
        Reward reward = null;
        if(reward == null)
        {
            if (RewardType == -1 && Utils.RandFloat(1) < HealingChance * 0.05f && PreRewards.Count <= 0)
            {
                RewardType = 1;
                reward = new HealReward(beforeWaveReward ? (int)(PointsAvailable + 0.5f) : (int)(PointsAvailable * 0.75f + 0.5f));
            }
            else if (RewardType == -1 && Utils.RandFloat(1) < 0.40f)
            {
                RewardType = 2;
                float conversionRate = (beforeWaveReward ? 18 : 12) + WaveDirector.WaveNum;
                int possibleMaxKeys = (int)Mathf.Max(1, PointsAvailable / conversionRate);
                possibleMaxKeys = (int)Mathf.Max(1, (possibleMaxKeys + 1) * Utils.RandFloat());
                int otherKeyValue = (int)Mathf.Max(1, (possibleMaxKeys + 1) * Utils.RandFloat(0.5f, 1.0f) * Utils.RandFloat(0.25f, 1.0f));
                possibleMaxKeys = Mathf.Min(possibleMaxKeys, otherKeyValue);
                reward = new KeyReward((int)(possibleMaxKeys * conversionRate), possibleMaxKeys);
            }
            else if (Utils.RandFloat(1) < 0.4f || Points < 10)
            {
                reward = new CoinReward(beforeWaveReward ? (int)(PointsAvailable / 2f + 0.5f) : (int)(PointsAvailable + 0.5f));
            }
            else if(RewardType == -1 && Utils.RandFloat(1) < 0.2f)
            {
                RewardType = 3;
                float conversionRate = (beforeWaveReward ? 12 : 10) + WaveDirector.WaveNum;
                int maxGems = (int)Mathf.Max(1, PointsAvailable / conversionRate);
                maxGems = (int)Mathf.Max(1, (maxGems + 1) * Utils.RandFloat());
                int otherKeyValue = (int)Mathf.Max(1, (maxGems + 1) * Utils.RandFloat(0.6f, 1.0f) * Utils.RandFloat(0.6f, 1.0f));
                maxGems = Mathf.Min(maxGems, otherKeyValue);
                reward = new GemReward((int)(maxGems * conversionRate), maxGems);
            }
            if(reward == null)
            {
                if(Utils.RandFloat() < 0.3f)
                {
                    reward = GeneratePower(ref PointsAvailable);
                }
                else
                {
                    reward = GenerateChest(ref PointsAvailable);
                }
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