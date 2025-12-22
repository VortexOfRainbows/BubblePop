using System.Collections.Generic;
using UnityEngine;

public class LightSpear : Projectile
{
    public override bool OnInsideTile()
    {
        return false;
    }
    public override bool OnTileCollide(Collider2D collision)
    {
        return false;
    }
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
        bool isCrown = Player.Instance.Hat is Crown;
        float specialColorMult = .75f;
        if(Data.Length > 3 && Data[3] >= 0)
        {
            Color c = Utils.PastelRainbow(Data[3] + (isCrown ? 2f : 0), 0.67f) * 1.2f;
            SpriteRenderer.color *= c;
            SpriteRendererGlow.color *= c;
            specialColorMult = 0.375f;
        }
        if(isCrown)
        {
            SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, new Color(1f, 0, 0, 0.5f), specialColorMult);
            SpriteRendererGlow.color = Color.Lerp(SpriteRendererGlow.color, new Color(1f, 0, 0, 0.5f), specialColorMult);
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
        Color c = Player.Instance.Hat is Crown ? new(1, 0, 0, 1f) : SpriteRenderer.color;
        Vector2 destination = new Vector2(Data1, Data2);
        Vector2 toDest = destination - (Vector2)transform.position;
        float scaleX = toDest.magnitude;
        for(int j = 0; j < 12; ++j)
        {
            ParticleManager.NewParticle(transform.position, Utils.RandFloat(0.2f, 0.4f), RB.velocity * Utils.RandFloat(0.8f, 4.5f), 3.6f, Utils.RandFloat(0.45f, 0.6f), 2, c);
        }
        for(float i = 0; i < scaleX; i += 0.33f)
        {
            Vector2 inBetween = Vector2.Lerp(transform.position, destination, i / scaleX);
            ParticleManager.NewParticle(inBetween, Utils.RandFloat(0.1f, 0.3f), RB.velocity * Utils.RandFloat(0.1f, 0.7f), 2, Utils.RandFloat(0.3f, 0.45f), 2, c);
        }
        for (int j = 0; j < 20; ++j)
        {
            ParticleManager.NewParticle(destination, Utils.RandFloat(0.2f, 0.4f), RB.velocity * Utils.RandFloat(0.4f, 1.6f), 8f, Utils.RandFloat(0.45f, 0.6f), 2, c);
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
            float damage = Damage * 0.8f;
            if(damage >= 0.5f)
                Projectile.NewProjectile<LightSpearCaster>(target.transform.position, new Vector2(Utils.RandFloat(-4, 4), 20), damage, Data[2]).GetComponent<LightSpearCaster>().ignore = e;
            HasFiredLaser = true;
        }
        Damage = 0;
    }
}
public class LightSpearCaster : Projectile
{
    public override bool OnInsideTile()
    {
        return false;
    }
    public override bool OnTileCollide(Collider2D collision)
    {
        return false;
    }
    public override bool CanBeAffectedByHoming()
    {
        return false;
    }
    public Enemy ignore;
    public float alphaScale = 0.2f;
    public override void Init()
    {
        if(Player.Instance.Hat is Crown)
        {
            SpriteRendererGlow.transform.localPosition = new Vector3(0, 0.44f, -1);
            SpriteRendererGlow.transform.localScale = new Vector3(1.1f, 1.35f, 1f);
            SpriteRendererGlow.color = new Color(0.8f, 0f, 0f, 1f);
            SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
            SpriteRendererGlow.sortingOrder = 0;
            SpriteRendererGlow.material = Resources.Load<Material>("Materials/Additive");
            alphaScale = 1f;
            transform.localScale = Vector3.one * 1.2f;
        }
        else
        {
            SpriteRendererGlow.transform.localPosition = new Vector3(0, 0.66f, -1);
            SpriteRendererGlow.transform.localScale = Vector3.one * 1.6f;
            SpriteRendererGlow.color = new Color(0.9960784f, 0.9764706f, 0.2313726f, 0f);
            SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
            SpriteRendererGlow.sortingOrder = 5;
            SpriteRendererGlow.material = Resources.Load<Material>("Materials/Additive");
            transform.localScale = Vector3.one;
            RB.rotation = 20f;
        }
        SpriteRenderer.color = new Color(1, 1, 1, 0);
        SpriteRenderer.sprite = Player.Instance.Hat.spriteRender.sprite;
        SpriteRenderer.flipX = false;
        Friendly = false;
        Hostile = false;
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
                    Vector2 shootFromPos = SpriteRendererGlow.transform.position;
                    if (Bulb.LaunchSpear(shootFromPos, out Vector2 norm, new List<Enemy> { ignore }, (int)Data1 - 1, bonusRange: 5 + 1f * Player.Instance.LightChainReact, 1, Damage))
                        RB.velocity -= norm * 6;
                    HasShot = true;
                }
                float percent = (timer - speed) / speed * 5;
                SpriteRendererGlow.color = SpriteRendererGlow.color.WithAlpha(0.9f * (1 - percent) * alphaScale);
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
            SpriteRendererGlow.color = SpriteRendererGlow.color.WithAlpha(Mathf.Lerp(SpriteRendererGlow.color.a, 0.9f * alphaScale, 0.06f));
            SpriteRenderer.color = new Color(1, 1, 1, Mathf.Lerp(SpriteRenderer.color.a, 1, 0.06f));
        }
    }
    public override void OnKill()
    {
        Color c = Player.Instance.Hat is Crown ? new(1, 0, 0, 1f) : new(1, 1, .9f, 0.5f);
        for (int j = 0; j < 15; ++j)
        {
            ParticleManager.NewParticle(SpriteRendererGlow.transform.position, Utils.RandFloat(0.2f, 0.3f), RB.velocity * Utils.RandFloat(0.4f, 1.6f), 4f, Utils.RandFloat(0.3f, 0.4f), 2, c);
        }
    }
}
public class ThunderLightSpearCaster : Projectile
{
    public override bool OnInsideTile()
    {
        return false;
    }
    public override bool OnTileCollide(Collider2D collision)
    {
        return false;
    }
    public override bool CanBeAffectedByHoming()
    {
        return false;
    }
    public SpecialTrail MyTrail;
    public Transform FakeParent;
    public float alphaScale = 0.2f;
    public override void Init()
    {
        SpriteRendererGlow.transform.localScale = Vector3.one * 1.5f;
        SpriteRendererGlow.sprite = SpriteRenderer.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.sortingOrder = 0;
        SpriteRendererGlow.material = SpriteRenderer.material = Resources.Load<Material>("Materials/Additive");
        alphaScale = 1f;
        if (Player.Instance.Hat is Crown)
            SpriteRendererGlow.color = new Color(1f, 0f, 0f, 0.5f);
        else
            SpriteRendererGlow.color = new Color(0.996f, 0.9765f, 0.2314f, 0.5f);
        Friendly = false;
        Hostile = false;
        transform.localScale *= 0.75f;
        MyTrail = SpecialTrail.NewTrail(transform, SpriteRendererGlow.color * 0.9f, 1, 0.2f);
        cmp.c2D.enabled = false;
    }
    public bool HasShot = false;
    public bool HasClosed = false;
    public override void AI()
    {
        if(FakeParent != null && !HasShot)
        {
            float myNum = Data[0];
            float total = Data[1];
            float dir = Data[3];
            float r = myNum / total * Mathf.PI * 2f;
            timer2 += Time.fixedDeltaTime;
            float percentStarted = Mathf.Min(1, timer2 * 2);
            float maxDist = 1.5f + total * 0.025f;
            float iPer = 1;
            if (Book.InClosingAnimation)
            {
                iPer -= Book.ClosingPercent;
                maxDist *= iPer;
                HasClosed = true;
                if (MyTrail != null)
                {
                    MyTrail.Trail.startColor = MyTrail.Trail.startColor.WithAlpha(iPer * 0.9f);
                    MyTrail.originalAlpha = iPer * 0.9f;
                    SpriteRendererGlow.color = SpriteRendererGlow.color.WithAlpha(0.5f * iPer);
                    SpriteRenderer.color = SpriteRenderer.color.WithAlpha(iPer);
                }
            }
            float max = Mathf.Max(8, Mathf.Min(8, total), total > 32 ? (int)(Mathf.Sqrt(total) * 4) : total > 16 ? (int)(Mathf.Sqrt(total) * 2) : 0);
            Vector2 circular = new(0, maxDist * percentStarted + maxDist * 0.4f * Mathf.Sin(r * max + timer2 * Mathf.Deg2Rad * 120));
            circular = circular.RotatedBy((timer2 * Mathf.Deg2Rad * 240 + r) * dir);
            transform.position = transform.position.Lerp((Vector2)FakeParent.position + circular, 0.7f);

            float startUpTime = (1.5f + total * .1f) * (myNum / total);
            float speed = 0.5f;
            float speedMod = Bulb.SpeedModifier;
            timer += (0.5f + 0.5f * speedMod) * Time.fixedDeltaTime * iPer;
            if (timer > speed + startUpTime)
            {
                float rangeBonus = Data[2];
                Vector2 shootFromPos = SpriteRendererGlow.transform.position;
                if (Bulb.LaunchSpear(shootFromPos, out Vector2 norm, new(), Player.Instance.LightChainReact, bonusRange: 3 + total * 0.5f + rangeBonus, 0f, Damage))
                    HasShot = true;
                timer -= speed;
            }
        }
        else
        {
            Kill();
        }
    }
    public override void OnKill()
    {
        if (HasClosed)
            return;
        Color c = Player.Instance.Hat is Crown ? new(1, 0, 0, 1f) : new(1, 1, .9f, 0.5f);
        for (int j = 0; j < 15; ++j)
        {
            ParticleManager.NewParticle(SpriteRendererGlow.transform.position, Utils.RandFloat(0.2f, 0.3f), RB.velocity * Utils.RandFloat(0.4f, 1.6f), 4f, Utils.RandFloat(0.3f, 0.4f), 2, c);
        }
    }
}
