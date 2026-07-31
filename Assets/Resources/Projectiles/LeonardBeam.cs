using UnityEngine;
public class Laser : Projectile
{
    public override void Init()
    {
        transform.localScale = new Vector3(0f, 0f, 1);
        SpriteRendererGlow.transform.localScale = new Vector3(2.1f, 1.9f, 1.9f);
        SpriteRendererGlow.color = new Color(.66f, .243f, .2745f);
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.material = Main.TextureAssets.AdditiveShader;
        SpriteRenderer.sprite = Main.TextureAssets.Laser;
        SpriteRenderer.material = Main.TextureAssets.SpriteGlowmask;
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
        transform.LerpLocalScale(new Vector2(0.9f, 0.8f), 0.1f);
        RB.rotation = RB.velocity.ToRotation() * Mathf.Rad2Deg + 180;
        for (float i = 0; i < 1; i += 0.5f)
            ParticleManager.NewParticle((Vector2)transform.position + i * Time.fixedDeltaTime * RB.velocity, 0.55f, -RB.velocity.normalized * 2.5f, 0f, 0.2f, ParticleManager.ID.Circle, SpriteRendererGlow.color);
        if (timer < 200)
            RB.velocity += RB.velocity.normalized * 0.02f;
        if (timer > 610)
        {
            float alphaOut = 1 - (timer - 610) / 90f;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alphaOut);
            SpriteRendererGlow.color = new Color(SpriteRendererGlow.color.r, SpriteRendererGlow.color.g, SpriteRendererGlow.color.b, SpriteRendererGlow.color.a) * alphaOut;
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