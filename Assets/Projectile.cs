using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static GameObject NewProjectile(Vector2 pos, Vector2 velo, int type = 0, float data1 = 0)
    {
        GameObject Proj = Instantiate(GlobalDefinitions.Projectile, pos, Quaternion.identity);
        Proj.GetComponent<Rigidbody2D>().velocity = velo;
        Projectile proj = Proj.GetComponent<Projectile>();
        proj.Type = type;
        proj.Data1 = data1;
        proj.Init();
        return Proj;
    }
    public void Kill()
    {
        OnKill();
        Destroy(gameObject);
    }
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public CircleCollider2D c2D;
    public float timer = 0f;
    public int Type = 0;
    public float Data1;
    public void FixedUpdate()
    {
        if(Type == 0)
            BubbleAI();
        if (Type == 1)
            BathBombAI();
        if (Type == 2)
            SpikeAI();
    }
    public void Init()
    {
        if (Type == 0)
            transform.localScale *= 0.3f;
        if (Type == 2)
            spriteRenderer.sprite = GlobalDefinitions.SpikyProjectileSprite;
    }
    public void BubbleAI()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.6f, 0.085f);

        Vector2 velo = rb.velocity;
        velo *= 0.995f - timer / 5000f;
        velo.y += 0.005f;
        rb.velocity = velo;

        if (rb.velocity.magnitude < 0.5f || timer > 200)
        {
            Kill();
        }
        if ((int)timer % 3 == 0)
        {
            Vector2 norm = rb.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f, .25f, norm * -.75f, 0.6f, .3f);
        }
        timer++;
    }
    public void BathBombAI()
    {
        float yPointBeforeLanding = Data1;
        float distTillLanding = transform.position.y - Data1;
        transform.localScale = Vector3.one * (1 + distTillLanding / 20f);

        Vector2 velo = rb.velocity;
        if (transform.position.y < yPointBeforeLanding)
        {
            if(velo.y < 0)
                velo.y *= 0.75f;
            velo.x *= 0.9f;
            velo.y += 0.2f;
            if (timer <= 0)
            {
                for(int i = 0; i < 15; i++)
                    ParticleManager.NewParticle((Vector2)transform.position + new Vector2(0, -0.5f), .7f, velo * 0.2f + new Vector2(0, Utils.RandFloat(1, 3)), 4f, 1.5f);
                timer++;
            }
            else if(timer % 2 == 0)
                ParticleManager.NewParticle((Vector2)transform.position + new Vector2(0, -0.5f), .5f, velo * 0.15f + new Vector2(0, Utils.RandFloat(1)), 3f, 1.0f);
        }
        else
        {
            velo.x *= 0.925f;
            velo.y -= 0.125f;
        }
        rb.velocity = velo;

        if(timer > 0)
        {
            float sqr = timer / 240f;
            sqr *= sqr;
            float sin = Mathf.Sin(sqr * Mathf.PI * 12f);
            transform.localScale *= 1f + (sin * 0.1f + sqr * 0.2f) + sqr * 0.4f;
            timer++;
            spriteRenderer.color = Color.Lerp(Color.white, Color.red, 0.0f + Mathf.Max(0, sin * 0.4f) + 0.6f * sqr);
        }
        if (timer > 240)
        {
            Kill();
        }
    }
    public void SpikeAI()
    {
        rb.velocity *= 1.011f;
        rb.rotation = rb.velocity.ToRotation() * Mathf.Rad2Deg;
        timer++;
        if(timer > 610)
        {
            float alphaOut = (timer - 610) / 90f;
            spriteRenderer.color = Color.white * alphaOut;
        }
        if(timer > 700)
        {
            Kill();
        }
    }
    public void OnKill()
    {
        if(Type == 1)
        {
            for (int i = 0; i < 50; i++)
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * i / 25f);
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.5f, 1.0f), circular * Utils.RandFloat(4, 20) + new Vector2(0, Utils.RandFloat(-1, 3)), 4f, 0.5f);
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * i / 5f);
                NewProjectile((Vector2)transform.position + circular * 0.5f, circular * 2.0f, 2, 0);
            }
        }
    }
}
