using UnityEngine;

public class BathBomb : Projectile
{
    public override void Init()
    {
        SpriteRenderer.sprite = Main.BathBombSprite;
    }
    public override void AI()
    {
        Color color = PickColor(Data2, timer2++);
        float yPointBeforeLanding = Data1;
        float distTillLanding = transform.position.y - Data1;
        transform.localScale = Vector3.one * (1 + 0.1f * Data2 + distTillLanding / 15f);

        SpriteRendererGlow.gameObject.transform.localPosition = new Vector3(0, -distTillLanding / transform.localScale.y, 0);
        SpriteRendererGlow.color = color * 1.5f / transform.localScale.x * Mathf.Min(timer2 / 100f, 1);

        Vector2 velo = RB.velocity;
        if (transform.position.y < yPointBeforeLanding + 1)
        {
            if (velo.y < 0)
                velo.y *= 0.75f;
            velo.x *= 0.9f;
            velo.y += 0.2f;
            if (timer <= 0)
            {
                AudioManager.PlaySound(SoundID.BathBombSizzle, transform.position, 1, 0.9f);
                AudioManager.PlaySound(SoundID.BathBombSplash, transform.position, 1, 1.1f);
                for (int i = 0; i < 20; i++)
                    ParticleManager.NewParticle((Vector2)transform.position + new Vector2(Utils.RandFloat(-1f, 1f), -1.1f * transform.localScale.y), .7f, velo * 0.2f + new Vector2(0, Utils.RandFloat(1, 3)), 4f, Utils.RandFloat(1.2f, 1.5f), 0, ParticleManager.BathColor);
                timer++;
            }
            else if (timer % 2 == 0)
                ParticleManager.NewParticle((Vector2)transform.position + new Vector2(Utils.RandFloat(-1f, 1f), -1.1f * transform.localScale.y), .5f, velo * 0.15f + new Vector2(0, Utils.RandFloat(1)), 3f, Utils.RandFloat(0.8f, 1.2f), 0, ParticleManager.BathColor);
        }
        else
        {
            velo.x *= 0.925f;
            velo.y -= 0.125f;
        }
        RB.velocity = velo;
        if (timer > 0)
        {
            float sqr = timer / 240f;
            sqr *= sqr;
            float sin = Mathf.Sin(sqr * Mathf.PI * 12f);
            transform.localScale *= 1f + (sin * 0.1f + sqr * 0.2f) + sqr * 0.4f;
            timer++;
            if (Utils.RandFloat(1) < 0.6f)
            {
                ParticleManager.NewParticle((Vector2)transform.position + new Vector2(0, 0.9f * transform.localScale.y), Utils.RandFloat(0.4f, 0.5f), velo * 0.08f + new Vector2(0, Utils.RandFloat(4 + sqr * 3.5f, 5 + sqr * 5)), 3f, Utils.RandFloat(0.8f, 1.2f), 1, Color.Lerp(color, Color.red, sqr * 0.8f));
            }
            else
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * Utils.RandFloat(2));
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.5f, 0.7f), circular * Utils.RandFloat(4 + sqr * 6, 12 + sqr * 6) + new Vector2(0, Utils.RandFloat(-1, 2)), 4f, Utils.RandFloat(0.5f, .6f), 0, color);
            }
            SpriteRenderer.color = Color.Lerp(color, Color.red, 0.0f + Mathf.Max(0, sin * 0.4f) + 0.6f * sqr);
        }
        else
            SpriteRenderer.color = color;
        if (timer > 240)
        {
            Kill();
        }
    }
    public override void OnKill()
    {
        AudioManager.PlaySound(SoundID.BathBombBurst, transform.position, 1.1f, 0.9f);
        Color c = PickColor(Data2, timer2);
        for (int i = 0; i < 70; i++)
        {
            Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * i / 35f);
            Color color = Data2 == 6 ? PickColor(Data2, i / 69f * Mathf.Deg2Rad) : c;
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.5f, 0.9f), circular * Utils.RandFloat(4, 20) + new Vector2(0, Utils.RandFloat(-1, 3)), 4f, Utils.RandFloat(0.7f, 1f), 0, c);
        }
        if (Data2 == 6)
        {
            for (int j = 0; j < 3; j++)
            {
                float rand = Utils.RandFloat(360);
                for (int i = 0; i < 14; i++)
                {
                    float r = (j * 8 + i / 14f * 360) + rand;
                    Vector2 circular = new Vector2(1, 0).RotatedBy(r * Mathf.Deg2Rad);
                    NewProjectile<Spike>((Vector2)transform.position + circular * (j * 2f), circular * 2.0f, Mathf.Sign(j % 2 * 2 - 1) * (1 + j * 0.5f), Data2 + r / 2.5f);
                }
            }
        }
        if (Data2 == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                float r = Mathf.PI * i / 5f;
                Vector2 circular = new Vector2(1, 0).RotatedBy(r);
                NewProjectile<Spike>((Vector2)transform.position + circular * 0.5f, circular * 2.0f, 0, Data2);
            }
        }
        if (Data2 == 1)
        {
            for (float j = 1; j < 2.5f; j += 0.2f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 circular = new Vector2(j, 0).RotatedBy(Mathf.PI * i / 2f);
                    NewProjectile<Spike>((Vector2)transform.position + circular * 0.5f, circular * 2.0f, 0, Data2);
                }
            }
        }
        if (Data2 == 2)
        {
            for (int i = 0; i < 24; i++)
            {
                Vector2 circular = Utils.RandCircle(3);
                NewProjectile<Spike>((Vector2)transform.position + circular * 0.5f, circular * 1.5f, 0, Data2);
            }
        }
        if (Data2 == 3)
        {
            float r = Utils.RandFloat(Mathf.PI);
            float petals = 10;
            for (int i = 0; i < 36; i++)
            {
                float sin = Mathf.Sin(i * Mathf.PI / 36f * petals);
                Vector2 circular = new Vector2(0.7f + 0.3f * sin, 0).RotatedBy(r + Mathf.PI * i / 18f);
                NewProjectile<Spike>((Vector2)transform.position + circular * 0.5f, circular * 2.0f, 0, Data2);
            }
        }
        if (Data2 == 4)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * i / 5f);
                    NewProjectile<Spike>((Vector2)transform.position + circular * 0.5f, circular * 2.0f, j * 1f, Data2);
                }
            }
        }
        if (Data2 == 5)
        {
            for (float j = 1; j < 3.5f; j += 0.5f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 circular = new Vector2(j, 0).RotatedBy(Mathf.PI * i / 2f + j * -25 * Mathf.Deg2Rad);
                    NewProjectile<Spike>((Vector2)transform.position + circular * 0.25f, circular * 1.7f, 1, Data2);
                }
            }
        }
        bool LuckyDrop = Utils.RandFloat(1) < 0.04f;
        if (WaveDirector.CanSpawnPower() || LuckyDrop)
            PowerUp.Spawn(PowerUp.RandomFromPool(), transform.position, LuckyDrop ? 0 : 100);
    }
}
public class Spike : Projectile
{
    public override void Init()
    {
        SpriteRenderer.sprite = Main.bathBombShards[Utils.RandInt(4)];
        SpriteRenderer.color = PickColor(Mathf.Min(Data2, 6), Data2 - 6);
        SpriteRendererGlow.transform.localScale = new Vector3(1, 1, 1) * 1.4f;
        SpriteRendererGlow.color = SpriteRenderer.color;
        Data2 = 6;
        Hostile = true;
    }
    public override void AI()
    {
        if (startPos == Vector2.zero)
            startPos = (Vector2)transform.position - RB.velocity.normalized;
        float accelerationStop = Data2 == 5 ? 100 : 200;
        if (timer < accelerationStop && Data2 != 4 && Data2 != 6)
        {
            RB.velocity *= Data2 == 5 ? 1.004f : 1.0045f;
        }
        RB.rotation += RB.velocity.magnitude * 0.2f * Mathf.Sign(RB.velocity.x) + 0.2f * RB.velocity.x;
        timer++;
        if (timer > 610)
        {
            float alphaOut = 1 - (timer - 610) / 90f;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alphaOut);
            SpriteRendererGlow.color = new Color(SpriteRendererGlow.color.r, SpriteRendererGlow.color.g, SpriteRendererGlow.color.b) * alphaOut;
            if (timer > 620)
                Hostile = false;
        }
        if (timer > 700)
        {
            Kill();
        }
        if (Data2 == 5)
        {
            RB.velocity = RB.velocity.RotatedBy(Data1 * Mathf.Deg2Rad);
        }
        Color color = SpriteRenderer.color;
        if (Data2 == 4 || Data2 == 6)
        {
            Vector2 fromStart = (Vector2)transform.position - startPos;
            Vector2 rotate = fromStart.RotatedBy(Data1 * Mathf.Deg2Rad);
            float rotateDist = rotate.magnitude;
            rotate = rotate.normalized * (rotateDist + RB.velocity.magnitude * Time.fixedDeltaTime);
            transform.position = startPos + rotate - RB.velocity * Time.fixedDeltaTime;
            if (Data1 != 0)
                Data1 = Mathf.Sign(Data1) * (0.3f + 1.1f / rotateDist);
            if (timer < 200)
                RB.velocity *= 1.0045f;
            color.a *= 0.5f;
        }
        ParticleManager.NewParticle((Vector2)transform.position, 0.175f, Utils.RandCircle(0.01f), 1f, 0.175f, 1, color);
    }
    public override void OnKill()
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 circular = new Vector2(.5f, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.2f, 0.4f), circular * Utils.RandFloat(3, 6), 4f, 0.4f, 1, SpriteRenderer.color);
        }
    }
}