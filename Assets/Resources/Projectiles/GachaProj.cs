using System.IO;
using UnityEngine;

public class GachaProj : Projectile
{
    public Color c = Player.ProjectileColor;
    public override void Init()
    {
        c.a = 0.68f;
        SpriteRenderer.color = Data1 == 0 ? c : Color.white;
        SpriteRenderer.sprite = Main.TextureAssets.BubbleSmall;
        timer2 = 0;
        transform.localScale *= 0.5f;
        Damage = 5 + Player.Instance.ConsolationPrize;
        Penetrate = 1;
        Friendly = true;
        if (Data1 == 1)
        {
            Damage = 10 + Player.Instance.PhilosophersStone;
            SpriteRenderer.sprite = Main.TextureAssets.CoinProj;
            c = ColorHelper.RarityColors[0];
        }
        else if (Data1 == 2)
        {
            Damage = 20 + Player.Instance.PhilosophersStone * 2;
            SpriteRenderer.sprite = Main.TextureAssets.GoldProj;
            c = ColorHelper.RarityColors[4];
            Penetrate = 2;
        }
        else if (Data1 == 3)
        {
            Damage = 40 + Player.Instance.PhilosophersStone * 4;
            SpriteRenderer.sprite = Main.TextureAssets.GemProj;
            c = ColorHelper.RarityColors[1];
            Penetrate = -1;
        }
        RB.velocity *= 1 + 0.1f * Player.Instance.FasterBulletSpeed;
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.color = c * 0.875f;
        SpriteRendererGlow.transform.localScale *= 0.75f;
    }
    public override void AI()
    {
        float deathTime = 180;
        float speed = RB.velocity.magnitude;
        float rtSpeed = Mathf.Sqrt(speed);
        RB.rotation += rtSpeed * Mathf.Sign(RB.velocity.x) * 1.4f;
        float targetScale = 1f + Data1 * 0.125f;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, 0.075f + 0.02f * rtSpeed);
        float FadeOutTime = 20;
        if (timer > deathTime + FadeOutTime)
        {
            Kill();
        }
        if ((int)timer % 3 == 0)
        {
            Vector2 norm = RB.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.1f + Utils.RandCircle(transform.lossyScale.x * 0.3f), .35f, norm * -.75f, 0.8f, Utils.RandFloat(0.225f, 0.35f), Data1 == 2 ? 1 : Data1 == 3 ? 2 : 0, c.WithAlphaMultiplied(0.5f));
        }
        if (timer > deathTime)
        {
            float alphaOut = 1 - (timer - deathTime) / FadeOutTime;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 0.68f * alphaOut);
            SpriteRendererGlow.color = c * 0.8f * alphaOut;
        }
        timer++;
    }
    public override void OnHitTarget(Entity target)
    {
        if (Data1 == 0)
            return;
        float count = 1;
        if (Data1 == 2)
            count = 3;
        else if (Data1 == 3)
            count = 5;
        count *= 1.0f + 0.5f * Player.Instance.PhilosophersStone;
        float bonus = count - (int)count;
        if (Utils.RandFloat() < bonus)
            count++;
        CoinManager.SpawnCoin(transform.position, (int)count, 1);
    }
    public override void OnKill()
    {
        int c2 = Data.Length > 0 ? (int)Data1 * 2 + 3 : 3;
        for (int i = 0; i < c2; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(0, 0.5f), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(0.5f) * transform.localScale.x, Utils.RandFloat(0.3f, 0.5f), circular * Utils.RandFloat(4, 6), 4f, 0.36f, 0, c.WithAlphaMultiplied(0.8f));
        }
        AudioManager.PlaySound(SoundID.BubblePop, transform.position, 0.7f, 1.1f);
    }
}