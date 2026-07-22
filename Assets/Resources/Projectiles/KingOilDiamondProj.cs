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
        C2D.radius *= 0.75f;
        startPos = transform.position;
        trail = SpecialTrail.NewTrail(transform, c * 0.7f, 1.0f, 0.2f, 0.25f);
        Direction = Utils.SignNoZero(Data1 - startPos.x);
        immunityFrames = 20;
        SpriteRenderer.flipX = Direction == 1;
    }
    public bool SwitchedPos = false;
    public float deathPercent = 1f;
    public Sound activeSound = null;
    public Vector2 PrimaryTarget;
    public Vector2? SecondaryTarget = null;
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
        PrimaryTarget = Vector2.Lerp(startPos, endPoint, cosModifier);

        float arcSize = Direction * ((toEnd.magnitude * 0.125f * cosModifier) - (halfTimer + Mathf.Sin(halfTimer * Mathf.PI)) * (0.5f + fullTimer * fullTimer * 1.5f) * Mathf.Cos(Mathf.PI * halfTimer));

        PrimaryTarget += new Vector2(0, arcSize * Mathf.Sin(halfTimer * Mathf.PI)).RotatedBy(toEnd.ToRotation());
        Vector2 target = PrimaryTarget;

        float scaler = Mathf.Sqrt(Mathf.Abs(Mathf.Sin(fullTimer * Mathf.PI)));
        if (SecondaryTarget.HasValue)
        {
            float range = PlayerOwner.HomingRange * scaler;
            float distance = target.Distance(SecondaryTarget.Value);
            if (distance < range)
            {
                float percent = 1 - distance / range;
                target = Vector2.Lerp(target, SecondaryTarget.Value, percent + scaler);
            }
        }


        Vector2 toTarget = target - (Vector2)transform.position;
        float speed = Mathf.Min(55f, toTarget.magnitude * 100);
        RB.velocity = toTarget.normalized * speed;

        if ((toTarget.magnitude < 0.25f && SwitchedPos && halfTimer >= 2) || (PlayerOwner.Weapon is not OilScepter o || o.ActiveDiamondProjectile <= 0))
            Kill();

        transform.localScale = Vector2.Lerp(Vector2.one, new Vector2(2f, 3f), scaler);
        SpriteRendererGlow.color = c * scaler;
        transform.SetLocalEulerZ(RB.velocity.x * -0.2f);

        if(halfTimer > 0.25f && halfTimer < 1.7f)
        {
            if(activeSound == null || activeSound.Source.time > 0.2f)
                activeSound = AudioManager.PlaySound(SoundID.ElectricZap, transform.position, 0.5f, 2f, 1);
            float sin = Mathf.Sin(((halfTimer - 0.25f) / 1.45f) * Mathf.PI);
            ParticleManager.NewParticle(new Vector2(transform.position.x, transform.position.y - 1f), Mathf.Lerp(0.5f, 1.0f, sin), Utils.RandCircle(2 + sin), 0, 0.3f + 0.2f * sin, ParticleManager.ID.Fire, new Color(1, 0.8f, 0.7f));
            if(Utils.RandFloat() < 0.25f)
            {
                ParticleManager.NewParticle(new Vector2(transform.position.x, transform.position.y - 1f), Mathf.Lerp(1.0f, 1.2f, sin), Utils.RandCircle(0.1f), 0, 0.6f + 0.2f * sin, ParticleManager.ID.Fire, new Color(1, 0.8f, 0.7f));
            }
        }
        SecondaryTarget = null;
        homingCounter = 0;
    }
    public override void OnHitTarget(Entity target)
    {
        AudioManager.PlaySound(SoundID.StarbarbImpact, transform.position, 0.3f, 0.475f, 0);
        AudioManager.PlaySound(SoundID.SoapDie, transform.position, 1.125f, 1.7f, 0);
        for (int i = 0; i < 15; ++i)
            ParticleManager.NewParticle(target.transform.position + new Vector3(Utils.RandFloat(-1f, 1f), Utils.RandFloat(-1f, 1f)), 3, RB.velocity * Utils.RandFloat(0.3f) + Utils.RandCircle(5), 5, Utils.RandFloat(0.7f, 0.8f), 3,
                Color.Lerp(Color.red, ColorHelper.RarityColors[5], Utils.RandFloat()) * 0.95f);
        if(target is Enemy e)
            e.DetonateAllDebuffs();
    }
    public override void OnKill()
    {
        if (PlayerOwner.Weapon is OilScepter scepter)
            scepter.ActiveDiamondProjectile--;
    }
    public override bool? CanBeAffectedByHoming() => null;
    public override bool DoHomingBehavior(Enemy target, Vector2 norm, float scale)
    {
        if (target.transform.position.Distance(PlayerOwner.Position) > PrimaryTarget.Distance(PlayerOwner.Position) && !SwitchedPos)
            SecondaryTarget = target.transform.position;
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
