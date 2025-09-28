using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemyID
{
    public class StaticEnemyData
    {
        public string SaveTag { get; set; }
        public StaticEnemyData(string saveTag)
        {
            SaveTag = saveTag;
            LoadData();
        }
        public void LoadData() //Only called on load
        {
            TimesKilled = PlayerData.GetInt($"{SaveTag}_0", 0);
            TimesKilledSkull = PlayerData.GetInt($"{SaveTag}_1", 0);
        }
        public void SaveData() //Called when an enemy is killed
        {
            PlayerData.SaveInt($"{SaveTag}_0", TimesKilled);
            PlayerData.SaveInt($"{SaveTag}_1", TimesKilledSkull);
        }
        public Sprite CardBG;
        public Sprite Card;
        public int TimesKilled { get; set; } = 0;
        public int TimesKilledSkull { get; set; } = 0;
        public int Rarity { get; set; } = 0;
        public float Cost { get; set; } = 0;
        public float BaseMaxLife { get; set; } = 10;
        public float BaseMinCoin { get; set; } = 1;
        public float BaseMaxCoin { get; set; } = 1;
        public bool Unlocked => TimesKilled > 0;
    }
    public static Dictionary<int, StaticEnemyData> EnemyData { get; private set; } = new();
    public static List<Enemy> SpawnableEnemiesList { get; private set; } = new();
    public static List<Enemy> AllEnemiesList { get; private set; } = new();
    public static int MaxRandom => SpawnableEnemiesList.Count;
    public static int Max => CurrentIndex;
    private static int CurrentIndex = 0;
    public static GameObject LoadNPC(string str, bool SpawnList = true)
    {
        GameObject prefab = Resources.Load<GameObject>($"NPCs/{str}");
        Enemy e = prefab.GetComponent<Enemy>();
        string saveTag = e.name;
        var d = new StaticEnemyData(saveTag);
        e.InitStaticDefaults(ref d);
        EnemyData.Add(CurrentIndex, d);
        e.SetIndexInAllEnemyArray(CurrentIndex++);
        AllEnemiesList.Add(e);
        if (SpawnList)
            SpawnableEnemiesList.Add(e);
        return prefab;
    }
    public static readonly GameObject PortalPrefab = Resources.Load<GameObject>("NPCs/Portal");
    public static readonly GameObject OldDuck = LoadNPC("Old/EnemyDuck");
    public static readonly GameObject OldLeonard = LoadNPC("Old/EnemyLaserDuck");
    public static readonly GameObject OldFlamingo = LoadNPC("Old/EnemyFlamingo");
    public static readonly GameObject OldSoap = LoadNPC("Old/EnemySoap");
    public static readonly GameObject OldSmallSoap = LoadNPC("Old/EnemySoapTiny", false);
    public static readonly GameObject Chicken = LoadNPC("Chicken/Chicken");
    public static readonly GameObject Gatligator = LoadNPC("Gatligator/Gatligator");
    public static readonly GameObject Crow = LoadNPC("Crow/Crow");
    public static readonly GameObject Ent = LoadNPC("Ent/Ent");
}
public class Enemy : Entity
{
    public void SetIndexInAllEnemyArray(int i) => IndexInAllEnemyArray = i;
    public int GetIndex() => IndexInAllEnemyArray;
    [SerializeField]
    private int IndexInAllEnemyArray = -1; //This must be a field, not a property, as fields are naturally cloned over from spawn from the initial prefab
    public EnemyID.StaticEnemyData StaticData => EnemyID.EnemyData[IndexInAllEnemyArray];
    public virtual void InitStatics(ref EnemyID.StaticEnemyData data)
    {

    }
    public void InitStaticDefaults(ref EnemyID.StaticEnemyData data)
    {
        data.Cost = CostMultiplier;
        data.Rarity = (int)Mathf.Clamp(data.Cost, 1, 5);
        InitStatics(ref data);
        if (data.Card == null)
            data.Card = Resources.Load<Sprite>("NPCs/Old/rubber_duck");
        if (data.CardBG == null)
            data.CardBG = Resources.Load<Sprite>("UI/Background");
    }
    public sealed override void Init()
    {
        SetUpStats();
        OnSpawn();
    }
    public virtual void OnSpawn()
    {

    }
    public void SetUpStats()
    {
        Life = MaxLife = StaticData.BaseMaxLife;
        MinCoins = StaticData.BaseMinCoin;
        MaxCoins = StaticData.BaseMaxCoin;
    }
    public static Enemy Spawn(GameObject EnemyPrefab, Vector2 position, bool skull = false)
    {
        Enemy e = Instantiate(EnemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
        e.SetSkullEnemy(skull);
        return e;
    }
    public static HashSet<Enemy> Enemies = new();
    public static Enemy FindClosest(Vector3 position, float searchDistance, out Vector2 norm, bool requireNonImmune = true, params Enemy[] ignore)
    {
        Enemy e = FindClosest(position, searchDistance, out Vector2 newNorm, ignore.ToList(), requireNonImmune);
        norm = newNorm;
        return e;
    }
    public static Enemy FindClosest(Vector3 position, float searchDistance, out Vector2 norm, List<Enemy> ignore, bool requireNonImmune = true)
    {
        norm = Vector2.zero;
        Enemy best = null;
        foreach (Enemy e in Enemies)
        {
            Vector2 toDest = e.transform.position - position;
            float dist = toDest.magnitude;
            //Debug.Log(e.tag);
            if (dist <= searchDistance && (!requireNonImmune || e.UniversalImmuneFrames <= 0))
            {
                bool blackListed = ignore != null && ignore.Contains(e);
                if (!blackListed)
                {
                    best = e;
                    searchDistance = dist;
                    norm = toDest;
                }
            }
        }
        norm = norm.normalized;
        return best;
    }
    public class ImmunityData
    {
        public ImmunityData(Projectile attacker, int frames)
        {
            this.attacker = attacker;
            immuneFrames = frames;
        }
        public Projectile attacker;
        public int immuneFrames;
    }
    public List<ImmunityData> SpecializedImmuneFrames = new();
    public void DeathParticles(int count = 10, float size = 0, Color c = default)
    {
        BoxCollider2D c2D = GetComponent<BoxCollider2D>();
        for (int i = 0; i < count; i++)
        {
            Vector2 randPos = c2D.bounds.min + new Vector3(c2D.bounds.extents.x * Utils.RandFloat(1), c2D.bounds.extents.y * Utils.RandFloat(1));
            ParticleManager.NewParticle(randPos, size * Utils.RandFloat(0.9f, 1.1f), Utils.RandCircle(1) * Utils.RandFloat(4, 12) + Vector2.up * Utils.RandFloat(3), 3, .75f, 1, c);
        }
    }
    private bool JustSpawnedIn = true;
    public sealed override void OnFixedUpdate()
    {
        if (JustSpawnedIn)
        {
            if (IsSkull)
            {
                MinCoins += 4;
                MaxCoins += 4;
                transform.localScale = new Vector3(1.14f, 1.14f, transform.localScale.z);
                BossHealthBar.Spawn(this);
                if (IsSkull)
                    WaveDirector.SkullEnemiesActive += 1;
            }
            Enemies.Add(this);
            UpdateRendererColor(new Color(1, 0, 0, 0), 1);
            JustSpawnedIn = false;
        }
        if (SpecializedImmuneFrames.Count > 0)
        {
            for (int i = SpecializedImmuneFrames.Count - 1; i >= 0; --i)
                if (--SpecializedImmuneFrames[i].immuneFrames <= 0)
                    SpecializedImmuneFrames.RemoveAt(i);
        }
        UpdateBuffs();
        AI();
    }
    public bool AlreadyDead = false;
    private void SetDead() => AlreadyDead = true;
    public bool FirstStrike = true;
    public sealed override void OnHurtByProjectile(Projectile proj)
    {
        if (SpecializedImmuneFrames.Contains(proj) || proj.Penetrate == 0 || !SpawnedIn || proj.Damage <= 0)
            return;
        bool piercingProjectile = proj.Penetrate > 1 || proj.Penetrate == -1;
        if (piercingProjectile && UniversalImmuneFrames > 0)
            return;
        if (proj.Friendly && !AlreadyDead)
        {
            float damage = proj.Damage * Player.Instance.DamageMultiplier;
            bool crit = false;
            if (FirstStrike)
            {
                FirstStrike = false;
                int initiative = Player.Instance.RollInit;
                if (initiative > 0)
                {
                    if (initiative >= 81 || Utils.RandFloat(1) < 0.19f + initiative * 0.01f)
                    {
                        float minIncrease = 2.25f + 0.25f * initiative;
                        float maxIncrease = 4.50f + 0.50f * initiative;
                        float increase = Utils.RandFloat(minIncrease, maxIncrease);
                        damage += damage * increase;
                        crit = true;
                    }
                }
            }
            Injure(damage, crit ? 1 : 0, crit ? new Color(1f, 0.9f, 0.3f) : default);
            proj.HitTarget(this);
            if (proj.Penetrate != -1)
            {
                --proj.Penetrate;
                if (proj.Penetrate <= 0)
                    proj.Kill();
            }
            if (piercingProjectile)
            {
                SpecializedImmuneFrames.Add(new ImmunityData(proj, proj.immunityFrames));
            }
        }
    }
    public void Injure(float damage, int damageType = 0, Color popupTextColor = default)
    {
        Life -= damage;
        DamageTaken += damage;
        BoxCollider2D c2D = GetComponent<BoxCollider2D>();
        Vector2 randPos = c2D.bounds.min + new Vector3(c2D.bounds.extents.x * Utils.RandFloat(1), c2D.bounds.extents.y * Utils.RandFloat(1));
        if (popupTextColor == default)
            popupTextColor = new Color(1f, 0.5f, 0.4f);
        Vector2 velo = Utils.RandCircle(3) + Vector2.up * 2;
        if (damageType == 2)
        {
            velo.x *= 0.5f;
            velo.y += 0.5f;
        }
        GameObject g = PopupText.NewPopupText(randPos, velo, popupTextColor, damage.ToString("0.#"), damageType == 1, damageType == 2 ? 0.8f : 1f);
        if (Life <= 0 && !AlreadyDead)
        {
            SetDead();
            Kill();
        }
    }
    public virtual float PowerDropChance => 0.04f;
    protected float MaxCoins { get; set; } = 1;
    protected float MinCoins { get; set; } = 1;
    protected int CoinRandomizationAggressiveness = 3;
    /// <summary>
    /// The cost multiplier to spawn this specific enemy by the director
    /// </summary>
    public virtual float CostMultiplier => 1;
    public sealed override void Kill()
    {
        if (IsSkull)
        {
            StaticData.TimesKilledSkull++;
            WaveDirector.SkullEnemiesActive -= 1;
            if(Player.Instance.HasResearchNotes)
            {
                Player.Instance.ResearchNoteKillCounter += 1;
            }
        }
        StaticData.TimesKilled++;
        StaticData.SaveData();
        //Debug.Log($"[{IndexInAllEnemyArray}] Killed: {StaticData.TimesKilled}");
        Enemies.Remove(this);
        OnKill();
        float rand = 1;
        for (int i = 0; i < CoinRandomizationAggressiveness; ++i)
        {
            rand *= Utils.RandFloat();
        }
        float coins = MinCoins + (Mathf.Max(0, MaxCoins - MinCoins)) * rand + 0.5f;
        if(IsSkull)
            coins += Player.Instance.FlatSkullCoinBonus;
        CoinManager.SpawnCoin(transform.position, (int)coins);
        float reduceRelativeDropRates = Mathf.Max(0.25f, Mathf.Min(1, 0.25f + (400 - WaveDirector.TotalPowersSpawned) / 400f)); //At 400 powers, this number is 0.25, meaning power drop rates will be reduced
        bool LuckyDrop = Utils.RandFloat(1) < PowerDropChance * reduceRelativeDropRates;
        WaveDirector.Point += (int)MaxCoins;
        if (/*WaveDirector.CanSpawnPower() ||*/ LuckyDrop)
            PowerUp.Spawn(PowerUp.RandomFromPool(0.15f), transform.position, LuckyDrop ? 0 : (100 + (int)WaveDirector.PityPowersSpawned * 8));
        Destroy(gameObject);
    }
    public virtual void AI()
    {

    }
    public virtual void OnKill()
    {

    }
    public void OnDestroy()
    {
        Enemies.Remove(this);
    }
    public virtual string Name()
    {
        return Utils.ToSpacedString(name);
    }
    public bool IsSkull { get; private set; } = false;
    public void SetSkullEnemy(bool value = true)
    {
        IsSkull = value;
    }
    public virtual float HealthBarOffset => 0;
    public int GetRarity()
    {
        return StaticData.Rarity;
    }
}
