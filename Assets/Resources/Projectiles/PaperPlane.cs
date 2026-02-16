using System;
using UnityEngine;
public class PaperPlane : Projectile
{
    public override void Init()
    {
        Color c = Color.Lerp(Color.white, Player.ProjectileColor, 0.5f);
        c.a = 0.8f;
        SpriteRenderer.color = c;
        SpriteRenderer.sprite = Resources.Load<Sprite>("Projectiles/PaperPlane");
        if(Data.Length > 0)
            Data1 = 0;
        transform.localScale *= 0.1f;
        Damage = 2;
        Penetrate = 1;
        Friendly = true;
        SpriteRendererGlow.gameObject.SetActive(false);
    }
    public override bool CanBeAffectedByHoming()
    {
        return true;
    }
    public bool Recalled = false;
    public override void AI()
    {
        Vector2 velo = RB.velocity;
        float speed = RB.velocity.magnitude;
        float percent = Mathf.Max(0, 1 - timer / 40f);
        if(percent > 0)
            transform.position -= percent * Time.fixedDeltaTime * (Vector3)RB.velocity;
        float rtSpeed = Mathf.Sqrt(speed);
        RB.rotation = Utils.WrapAngle(RB.velocity.ToRotation()) * Mathf.Rad2Deg;
        cmp.spriteRenderer.flipY = RB.rotation > 90 || RB.rotation < -90;
        float targetScale = 0.75f;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, 0.04f + 0.01f * rtSpeed);
        if (Book.InClosingAnimation)
        {
            if(!Recalled)
            {
                Friendly = false;
                Recalled = true;
            }
            if(RB.position.Distance(Utils.MouseWorld) < 4)
            {
                RB.velocity *= 0.96f;
            }
            else
                RB.velocity *= 0.9825f;
        }
        else
        {
            if(Recalled)
            {
                Vector2 norm = velo.normalized;
                int shotCount = 5 + PlayerOwner.bonusBubbles;
                for (int i = 0; i < shotCount; ++i)
                {
                    float sp = i * 2 + 12;
                    NewProjectile<SmallBubble>((Vector2)transform.position, norm * sp, 1, PlayerOwner);
                }
                Kill();
                return;
            }
            RB.velocity = velo * 1.002f;
        }

        float deathTime = 240;
        if (PlayerOwner.EternalBubbles > 0)
        {
            int bonus = PlayerOwner.EternalBubbles;
            if (bonus > 9)
            {
                deathTime += 10 * (bonus - 9);
                bonus = 9;
            }
            deathTime += 40 + 40 * bonus;
        }
        float FadeOutTime = 20;
        if (timer > deathTime + FadeOutTime)
        {
            Kill();
        }
        if ((int)++timer2 % 2 == 0)
        {
            Vector2 norm = RB.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.5f, .225f, norm * -.25f, 0.75f, Utils.RandFloat(0.2f, 0.3f), 1, SpriteRenderer.color.WithAlphaMultiplied(0.8f));
        }
        if (timer > deathTime)
        {
            float alphaOut = 1 - (timer - deathTime) / FadeOutTime;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 0.8f * alphaOut);
        }
        if(!Recalled)
            timer++;
        HomeTowardsCursor();
    }
    public void HomeTowardsCursor()
    {
        Vector2 toCursor = Utils.MouseWorld - (Vector2)transform.position;
        float speed = RB.velocity.magnitude;
        float lerpAmt = 0.036f + 0.01f * PlayerOwner.HomingRangeSqrt;
        RB.velocity = Vector2.Lerp(RB.velocity, toCursor.normalized * speed, lerpAmt).normalized * speed;
    }
    public override void OnKill()
    {
        int c = Data.Length > 0 ? (int)Data1 * 2 + 3 : 3;
        for (int i = 0; i < c; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(0, 0.5f), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(0.5f) * transform.localScale.x, Utils.RandFloat(0.3f, 0.5f), circular * Utils.RandFloat(4, 6), 4f, 0.36f, 0, Player.ProjectileColor.WithAlphaMultiplied(0.8f));
        }
    }
}
public class LatentCharge : Projectile
{
    private Color ColorVar;
    public override void Init()
    {
        ColorVar = Color.Lerp(Player.ProjectileColor, Color.blue, 0.5f);
        SpriteRenderer.sprite = Main.TextureAssets.BubbleSmall;
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRenderer.color = Color.clear;
        SpriteRendererGlow.color = Color.clear;
        Damage = 0;
        Penetrate = -1;
        transform.localScale *= 0.1f;
        Friendly = false;
    }
    public override bool CanBeAffectedByHoming()
    {
        return true;
    }
    public bool Recalled = false;
    public override void AI()
    {
        SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, ColorVar.WithAlphaMultiplied(0.5f), 0.1f);
        SpriteRendererGlow.color = Color.Lerp(SpriteRendererGlow.color, ColorVar * 1f, 0.1f);
        transform.localScale = transform.localScale.Lerp(Vector3.one * 0.85f, 0.2f);
        RB.velocity *= 0.5f;
        RB.rotation = Utils.WrapAngle(RB.velocity.ToRotation()) * Mathf.Rad2Deg;
        if (Book.InClosingAnimation)
        {
            if (!Recalled)
            {
                Friendly = false;
                Recalled = true;
            }
            else
            {
                float percent = Book.ClosingPercent;
                int total = Math.Min(15, (int)Mathf.Pow(Data1 + 1, 0.75f) + 1);
                timer2 += total;
                float size = 1.9f + total * 0.1f;
                for(; timer2 >= 3; timer2 -= 3)
                {
                    int i = (int)timer % total;
                    float offset = i / (float)total * Mathf.PI * 2f;
                    Vector2 circular = new Vector2(0, size - percent * size).RotatedBy(percent * Mathf.PI * 1 + offset);
                    ParticleManager.NewParticle(RB.position + circular, transform.localScale.x * 2.2f * percent, circular * 0.5f, 0.2f, Utils.RandFloat(0.1f, 0.15f), 2,
                        SpriteRendererGlow.color.WithAlphaMultiplied(.6f - percent * 0.3f));
                    timer++;
                }
                //Pylon.SummonLightning2(RB.position + circular, RB.position - circular.normalized.RotatedBy(Mathf.PI * 0.5f), ColorVar, 0.1f);
            }
        }
        else
        {
            if (Recalled)
            {
                Kill();
                return;
            }
        }
    }
    public override void OnKill()
    {
        float amt = Data1;
        float speed = (4.0f + amt * 0.6f) * (0.8f + 0.2f * PlayerOwner.ZapRadiusMult);
        for (int i = 0; i < Data1; i++)
            NewProjectile<SmallBubble>(transform.position, new Vector2(speed * Mathf.Sqrt(Utils.RandFloat(0.0f, 1.1f)), 0).RotatedBy((i + Utils.RandFloat(1)) / (int)amt * Mathf.PI * 2f), 1, PlayerOwner);
        int c = Data.Length > 0 ? (int)Mathf.Sqrt(Data1 + 6) * 2 + 3 : 3;
        for (int i = 0; i < c; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(0, 0.5f), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(0.5f) * transform.localScale.x, Utils.RandFloat(0.5f, 0.6f), circular * Utils.RandFloat(4, 6), 4f, Utils.RandFloat(0.5f, 0.6f), 2, SpriteRendererGlow.color);
        }
    }
    public override bool DoHomingBehavior(Enemy target, Vector2 norm, float scale)
    {
        return false;
    }
}
