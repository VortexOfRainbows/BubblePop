using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static void StaticUpdate()
    {
        if(Main.GameUpdateCount % 2 == 0 && Player.Instance != null)
        {
            float absorptionMaxTime = Player.Instance.Coalescence + 19;
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
    public CircleCollider2D c2D => cmp.c2D;
    public static int colorGradient = 0;
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
    public static GameObject NewProjectile<T>(Vector2 pos, Vector2 velo, float damage = 1, params float[] data) where T : Projectile
    {
        bool hasMerged = true;
        if(Player.Instance.Coalescence > 0 && typeof(T) == typeof(SmallBubble))
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
                    float startingSpeed = otherBubbles.RB.velocity.magnitude;
                    otherBubbles.RB.velocity = Vector2.Lerp(otherBubbles.RB.velocity * 1.5f, velo, 3f / (otherBubbles.Data1 + 3f)).normalized * startingSpeed;
                    if(otherBubbles.Data1 >= Player.Instance.Coalescence)
                        RecentlySpawnedSmallBubbles.RemoveAt(i);
                    return null;
                }
            }
        }
        GameObject Proj = Instantiate(Main.PrefabAssets.DefaultProjectile, pos, Quaternion.identity);
        Projectile proj = Proj.AddComponent<T>();
        proj.cmp = Proj.GetComponent<ProjComponents>();
        proj.RB.velocity = velo;
        if(!hasMerged)
        {
            proj.Data = new float[] { 0 };
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
        if(collision.CompareTag("Tub") && this is not BathBomb)
        {
            if(OnTileCollide(collision))
                Kill();
        }
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
        AI();
        if(CanBeAffectedByHoming() && (Friendly || this is SupernovaProj) && Player.Instance.HomingRange > 0)
            HomingBehavior();
        if(!World.WithinBorders(transform.position))
            if(OnInsideTile())
                Kill();
    }
    public void HitTarget(Entity target)
    {
        if(Player.Instance.RecursiveSubspaceLightning > 0)
        {
            int eyes = Player.Instance.RecursiveSubspaceLightning + 2;
            float recursiveDepth = 1;
            if (this is SnakeLightning)
            {
                recursiveDepth += Data1 * 1.2f;
            }
            float chanceOfSuccess = Main.SnakeEyeChance * (eyes - recursiveDepth);
            if(Utils.RandFloat() < chanceOfSuccess)
                NewProjectile<SnakeLightning>(transform.position, (target.transform.position - transform.position).normalized * 2.5f, 10, recursiveDepth);
        }
        if(Player.Instance.SnakeEyes > 0)
        {
            int poison = Player.Instance.SnakeEyes;
            if (poison > 0)
            {
                if (poison >= 81 || Utils.RandFloat(1) < 0.19f + poison * 0.01f)
                {
                    float duration = 10;
                    target.AddBuff<Poison>(duration);
                }
            }
        }
        OnHitTarget(target);
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
            if (Player.Instance.Starbarbs > 0)
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
                int stars = 2 + Player.Instance.Starbarbs;
                for (; stars > 0; --stars)
                {
                    Vector2 targetPos = (Vector2)target.transform.position + norm * 9 + Utils.RandCircle(7);
                    NewProjectile<StarProj>(target.transform.position, norm.RotatedBy(Utils.RandFloat(360) * Mathf.Deg2Rad) * -Utils.RandFloat(16f, 24f), 2, targetPos.x, targetPos.y, Utils.RandInt(2) * 2 - 1);
                }
            }
            if (Player.Instance.LuckyStar > 0 && Player.Instance.LuckyStarItemsAcquiredThisWave < Player.Instance.LuckyStarItemsAllowedPerWave)
            {
                float chance = 0.03f + Player.Instance.LuckyStar * 0.01f;
                if (Utils.RandFloat(1) < chance)
                {
                    PowerUp.Spawn(PowerUp.RandomFromPool(), (Vector2)target.transform.position, 0);
                    Player.Instance.LuckyStarItemsAcquiredThisWave++;
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
    public Color PickColor(float data, float counter)
    {
        Color color = Color.white;
        if (data == 0)
            color = new Color(112 / 255f, 54 / 255f, 157 / 255f);
        if (data == 1)
            color = new Color(75 / 255f, 54 / 255f, 255 / 255f);
        if (data == 2)
            color = new Color(121 / 255f, 195 / 255f, 20 / 255f);
        if (data == 3)
            color = new Color(250 / 255f, 235 / 255f, 54 / 255f);
        if (data == 4)
            color = new Color(255 / 255f, 165 / 255f, 0 / 255f);
        if (data == 5)
            color = new Color(232 / 255f, 20 / 255f, 22 / 255f);
        if (data == 6)
            color = Rainbow(counter);
        return color;
    }
    public Color Rainbow(float timer)
    {
        timer = timer * Mathf.Deg2Rad * 2.5f;
        double center = 130;
        Vector2 circlePalette = new Vector2(1, 0).RotatedBy(timer);
        double width = 125 * circlePalette.y;
        int red = (int)(center + width);
        circlePalette = new Vector2(1, 0).RotatedBy(timer + Mathf.PI * 2f / 3f);
        width = 125 * circlePalette.y;
        int grn = (int)(center + width);
        circlePalette = new Vector2(1, 0).RotatedBy(timer + Mathf.PI * 4f / 3f);
        width = 125 * circlePalette.y;
        int blu = (int)(center + width);
        return new Color(red / 255f, grn / 255f, blu / 255f);
    }
    public virtual bool CanBeAffectedByHoming()
    {
        return true;
    }
    public int homingCounter = 0;
    public void HomingBehavior()
    {
        if(homingCounter++ % 4 == 0)
        {
            float range = Player.Instance.HomingRange;
            Enemy target = Enemy.FindClosest(transform.position, range, out Vector2 norm2, true);
            if (target != null && DoHomingBehavior(target, norm2, range))
            {
                float currentSpeed = RB.velocity.magnitude + Player.Instance.HomingRangeSqrt * 0.225f;
                float modAmt = 0.0625f + Player.Instance.HomingRangeSqrt * 0.03f;
                RB.velocity = Vector2.Lerp(RB.velocity * (1 - modAmt), norm2 * currentSpeed, modAmt).normalized * currentSpeed;
            }
        }
    }
    public virtual bool DoHomingBehavior(Enemy target, Vector2 norm, float range)
    {
        return true;
    }
}
public class FlamingoFeather : Projectile
{
    public override void Init()
    {
        transform.localScale = new Vector3(0.65f, 0.45f, 1);
        SpriteRendererGlow.transform.localScale = new Vector3(1.5f, 4f, 1);
        SpriteRendererGlow.color = new Color(253 / 255f, 181 / 255f, 236 / 255f, 0.5f);
        SpriteRenderer.sprite = Main.TextureAssets.Feather;
        Hostile = true;
    }
    public override void AI()
    {
        FeatherAI();
    }
    public override void OnKill()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 circular = new Vector2(.5f, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.2f, 0.4f), circular * Utils.RandFloat(3, 6), 4f, 0.4f, 1, SpriteRendererGlow.color);
        }
    }
    public void FeatherAI()
    {
        RB.rotation = RB.velocity.ToRotation() * Mathf.Rad2Deg + 90;

        timer++;
        if (timer < 300)
        {
            RB.velocity += RB.velocity.normalized * 0.003f;
            RB.velocity += (Player.Position - (Vector2)transform.position).normalized * 0.03f;
        }
        if (timer > 510)
        {
            float alphaOut = 1 - (timer - 510) / 90f;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alphaOut);
            SpriteRendererGlow.color = new Color(SpriteRendererGlow.color.r, SpriteRendererGlow.color.g, SpriteRendererGlow.color.b) * alphaOut;
            if (timer > 550)
                Hostile = false;
        }
        if (timer > 600)
        {
            Kill();
        }
        if (Utils.RandFloat(1) < 0.5f)
            ParticleManager.NewParticle((Vector2)transform.position, 0.5f, Utils.RandCircle(0.02f), 1.5f, 0.4f, 0, SpriteRendererGlow.color);
    }
}
public class Laser : Projectile
{
    public override void Init()
    {
        transform.localScale = new Vector3(0.9f, 0.8f, 1);
        SpriteRendererGlow.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        SpriteRendererGlow.color = new Color(168f / 255f, 62f / 255f, 70f / 255f, 0.4f);
        SpriteRenderer.sprite = Main.TextureAssets.Laser;
        Hostile = true;
    }
    public override void AI()
    {
        LaserAI();
    }
    public override void OnKill()
    {
        base.OnKill();
    }
    public void LaserAI()
    {
        RB.rotation = RB.velocity.ToRotation() * Mathf.Rad2Deg + 180;
        for (float i = 0; i < 1; i += 0.5f)
            ParticleManager.NewParticle((Vector2)transform.position + RB.velocity * i * Time.fixedDeltaTime, 0.5f, -RB.velocity.normalized * 2.5f, 0f, 0.3f, 1, SpriteRendererGlow.color);
        if (timer < 200)
        {
            RB.velocity += RB.velocity.normalized * 0.02f;
            RB.velocity += (Player.Position - (Vector2)transform.position).normalized * 0.08f * (1 - timer / 200f);
        }
        if (timer > 610)
        {
            float alphaOut = 1 - (timer - 610) / 90f;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alphaOut);
            SpriteRendererGlow.color = new Color(SpriteRendererGlow.color.r, SpriteRendererGlow.color.g, SpriteRendererGlow.color.b) * alphaOut;
            if (timer > 650)
                Hostile = false;
        }
        if (timer > 700)
        {
            Kill();
        }
        timer++;
    }
}
public class PhoenixFire : Projectile
{
    public override void Init()
    {
        transform.localScale = Vector3.one * 0.5f;
        SpriteRenderer.color = new Color(1, 1, 1, 0.5f);
        SpriteRendererGlow.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        SpriteRendererGlow.color = new Color(1f, 0.45f, .25f);
        SpriteRenderer.sprite = Resources.Load<Sprite>("Projectiles/PhoenixFire");
        immunityFrames = 100;
        Penetrate = 3;
        Friendly = true;
        Hostile = false;
    }
    public override void AI()
    {
        FireAI();
    }
    public void FireAI()
    {
        float deathTime = 100;
        float FadeOutTime = 20;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 3 * (1f - 0.8f * timer / deathTime), 0.1f);

