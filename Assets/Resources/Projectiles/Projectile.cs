using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public class ImmunityData
    {
        public ImmunityData(IImpactedByProjIFrames target, int frames)
        {
            this.target = target;
            immuneFrames = frames;
        }
        public IImpactedByProjIFrames target;
        public int immuneFrames;
    }
    public List<ImmunityData> SpecializedImmuneFrames = new();
    public void UpdateSpecialImmuneFrames()
    {
        if (SpecializedImmuneFrames.Count > 0)
        {
            for (int i = SpecializedImmuneFrames.Count - 1; i >= 0; --i)
                if (--SpecializedImmuneFrames[i].immuneFrames <= 0 || SpecializedImmuneFrames[i].target == null)
                    SpecializedImmuneFrames.RemoveAt(i);
        }
    }
    public static void StaticUpdate()
    {
        if(Main.GameUpdateCount % 2 == 0)
        {
            float absorptionMaxTime = 20;
            for (int i = RecentlySpawnedSmallBubbles.Count - 1; i >= 0; --i)
            {
                SmallBubble otherBubbles = RecentlySpawnedSmallBubbles[i];
                if (otherBubbles == null || otherBubbles.timer2 > absorptionMaxTime )
                    RecentlySpawnedSmallBubbles.RemoveAt(i);
            }
        }
    }
    public static List<SmallBubble> RecentlySpawnedSmallBubbles = new();
    public ProjComponents cmp;
    public SpriteRenderer SpriteRendererGlow => cmp.spriteRendererGlow;
    public SpriteRenderer SpriteRenderer => cmp.spriteRenderer;
    public Rigidbody2D RB => cmp.rb;
    public CircleCollider2D C2D => cmp.c2D;
    public float timer = 0f;
    public float timer2 = 0f;
    public float[] Data;
    public float Data1 { get => Data[0]; set => Data[0] = value; }
    public float Data2 { get => Data[1]; set => Data[1] = value; }
    public float Damage = 0;
    /// <summary>
    /// How many enemies this projectile can hit
    /// </summary>
    public int Penetrate = 1;
    public bool Friendly = false;
    private bool Dead = false;
    public bool Hostile = false;
    public int immunityFrames = 100;
    public Vector2 startPos = Vector2.zero;
    public Player PlayerOwner { get; private set; }
    public Enemy EnemyOwner { get; private set; }
    public Entity Owner { get; private set; }
    //public static GameObject NewProjectile<T>(Vector2 pos, Vector2 velo, float damage = 1, params float[] data) where T : Projectile
    //{
    //    return NewProjectile<T>(pos, velo, damage, null, data);
    //}
    public static GameObject NewProjectile<T>(Vector2 pos, Vector2 velo, float damage, Entity owner, params float[] data) where T : Projectile
    {
        bool hasMerged = true;
        if(owner != null && owner is Player p && p.Coalescence > 0 && typeof(T) == typeof(SmallBubble))
        {
            hasMerged = false;
            for(int i = RecentlySpawnedSmallBubbles.Count - 1; i >= 0; --i)
            {
                SmallBubble otherBubbles = RecentlySpawnedSmallBubbles[i];
                if (otherBubbles == null)
                {
                    RecentlySpawnedSmallBubbles.RemoveAt(i);
                    continue;
                }
                float absorbRange = 0.5f + otherBubbles.transform.localScale.x;
                float distToOtherBubble = otherBubbles.transform.position.Distance(pos + velo * Time.fixedDeltaTime);
                //Debug.Log(distToOtherBubble);
                if (distToOtherBubble < absorbRange)
                {
                    otherBubbles.Damage += 2;
                    otherBubbles.Penetrate += 1;
                    otherBubbles.Data1 += 1;
                    if(otherBubbles.Data.Length > 1 || data.Length > 1)
                    {
                        float d1 = otherBubbles.Data.Length > 1 ? otherBubbles.Data[1] : 0;
                        float d2 = data.Length > 1 ? data[1] : 0;
                        if (otherBubbles.Data.Length <= 1)
                            otherBubbles.Data = new float[] { otherBubbles.Data1, 0 };
                        otherBubbles.Data2 = (d1 + d2) * 0.5f;
                        Color c = Color.Lerp(Player.ProjectileColor, (owner as Player).SecondColor, otherBubbles.Data2).WithAlpha(0.68f);
                        otherBubbles.SpriteRenderer.color = c.WithAlpha(otherBubbles.SpriteRenderer.color.a);
                    }
                    float startingSpeed = otherBubbles.RB.velocity.magnitude;
                    otherBubbles.RB.velocity = Vector2.Lerp(otherBubbles.RB.velocity * 1.5f, velo, 3f / (otherBubbles.Data1 + 3f)).normalized * startingSpeed;
                    if(otherBubbles.Data1 >= p.Coalescence)
                        RecentlySpawnedSmallBubbles.RemoveAt(i);
                    return null;
                }
            }
        }
        GameObject Proj = Instantiate(Main.PrefabAssets.DefaultProjectile, pos, Quaternion.identity, Main.GenericSuperParent);
#if UNITY_EDITOR
        Proj.name = typeof(T).Name;
#endif
        Projectile proj = Proj.AddComponent<T>();
        proj.cmp = Proj.GetComponent<ProjComponents>();
        if (owner != null)
        {
            proj.Owner = owner;
            if (owner is Player p2)
                proj.PlayerOwner = p2;
            else if(owner is Enemy e2)
                proj.EnemyOwner = e2;
        }
        if (proj is BoxProjectile)
        {
            proj.cmp.c2D.enabled = false;
            proj.cmp.rectCollider = Proj.AddComponent<BoxCollider2D>();
            proj.cmp.rectCollider.includeLayers = proj.cmp.c2D.includeLayers;
            proj.cmp.rectCollider.excludeLayers = proj.cmp.c2D.excludeLayers;
            proj.cmp.rectCollider.contactCaptureLayers = proj.cmp.c2D.contactCaptureLayers;
            proj.cmp.rectCollider.callbackLayers = proj.cmp.c2D.callbackLayers;
        }
        proj.RB.velocity = velo;
        if(!hasMerged)
        {
            proj.Data = new float[] { 0, data.Length > 1 ? data[1] : 0 };
            RecentlySpawnedSmallBubbles.Add(proj as SmallBubble);
        }
        else
            proj.Data = data;
        proj.Damage = damage;
        proj.Init();
        return Proj;
    }
    public void Kill()
    {
        if (!Dead)
            Dead = true;
        else
            return;
        OnKill();
        Destroy(gameObject);
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Tub"))
            if(OnTileCollide(collision))
                Kill();
    }
    public virtual bool OnTileCollide(Collider2D collision)
    {
        return true;
    }
    /// <summary>
    /// Return true to allow the projectile to die while inside a tile. True by default.
    /// </summary>
    /// <returns></returns>
    public virtual bool OnInsideTile()
    {
        return true;
    }
    public virtual void Init()
    {
        FixedUpdate();
    }
    public void FixedUpdate()
    {
        UpdateSpecialImmuneFrames();
        AI();
        bool? homing = CanBeAffectedByHoming();
        if (((!homing.HasValue && Friendly) || (homing.HasValue && CanBeAffectedByHoming().Value)) && PlayerOwner.HomingRange > 0)
            HomingBehavior();
        if(!World.WithinBorders(transform.position))
            if(OnInsideTile())
                Kill();
    }
    public void HitTarget(Entity target)
    {
        if(target is RockGolem)
        {
            if (Damage > 0.1f)
                Damage = Mathf.Max(0.1f, Damage * 0.875f);
        }
        if(PlayerOwner.SkullBomb > 0)
        {
            if(target.Life <= 0 && target is Enemy e && e.IsSkull)
            {
                Vector2 velo = Utils.RandCircle(3);
                Vector2 endPos = velo + (Vector2)transform.position;
                Projectile.NewProjectile<BathBomb>(e.transform.position, velo, PlayerOwner.SkullBomb * 5, PlayerOwner, endPos.x, endPos.y, 4, 0.8f);
            }
        }
        OnHitTarget(target);
        if (Penetrate != -1)
        {
            --Penetrate;
            if (Penetrate <= 0)
                Kill();
        }
        if (PlayerOwner.SnakeEyes > 0)
        {
            int poison = PlayerOwner.SnakeEyes;
            if (poison > 0)
            {
                if (poison >= 81 || Utils.RandFloat(1) < 0.19f + poison * 0.01f)
                {
                    float duration = 10;
                    target.AddBuff<Poison>(duration);
                }
            }
        }
        if (PlayerOwner.ChillDuration > 0)
        {
            //if(target is not IceGolem)
            target.AddBuff<Chill>(PlayerOwner.ChillDuration);
        }
        bool piercingProjectile = Penetrate > 1 || Penetrate == -1;
        if (piercingProjectile && target is Enemy enemy)
            SpecializedImmuneFrames.Add(new ImmunityData(enemy, immunityFrames));
    }
    /// <summary>
    /// Called after damage is registered on the enemy, including when the projectile would kill the enemy
    /// </summary>
    /// <param name="target"></param>
    public virtual void OnHitTarget(Entity target)
    {

    }
    public void OnHitByStar(Entity target)
    {
        if (target.Life <= 0)
        {
            if (PlayerOwner.Starbarbs > 0)
            {
                AudioManager.PlaySound(SoundID.Starbarbs, transform.position, 1, 1);
                Vector2 norm = RB.velocity.normalized;
                if(this is SupernovaExplode || this is SupernovaProj)
                    norm = Utils.RandCircle(1).normalized;
                float randRot = norm.ToRotation();
                for (int i = 0; i < 30; i++)
                {
                    Vector2 randPos = new Vector2(3.5f, 0).RotatedBy(i / 15f * Mathf.PI);
                    randPos.x *= Utils.RandFloat(0.5f, 0.7f);
                    randPos = randPos.RotatedBy(randRot);
                    ParticleManager.NewParticle(target.transform.position, Utils.RandFloat(0.95f, 1.05f), -norm * 4.5f + randPos * Utils.RandFloat(4, 5) + Utils.RandCircle(.3f), 0.1f, .6f, 0, SpriteRenderer.color);
                }
                int stars = 2 + PlayerOwner.Starbarbs;
                for (; stars > 0; --stars)
                {
                    Vector2 targetPos = (Vector2)target.transform.position + norm * 9 + Utils.RandCircle(7);
                    NewProjectile<StarProj>(target.transform.position, norm.RotatedBy(Utils.RandFloat(360) * Mathf.Deg2Rad) * -Utils.RandFloat(16f, 24f), 2, Owner, targetPos.x, targetPos.y, Utils.RandInt(2) * 2 - 1);
                }
            }
            if (PlayerOwner.LuckyStar > 0 && PlayerOwner.LuckyStarItemsAcquiredThisWave < PlayerOwner.LuckyStarItemsAllowedPerWave)
            {
                float chance = 0.03f + PlayerOwner.LuckyStar * 0.01f;
                if (Utils.RandFloat(1) < chance)
                {
                    PowerUp.Spawn(PowerUp.RandomFromPool(), (Vector2)target.transform.position);
                    PlayerOwner.LuckyStarItemsAcquiredThisWave++;
                }
            }
        }
    }
    public virtual void AI()
    {

    }
    public virtual void OnKill()
    {

    }
    /// <summary>
    /// Return false to not be affected by homing, null to be effected only when friendly, true to be affected regardless
    /// </summary>
    /// <returns></returns>
    public virtual bool? CanBeAffectedByHoming() => null;
    public virtual Vector3 HomingStartPosition() => transform.position;
    public int homingCounter = 0;
    public void HomingBehavior()
    {
        if(homingCounter++ % 4 == 0)
        {
            float range = PlayerOwner.HomingRange;
            Enemy target = Enemy.FindClosest(HomingStartPosition(), range, out Vector2 norm2, true);
            if (target != null && DoHomingBehavior(target, norm2, range))
            {
                float currentSpeed = RB.velocity.magnitude + PlayerOwner.HomingRangeSqrt * 0.225f;
                float modAmt = 0.0625f + PlayerOwner.HomingRangeSqrt * 0.03f;
                RB.velocity = Vector2.Lerp(RB.velocity * (1 - modAmt), norm2 * currentSpeed, modAmt).normalized * currentSpeed;
            }
        }
    }
    public virtual bool DoHomingBehavior(Enemy target, Vector2 norm, float range)
    {
        return true;
    }
}
public abstract class BoxProjectile : Projectile
{
    public new BoxCollider2D C2D => cmp.rectCollider;
}
public interface IImpactedByProjIFrames
{

}