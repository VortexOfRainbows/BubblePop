using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilBomb : Projectile
{
    public Vector2 SkyPos;
    public bool OnSolidTile = false;
    public override void Init()
    {
        SpriteRendererGlow.enabled = false;
        SpriteRenderer.sprite = Main.TextureAssets.KingOilBomb;
        SpriteRenderer.color = Color.white.WithAlpha(0);
        SpriteRenderer.sortingOrder = LayerHelper.TreeSortingOrder + 1;
        Friendly = Hostile = false;
        transform.localScale = new Vector3(0, 0, 1);
        startPos = transform.position;
        startPos.y -= 0.5f;
        OnSolidTile = World.SolidTile(World.RealTileMap.Map.WorldToCell(startPos));
        if (OnSolidTile)
            startPos.y += 0.25f;
        SkyPos = startPos + new Vector2(0, 26);
        AudioManager.PlaySound(SoundID.Infect, transform.position, 1, 3);
    }
    public override void AI()
    {
        timer += Mathf.Max(0.1f, PlayerOwner.AttackSpeedModifier);
        timer *= 1.005f;
        if (timer > 200)
            Kill();
        float percent = timer / 200f;
        transform.position = Vector2.Lerp(SkyPos, startPos, percent);
        transform.LerpLocalScale(Vector2.one * (2f - percent * 1f), 0.1f);
        SpriteRenderer.color = Color.white.WithAlpha(percent);
    }
    public override void OnKill()
    {
        AudioManager.PlaySound(SoundID.BathBombBurst, transform.position, 1, 1);
        Projectile.NewProjectile<ColaExplode>(transform.position, Vector2.zero, 5, PlayerOwner, 1.5f, 1.5f);
        for(int i = 0; i < 40; ++i)
        {
            float size = Utils.RandFloat(0.5f, 1.0f);
            ParticleManager.NewParticle(transform.position, size, Utils.RandCircle(4 - size * 2) + Vector2.up * 2, 1, 1 + 2 * size, ParticleManager.ID.Smoke, Color.black.WithAlpha(0.5f));
        }
        for (int i = 0; i < 30; ++i)
        {
            float size = Utils.RandFloat(0.5f, 1.0f);
            ParticleManager.NewParticle(transform.position + new Vector3(Utils.RandFloat(-1, 1), Utils.RandFloat(-1, 4)) * 0.75f, size, Utils.RandCircle(5 - size * 2) + Vector2.up * 3, 1, 0.5f + 1f * size, ParticleManager.ID.Square, 
                Color.Lerp(Color.black, ColorHelper.KingOilColor, Utils.RandFloat(0.3f, 0.7f)).WithAlpha(0.8f));
        }
        float sizeOil = 16;
        float scaleMult = 1;
        for (int i = 0; i < 8; ++i)
            Projectile.NewProjectile<SmallBubble>(transform.position, new Vector2(Utils.RandFloat(sizeOil) * 0.5f, 0).RotatedBy(i * Mathf.PI / 4f) + Utils.RandCircle(sizeOil * 0.1f), 1, PlayerOwner);
        HazardSystem.SpreadCircle(transform.position, (int)(400 + Player.Instance.TarBonusDuration * 100), sizeOil * scaleMult * scaleMult, HazardSystem.HazardType.Oil);
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
        Color c = new Color(0.7f, 0.1f, 0.1f, (windUpPercent * 0.5f + percent * 0.2f) * windDownPercent);
        Vector2 sizeMult = 2 * scaleMult * Vector2.one;
        TargetRotation = Mathf.Lerp(TargetRotation, 90 * windUpPercent, Utils.DeltaTimeLerpFactor(0.1f));
        SpriteBatch.Draw(Main.TextureAssets.CrosshairOuter, startPos, sizeMult, TargetRotation, c, order + 2, Main.TextureAssets.SpriteGlowmask);
        SpriteBatch.Draw(Main.TextureAssets.CrosshairInner, startPos, Vector2.one * scaleMult, 0, c, order + 3, Main.TextureAssets.SpriteGlowmask);
        SpriteBatch.Draw(Main.TextureAssets.CrosshairEmblem, startPos, Vector2.one * scaleMult, 0, c.WithAlpha(c.a * 0.4f), order + 2, Main.TextureAssets.SpriteGlowmask);
        SpriteBatch.Draw(Main.TextureAssets.CrosshairFill, startPos, sizeMult, 0, c.WithAlpha(c.a * 0.2f), order + 1, Main.TextureAssets.SpriteGlowmask);
        SpriteBatch.Draw(Main.TextureAssets.Shadow, startPos, new Vector2(3, 2) * transform.localScale, 0, new Color(0, 0, 0, 0.3f * percent * windDownPercent), order, Main.TextureAssets.AlphaShader);
    }
    public override bool? CanBeAffectedByHoming() => false;
    public override bool OnInsideTile() => false;
    public override bool OnTileCollide(Collider2D collision) => false;
    public override void OnHitTarget(Entity target)
    {

    }
}