        Vector2 velo = RB.velocity;
        velo *= 0.99f;
        RB.velocity = velo;

        if (timer > deathTime + FadeOutTime)
        {
            Kill();
        }
        if ((int)timer % 5 == 0)
        {
            Vector2 norm = RB.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f, .25f, norm * -.75f, 0.8f, Utils.RandFloat(0.25f, 0.4f), 0, SpriteRendererGlow.color);
        }
        if (timer > deathTime)
        {
            float alphaOut = 1 - (timer - deathTime) / FadeOutTime;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 0.5f * alphaOut);
            SpriteRendererGlow.color = new Color(SpriteRendererGlow.color.r, SpriteRendererGlow.color.g, SpriteRendererGlow.color.b) * alphaOut;
        }
        timer++;
    }
}
public class SnakeLightning : Projectile
{
    public override void Init()
    {
        transform.localScale = Vector3.one * 3f;
        SpriteRenderer.enabled = false;
        SpriteRendererGlow.gameObject.SetActive(false);
        SpriteRenderer.sprite = null;
        Damage = 1;
        Penetrate = -1;
        Friendly = false;
        Hostile = false;
        Lightning();
    }
    public void Lightning()
    {
        for (int i = 0; i < 15; ++i)
        {
            Vector2 circular = new Vector2(3 + Utils.RandFloat(6), 0).RotatedBy(Mathf.PI * (i / 5f * 2) + Utils.RandFloat(Mathf.PI * 0.4f));
            ParticleManager.NewParticle(transform.position, Utils.RandFloat(2, 3), circular, 5, Utils.RandFloat(0.5f, 1.5f), 3, Color.green * 1.5f);
        }
        Pylon.SummonLightning((Vector2)transform.position - RB.velocity * 0.4f, (Vector2)transform.position + RB.velocity * 2.4f, Color.green);
    }
    public override void AI()
    {
        timer++;
        if (timer > 15)
            Friendly = true;
        if (timer > 40)
            Kill();
    }
    public override bool CanBeAffectedByHoming()
    {
        return false;
    }
}