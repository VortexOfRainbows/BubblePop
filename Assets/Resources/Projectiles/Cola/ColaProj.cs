using System;
using Unity.VisualScripting;
using UnityEngine;

public class ColaProj : Projectile
{
    public Vector2 Destination => new(Data[0], Data[1]);
    public Vector2 playerStartPos = Vector2.zero;
    public override void Init()
    {
        SpriteRenderer.color = SpriteRenderer.color.WithAlpha(0.85f);
        SpriteRenderer.sprite = Resources.Load<Sprite>("Player/Fizzy/bottle");
        SpriteRenderer.flipX = Data[2] < 0;
        timer2 = 0;
        transform.localScale *= 1f;
        Penetrate = -1;
        Friendly = false;
        SpriteRendererGlow.gameObject.SetActive(false);
        startPos = transform.position;
        playerStartPos = PlayerOwner.lastPos;
    }
    public override void AI()
    {
        float speedMult = PlayerOwner.SecondaryAttackSpeedModifier * 0.5f + 0.5f;
        float speed = RB.velocity.magnitude;
        if (speed < 1)
            speed = 1;
        float rtSpeed = Mathf.Sqrt(speed) * speedMult;
        RB.rotation -= rtSpeed * Mathf.Sign(RB.velocity.x);
        float targetScale = 1.1f;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, 0.075f + 0.02f * rtSpeed);

        if ((int)timer % 4 == 0)
        {
            Vector2 norm = RB.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f + Utils.RandCircle(transform.lossyScale.x * 0.4f), .225f, norm * -.75f, 0.6f, Utils.RandFloat(0.225f, 0.35f), 0, Player.ProjectileColor.WithAlphaMultiplied(0.8f));
        }

        float dist = startPos.Distance(Destination);
        float timeNeeded = 0.9f + dist / speed;
        float percent = timer / 100f;
        timer += speedMult / timeNeeded;
        if (percent > 1)
        {
            Kill();
            return;
        }
        Vector2 pos = Vector2.Lerp(startPos, Destination, percent);
        float arcY = Mathf.Sin(percent * Mathf.PI) * dist * 0.4f;
        pos.y += arcY;
        RB.MovePosition(pos);
    }
    public void Update()
    {
        float percent = timer / 100f;
        float sin = Mathf.Sin(percent * Mathf.PI);
        Vector3 drawPos = Vector2.Lerp(playerStartPos, Destination, percent);
        drawPos.y -= 0.5f;
        SpriteBatch.Draw(Main.TextureAssets.Shadow, drawPos, new Vector2(2.0f, 1.3f), 0, 
            new Color(0, 0, 0, 0.5f * sin), -50, Main.TextureAssets.AlphaShader);
    }
    public override void OnKill()
    {
        float exitSpeed = 8;
        int projCount = 8;
        int c = projCount * 5;
        for (int i = 0; i < c; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(0, exitSpeed), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(0.5f), 
                Utils.RandFloat(0.33f, .44f), circular * Utils.RandFloat(2, 4), 4f, 
                0.9f, 0, Player.ProjectileColor);
        }
        AudioManager.PlaySound(SoundID.BubblePop, transform.position, 0.5f, 1.1f);
        for(int i = 0; i < projCount; ++i)
        {
            float p = (float)i / projCount;
            Vector2 direction = new Vector2(exitSpeed, 0).RotatedBy(p * Mathf.PI * 2);
            Projectile.NewProjectile<SmallBubble>(Destination, 
                direction, 1, PlayerOwner, 0, 0, Data[2] < 0 ? -1 : 1);
        }
        Projectile.NewProjectile<ColaExplode>(Destination, Vector2.zero, Damage, PlayerOwner, exitSpeed / 8f);
    }
    public override bool? CanBeAffectedByHoming() => true;
    public override Vector3 HomingStartPosition() => Destination;
    public override bool DoHomingBehavior(Enemy target, Vector2 norm, float range)
    {
        Vector2 target2 = Vector2.Lerp(Destination, target.transform.position, 0.025f + PlayerOwner.HomingRangeSqrt * 0.05f);
        Data[0] = target2.x;
        Data[1] = target2.y;
        return false;
    }
}
public class ColaExplode : SupernovaExplode
{
    private Color ColorVar;
    public override void Init()
    {
        ColorVar = Player.ProjectileColor;
        SpriteRenderer.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.color = SpriteRenderer.color = Color.clear;
        SpriteRenderer.material = SpriteRendererGlow.material;
        Penetrate = -1;
        transform.localScale *= 1.2f;
        cmp.c2D.radius *= 2.0f * Data1;
        Friendly = true;
        immunityFrames = 100;
    }
    public override void AI()
    {
        if (runOnce)
        {
            int c = 30;
            for (int i = 0; i < c; ++i)
            {
                float r2 = Utils.RandFloat(0.4f, 1.6f);
                Vector2 circular = new Vector2(r2, 0).RotatedBy(i / (float)(0.5f * c) * Mathf.PI);
                ParticleManager.NewParticle((Vector2)transform.position, 0.56f - r2 * 0.1f, circular * 10.2f, 2, Utils.RandFloat(0.4f, 0.9f), 5, ColorVar);
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
        if (percent > 0.5f)
            Friendly = false;
        if (percent > 1)
        {
            Kill();
        }
    }
    public override void OnHitTarget(Entity target)
    {
        //do not do the original thing
    }
}