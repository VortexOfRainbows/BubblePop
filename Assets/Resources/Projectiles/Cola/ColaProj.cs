using UnityEngine;
public class ColaProj : Projectile
{
    public Vector2 Destination => new(Data[0], Data[1]);
    public override void Init()
    {
        SpriteRenderer.color = SpriteRenderer.color.WithAlpha(0.85f);
        SpriteRenderer.sprite = Resources.Load<Sprite>("Player/Fizzy/bottle");
        timer2 = 0;
        transform.localScale *= 1f;
        Penetrate = -1;
        Friendly = false;
        SpriteRendererGlow.gameObject.SetActive(false);
    }
    public override void AI()
    {
        float deathTime = 180;
        if (++timer2 > 3)
            Friendly = true;
        Vector2 velo = RB.velocity;
        RB.velocity = velo;
        float speed = RB.velocity.magnitude;
        float rtSpeed = Mathf.Sqrt(speed);
        RB.rotation -= rtSpeed * Mathf.Sign(RB.velocity.x);
        float targetScale = 1.1f;

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, 0.075f + 0.02f * rtSpeed);

        float FadeOutTime = 20;
        if (timer > deathTime + FadeOutTime)
        {
            Kill();
        }
        if ((int)timer % 4 == 0)
        {
            Vector2 norm = RB.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f + Utils.RandCircle(transform.lossyScale.x * 0.4f), .225f, norm * -.75f, 0.6f, Utils.RandFloat(0.225f, 0.35f), 0, Player.ProjectileColor.WithAlphaMultiplied(0.8f));
        }
        if (timer > deathTime)
        {
            float alphaOut = 1 - (timer - deathTime) / FadeOutTime;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 0.68f * alphaOut);
        }
        timer++;
    }
    public override void OnKill()
    {
        int c = Data.Length > 0 ? (int)Data1 * 2 + 3 : 3;
        for (int i = 0; i < c; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(0, 0.5f), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(0.5f) * transform.localScale.x, Utils.RandFloat(0.3f, 0.5f), circular * Utils.RandFloat(4, 6), 4f, 0.36f, 0, Player.ProjectileColor.WithAlphaMultiplied(0.8f));
        }
        AudioManager.PlaySound(SoundID.BubblePop, transform.position, 0.5f, 1.1f);
    }
}