using UnityEngine;
public class FlamingoFeather : Projectile
{
    public override void Init()
    {
        transform.localScale = new Vector3(0, 0, 1);
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
    public Player target;
    public void FeatherAI()
    {
        transform.LerpLocalScale(new Vector3(0.65f, 0.45f, 1), 0.05f);
        RB.rotation = RB.velocity.ToRotation() * Mathf.Rad2Deg + 90;

        if(timer % 10 == 0 && timer < 100)
            target = Player.FindClosest(transform.position, out Vector2 _, out float _, 50);
        timer++;
        if (timer < 300)
        {
            float magnitude = RB.velocity.magnitude + 0.03f;
            if(target != null)
            {
                RB.velocity += (target.Position - (Vector2)transform.position).normalized * 0.12f;
                RB.velocity = RB.velocity.normalized * magnitude;
            }
        }
        if (timer > 410)
        {
            float alphaOut = 1 - (timer - 410) / 90f;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alphaOut);
            SpriteRendererGlow.color = new Color(SpriteRendererGlow.color.r, SpriteRendererGlow.color.g, SpriteRendererGlow.color.b) * alphaOut;
            if (timer > 450)
                Hostile = false;
        }
        if (timer > 500)
        {
            Kill();
        }
        if (Utils.RandFloat(1) < 0.5f)
            ParticleManager.NewParticle((Vector2)transform.position, 0.5f, Utils.RandCircle(0.02f), 1.5f, 0.4f, 0, SpriteRendererGlow.color);
    }
}