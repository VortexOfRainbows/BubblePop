using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        float deathTime = 100;
        float FadeOutTime = 20;
        transform.localScale = Vector3.Lerp(transform.localScale, (1f - 0.8f * timer / deathTime) * 3 * Vector3.one, 0.1f);

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
            SpriteRenderer.color = SpriteRenderer.color.WithAlpha(0.5f * alphaOut);
            SpriteRendererGlow.color = new Color(SpriteRendererGlow.color.r, SpriteRendererGlow.color.g, SpriteRendererGlow.color.b) * alphaOut;
        }
        timer++;
    }
    public override void OnHitTarget(Entity target)
    {
        if (PlayerOwner.Body is Gachapon && target is EnemyBossDuck)
        {
            if (target.Life <= 0)
            {
                UnlockCondition.Get<GachaponBubblebirb>().SetComplete();
            }
        }
    }
}
public class OilFire : Projectile
{
    public override void Init()
    {
        transform.localScale = Vector3.zero;
        SpriteRenderer.color = new Color(1, 1, 1, 0.5f);
        SpriteRenderer.sprite = Main.TextureAssets.FireOffsetProj;
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.transform.localPosition = new Vector3(0, 0.5f, 0);
        SpriteRendererGlow.transform.localScale = new Vector3(2f, 2f, 2f);
        SpriteRendererGlow.color = ColorHelper.BurningOilColor * 0.5f;
        cmp.c2D.offset = new Vector2(0, .5f);
        cmp.c2D.radius *= 1.25f;
        immunityFrames = 200;
        Penetrate = -1;
        Friendly = true;
        Hostile = false;
        for(int i = 0; i < 6; ++i)
                ParticleManager.NewParticle(transform.position, Utils.RandFloat(1f, 1.5f) * Data1, Utils.RandCircle(2.5f), 0.5f, Utils.RandFloat(0.9f, 1.3f), ParticleManager.ID.Fire, Color.white.WithAlpha(0.5f));
    }
    public override void AI()
    {
        float deathTime = 100;
        float FadeOutTime = 20;
        float deathPercent = Mathf.Sqrt(Mathf.Max(0, 1 - timer / deathTime));
        transform.localScale = Vector3.Lerp(transform.localScale, deathPercent * 2 * Data1 * Vector3.one, 0.2f);

        Vector2 velo = RB.velocity;
        velo *= 0.99f;
        RB.velocity = velo;

        if (timer > deathTime + FadeOutTime)
        {
            Kill();
        }
        if (timer > deathTime)
        {
            float alphaOut = 1 - (timer - deathTime) / FadeOutTime;
            SpriteRenderer.color = SpriteRenderer.color.WithAlpha(alphaOut * 0.5f);
            SpriteRendererGlow.color = SpriteRendererGlow.color * alphaOut * 0.5f;
        }
        timer++;
    }
    public override void OnHitTarget(Entity target)
    {
        if (target is Enemy e)
            e.DetonateAllDebuffs();
    }
    public override bool OnInsideTile()
    {
        return false;
    }
    public override bool OnTileCollide(Collider2D collision)
    {
        return false;
    }
}