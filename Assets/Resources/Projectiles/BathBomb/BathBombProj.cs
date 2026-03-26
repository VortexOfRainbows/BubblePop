using UnityEngine;

public class BathBomb : Projectile
{
    public Vector2 Destination
    {
        get => new(Data[0], Data[1]);
        set
        {
            Data[0] = value.x;
            Data[1] = value.y;
        }
    }
    public Vector2 playerStartPos = Vector2.zero;
    public float BounceTimeNeeded = 67f;
    public override void Init()
    {
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.color = new Color(0.0f, 0.0f, 0.0f);
        SpriteRendererGlow.sortingOrder = SpriteRenderer.sortingOrder + 1;
        SpriteRenderer.sprite = Main.TextureAssets.BathBombSprite;
        Friendly = Hostile = false;
        transform.localScale = new Vector3(0, 0, 1);
        startPos = transform.position;
        playerStartPos = PlayerOwner.lastPos;
        if (Data.Length > 2)
            playerStartPos = startPos;
        if (Data.Length > 4)
            BounceTimeNeeded *= 0.9f;
        BounceTimeNeeded *= Utils.RandFloat(0.9f, 1.0f);
    }
    public override void AI()
    {
        float speedMult = PlayerOwner.PassiveAttackSpeedModifier * 0.2f + 0.8f;
        float speed = RB.velocity.magnitude;
        float rtSpeed = Mathf.Sqrt(speed) * speedMult;
        RB.rotation -= rtSpeed * Mathf.Sign(RB.velocity.x);

        if ((int)timer % 4 == 0)
        {
            Vector2 norm = RB.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f + Utils.RandCircle(transform.lossyScale.x * 0.4f), .225f, norm * -.75f, 0.6f, Utils.RandFloat(0.225f, 0.35f), 0, Player.ProjectileColor.WithAlphaMultiplied(0.8f));
        }

        float dist = BounceTimeNeeded / 20f / (timer2 + 1) + startPos.Distance(Destination);
        float percent = timer / BounceTimeNeeded;
        timer += speedMult;
        if (percent > 1)
        {
            timer2++;
            if (timer2 > 6)
                Kill();
            else
            {
                AudioManager.PlaySound(SoundID.StarbarbImpact, transform.position, 0.8f * (Data.Length > 3 ? Data[3] : 1.0f), 1.2f + 0.2f * timer2, 0);
                playerStartPos = startPos = Destination;
                Destination += RB.velocity * (Time.fixedDeltaTime * 50);
                RB.velocity *= 0.5f;
                timer = 0;
                percent = 0;
                BounceTimeNeeded *= 0.75f;
            }
        }
        Vector2 pos = Vector2.Lerp(startPos, Destination, percent);
        float sin = Mathf.Sin(percent * Mathf.PI);
        float arcY = sin * dist * 0.4f;
        pos.y += arcY;
        RB.MovePosition(pos);

