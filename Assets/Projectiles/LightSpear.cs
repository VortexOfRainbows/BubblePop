using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSpear : Projectile
{
    public override bool CanBeAffectedByHoming()
    {
        return false;
    }
    public override void Init()
    {
        transform.localScale = Vector3.one * 0.5f;
        SpriteRenderer.color = new Color(1, 1, .9f, 0.5f);
        SpriteRendererGlow.transform.localPosition = new Vector3(0.2f, 0, 0);
        SpriteRendererGlow.transform.localScale = new Vector3(0.4f, 3f, 3f);
        SpriteRendererGlow.color = new Color(1.2f, 1.2f, 1.2f);
        SpriteRenderer.sprite = Resources.Load<Sprite>("Projectiles/LaserSquare");
        if(Data.Length > 3)
        {
            Color c = Utils.PastelRainbow(Data[3], 0.67f) * 1.2f;
            SpriteRenderer.color *= c;
            SpriteRendererGlow.color *= c;
        }
        Damage = 3;
        if(Player.Instance.LightSpear > 0)
        {
            Damage = (int)(2.5f + Player.Instance.LightSpear * 0.5f);
        }
        Friendly = true;
        Hostile = false;
        cmp.c2D.offset = new Vector2(1, 0);
        cmp.c2D.radius = 0.02f;
        transform.localScale = new Vector3(1, 0.1f, transform.localScale.z);
        immunityFrames = 25;
        Penetrate = -1;
        SetSize();
        SpawnParticles();
        AudioManager.PlaySound(SoundID.LenardLaser, transform.position, 0.65f, 2.5f, 0);
    }
    public void SetSize()
    {
        Vector2 destination = new Vector2(Data1, Data2);
        Vector2 toDest = destination - (Vector2)transform.position;
        float scaleX = toDest.magnitude;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        transform.eulerAngles = new Vector3(0, 0, toDest.ToRotation() * Mathf.Rad2Deg);
    }
    public void SpawnParticles()
    {
        Vector2 destination = new Vector2(Data1, Data2);
        Vector2 toDest = destination - (Vector2)transform.position;
        float scaleX = toDest.magnitude;
        for(int j = 0; j < 12; ++j)
        {
            ParticleManager.NewParticle(transform.position, Utils.RandFloat(0.2f, 0.4f), RB.velocity * Utils.RandFloat(0.8f, 4.5f), 3.6f, Utils.RandFloat(0.45f, 0.6f), 2, SpriteRenderer.color);
        }
        for(float i = 0; i < scaleX; i += 0.33f)
        {
            Vector2 inBetween = Vector2.Lerp(transform.position, destination, i / scaleX);
            ParticleManager.NewParticle(inBetween, Utils.RandFloat(0.1f, 0.3f), RB.velocity * Utils.RandFloat(0.1f, 0.7f), 2, Utils.RandFloat(0.3f, 0.45f), 2, SpriteRenderer.color);
        }
        for (int j = 0; j < 20; ++j)
        {
            ParticleManager.NewParticle(destination, Utils.RandFloat(0.2f, 0.4f), RB.velocity * Utils.RandFloat(0.4f, 1.6f), 8f, Utils.RandFloat(0.45f, 0.6f), 2, SpriteRenderer.color);
        }
    }
    public override void AI()
    {
        ++timer;
        if(timer <= 7)
        {
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 0.4f, 0.2f), 1);
            SpriteRendererGlow.transform.localPosition = new Vector3(0.2f + 0.6f * timer / 7f, 0, 0);
        }
        else
        {
            SpriteRendererGlow.enabled = false;
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 0f, 0.1f), 1);
            if (transform.lossyScale.y < 0.01f)
                timer = 101;
            if (timer > 14)
                Friendly = false; //turn off hitbox after a bit
        }
        //RB.position -= RB.velocity * Time.fixedDeltaTime * 0.9f; //basically this should not be very effected by velocity
        if (timer > 100)
        {
            Kill();
            return;
        }
        SetSize();
    }
    public bool HasFiredLaser = false;
    public override void OnHitTarget(Entity target)
    {
        if(Player.Instance.LightChainReact > 0 && !HasFiredLaser && Data[2] > 0 && target is Enemy e)
        {
            Projectile.NewProjectile<LightSpearCaster>(target.transform.position, new Vector2(Utils.RandFloat(-4, 4), 20), Data[2]).GetComponent<LightSpearCaster>().ignore = e;
            HasFiredLaser = true;
        }
    }
}
public class LightSpearCaster : Projectile
{
    public override bool CanBeAffectedByHoming()
    {
        return false;
    }
    public Light2D Glow;
    public Enemy ignore;
    public override void Init()
    {
        Destroy(SpriteRendererGlow);
        Glow = Instantiate(Resources.Load<GameObject>("Projectiles/LightBulbGlow"), transform).GetComponent<Light2D>();
        Glow.transform.localPosition = new Vector3(0, 0.6f, 0);
        Glow.transform.localScale = new Vector3(1, 1, 1);
        Glow.intensity = 0;
        transform.localScale = Vector3.zero;
        SpriteRenderer.color = new Color(1, 1, 1, 0);

        transform.localScale = Vector3.one;
        RB.rotation = -20f;
        SpriteRenderer.sprite = Player.Instance.Hat.spriteRender.sprite;
        SpriteRendererGlow.enabled = false;
        Damage = 0;
        Friendly = false;
        Hostile = false;
        transform.localScale = new Vector3(1, 1f, 1);
    }
    public bool HasShot = false;
    public override void AI()
    {
        RB.velocity *= 0.945f;
        float speed = Bulb.DefaultShotSpeed / 5f;
        if (RB.velocity.sqrMagnitude < 1)
        {
            float speedMod = Bulb.SpeedModifier;
            timer += Time.fixedDeltaTime * speedMod;
            if (timer > speed)
            {
                if (!HasShot)
                {
                    Vector2 shootFromPos = Glow.transform.position;
                    if (Bulb.LaunchSpear(shootFromPos, out Vector2 norm, new List<Enemy> { ignore }, (int)Data1 - 1, bonusRange: 5 + 1f * Player.Instance.LightChainReact))
                        RB.velocity -= norm * 6;
                    HasShot = true;
                }
                float percent = (timer - speed) / speed * 5;
                Glow.intensity = 0.9f * (1 - percent);
                SpriteRenderer.color = new Color(1, 1, 1, 1 - percent);
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.03f);
                if (percent >= 1)
                {
                    Kill();
                }
            }
        }
        if (timer < speed)
        {
            Glow.intensity = Mathf.Lerp(Glow.intensity, 0.9f, 0.06f);
            SpriteRenderer.color = new Color(1, 1, 1, Mathf.Lerp(SpriteRenderer.color.a, 1, 0.06f));
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.06f);
        }
    }
    public override void OnKill()
    {
        for (int j = 0; j < 15; ++j)
        {
            ParticleManager.NewParticle(Glow.transform.position, Utils.RandFloat(0.2f, 0.3f), RB.velocity * Utils.RandFloat(0.4f, 1.6f), 4f, Utils.RandFloat(0.3f, 0.4f), 2, Glow.color * 0.5f);
        }
    }
}
