using UnityEngine;

public class Bullet : Projectile
{
    public override void Init()
    {
        SpriteRendererGlow.color = new Color(245 / 255f, 191 / 255f, 7 / 255f);
        SpriteRenderer.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.transform.localScale *= 1.1f;
        SpriteRendererGlow.color = new Color(1, 0.1f, 0.1f, 1f);
        SpriteRenderer.color = new Color(1, 1f, 1f, 5f);
        SpriteRenderer.material = Resources.Load<Material>("Materials/Additive");
        SpriteRendererGlow.material = Resources.Load<Material>("Particles/Bubble2");
        cmp.c2D.radius *= 0.8f;
        transform.localScale *= 0.2f;
        Friendly = false;
        Hostile = true;
    }
    public override void AI()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.5f, 0.06f);
        transform.localEulerAngles = Vector3.forward * (RB.velocity.ToRotation() * Mathf.Rad2Deg - 90);
        RB.velocity *= 1.001f;
        float deathTime = 330;
        float FadeOutTime = 15;
        if (timer > deathTime + FadeOutTime)
        {
            Kill();
        }
        if (Utils.RandFloat() < 0.5f)
        {
            Vector2 norm = RB.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f, 1.2f, norm * -.75f, 0.5f, Utils.RandFloat(0.45f, 0.6f), 3, SpriteRendererGlow.color);
        }
        if (timer > deathTime)
        {
            float alphaOut = 1 - (timer - deathTime) / FadeOutTime;
            SpriteRenderer.color = SpriteRenderer.color.WithAlpha(alphaOut);
            SpriteRendererGlow.color = SpriteRendererGlow.color.WithAlpha(alphaOut);
        }
        timer++;
    }
    public override void OnKill()
    {
        Color c = new Color(1, 0.1f, 0.1f, 1f);
        for (int i = 0; i < 6; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(3), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position, Utils.RandFloat(0.3f, 0.4f), circular, 1f, 0.3f, 2, c);
            ParticleManager.NewParticle((Vector2)transform.position, Utils.RandFloat(2f, 4f), circular, 1f, 0.3f, 3, c);
        }
        AudioManager.PlaySound(SoundID.BubblePop, transform.position, 0.7f, 0.8f);
    }
}
