using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Gatorade : Projectile
{
    public override void Init()
    {
        SpriteRendererGlow.color = new Color(245 / 255f, 191 / 255f, 7 / 255f);
        SpriteRenderer.sprite = Resources.Load<Sprite>("Projectiles/SODA");
        SpriteRendererGlow.transform.localScale *= 0.5f;
        cmp.c2D.radius *= 0.5f;
        timer += Utils.RandInt(41);
        transform.localScale *= 0.2f;
        Friendly = false;
        Hostile = true;
    }
    public override void AI()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 1.5f, 0.06f);
        transform.localEulerAngles = Vector3.forward * (RB.velocity.ToRotation() * Mathf.Rad2Deg - 90);
        RB.velocity *= 1.001f;
        float deathTime = 250;
        float FadeOutTime = 10;
        if (timer > deathTime + FadeOutTime)
        {
            Kill();
        }
        if (Utils.RandFloat() < 0.4f)
        {
            Vector2 norm = RB.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f, 1.2f, norm * -.75f, 0.8f, Utils.RandFloat(0.45f, 0.6f), 3, SpriteRendererGlow.color);
        }
        if (timer > deathTime)
        {
            float alphaOut = 1 - (timer - deathTime) / FadeOutTime;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alphaOut);
        }
        timer++;
    }
    public override void OnKill()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 circular = new Vector2(1, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(1, 2), circular * Utils.RandFloat(3, 6), 4f, 0.7f, 3, SpriteRendererGlow.color);
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.2f, 0.4f), circular * Utils.RandFloat(3, 6), 4f, 0.5f, 0, SpriteRendererGlow.color);
        }
        AudioManager.PlaySound(SoundID.BubblePop, transform.position, 0.7f, 0.8f);
    }
}
