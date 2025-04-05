using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
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
    public int Damage = 0;
    public int Penetrate = 1;
    public bool Friendly = false;
    private bool Dead = false;
    public bool Hostile = false;
    public int immunityFrames = 100;
    public Vector2 startPos = Vector2.zero;
    public static GameObject NewProjectile<T>(Vector2 pos, Vector2 velo, params float[] data) where T : Projectile
    {
        GameObject Proj = Instantiate(Main.Projectile, pos, Quaternion.identity);
        Projectile proj = Proj.AddComponent<T>();
        proj.cmp = Proj.GetComponent<ProjComponents>();
        proj.RB.velocity = velo;
        proj.Data = data;
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
        if(collision.tag == "Tub" && this is not BathBomb)
        {
            if (this is BigBubble)
            {
                if(timer > 10)
                {
                    Vector2 toClosest = collision.ClosestPoint(transform.position) - (Vector2)transform.position;
                    RB.velocity = -toClosest.normalized * 0.75f * RB.velocity.magnitude;
                }
                return;
            }
            Kill();
        }
    }
    public virtual void Init()
    {
        FixedUpdate();
    }
    public void FixedUpdate()
    {
        AI();
    }
    public virtual void OnHitTarget(Entity target)
    {

    }
    public virtual void AI()
    {

    }
    public virtual void OnKill()
    {

    }
    #region bath bomb colors
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
    #endregion
}
public class StarProj : Projectile
{
    public override void Init()
    {
        SpriteRenderer.sprite = Main.Sparkle;
        SpriteRenderer.color = SpriteRendererGlow.color = new Color(1f, 1f, 0.2f, 0.6f);
        Damage = 2;
        Friendly = true;
    }
    public override void AI()
    {
        SparkleAI();
    }
    public override void OnKill()
    {
        if (timer >= 165)
            return;
        for (int i = 0; i < 8; i++)
        {
            Vector2 circular = new Vector2(1, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.4f, 0.5f), circular * Utils.RandFloat(3, 6), 4f, 0.4f, 0, SpriteRenderer.color);
        }
        AudioManager.PlaySound(SoundID.StarbarbImpact.GetVariation(2), transform.position, 0.7f, 0.66f);
    }
    public void SparkleAI()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.66f, 0.085f);

        Vector2 target = new Vector2(Data1, Data2);
        Vector2 toTarget = (target - (Vector2)transform.position);
        float dist = toTarget.magnitude;
        toTarget = toTarget.normalized;
        Vector2 newVelo = RB.velocity.magnitude * toTarget;
        if (timer < 60)
            RB.velocity *= 1.002f;
        if (timer < 100 && dist > 1)
            RB.velocity = Vector2.Lerp(RB.velocity, newVelo, 0.065f).normalized * RB.velocity.magnitude;
        else if (timer < 100)
            timer = 100;
        RB.rotation += Mathf.Sqrt(RB.velocity.magnitude) * Mathf.Sign(RB.velocity.x);
        if (timer > 170)
        {
            Kill();
        }
        Vector2 norm = RB.velocity.normalized;
        ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f, .175f, norm * -.75f, 0.1f, Utils.RandFloat(0.55f, 0.65f), 0, SpriteRenderer.color);
        if (timer > 150)
        {
            float alphaOut = 1 - (timer - 150) / 20f;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alphaOut);
            SpriteRendererGlow.color = new Color(SpriteRendererGlow.color.r, SpriteRendererGlow.color.g, SpriteRendererGlow.color.b) * alphaOut;
        }
        timer++;
    }
    public override void OnHitTarget(Entity target)
    {
        if (target.Life <= 0)
        {
            if(Player.Instance.Starbarbs > 0)
            {
                Vector2 norm = RB.velocity.normalized;
                float randRot = norm.ToRotation();
                for (int i = 0; i < 30; i++)
                {
                    Vector2 randPos = new Vector2(3.5f, 0).RotatedBy(i / 15f * Mathf.PI);
                    randPos.x *= Utils.RandFloat(0.5f, 0.7f);
                    randPos = randPos.RotatedBy(randRot);
                    ParticleManager.NewParticle(target.transform.position, Utils.RandFloat(0.95f, 1.05f), -norm * 4.5f + randPos * Utils.RandFloat(4, 5) + Utils.RandCircle(.3f), 0.1f, .6f, 0, SpriteRenderer.color);
                }
                int stars = 3 + Player.Instance.Starbarbs * 2;
                for (; stars > 0; --stars)
                {
                    Vector2 targetPos = (Vector2)target.transform.position + norm * 9 + Utils.RandCircle(7);
                    NewProjectile<StarProj>(target.transform.position, norm.RotatedBy(Utils.RandFloat(360) * Mathf.Deg2Rad) * -Utils.RandFloat(16f, 24f), targetPos.x, targetPos.y);
                }
            }
            if(Player.Instance.LuckyStar > 0)
            {
                float chance = 0.04f + Player.Instance.LuckyStar * 0.02f;
                if(Utils.RandFloat(1) < chance)
                {
                    PowerUp.Spawn(PowerUp.RandomFromPool(), (Vector2)target.transform.position, 0);
                }
            }
        }
    }
}
public class FlamingoFeather : Projectile
{
    public override void Init()
    {
        transform.localScale = new Vector3(0.65f, 0.45f, 1);
        SpriteRendererGlow.transform.localScale = new Vector3(1.5f, 4f, 1);
        SpriteRendererGlow.color = new Color(253 / 255f, 181 / 255f, 236 / 255f, 0.5f);
        SpriteRenderer.sprite = Main.Feather;
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
            RB.velocity += RB.velocity.normalized * 0.005f;
            RB.velocity += (Player.Position - (Vector2)transform.position).normalized * 0.07f;
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
        SpriteRenderer.sprite = Main.Laser;
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
        Damage = 15;
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