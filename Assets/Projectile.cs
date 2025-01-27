using UnityEngine;
public class Projectile : MonoBehaviour
{
    public static int colorGradient = 0;
    public static GameObject NewProjectile(Vector2 pos, Vector2 velo, int type = 0, float data1 = 0, float data2 = 0)
    {
        GameObject Proj = Instantiate(GlobalDefinitions.Projectile, pos, Quaternion.identity);
        Proj.GetComponent<Rigidbody2D>().velocity = velo;
        Projectile proj = Proj.GetComponent<Projectile>();
        proj.Type = type;
        proj.Data1 = data1;
        proj.Data2 = data2;
        proj.Init();
        return Proj;
    }
    public void Kill()
    {
        if (!Dead)
            Dead = true;
        else
            return;
        OnKill();
        Destroy(gameObject);
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Tub" && Type != 1)
        {
            if (Type == 3)
            {
                if(timer > 10)
                {
                    Vector2 toClosest = collision.ClosestPoint(transform.position) - (Vector2)transform.position;

                    rb.velocity = -toClosest.normalized * 0.75f * rb.velocity.magnitude;
                }
                return;
            }
            Kill();
        }
    }
    public SpriteRenderer spriteRendererGlow;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public CircleCollider2D c2D;
    public float timer = 0f;
    public float timer2 = 0f;
    public int Type = 0;
    public float Data1 = 0;
    public float Data2 = 0;
    public int Damage = 0;
    public bool Friendly = false;
    public bool Hostile = false;
    public void Init()
    {
        if (Type == 0 || Type == 3)
        {
            spriteRenderer.sprite = Type == 0 ? GlobalDefinitions.BubbleSmall : GlobalDefinitions.BubbleSprite;
            Color c = ParticleManager.DefaultColor;
            c.a = 0.68f;
            spriteRenderer.color = c;
            if (Type == 0)
                timer += Random.Range(0, 40);
            transform.localScale *= 0.3f;
            Damage = 1;
            Friendly = Type == 0;
        }
        if (Type == 1)
        {
            spriteRenderer.sprite = GlobalDefinitions.BathBombSprite;
            //Hostile = true;
        }
        if (Type == 2)
        {
            spriteRenderer.sprite = GlobalDefinitions.bathBombShards[Random.Range(0, 4)];
            spriteRenderer.color = PickColor(Mathf.Min(Data2, 6), Data2 - 6);
            spriteRendererGlow.transform.localScale = new Vector3(1, 1, 1) * 1.4f;
            Data2 = 6;
            Hostile = true;
        }
        if(Type == 4)
        {
            spriteRenderer.sprite = Data1 == 0 ? GlobalDefinitions.BubblePower1 :  GlobalDefinitions.BubblePower2;
            spriteRendererGlow.color = new Color(241 / 255f, 215 / 255f, 1f, 0.6f);
            spriteRendererGlow.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
            Hostile = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
        if(Type == 5)
        {
            transform.localScale = new Vector3(0.65f, 0.45f, 1);
            spriteRendererGlow.transform.localScale = new Vector3(1.5f, 4f, 1);
            spriteRendererGlow.color = new Color(253 / 255f, 181 / 255f, 236 / 255f, 0.5f);
            spriteRenderer.sprite = GlobalDefinitions.Feather;
            Hostile = true;
        }
        if (Type == 6)
        {
            transform.localScale = new Vector3(0.9f, 0.8f, 1);
            spriteRendererGlow.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
            spriteRendererGlow.color = new Color(168f / 255f, 62f / 255f, 70f / 255f, 0.4f);
            spriteRenderer.sprite = GlobalDefinitions.Laser;
            Hostile = true;
        }
        FixedUpdate();
    }
    public void FixedUpdate()
    {
        if(Type == 0)
            BubbleAI();
        if (Type == 1)
            BathBombAI();
        if (Type == 2)
            SpikeAI();
        if (Type == 3)
            BigBubbleAI();
        if (Type == 4)
            PowerUpAI();
        if (Type == 5)
            FeatherAI();
        if (Type == 6)
            LaserAI();
        if (Type == 2)
            spriteRendererGlow.color = spriteRenderer.color;
        else if(Type == 0 || Type == 3)
            spriteRendererGlow.gameObject.SetActive(false);
    }
    public void BubbleAI()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.66f, 0.085f);

