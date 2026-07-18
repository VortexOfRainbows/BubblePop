using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class KingOilDiamondProj : Projectile
{
    public Color c = Color.red;
    public SpecialTrail trail;
    public float Direction = 1;
    public override void Init()
    {
        c.a = 0.68f;
        SpriteRenderer.sprite = Main.TextureAssets.KingOilDiamond;
        SpriteRenderer.material = Main.TextureAssets.SpriteGlowmask;
        timer2 = 0;
        Penetrate = -1;
        Friendly = true;
        RB.velocity *= 1 + 0.1f * PlayerOwner.FasterBulletSpeed;
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.color = Color.clear;
        SpriteRendererGlow.transform.localScale *= 0.5f;
        C2D.radius *= 0.6f;
        startPos = transform.position;
        trail = SpecialTrail.NewTrail(transform, c * 0.7f, 1.0f, 0.2f, 0.25f);
        Direction = Utils.SignNoZero(Data1 - startPos.x);
        immunityFrames = 20;
        SpriteRenderer.flipX = Direction == 1;
    }
    public bool SwitchedPos = false;
    public float deathPercent = 1f;
    public override void AI()
    {
        if (SwitchedPos && timer < 50)
        {
            Data1 = transform.position.x;
            Data2 = transform.position.y;
            timer = 50;
        }
        if (SwitchedPos)
        {
            if (PlayerOwner.Weapon is OilScepter scepter)
                startPos = scepter.Gem.position;
            else
                startPos = PlayerOwner.Weapon.transform.position;
        }
        float halfTimer = timer / 50f;
        float fullTimer = timer / 100f;
        if (fullTimer > 1)
            fullTimer = 1;
        if (halfTimer > 1)
            SwitchedPos = true;
        if (halfTimer >= 2)
            halfTimer = 2;
        Vector2 endPoint = new(Data1, Data2);
        Vector2 toEnd = endPoint - startPos;

        float socketSlowdown = 0.85f;
        timer += 16f / (Mathf.Max(8, toEnd.magnitude)) * (fullTimer > socketSlowdown ? 0.4f + 0.6f * (1 - (fullTimer - socketSlowdown) / (1 - socketSlowdown)) : 1.0f);

        float cosModifier = 0.5f - 0.5f * Mathf.Cos(halfTimer * Mathf.PI);
        Vector2 target = Vector2.Lerp(startPos, endPoint, cosModifier);

        float arcSize = Direction * ((toEnd.magnitude * 0.125f * cosModifier) - (halfTimer + Mathf.Sin(halfTimer * Mathf.PI)) * (0.5f + fullTimer * fullTimer * 1.5f) * Mathf.Cos(Mathf.PI * halfTimer));

        target += new Vector2(0, arcSize * Mathf.Sin(halfTimer * Mathf.PI)).RotatedBy(toEnd.ToRotation());
        Vector2 toTarget = target - (Vector2)transform.position;
        float speed = Mathf.Min(55f, toTarget.magnitude * 100);
        RB.velocity = toTarget.normalized * speed;

        if (toTarget.magnitude < 0.15f && SwitchedPos && halfTimer >= 2)
            Kill();

        float scaler = Mathf.Sqrt(Mathf.Abs(Mathf.Sin(fullTimer * Mathf.PI)));
        transform.localScale = Vector2.Lerp(Vector2.one, new Vector2(2f, 3f), scaler);
        SpriteRendererGlow.color = c * scaler;
        transform.SetLocalEulerZ(RB.velocity.x * -0.2f);
    }
    public override void OnHitTarget(Entity target)
    {
        AudioManager.PlaySound(SoundID.StarbarbImpact, transform.position, 0.3f, 0.475f, 0);
        AudioManager.PlaySound(SoundID.SoapDie, transform.position, 1.125f, 1.7f, 0);
        for (int i = 0; i < 15; ++i)
            ParticleManager.NewParticle(target.transform.position + new Vector3(Utils.RandFloat(-1f, 1f), Utils.RandFloat(-1f, 1f)), 3, RB.velocity * Utils.RandFloat(0.3f) + Utils.RandCircle(5), 5, Utils.RandFloat(0.7f, 0.8f), 3,
                Color.Lerp(Color.red, ColorHelper.RarityColors[5], Utils.RandFloat()) * 0.95f);
    }
    public override void OnKill()
    {
        if (PlayerOwner.Weapon is OilScepter scepter)
            scepter.ActiveDiamondProjectile--;
    }
    public override bool? CanBeAffectedByHoming() => null;
    public override bool DoHomingBehavior(Enemy target, Vector2 norm, float scale)
    {
        Vector2 target2 = Vector2.Lerp(new Vector2(Data1, Data2), target.transform.position, 0.025f + PlayerOwner.HomingRangeSqrt * 0.05f);
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
        SwitchedPos = true;
        return false;
    }
}
