using UnityEngine;

public class OilBomb : Projectile
{
    public override bool TachyonCompatible() => true;
    public Vector2 SkyPos;
    public bool OnSolidTile = false;
    public float BarrelScaleMult { get; set; } = 1;
    public float BarrelScaleMultSqrt { get; set; } = 1;
    public int BarrelsSpawned { get; set; } = 0;
    public float AudioVolumeMult { get; set; } = 1;
    public bool IsFireBomb => Data.Length > 2 && Data[2] <= -1;
    public override float TachyonSize()
    {
        return 1.5f;
    }
    public override void Init()
    {
        SpriteRendererGlow.enabled = false;
        if(IsFireBomb) //REPLACE THIS WITH FIREBOMB SPRITE LATER
        {
            SpriteRenderer.sprite = Main.TextureAssets.KingOilBomb;
            SpriteRenderer.color = new Color(1, 0.5f, 0).WithAlpha(0);
        }
        else
        {
            SpriteRenderer.sprite = Main.TextureAssets.KingOilBomb;
            SpriteRenderer.color = Color.white.WithAlpha(0);
        }
        C2D.radius = 0.9f;
        SpriteRenderer.sortingOrder = LayerHelper.TreeSortingOrder + 1;
        Friendly = Hostile = false;
        transform.localScale = new Vector3(0, 0, 1);
        startPos = transform.position;
        startPos.y -= 0.5f;
        OnSolidTile = World.SolidTile(World.RealTileMap.Map.WorldToCell(startPos));
        if (OnSolidTile)
            startPos.y += 0.25f;
        SkyPos = startPos + new Vector2(0, 26);
        BarrelScaleMult = 1;
        if (!IsFireBomb)
            BarrelScaleMult += .14f * PlayerOwner.OilBarrelSize;
        if (Data.Length > 0 && Data1 != 1)
            BarrelScaleMult *= Data[0] * 0.5f;
        BarrelScaleMultSqrt = Mathf.Sqrt(BarrelScaleMult);
        AudioVolumeMult = Mathf.Max(0.1f, (1 + Data[0]) / (1 + PlayerOwner.AttackSpeedModifier));
        AudioManager.PlaySound(SoundID.Infect, transform.position * Data[0], AudioVolumeMult, (IsFireBomb ? 3 : 4) - Data[0]);
    }
    public override void AI()
    {
        timer += Mathf.Max(0.1f, PlayerOwner.AttackSpeedModifier);
        if (IsFireBomb)
            timer += 0.5f;
        timer *= 1.005f;
        if (Data.Length > 1)
        {
            int bonusBarrels = (int)Data[1];
            if (bonusBarrels > 0)
            {
                float upcomingPercent = (float)(BarrelsSpawned + 1) / (bonusBarrels + 1);
                float upcoming = 100 * upcomingPercent;
                while (timer > upcoming && bonusBarrels > BarrelsSpawned)
                {
                    Vector2 spawnPos = startPos + Utils.RandCircle(3);
                    spawnPos.y += OnSolidTile ? 0.25f : 0.5f;
                    Projectile.NewProjectile<OilBomb>(spawnPos, Utils.RandCircle(4 + BarrelsSpawned), 3, PlayerOwner, 0.75f);
                    BarrelsSpawned++;
                    upcomingPercent = (float)(BarrelsSpawned + 1) / (bonusBarrels + 1);
                    upcoming = 100 * upcomingPercent;
                }
            }
        }
        if (timer > 200)
            Kill();
        float percent = timer / 200f;
        transform.position = Vector2.Lerp(SkyPos, startPos, percent);
        transform.LerpLocalScale((2f - percent * 1f) * Data[0] * Vector2.one, 0.1f);
        SpriteRenderer.color = SpriteRenderer.color.WithAlpha(percent);
        startPos += RB.velocity * Time.fixedDeltaTime;
        transform.SetLocalEulerZ(RB.velocity.x * 2);
        RB.velocity *= 0.98f;
    }
    public void DeathParticles()
    {
        AudioManager.PlaySound(SoundID.BathBombBurst, transform.position, AudioVolumeMult, 1);
        for (int i = 0; i < 40 * Data[0]; ++i)
        {
            float size = Utils.RandFloat(0.5f, 1.0f);
            ParticleManager.NewParticle(transform.position, size * Data[0], Utils.RandCircle(4 - size * 2) + Vector2.up * 2, 1, 1 + 2 * size, ParticleManager.ID.Smoke, Color.black.WithAlpha(0.5f));
        }
        for (int i = 0; i < 30 * Data[0]; ++i)
        {
            float size = Utils.RandFloat(0.5f, 1.0f);
            ParticleManager.NewParticle(transform.position + 0.75f * Data[0] * new Vector3(Utils.RandFloat(-1, 1), Utils.RandFloat(-1, 4)), size * Data[0], Utils.RandCircle(5 - size * 2) + Vector2.up * 3, 1, 0.5f + 1f * size, ParticleManager.ID.Square,
                Color.Lerp(Color.black, ColorHelper.KingOilColor, Utils.RandFloat(0.3f, 0.7f)).WithAlpha(0.8f));
        }
    }
    public override void OnKill()
    {
        DeathParticles();
        Projectile.NewProjectile<ColaExplode>(transform.position, Vector2.zero, Damage, PlayerOwner, 1.5f * BarrelScaleMultSqrt, 1.5f);
        if(IsFireBomb)
        {
            HazardSystem.TryDetonatingOil(transform.position, PlayerOwner, PlayerOwner.FlintAndSteel);
            return;
        }
        float sizeOil = 16 * BarrelScaleMult;
        int totalBubbles = Mathf.RoundToInt(8 * BarrelScaleMult);
        float projectileReleaseSize = 6 * BarrelScaleMultSqrt;
        for (int i = 0; i < totalBubbles; ++i)
        {
            Vector2 spawnOffset = new Vector2(1f, 0).RotatedBy(i * Utils.TwoPI / totalBubbles);
            float rand = Mathf.Max(Utils.RandFloat(1), Utils.RandFloat(1));
            Projectile.NewProjectile<SmallBubble>((Vector2)transform.position + ((1 - rand) * 0.5f * projectileReleaseSize * spawnOffset), projectileReleaseSize * rand * spawnOffset + Utils.RandCircle(projectileReleaseSize * 0.25f), 1, PlayerOwner);
        }
        if(PlayerOwner.DashSparkle > 0)
        {
            float speedMax = 18;
            int c = (int)((PlayerOwner.DashSparkle * 2 + 2) * (Data1 != 1 ? 0.5f : 1.0f));
            float spreadAmt = Mathf.PI * 2f / (float)c;
            Vector2 circular = Utils.RandCircleEdge(1);
            for (int i = 0; i < c; i++)
            {
                circular = circular.RotatedBy(spreadAmt);
                Vector2 target = (Vector2)transform.position + circular * speedMax;
                Projectile.NewProjectile<StarProj>(transform.position, circular.RotatedBy(Mathf.PI * 0.875f) * speedMax, 2, PlayerOwner, target.x, target.y, -1);
            }
        }
        HazardSystem.SpreadCircle(transform.position, (int)(400 + Player.Instance.TarBonusDuration * 100), sizeOil, HazardSystem.HazardType.Oil);
        if(PlayerOwner.FlintAndSteel > 0 && Data1 == 1)
        { 
            Vector2 spawnPos = startPos;
            spawnPos.y += OnSolidTile ? 0.25f : 0.5f;
            Projectile.NewProjectile<OilBomb>(spawnPos, Vector2.zero, 3, PlayerOwner, 0.95f, 0, -1);
        }
    }
    public float TargetRotation = 0;
    public void Update()
    {
        float percent = timer / 200f;
        int order = OnSolidTile ? LayerHelper.SolidTileSortingOrder + 1 : LayerHelper.ShadowSortingOrder;
        float windUpPercent = Mathf.Clamp01(percent * 3);
        float windDownPercent = 1 - Mathf.Clamp01(percent * 3 - 2);
        float sin = Mathf.Sin(windUpPercent * Mathf.PI);
        float scaleMult = windUpPercent + sin * 0.5f;
        float a = (windUpPercent * 0.5f + percent * 0.2f) * windDownPercent;
        Color c = IsFireBomb ? new(0.8f, 0.6f, 0.2f, a) : new(0.7f, 0.1f, 0.1f, a);
        Vector2 sizeMult = 2 * scaleMult * BarrelScaleMultSqrt * Vector2.one;
        TargetRotation = Mathf.Lerp(TargetRotation, (IsFireBomb ? -90 : 90) * windUpPercent, Utils.DeltaTimeLerpFactor(0.1f));
        SpriteBatch.Draw(Main.TextureAssets.CrosshairOuter, startPos, sizeMult, TargetRotation, c, order + 2, Main.TextureAssets.SpriteGlowmask);
        SpriteBatch.Draw(Main.TextureAssets.CrosshairInner, startPos, Vector2.one * scaleMult, 0, c, order + 3, Main.TextureAssets.SpriteGlowmask);
        SpriteBatch.Draw(Main.TextureAssets.CrosshairEmblem, startPos, Vector2.one * scaleMult, 0, c.WithAlpha(c.a * 0.4f), order + 2, Main.TextureAssets.SpriteGlowmask);
        SpriteBatch.Draw(Main.TextureAssets.CrosshairFill, startPos, sizeMult, 0, c.WithAlpha(c.a * 0.2f), order + 1, Main.TextureAssets.SpriteGlowmask);
        SpriteBatch.Draw(Main.TextureAssets.Shadow, startPos, new Vector2(3, 2) * transform.localScale, 0, new Color(0, 0, 0, 0.3f * percent * windDownPercent), order, Main.TextureAssets.AlphaShader);
    }
    public override bool? CanBeAffectedByHoming() => !IsFireBomb;
    public override Vector3 HomingStartPosition() => startPos;
    public override bool DoHomingBehavior(Enemy target, Vector2 norm, float range)
    {
        float currentSpeed = RB.velocity.magnitude + PlayerOwner.HomingRangeSqrt * 2.5f + 5;
        float modAmt = 0.05f + PlayerOwner.HomingRangeSqrt * 0.025f;
        RB.velocity = Vector2.Lerp(RB.velocity * (1 - modAmt), norm * currentSpeed, modAmt);
        return false;
    }
    public override bool OnInsideTile() => false;
    public override bool OnTileCollide(Collider2D collision) => false;
    public override void OnHitTarget(Entity target)
    {

    }
}