        float targetScale = 1.0f + sin * 0.05f * (timer2 + 2);
        targetScale *= Data.Length > 3 ? Data[3] : 1.0f;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, 0.075f + 0.02f * rtSpeed);
        SpriteRendererGlow.transform.LerpLocalScale(Vector2.one * (2 + timer2 * 0.15f), .1f);
        SpriteRendererGlow.color = Color.Lerp(SpriteRendererGlow.color, new Color(sin + timer2 * 0.1f, 0.0f, 0.0f), 0.1f);
    }
    public override void OnKill()
    {
        float range = 1.0f + Mathf.Sqrt(Damage / 10f) * 0.75f; //Starting damage is 10, so scaling is based off of that
        range *= Data.Length > 3 ? Data[3] : 1.0f;
        AudioManager.PlaySound(SoundID.BathBombBurst, transform.position, 1.1f, 0.7f);
        float count = Data.Length > 2 ? Data[2] : 8;
        float actualCount = Mathf.Round(count * (1 + PlayerOwner.BonusShrapnel) + 0.49f);
        if (actualCount > 1024)
            actualCount = 1024; //Most items have no hard limit, but I feel this is necessary
        for (int i = 0; i < actualCount; i++)
        {
            float r = Mathf.PI * i / actualCount * 2;
            Vector2 circular = new Vector2(1, 0).RotatedBy(r + Utils.RandFloat(-Mathf.PI / actualCount, Mathf.PI / actualCount));
            NewProjectile<BathBombShrapnel>((Vector2)transform.position + circular * 0.5f, circular * Utils.RandFloat(1, 8), Damage / 2f, Owner, 0, Data2);
        }
        if (PlayerOwner.ClusterBomb > 0 && Data.Length < 5)
        {
            int count2 = Data.Length <= 2 ? 3 : Data[3] > 0.7f ? 2 : 1;
            float size = Data.Length > 2 ? Data[3] * 0.9f : 0.9f;
            for (int i = 0; i < count2; ++i)
            {
                Vector2 velo = Utils.RandCircle(3);
                Vector2 endPos = velo + (Vector2)transform.position;
                Projectile.NewProjectile<BathBomb>(transform.position, velo, Damage * 0.5f * PlayerOwner.ClusterBomb, PlayerOwner, endPos.x, endPos.y, count * 0.5f, size, -1);
            }
        }
        else if (Data.Length >= 5)
            range *= 1.1f;
        NewProjectile<ColaExplode>((Vector2)transform.position, Vector2.zero, Damage, Owner, range, 2.5f * (Data.Length > 3 ? Data[3] : 1.0f), 1);
    }
    public void Update()
    {
        float percent = timer / BounceTimeNeeded;
        float sin = Mathf.Sin(percent * Mathf.PI);
        Vector3 drawPos = Vector2.Lerp(playerStartPos, Destination, percent);
        drawPos.y -= 0.5f;
        Vector2 scale = new(2.0f, 1.4f);
        scale *= Data.Length > 3 ? Data[3] : 1.0f;
        SpriteBatch.Draw(Main.TextureAssets.Shadow, drawPos, scale, 0,
            new Color(0, 0, 0, 0.3f + 0.2f * sin), -50, Main.TextureAssets.AlphaShader);
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
    public override void OnHitTarget(Entity target)
    {
        if(target.Life <= 0)
        {
            int count = PlayerOwner.PrizeBombCoins;
            if (count > 0)
            {
                CoinManager.SpawnCoin(target.transform.position, (int)count, 1);
            }
        }
    }
}
public class BathBombShrapnel : Projectile
{
    public override void Init()
    {
        SpriteRenderer.sprite = Main.TextureAssets.BathBombShards[Utils.RandInt(Main.TextureAssets.BathBombShards.Length)];
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.transform.localScale *= 0.5f;
        SpriteRendererGlow.color = new Color(.4f, .52f, .52f) * 0.5f;
        Data2 = 6;
        Hostile = false;
        Friendly = true;
        timer -= Utils.RandInt(20);
        timer -= 20 * PlayerOwner.BonusShrapnel;
        if (timer < -410)
            timer = -410;
    }
    public override void AI()
    {
        if (startPos == Vector2.zero)
            startPos = (Vector2)transform.position - RB.velocity.normalized;
        RB.rotation += RB.velocity.magnitude * 0.2f * Mathf.Sign(RB.velocity.x) + 0.2f * RB.velocity.x;
        RB.velocity *= 1.006f;
        timer++;
        if (timer > 90)
        {
            Kill();
        }
        if(Utils.RandFloat() < 0.2f)
            ParticleManager.NewParticle((Vector2)transform.position, 0.2f, Utils.RandCircle(0.01f), 1f, 0.2f, ParticleManager.ID.Bubble, SpriteRendererGlow.color);
    }
    public override void OnKill()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 circular = new Vector2(.5f, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position, Utils.RandFloat(0.2f, 0.3f), circular * Utils.RandFloat(3, 6), 3f, 0.4f, ParticleManager.ID.Bubble, SpriteRendererGlow.color);
        }
        AudioManager.PlaySound(SoundID.SoapDie, transform.position, 0.4f, 0.8f);
    }
    public override void OnHitTarget(Entity target)
    {
        if (target.Life <= 0)
        {
            int count = PlayerOwner.PrizeBombCoins;
            if (count > 0)
            {
                CoinManager.SpawnCoin(target.transform.position, (int)count, 1);
            }
        }
    }
}