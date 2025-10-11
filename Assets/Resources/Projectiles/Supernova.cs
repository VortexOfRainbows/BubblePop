using System;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
public class SupernovaProj : Projectile
{
    private Color ColorVar;
    public override void Init()
    {
        ColorVar = (Color.yellow * 0.9f).WithAlpha(0.5f);
        SpriteRenderer.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRenderer.color = Color.clear;
        SpriteRendererGlow.color = Color.clear;
        SpriteRenderer.material = SpriteRendererGlow.material;
        Penetrate = -1;
        transform.localScale *= 0.1f;
        Friendly = false;
    }
    public override bool CanBeAffectedByHoming()
    {
        return true;
    }
    public override bool DoHomingBehavior(Enemy target, Vector2 norm, float scale)
    {
        return base.DoHomingBehavior(target, norm, scale);
    }
    public bool Recalled = false;
    public int Counter = 0;
    public override void AI()
    {
        float percent = timer++ / 80f;
        float sqrtP = Mathf.Sqrt(percent);
        SpriteRenderer.color = Color.Lerp(ColorVar, new Color(0.56f, 0.3f, 0.67f) * 2, percent * 0.8f + 0.2f * sqrtP);
        SpriteRendererGlow.color = SpriteRenderer.color;
        transform.localScale = transform.localScale.Lerp(Vector3.one * 0.85f, 0.2f);
        RB.velocity *= 0.935f;
        RB.rotation = Utils.WrapAngle(RB.velocity.ToRotation()) * Mathf.Rad2Deg;

        float sin = Mathf.Sin(Mathf.PI * percent);
        float sinusoid = 1.65f * (0.5f + 0.5f * sqrtP) * sin;
        SpriteRenderer.transform.localScale = Vector3.one * sinusoid;
        int total = 5;
        timer2 += total * 2;
        float size = (2.5f + total * 0.1f) * sinusoid;
        for (; timer2 >= 3; timer2 -= 3)
        {
            int i = Counter % total;
            float offset = i / (float)total * Mathf.PI * 2f;
            Vector2 circular = new Vector2(0, size - sqrtP * size).RotatedBy(percent * Mathf.PI * 1 + offset);
            ParticleManager.NewParticle(RB.position + circular, transform.localScale.x * 1.6f * percent, circular * 0.5f, 0.2f, Utils.RandFloat(0.1f, 0.15f), 2,
                (SpriteRendererGlow.color * (.5f - sqrtP * 0.4f)));
            Counter++;
        }
        if (Utils.RandFloat(1) < 0.5f)
        {
            Vector2 circleEdge = Utils.RandCircle(1).normalized * 0.6f;
            ParticleManager.NewParticle((Vector2)transform.position + circleEdge * size, 0.25f * sinusoid, -circleEdge * size * 5.2f, 0, 1.2f * (1 - percent), 5, SpriteRenderer.color * sin);
        }
        if (percent > 1)
        {
            Kill();
            return;
        }
    }
    public override void OnKill()
    {
        NewProjectile<SupernovaExplode>(transform.position, Vector2.zero, Damage);
    }
    public override bool OnInsideTile() => false;
    public override bool OnTileCollide(Collider2D collision) => false;
}
public class SupernovaExplode : Projectile
{
    private Color ColorVar;
    public override void Init()
    {
        ColorVar = new Color(0.56f, 0.3f, 0.67f) * 2;
        SpriteRenderer.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.color = SpriteRenderer.color = Color.clear;
        SpriteRenderer.material = SpriteRendererGlow.material;
        Penetrate = -1;
        transform.localScale *= 1.2f;
        cmp.c2D.radius *= 2.5f;
        Friendly = true;
        immunityFrames = 100;
    }
    public bool runOnce = true;
    public override void AI()
    {
        if(runOnce)
        {
            int c = 30;
            for (int i = 0; i < c; ++i)
            {
                float r2 = Utils.RandFloat(0.4f, 1.6f);
                Vector2 circular = new Vector2(r2, 0).RotatedBy(i / (float)(0.5f * c) * Mathf.PI);
                ParticleManager.NewParticle((Vector2)transform.position, 0.56f - r2 * 0.1f, circular * 10.2f, 2, Utils.RandFloat(0.4f, 0.9f), 5, ColorVar);
            }
            float r = Utils.RandFloat(-15, 15);
            c = 40;
            for (int i = 0; i < c; ++i)
            {
                float p = i / (float)c;
                Vector2 circular = new Vector2(Utils.RandFloat(2.0f, 2.1f), 0).RotatedBy(p * 2 * Mathf.PI);
                circular.y *= 0.3f;
                circular = circular.RotatedBy(r * Mathf.Deg2Rad);
                ParticleManager.NewParticle((Vector2)transform.position, 0.5f, circular * 10.2f, 0, Utils.RandFloat(0.55f, 0.7f), 5, ColorVar * 0.8f);
            }
            runOnce = false;
        }
        timer++;
        float percent = timer / 50f;
        float sqrtP = Mathf.Sqrt(percent);
        float sin = MathF.Sin(sqrtP * MathF.PI);
        Color targetColor = Color.Lerp(ColorVar, Color.clear, percent);
        SpriteRenderer.color = SpriteRendererGlow.color = Color.Lerp(SpriteRendererGlow.color, targetColor, 0.3f);
        transform.localScale = Vector2.one * (2.1f + sqrtP + 0.5f * sin);
        if (percent > 1)
        {
            Kill();
        }
    }
    public override bool CanBeAffectedByHoming()
    {
        return false;
    }
    public override void OnHitTarget(Entity target)
    {
        OnHitByStar(target);
    }
    public override bool OnInsideTile() => false;
    public override bool OnTileCollide(Collider2D collision) => false;
}
