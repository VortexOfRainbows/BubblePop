using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Bullet : Projectile
{
    public float SizeMult => Data.Length > 0 ? Data[0] : 1f;
    public override void Init()
    {
        //SpriteRendererGlow.color = new Color(245 / 255f, 191 / 255f, 7 / 255f);
        SpriteRenderer.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.transform.localScale *= 1.1f;
        if (Data.Length > 3)
            SpriteRendererGlow.color = new Color(Data[1], Data[2], Data[3], Data.Length == 4 ? 1 : Data[4]); // new Color(0.2f, 0.3f, 1f, 1f);
        else
            SpriteRendererGlow.color = new Color(1, 0.1f, 0.1f, 1f);
        SpriteRenderer.color = new Color(1, 1f, 1f, 5f);
        SpriteRenderer.material = Resources.Load<Material>("Materials/Additive");
        SpriteRendererGlow.material = Resources.Load<Material>("Particles/Bubble2");
        cmp.c2D.radius *= 0.8f;
        transform.localScale *= 0.2f * SizeMult;
        Friendly = false;
        Hostile = true;
    }
    public override void AI()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.5f * SizeMult, 0.06f);
        RB.velocity *= 1.001f;
        float deathTime = 330;
        float FadeOutTime = 15;
        if (timer > deathTime + FadeOutTime)
        {
            Kill();
        }
        if (Utils.RandFloat() < 0.5f)
        {
            Vector2 norm = RB.velocity.normalized;
            if(this is SnowBullet && Utils.RandFloat() < 0.95f)
            {
                if (Utils.RandFloat() < 0.5f)
                    ParticleManager.NewParticle((Vector2)transform.position, 0.45f, RB.velocity * Utils.RandFloat(-0.1f, 0.5f) + Utils.RandCircle(), 1f, 0.5f, ParticleManager.ID.Snow, Color.white.WithAlpha(0.5f));
                else
                    ParticleManager.NewParticle((Vector2)transform.position + 0.3f * transform.localScale.x * Utils.RandCircle(), 0.35f, RB.velocity * -0.5f + Utils.RandCircle() * 3, 1f, 0.4f, ParticleManager.ID.Snow, Color.white.WithAlpha(0.5f));
            }
            else
                ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f, 1.2f * SizeMult, norm * -.75f, 0.5f, Utils.RandFloat(0.45f, 0.6f), 3, SpriteRendererGlow.color);
        }
        if (timer > deathTime)
        {
            float alphaOut = 1 - (timer - deathTime) / FadeOutTime;
            SpriteRenderer.color = SpriteRenderer.color.WithAlpha(alphaOut);
            SpriteRendererGlow.color = SpriteRendererGlow.color.WithAlpha(alphaOut);
        }
        timer++;
    }
    public override void OnKill()
    {
        Color c = SpriteRendererGlow.color.WithAlpha(1);
        for (int i = 0; i < 6; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(3) * SizeMult, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position, Utils.RandFloat(0.3f, 0.4f) * SizeMult, circular, 1f, 0.3f, 2, c);
            ParticleManager.NewParticle((Vector2)transform.position, Utils.RandFloat(2f, 4f) * SizeMult, circular, 1f, 0.3f, 3, c);
        }
        //AudioManager.PlaySound(SoundID.BubblePop, transform.position, 0.5f, 0.8f);
    }
}
public class Snowball : Projectile
{
    public override bool OnInsideTile()
    {
        return false;
    }
    public override bool OnTileCollide(Collider2D collision)
    {
        return false;
    }
    public Vector2 Destination = Vector2.zero;
    public override void Init()
    {
        //SpriteRendererGlow.color = new Color(245 / 255f, 191 / 255f, 7 / 255f);
        SpriteRenderer.sprite = Main.TextureAssets.Snowball;
        SpriteRendererGlow.transform.localScale *= 1.1f;
        SpriteRendererGlow.color = new Color(0.5f, 0.9f, 1.0f, 1f);
        SpriteRendererGlow.material = Main.TextureAssets.AdditiveShader;
        SpriteRendererGlow.transform.localScale *= 0.8f;
        Friendly = false;
        Hostile = false;
        if (Owner != null)
            startPos = Owner.transform.position;
        else
            startPos = transform.position;
            timer = -1;
        if (Data.Length >= 2)
        {
            transform.localScale *= 0.5f + 1.0f * Data2;
            timer2 = Data2 * Data1;
        }
        else
            transform.localScale *= 0.0f;

    }
    public float TravelTime = 8.75f;
    public float HeightMult = .3f;
    public override void AI()
    {
        float percent = timer2 / Data1;
        if (percent > 1)
            percent = 1;
        if (Owner == null) //owner is dead, fall to ground
        {
            if (timer < 0)
            {
                RB.velocity += Utils.RandCircleEdge() * (percent + 3);
                percent = 1.1f;
                Destination = startPos + RB.velocity;
                TravelTime *= 2.5f;
                timer = 0;
                HeightMult = .6f;
            }
        }
        else
        {
            transform.localScale = 1.4f * percent * Vector3.one;
        }
        if (percent >= 1 || timer >= 0)
        {
            if (timer < 0)
            {
                AudioManager.PlaySound(SoundID.GolemShoot, transform.position, 1, 1.0f, 0);
                Destination = EnemyOwner.Target.Position;
                Vector2 norm = Destination - startPos;
                float distToPlayer = norm.magnitude;
                norm = norm.normalized;
                RB.velocity = norm * 2;
                if (distToPlayer > 15)
                    distToPlayer = 15;
                Destination = startPos + norm * distToPlayer;
                timer = 0;
            }
            else
            {
                if (Utils.RandFloat() < 0.5f)
                    ParticleManager.NewParticle((Vector2)transform.position, 0.6f, RB.velocity * Utils.RandFloat(-0.1f, 0.5f) + Utils.RandCircle(), 1f, 0.5f, ParticleManager.ID.Snow, Color.white.WithAlpha(0.5f));
                else
                    ParticleManager.NewParticle((Vector2)transform.position + 0.4f * transform.localScale.x * Utils.RandCircle(), 0.3f, RB.velocity * -0.5f + Utils.RandCircle() * 3, 1f, 0.3f, ParticleManager.ID.Snow, Color.white.WithAlpha(0.5f));

                transform.SetLocalEulerZ(transform.localEulerAngles.z + 0.5f * RB.velocity.x);
                if (timer > TravelTime)
                    Kill();

                float dist = startPos.Distance(Destination);
                float timeNeeded = dist;
                float percent2 = timer / TravelTime;
                timer += 1.0f / timeNeeded;
                if (percent2 > 1)
                {
                    Kill();
                    return;
                }
                Vector2 pos = Vector2.Lerp(startPos, Destination, percent2);
                float arcY = Mathf.Sin(percent2 * Mathf.PI) * dist * HeightMult;
                pos.y += arcY;
                RB.MovePosition(pos);
                if (percent2 > 0.5f && arcY < 0.3f)
                {
                    Hostile = true;
                }
            }
        }
        else if (Owner != null)
        {
            startPos = EnemyOwner.transform.position + new Vector3(Utils.SignNoZero(EnemyOwner.Visual.transform.localScale.x) * 0.1f, -0.5f);
            transform.position = startPos;
        }
    }
    public void Update()
    {
        DoShadow();
    }
    public void DoShadow()
    {
        float percent = timer / TravelTime;
        float sin = Mathf.Sin(percent * Mathf.PI);
        Vector3 drawPos = Vector2.Lerp(startPos, Destination, percent);
        drawPos.y -= 0.5f;
        bool solidTile = World.SolidTile(World.RealTileMap.Map.WorldToCell(drawPos));
        if (solidTile)
            drawPos.y += 0.25f;
        SpriteBatch.Draw(Main.TextureAssets.Shadow, drawPos, new Vector2(2.0f, 1.3f), 0,
            new Color(0, 0, 0, 0.5f * sin), solidTile ? LayerHelper.SolidTileSortingOrder + 1 : -40, Main.TextureAssets.AlphaShader);
    }
    public override void OnKill()
    {
        for (int i = 0; i < 15; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(3, 6), 0).RotatedBy(i / 15f * Utils.TwoPI);
            ParticleManager.NewParticle((Vector2)transform.position, Utils.RandFloat(0.5f, 0.6f), circular + Vector2.up * 0.2f + Utils.RandCircle(0.4f), 1f, Utils.RandFloat(0.5f, 1.0f), ParticleManager.ID.Snow, Color.white);
        }
        AudioManager.PlaySound(SoundID.BathBombBurst, transform.position, 0.5f, 0.9f);
        float percent = timer2 / Data1;
        int count = (int)(2 + 6.5f * Mathf.Clamp01(percent));
        for (int i = 0; i < count; ++i)
        {
            NewProjectile<SnowBullet>(transform.position, new Vector2(0, Utils.RandFloat(5, 6)).RotatedBy((float)i / count * Utils.TwoPI) + Utils.RandCircle(), 1, null, 1.5f, 0.5f, 0.9f, 1.0f);
        }
    }
}
public class SnowBullet : Bullet
{
    public override void Init()
    {
        base.Init();
        SpriteRenderer.sprite = Main.TextureAssets.Snowball;
        SpriteRenderer.material = Main.TextureAssets.SpriteLit;
        SpriteRendererGlow.transform.localScale *= 0.8f;
    }
    public override void AI()
    {
        base.AI();
        RB.rotation += 1 * RB.velocity.magnitude * Utils.SignNoZero(RB.velocity.x);
    }
    public override void OnKill()
    {
        base.OnKill();
        for (int i = 0; i < 6; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(3) * SizeMult, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position, Utils.RandFloat(0.2f, 0.3f) * SizeMult, circular + Vector2.up * 0.2f, 1f, Utils.RandFloat(0.3f, 0.4f), ParticleManager.ID.Snow, Color.white);
        }
    }
}
