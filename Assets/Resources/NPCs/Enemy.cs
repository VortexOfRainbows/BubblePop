using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemyID
{
    public static List<GameObject> AllEnemyList = new();
    public static int Max => AllEnemyList.Count;
    public static GameObject LoadNPC(string str)
    {
        GameObject prefab = Resources.Load<GameObject>($"NPCs/{str}");
        AllEnemyList.Add(prefab);
        return prefab;
    }
    public static GameObject PortalPrefab = Resources.Load<GameObject>("NPCs/Portal");
    public static GameObject OldDuck = LoadNPC("Old/EnemyDuck");
    public static GameObject OldLeonard = LoadNPC("Old/EnemyLaserDuck");
    public static GameObject OldFlamingo = LoadNPC("Old/EnemyFlamingo");
    public static GameObject OldSoap = LoadNPC("Old/EnemySoap");
    public static GameObject OldSmallSoap = LoadNPC("Old/EnemySoapTiny");
    public static GameObject Chicken = LoadNPC("Chicken/Chicken");
    public static GameObject Gatligator = LoadNPC("Gatligator/Gatligator");
}
public class Enemy : Entity
{
    public static HashSet<Enemy> Enemies = new HashSet<Enemy>();
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
                bool blackListed = ignore.Contains(e);
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
    public List<ImmunityData> SpecializedImmuneFrames = new List<ImmunityData>();
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
        if(JustSpawnedIn)
        {
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
    public bool AlreadyDead => Life <= -50;
    private void SetDead() => Life = -50;
    public bool FirstStrike = true;
    public sealed override void OnHurtByProjectile(Projectile proj)
    {
        if (SpecializedImmuneFrames.Contains(proj) || proj.Penetrate == 0 || !SpawnedIn)
            return;
        bool piercingProjectile = proj.Penetrate > 1 || proj.Penetrate == -1;
        if (piercingProjectile && UniversalImmuneFrames > 0)
            return;
        if (proj.Friendly && !AlreadyDead)
        {
            float damage = proj.Damage;
            bool crit = false;
            if(FirstStrike)
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
        if (Life < 0)
            Life = 0;
        if (Life <= 0 && !AlreadyDead)
        {
            SetDead();
            Kill();
        }
    }
    public virtual float PowerDropChance => 0.04f;
    protected float MaxCoins = 0;
    protected float MinCoins = 1;
    protected int CoinRandomizationAggressiveness = 3;
    /// <summary>
    /// The cost multiplier to spawn this specific enemy by the director
    /// </summary>
    public virtual float CostMultiplier => 1;
    public sealed override void Kill()
    {
        Enemies.Remove(this);
        OnKill();
        float rand = 1;
        for(int i = 0; i < CoinRandomizationAggressiveness; ++i)
        {
            rand *= Utils.RandFloat();
        }
        CoinManager.SpawnCoin(transform.position, (int)(MinCoins + MaxCoins * rand));
        float reduceRelativeDropRates = Mathf.Max(0.25f, Mathf.Min(1, 0.25f + (400 - WaveDirector.TotalPowersSpawned) / 400f)); //At 400 powers, this number is 0.25, meaning power drop rates will be reduced
        bool LuckyDrop = Utils.RandFloat(1) < PowerDropChance * reduceRelativeDropRates;
        WaveDirector.Point += (int)MaxCoins;
        if (WaveDirector.CanSpawnPower() || LuckyDrop)
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
}