        Vector2 velo = rb.velocity;
        velo *= 0.9965f - timer / 6000f;
        velo.y += 0.005f;
        rb.velocity = velo;
        rb.rotation += Mathf.Sqrt(rb.velocity.magnitude) * Mathf.Sign(rb.velocity.x);

        if (timer > 200)
        {
            Kill();
        }
        if ((int)timer % 3 == 0)
        {
            Vector2 norm = rb.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f, .25f, norm * -.75f, 0.8f, Utils.RandFloat(0.25f, 0.4f));
        }
        if(timer > 180)
        {
            float alphaOut = 1 - (timer - 180) / 20f;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alphaOut);
        }
        timer++;
    }
    public Color PickColor(float data, float counter)
    {
        Color color = Color.white;
        if (data == 0)
            color = new Color(112 / 255f, 54 / 255f, 157 / 255f);
        if (data == 1)
            color = new Color(75 / 255f, 54 / 255f, 255 / 255f);
        if (data == 2)
            color = new Color(121 / 255f, 195 / 255f, 20 / 255f);
        if (data == 3)
            color = new Color(250 / 255f, 235 / 255f, 54 / 255f);
        if (data == 4)
            color = new Color(255 / 255f, 165 / 255f, 0 / 255f);
        if (data == 5)
            color = new Color(232 / 255f, 20 / 255f, 22 / 255f);
        if (data == 6)
            color = Rainbow(counter);
        return color;
    }
    public Color Rainbow(float timer)
    {
        timer = timer * Mathf.Deg2Rad * 2.5f;
        double center = 130;
        Vector2 circlePalette = new Vector2(1, 0).RotatedBy(timer);
        double width = 125 * circlePalette.y;
        int red = (int)(center + width);
        circlePalette = new Vector2(1, 0).RotatedBy(timer + Mathf.PI * 2f / 3f);
        width = 125 * circlePalette.y;
        int grn = (int)(center + width);
        circlePalette = new Vector2(1, 0).RotatedBy(timer + Mathf.PI * 4f / 3f);
        width = 125 * circlePalette.y;
        int blu = (int)(center + width);
        return new Color(red / 255f, grn / 255f, blu / 255f);
    }
    public void BathBombAI()
    {
        Color color = PickColor(Data2, timer2++);
        float yPointBeforeLanding = Data1;
        float distTillLanding = transform.position.y - Data1;
        transform.localScale = Vector3.one * (1 + 0.1f * Data2 + distTillLanding / 15f);

        spriteRendererGlow.gameObject.transform.localPosition = new Vector3(0, -distTillLanding / transform.localScale.y, 0);
        spriteRendererGlow.color = color * 1.5f / transform.localScale.x * Mathf.Min(timer2 / 100f, 1);

        Vector2 velo = rb.velocity;
        if (transform.position.y < yPointBeforeLanding + 1)
        {
            if(velo.y < 0)
                velo.y *= 0.75f;
            velo.x *= 0.9f;
            velo.y += 0.2f;
            if (timer <= 0)
            {
                AudioManager.PlaySound(GlobalDefinitions.audioClips[24], transform.position, 1, 0.9f);
                AudioManager.PlaySound(GlobalDefinitions.audioClips[26], transform.position, 1, 1.1f);
                for (int i = 0; i < 20; i++)
                    ParticleManager.NewParticle((Vector2)transform.position + new Vector2(Utils.RandFloat(-1f, 1f), -1.1f * transform.localScale.y), .7f, velo * 0.2f + new Vector2(0, Utils.RandFloat(1, 3)), 4f, Utils.RandFloat(1.2f, 1.5f), 0, ParticleManager.BathColor);
                timer++;
            }
            else if(timer % 2 == 0)
                ParticleManager.NewParticle((Vector2)transform.position + new Vector2(Utils.RandFloat(-1f, 1f), -1.1f * transform.localScale.y), .5f, velo * 0.15f + new Vector2(0, Utils.RandFloat(1)), 3f, Utils.RandFloat(0.8f, 1.2f), 0, ParticleManager.BathColor);
        }
        else
        {
            velo.x *= 0.925f;
            velo.y -= 0.125f;
        }
        rb.velocity = velo;
        if (timer > 0)
        {
            float sqr = timer / 240f;
            sqr *= sqr;
            float sin = Mathf.Sin(sqr * Mathf.PI * 12f);
            transform.localScale *= 1f + (sin * 0.1f + sqr * 0.2f) + sqr * 0.4f;
            timer++;
            if(Utils.RandFloat(1) < 0.6f)
            {
                ParticleManager.NewParticle((Vector2)transform.position + new Vector2(0, 0.9f * transform.localScale.y), Utils.RandFloat(0.4f, 0.5f), velo * 0.08f + new Vector2(0, Utils.RandFloat(4 + sqr * 3.5f, 5 + sqr * 5)), 3f, Utils.RandFloat(0.8f, 1.2f), 1, Color.Lerp(color, Color.red, sqr * 0.8f));
            }
            else
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * Utils.RandFloat(2));
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.5f, 0.7f), circular * Utils.RandFloat(4 + sqr * 6, 12 + sqr * 6) + new Vector2(0, Utils.RandFloat(-1, 2)), 4f, Utils.RandFloat(0.5f, .6f), 0, color);
            }
            spriteRenderer.color = Color.Lerp(color, Color.red, 0.0f + Mathf.Max(0, sin * 0.4f) + 0.6f * sqr);
        }
        else
            spriteRenderer.color = color;
        if (timer > 240)
        {
            Kill();
        }
    }
    private Vector2 startPos = Vector2.zero;
    public void SpikeAI()
    {
        if(startPos == Vector2.zero)
            startPos = (Vector2)transform.position - rb.velocity.normalized; 
        if (timer < 200 && Data2 != 5 && Data2 != 6)
        {
            rb.velocity *= 1.0045f;
        }
        rb.rotation += rb.velocity.magnitude * 0.2f * Mathf.Sign(rb.velocity.x) + 0.2f * rb.velocity.x;
        timer++;
        if(timer > 610)
        {
            float alphaOut = 1 - (timer - 610) / 90f;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alphaOut);
            spriteRendererGlow.color = new Color(spriteRendererGlow.color.r, spriteRendererGlow.color.g, spriteRendererGlow.color.b, alphaOut);
            if (timer > 650)
                Hostile = false;
        }
        if(timer > 700)
        {
            Kill();
        }
        if(Data2 == 2)
        {
            rb.velocity = rb.velocity.RotatedBy(Data1 * Mathf.Deg2Rad);
        }
        Color color = spriteRenderer.color;
        if (Data2 == 5 || Data2 == 6)
        {
            Vector2 fromStart = (Vector2)transform.position - startPos;
            Vector2 rotate = fromStart.RotatedBy(Data1 * Mathf.Deg2Rad);
            float rotateDist = rotate.magnitude;
            rotate = rotate.normalized * (rotateDist + rb.velocity.magnitude * Time.fixedDeltaTime);
            transform.position = startPos + rotate - rb.velocity * Time.fixedDeltaTime;
            if(Data1 != 0)
                Data1 = Mathf.Sign(Data1) * (0.3f + 1.1f / rotateDist);
            if(timer < 200)
                rb.velocity *= 1.0045f;
            color.a *= 0.5f;
        }
        ParticleManager.NewParticle((Vector2)transform.position, 0.175f, Utils.RandCircle(0.01f), 1f, 0.175f, 1, color);
    }
    public void BigBubbleAI()
    {
        Vector2 toMouse = Utils.MouseWorld - Player.Position;
        if(Player.Instance.AttackRight >= 50 && timer <= 0)
        {
            int target = (int)(Player.Instance.AttackRight - 50) / 100;
            float targetSize = target * 0.7f + 0.8f + Player.Instance.AttackRight / 240f;
            targetSize *= 1f + Mathf.Sqrt(Player.DamagePower) * 0.4f;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetSize, 0.1f);
            timer = -Player.Instance.AttackRight;
            Vector2 circular = new Vector2(targetSize, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            if(Utils.RandFloat(1) < 0.2f)
                ParticleManager.NewParticle((Vector2)transform.position + circular, .2f, -circular.normalized * 6 + Player.Instance.rb.velocity * 0.9f, 0.2f, 0.3f, 0, default);
            if (Player.Instance.AttackRight == 149|| Player.Instance.AttackRight == 249)
            {
                for (int i = 0; i < 30; i++)
                {
                    circular = new Vector2(targetSize, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
                    ParticleManager.NewParticle((Vector2)transform.position + circular * 1.1f, .3f, -circular.normalized * Utils.RandFloat(5, 10) + Player.Instance.rb.velocity * 0.9f, 0.2f, Utils.RandFloat(0.2f, 0.4f), 0, default);
                }
            }
            Vector2 awayFromWand = new Vector2(1.4f, (0.51f + targetSize * 0.49f) * Mathf.Sign(Player.Instance.PointDirOffset)).RotatedBy(Player.Instance.Wand.transform.eulerAngles.z * Mathf.Deg2Rad);
            transform.position = Vector2.Lerp(transform.position,(Vector2)Player.Instance.Wand.transform.position + awayFromWand, 0.15f);
            rb.velocity *= 0.8f;
            rb.velocity += Player.Instance.rb.velocity * 0.1f;
            Damage = (1 + target) * (2 + Player.DamagePower);
            //rb.rotation = toMouse.ToRotation() * Mathf.Rad2Deg;
        }
        else if(timer <= 0)
        {
            AudioManager.PlaySound(GlobalDefinitions.audioClips[15], transform.position, 1.1f, 0.6f);
            if (toMouse.magnitude < 6)
                toMouse = toMouse.normalized * 6;
            Vector2 mouse = Player.Position + toMouse;
            toMouse = mouse - (Vector2) transform.position;
            rb.velocity = toMouse * 0.1f + toMouse.normalized * (10f + Mathf.Min(24, 24f * (timer + 50) / -200f));
            timer = 1;

            for (int i = 0; i < 30; i++)
                ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(transform.localScale.x), Utils.RandFloat(.3f, .5f), rb.velocity * Utils.RandFloat(2f), 0.5f, Utils.RandFloat(.4f, 1f), 0, default);

            rb.rotation = rb.velocity.ToRotation() * Mathf.Rad2Deg;
        }
        else if(timer > 0)
        {
            rb.rotation += Mathf.Sqrt(rb.velocity.magnitude) * Mathf.Sign(rb.velocity.x);
            if (Utils.RandFloat(1) < 0.15f)
                ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(transform.localScale.x * 0.5f), Utils.RandFloat(.3f, .4f), rb.velocity * Utils.RandFloat(1f), 0.4f, Utils.RandFloat(.3f, .6f), 0, default);
            Friendly = true;
            rb.velocity *= 0.997f - timer / 6000f;
            timer++;
            if (timer > 160)
            {
                float alphaOut = 1 - (timer - 160) / 20f;
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alphaOut);
            }
            if (timer > 180)
                Kill();
        }
    }
    public void PowerUpAI()
    {
        timer++;
        float scale = 1.0f + 0.1f * Mathf.Sin(Mathf.Deg2Rad * timer * 2f);
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2f / scale, 2f * scale, 2), 0.1f);

        if(Utils.RandFloat(1) < 0.4f)
        {
            Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * Utils.RandFloat(2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.5f, 0.6f), circular * Utils.RandFloat(3, 6) + new Vector2(0, Utils.RandFloat(-1, 2)),
                2f, Utils.RandFloat(0.3f, .5f), 0, spriteRendererGlow.color * 0.9f);
        }
    }
    public void FeatherAI()
    {
        rb.rotation = rb.velocity.ToRotation() * Mathf.Rad2Deg + 90;

        timer++;
        if(timer < 300)
        {
            rb.velocity += rb.velocity.normalized * 0.006f;
            rb.velocity += (Player.Position - (Vector2)transform.position).normalized * 0.075f;
        }
        if (timer > 610)
        {
            float alphaOut = 1 - (timer - 610) / 90f;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alphaOut);
            spriteRendererGlow.color = new Color(spriteRendererGlow.color.r, spriteRendererGlow.color.g, spriteRendererGlow.color.b, alphaOut);
            if (timer > 650)
                Hostile = false;
        }
        if (timer > 700)
        {
            Kill();
        }
        if(Utils.RandFloat(1) < 0.5f)
            ParticleManager.NewParticle((Vector2)transform.position, 0.5f, Utils.RandCircle(0.02f), 1.5f, 0.4f, 0, spriteRendererGlow.color);
    }
    public void LaserAI()
    {
        rb.rotation = rb.velocity.ToRotation() * Mathf.Rad2Deg + 180;
        for (float i = 0; i < 1; i += 0.5f)
            ParticleManager.NewParticle((Vector2)transform.position + rb.velocity * i * Time.fixedDeltaTime, 0.5f, -rb.velocity.normalized * 2.5f, 0f, 0.3f, 1, spriteRendererGlow.color);
        if (timer < 200)
        {
            rb.velocity += rb.velocity.normalized * 0.02f;
            rb.velocity += (Player.Position - (Vector2)transform.position).normalized * 0.08f * (1 - timer / 200f);
        }
        if (timer > 610)
        {
            float alphaOut = 1 - (timer - 610) / 90f;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alphaOut);
            spriteRendererGlow.color = new Color(spriteRendererGlow.color.r, spriteRendererGlow.color.g, spriteRendererGlow.color.b, alphaOut);
            if (timer > 650)
                Hostile = false;
        }
        if (timer > 700)
        {
            Kill();
        }
        timer++;
    }
    private bool Dead = false;
    public void OnKill()
    {
        if(Type == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.3f, 0.6f), circular * Utils.RandFloat(3, 6), 4f, 0.4f);
            }
            AudioManager.PlaySound(GlobalDefinitions.audioClips[Random.Range(0, 8)], transform.position, 0.7f, 1.1f);
        }
        if (Type == 1)
        {
            AudioManager.PlaySound(GlobalDefinitions.audioClips[25], transform.position, 1.1f, 0.9f);
            Color c = PickColor(Data2, timer2);
            for (int i = 0; i < 70; i++)
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * i / 35f);
                Color color = Data2 == 6 ? PickColor(Data2, i / 69f * Mathf.Deg2Rad) : c;
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.5f, 0.9f), circular * Utils.RandFloat(4, 20) + new Vector2(0, Utils.RandFloat(-1, 3)), 4f, Utils.RandFloat(0.7f, 1f), 0, c);
            }
            if (Data2 == 6)
            {
                for(int j = 0; j < 3; j++)
                {
                    float rand = Utils.RandFloat(360);
                    for (int i = 0; i < 14; i++)
                    {
                        float r = (j * 8 + i / 14f * 360) + rand;
                        Vector2 circular = new Vector2(1, 0).RotatedBy(r * Mathf.Deg2Rad);
                        NewProjectile((Vector2)transform.position + circular * (j * 2f), circular * 2.0f, 2, Mathf.Sign(j % 2 * 2 - 1) * (1 + j * 0.5f), Data2 + r / 2.5f);
                    }
                }
            }
            if (Data2 == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    float r = Mathf.PI * i / 5f;
                    Vector2 circular = new Vector2(1, 0).RotatedBy(r);
                    NewProjectile((Vector2)transform.position + circular * 0.5f, circular * 2.0f, 2, 0, Data2);
                }
            }
            if(Data2 == 1)
            {
                for(float j = 1; j < 2.5f; j += 0.2f)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 circular = new Vector2(j, 0).RotatedBy(Mathf.PI * i / 2f);
                        NewProjectile((Vector2)transform.position + circular * 0.5f, circular * 2.0f, 2, 0, Data2);
                    }
                }
            }
            if(Data2 == 2)
            {
                for (float j = 1; j < 3.5f; j += 0.5f)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 circular = new Vector2(j, 0).RotatedBy(Mathf.PI * i / 2f + j * -25 * Mathf.Deg2Rad);
                        NewProjectile((Vector2)transform.position + circular * 0.25f, circular * 1.7f, 2, 1, Data2);
                    }
                }
            }
            if (Data2 == 3)
            {
                for (int i = 0; i < 24; i++)
                {
                    Vector2 circular = Utils.RandCircle(3);
                    NewProjectile((Vector2)transform.position + circular * 0.5f, circular * 1.5f, 2, 0, Data2);
                }
            }
            if (Data2 == 4)
            {
                float r = Utils.RandFloat(Mathf.PI);
                float petals = 10;
                for (int i = 0; i < 36; i++)
                {
                    float sin = Mathf.Sin(i * Mathf.PI / 36f * petals);
                    Vector2 circular = new Vector2(0.7f + 0.3f * sin, 0).RotatedBy(r + Mathf.PI * i / 18f);
                    NewProjectile((Vector2)transform.position + circular * 0.5f, circular * 2.0f, 2, 0, Data2);
                }
            }
            if (Data2 == 5)
            {
                for(int j = -1; j <= 1; j += 2)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * i / 5f);
                        NewProjectile((Vector2)transform.position + circular * 0.5f, circular * 2.0f, 2, j * 1f, Data2);
                    }
                }
            }
            if (EventManager.CanSpawnPower())
            {
                Projectile.NewProjectile(transform.position, Vector2.zero, 4, Random.Range(0, 2), 0);
                EventManager.PointsSpent += 100;
            }
        }
        if (Type == 2)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 circular = new Vector2(.5f, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.2f, 0.4f), circular * Utils.RandFloat(3, 6), 4f, 0.4f, 1, spriteRenderer.color);
            }
        }
        if (Type == 3)
        {
            AudioManager.PlaySound(GlobalDefinitions.audioClips[Random.Range(0, 8)], transform.position, 0.8f, 0.9f);
            float amt = 1 + 4 * (Damage / 8f);
            for (int i = 0; i < amt; i++)
                Projectile.NewProjectile(transform.position, new Vector2(Utils.RandFloat(2, 4), 0).RotatedBy((i + Utils.RandFloat(1)) / (int)amt * Mathf.PI * 2f), 0, 0, 0);
            for (int i = 0; i < 20; i++)
            {
                Vector2 circular = new Vector2(.5f + transform.localScale.x * 0.4f, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.3f, 0.6f), circular * Utils.RandFloat(4, 7), 3f, Utils.RandFloat(0.3f, 0.5f), 0, spriteRenderer.color);
            }
        }
        if (Type == 4)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 circular = new Vector2(.5f, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.6f, 0.7f), circular * Utils.RandFloat(3, 6), 4f, Utils.RandFloat(0.4f, 0.6f), 0, spriteRendererGlow.color);
            }
            if (Data1 == 0)
                Player.ShotgunPower++;
            else
                Player.DamagePower++;
            AudioManager.PlaySound(GlobalDefinitions.audioClips[39], transform.position, 1.2f, 0.9f);
        }
        if (Type == 5)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 circular = new Vector2(.5f, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.2f, 0.4f), circular * Utils.RandFloat(3, 6), 4f, 0.4f, 1, spriteRendererGlow.color);
            }
        }
    }
}
