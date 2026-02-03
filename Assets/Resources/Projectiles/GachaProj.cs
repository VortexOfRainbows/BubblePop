using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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
        Damage = 4 * (1 + 0.1f * Player.Instance.ConsolationPrize);
        Penetrate = 2;
        Friendly = true;
        if (Data1 == 1)
        {
            Damage = 10 + Player.Instance.PhilosophersStone;
            SpriteRenderer.sprite = Main.TextureAssets.CoinProj;
            c = ColorHelper.RarityColors[0];
            Penetrate = 2;
        }
        else if (Data1 == 2)
        {
            Damage = 20 + Player.Instance.PhilosophersStone * 2;
            SpriteRenderer.sprite = Main.TextureAssets.GoldProj;
            c = ColorHelper.RarityColors[4];
            Penetrate = 3;
        }
        else if (Data1 == 3)
        {
            Damage = 40 + Player.Instance.PhilosophersStone * 4;
            SpriteRenderer.sprite = Main.TextureAssets.GemProj;
            c = ColorHelper.RarityColors[1];
            Penetrate = -1;
        }
        else if(Data1 == 4)
        {
            Damage = Damage * 0.25f;
            c = ColorHelper.RarityColors[4] * 0.9f;
            Penetrate = -1;
            SpriteRenderer.sprite = Main.TextureAssets.FireProj;
            SpriteRenderer.color = SpriteRenderer.color.WithAlpha(0.5f);
            SpriteRendererGlow.transform.localScale *= 0.9f;
            SpriteRendererGlow.transform.localPosition = new Vector3(0, -0.1f);
            transform.localScale *= 0.5f;
            immunityFrames /= 2;
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
        float targetScale = 1f + Data1 * 0.125f;
        if (Data1 != 4)
        {
            RB.rotation += rtSpeed * Mathf.Sign(RB.velocity.x) * 1.4f;
            if ((int)timer % 3 == 0)
            {
                Vector2 norm = RB.velocity.normalized;
                ParticleManager.NewParticle((Vector2)transform.position - norm * 0.1f + Utils.RandCircle(transform.lossyScale.x * 0.3f), .35f, norm * -.75f, 0.8f, Utils.RandFloat(0.225f, 0.35f), Data1 == 2 ? 1 : Data1 == 3 ? 2 : 0, c.WithAlphaMultiplied(0.5f));
            }
        }
        else
        {
            SpriteRenderer.flipX = RB.velocity.x < 0;
            if ((int)timer % 5 == 0)
            {
                Vector2 norm = RB.velocity.normalized;
                ParticleManager.NewParticle((Vector2)transform.position - norm * 0.1f + Utils.RandCircle(transform.lossyScale.x * 0.3f), 2.0f, norm * 2.5f + Utils.RandCircle(3) + Vector2.up * 2.2f, 2.5f, Utils.RandFloat(0.5f, 0.7f), ParticleManager.ID.Pixel, c.WithAlphaMultiplied(0.5f));
            }
            targetScale = 0.9f;
            RB.velocity *= 0.9925f;
        }
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, 0.075f + 0.02f * rtSpeed);
        float FadeOutTime = 20;
        if (timer > deathTime + FadeOutTime)
        {
            Kill();
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
        Damage *= 0.8f; // 0.8f + 0.05f * Data1;
        if (Data1 == 0 || Data1 == 4)
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
        if (Data1 == 4)
            return;
        int c2 = Data.Length > 0 ? (int)Data1 * 2 + 3 : 3;
        for (int i = 0; i < c2; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(0, 0.5f), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(0.5f) * transform.localScale.x, Utils.RandFloat(0.3f, 0.5f), circular * Utils.RandFloat(4, 6), 4f, 0.36f, 0, c.WithAlphaMultiplied(0.8f));
        }
        AudioManager.PlaySound(SoundID.BubblePop, transform.position, 0.7f, 1.1f);
    }
    public override bool DoHomingBehavior(Enemy target, Vector2 norm, float scale)
    {
        float currentSpeed = RB.velocity.magnitude * 0.99f + Player.Instance.HomingRangeSqrt * 0.125f;
        float modAmt = 0.1f + Player.Instance.HomingRangeSqrt * 0.05f;
        RB.velocity = Vector2.Lerp((1 - modAmt) * RB.velocity, norm * currentSpeed, modAmt).normalized * currentSpeed;
        return false;
    }
}
public class GachaTokenProj : Projectile
{
    public Color c = ColorHelper.TokenColor;
    public SpecialTrail trail;
    public override void Init()
    {
        c.a = 0.68f;
        SpriteRenderer.sprite = Main.TextureAssets.TokenProj;
        timer2 = 0;
        transform.localScale *= 0.7f;
        Penetrate = -1;
        Friendly = true;
        RB.velocity *= 1 + 0.1f * Player.Instance.FasterBulletSpeed;
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.color = c;
        SpriteRendererGlow.transform.localScale *= 1.25f; //2.5
        C2D.radius *= 1.5f;
        startPos = transform.position;
        trail = SpecialTrail.NewTrail(transform, c * 0.9f, 1.8f, 0.18f, 0.3f);
    }
    public bool SwitchedPos = false;
    public float deathPercent = 1f;
    public override void AI()
    {
        float speed = RB.velocity.magnitude + (timer < 500 ? 0.2f : 0.0f);
        float rtSpeed = Mathf.Sqrt(speed);
        RB.rotation += rtSpeed * Mathf.Sign(RB.velocity.x) * 1.5f;
        float targetScale = 1f;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, 0.075f + 0.02f * rtSpeed);
        timer++;
        if (timer % 2 == 0)
        {
            ParticleManager.NewParticle(transform.position + new Vector3(Utils.RandFloat(-0.3f, 0.3f), Utils.RandFloat(-0.3f, 0.3f)), Utils.RandFloat(1.5f, 2.25f), -RB.velocity * Utils.RandFloat(0.0f, 0.2f), 2, Utils.RandFloat(0.5f, 0.6f), 3, ColorHelper.RarityColors[4] * 0.9f);
        }
        float deathArea = SwitchedPos ? 1f : 2f;
        Vector2 target = new(Data[0], Data[1]);
        if(SwitchedPos)
        {
            target = Player.Position;
            Data[0] = target.x;
            Data[1] = target.y;
        }
        Vector2 toTarget = target - (Vector2)transform.position;
        RB.velocity = Vector2.Lerp(RB.velocity * Mathf.Max(0.8f - timer / 2500f, (1.0f - timer / 550f)) * 0.75f, toTarget.normalized * speed, Mathf.Min(0.3f, 0.04f + timer / 550f)).normalized * speed;
        if(SwitchedPos)
        {
            deathPercent = Mathf.Lerp(deathPercent, 1 - Mathf.Clamp(1 - (toTarget.magnitude - 1f) / 12f, 0, 1), 0.2f);
            if (SwitchedPos)
                SpriteRenderer.color = SpriteRenderer.color.WithAlpha(deathPercent);
            SpriteRendererGlow.color = c * deathPercent;
            if (trail != null)
            {
                trail.Trail.startColor = SpriteRendererGlow.color * deathPercent * 0.9f;
                trail.originalAlpha = SpriteRendererGlow.color.a * deathPercent * 0.9f;
                trail.Trail.time = 0.18f * deathPercent;
            }
        }
        if (toTarget.magnitude < deathArea)
        {
            if(!SwitchedPos)
            {
                target = Player.Position;
                Data[0] = target.x;
                Data[1] = target.y;
                SwitchedPos = true;
            }
            else
            {
                Kill();
            }
        }
    }
    public override void OnHitTarget(Entity target)
    {
        Damage *= 0.5f;

        if (target.Life <= 0)
            CoinManager.SpawnToken(target.transform.position, 0.1f);

        AudioManager.PlaySound(SoundID.StarbarbImpact, transform.position, 0.3f, 0.475f, 0);
        AudioManager.PlaySound(SoundID.SoapDie, transform.position, 1.125f, 1.7f, 0);
        for (int i = 0; i < 15; ++i)
            ParticleManager.NewParticle(target.transform.position + new Vector3(Utils.RandFloat(-1f, 1f), Utils.RandFloat(-1f, 1f)), 3, RB.velocity * Utils.RandFloat(0.3f) + Utils.RandCircle(5), 5, Utils.RandFloat(0.7f, 0.8f), 3,
                Color.Lerp(ColorHelper.RarityColors[0], ColorHelper.RarityColors[4], Utils.RandFloat()) * 0.95f);
    }
    public override void OnKill()
    {

    }
    public override bool CanBeAffectedByHoming()
    {
        return true;
    }
    public override bool DoHomingBehavior(Enemy target, Vector2 norm, float scale)
    {
        Vector2 target2 = Vector2.Lerp(new Vector2(Data1, Data2), target.transform.position, 0.025f + Player.Instance.HomingRangeSqrt * 0.05f);
        Data[0] = target2.x;
        Data[1] = target2.y;
        return false;
    }
    public override bool OnInsideTile()
    {
        return false;
    }
    public override bool OnTileCollide(Collider2D collision)
    {
        if (SwitchedPos)
            return false;
        Vector2 closest = collision.ClosestPoint(transform.position);
        Vector2 diff = closest - RB.position;
        if (diff.magnitude > 1)
            return false;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            bool goingInThatDirection = Mathf.Sign(RB.velocity.x) == Mathf.Sign(diff.x);
            if (goingInThatDirection)
                RB.velocity = new Vector2(RB.velocity.x * -1.1f, RB.velocity.y);
        }
        else
        {
            bool goingInThatDirection = Mathf.Sign(RB.velocity.y) == Mathf.Sign(diff.y);
            if (goingInThatDirection)
                RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * -1.1f);
        }
        SwitchedPos = true;
        return false;
    }
}