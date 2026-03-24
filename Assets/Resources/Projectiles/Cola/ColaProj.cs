using System;
using UnityEngine;

public class ColaProj : Projectile
{
    public Vector2 Destination => new(Data[0], Data[1]);
    public Vector2 playerStartPos = Vector2.zero;
    public float SpeedLockContribution = 0.8f;
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
        if(Data.Length > 4)
        {
            playerStartPos = startPos;
            SpeedLockContribution = 0.8f * (10f / (10f + Data[4]));
        }
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
        float timeNeeded = SpeedLockContribution + dist / speed;
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
        int projCount = 8 + 4 * PlayerOwner.BubbleBlast;
        int c = projCount * 5;
        for (int i = 0; i < c; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(0, exitSpeed), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(0.5f), 
                Utils.RandFloat(0.33f, .44f), circular * Utils.RandFloat(2, 4), 4f, 
                0.9f, 0, Player.ProjectileColor);
        }
        for(int i = 0; i < projCount; ++i)
        {
            float p = (float)i / projCount;
            Vector2 direction = new Vector2(exitSpeed, 0).RotatedBy(p * Mathf.PI * 2);
            Projectile.NewProjectile<SmallBubble>(Destination, 
                direction, 1, PlayerOwner, 0, 0, Data[2] < 0 ? -1 : 1);
        }
        if (Data[3] > 0)
        {
            Vector2 toBouncePosition = Utils.RandCircleEdge(4 + 6 * SpeedLockContribution);
            Vector2 toPlayer = PlayerOwner.Position - (Vector2)transform.position;
            toBouncePosition += toPlayer.normalized;
            Vector2 bouncePosition = toBouncePosition + (Vector2)transform.position;
            Projectile.NewProjectile<ColaProj>(Destination, toBouncePosition.normalized * (RB.velocity.magnitude * 1.25f), Damage,
                PlayerOwner, bouncePosition.x, bouncePosition.y, Data[2], Data[3] - 1, Data.Length > 4 ? Data[4] + 1 : 1);
        }
        Projectile.NewProjectile<ColaExplode>(Destination, Vector2.zero, Damage, PlayerOwner, exitSpeed / 8f * 1.2f);
        AudioManager.PlaySound(SoundID.BathBombBurst, transform.position, 0.5f, 1.1f);
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
    public override bool OnInsideTile() => false;
    public override bool OnTileCollide(Collider2D collision) => false;
}
public class ColaExplode : SupernovaExplode
{
    public override bool? CanBeAffectedByHoming()
    {
        return false;
    }
    private Color ColorVar;
    public override void Init()
    {
        ColorVar = Player.ProjectileColor;
        SpriteRenderer.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.color = SpriteRenderer.color = Color.clear;
        SpriteRenderer.material = SpriteRendererGlow.material;
        Penetrate = -1;
        Data1 *= PlayerOwner.ExplodeRadiusMult;
        transform.localScale *= 1.0f * Data1;
        cmp.c2D.radius *= 2.0f;
        Friendly = true;
        immunityFrames = 100;
        if (Data.Length > 1)
            ColorVar *= Data2;
    }
    public override void AI()
    {
        if (runOnce)
        {
            float c = 20 * Data1;
            if (c > 60)
                c = 60;
            if (Data.Length > 1)
                c *= Data2;
            for (int i = 0; i < c; ++i)
            {
                float r2 = Utils.RandFloat(0.4f, 1.6f) * Data1;
                Vector2 circular = new Vector2(r2, 0).RotatedBy(i / (float)(0.5f * c) * Mathf.PI);
                float size = (0.46f + 0.1f * Data1) - r2 * 0.1f;
                ParticleManager.NewParticle((Vector2)transform.position, Mathf.Clamp(size, 0.1f, 1), circular * 10.2f, 2, Utils.RandFloat(0.4f, 0.9f), 5, ColorVar);
            }
            runOnce = false;
        }
        timer++;
        float timeOut = 50;
        if (Data.Length > 1)
            timeOut = timeOut * Data2;
        float percent = timer / timeOut;
        float sqrtP = Mathf.Sqrt(percent);
        float sin = MathF.Sin(sqrtP * MathF.PI);
        Color targetColor = Color.Lerp(ColorVar, Color.clear, percent);
        SpriteRenderer.color = SpriteRendererGlow.color = Color.Lerp(SpriteRendererGlow.color, targetColor, 0.3f);
        transform.localScale = (2.1f + sqrtP + 0.5f * sin) * Data1 * Vector2.one;
        if (percent > 0.5f)
            Friendly = false;
        if (percent > 1)
        {
            Kill();
        }
    }
    public override void OnHitTarget(Entity target)
    {
        //do not remove this. We don't want the original effect of working with starbarbs from inheriting supernova
    }
}
public class SkateboardProj : Projectile
{
    public Transform Wheel1, Wheel2;
    public float Angle => Data1;
    public override void Init()
    {
        SpriteRenderer.enabled = false;
        SpriteRendererGlow.enabled = false;
        SpriteRenderer.color = PlayerOwner.Body.PrimaryColor;
        Friendly = true;
        GameObject t = Instantiate(Resources.Load<GameObject>("Player/Fizzy/Skateboard"), transform, false);
        float xReduce = PlayerOwner.Accessory is BubblemancerCape || PlayerOwner.Accessory is LabCoat ? 0.7f : 1.0f;
        t.transform.localScale = new Vector3(xReduce, 1, 1);
        Wheel1 = t.transform.GetChild(0).GetChild(0);
        Wheel2 = t.transform.GetChild(0).GetChild(1);
        Penetrate = -1;
        cmp.c2D.offset = new Vector2(0, 0.525f);
        cmp.c2D.radius = 1.1f;
    }
    public float starTimer = 0.2f;
    public override void AI()
    {
        float speed = RB.velocity.magnitude * 0.9f * -Utils.SignNoZero(RB.velocity.x);
        Wheel1.transform.SetLocalEulerZ(Wheel1.transform.localEulerAngles.z + speed);
        Wheel2.transform.SetLocalEulerZ(Wheel2.transform.localEulerAngles.z + speed);
        int trail = PlayerOwner.DashSparkle;
        if (trail > 0)
        {
            starTimer -= Time.fixedDeltaTime;
            while (starTimer <= 0)
            {
                starTimer += 4f / (PlayerOwner.PassiveAttackSpeedModifier * (trail + 3f)); //4/4, 4/5, 4/6, 4/7, etc
                Vector2 circular = (Utils.RandCircle(1.3f) - RB.velocity).normalized;
                Vector2 randPos = (Vector2)transform.position + Utils.RandCircleEdge(3);
                Projectile.NewProjectile<StarProj>((Vector2)transform.position + circular * 2 + new Vector2(0, 0.3f), circular * 8, 2, PlayerOwner, randPos.x, randPos.y);
            }
        }
        Vector2 norm = RB.velocity.normalized;
        RB.rotation = norm.x * 5 + norm.y * 5;
        if ((int)timer % 2 == 0)
        {
            ParticleManager.NewParticle((Vector2)transform.position + new Vector2(0, .3f) + Utils.RandCircle(0.25f) - norm * .5f, .5f, norm * -Utils.RandFloat(15f), 1.0f, 0.6f, 0, SpriteRenderer.color);
        }
        timer++;
        if(RB.velocity.magnitude < 40)
        {
            RB.velocity += norm * 0.1f;
            if (RB.velocity.magnitude > 40)
                RB.velocity = norm * 40;
            RB.velocity = RB.velocity.RotatedBy(Angle * Mathf.Deg2Rad);
        }
        if(timer > 1000)
            Kill();
    }
    public override void OnKill()
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 circular = new Vector2(1, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.3f, 0.6f), circular * Utils.RandFloat(3, 6), 4f, 0.4f, 0, SpriteRenderer.color);
        }
        for (int i = 0; i < 24; i++)
        {
            Vector2 circular = new Vector2(1, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.3f, 0.6f), circular * Utils.RandFloat(3, 6), 4f, 0.4f, ParticleManager.ID.Square, Color.Lerp(Color.gray, Color.black, Utils.RandFloat()));
        }
        AudioManager.PlaySound(SoundID.BathBombBurst, transform.position, 0.7f, 0.6f);
    }
    public override void OnHitTarget(Entity target)
    {
        PopupText.NewPopupText(target.transform.position, new Vector2(0, 7) + Utils.RandCircle(4) + target.RB.velocity * 0.3f, Utils.PastelRainbow(Utils.RandFloat(Mathf.PI * -0.75f, Mathf.PI * 0.25f), 0.55f, default), Fizzy.CoolWords[Utils.RandInt(Fizzy.CoolWords.Length)], false, 0.8f, 50);
    }
}