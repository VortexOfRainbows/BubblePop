using UnityEngine;
using static Quest;

public class BreakableObject : MonoBehaviour, IImpactedByProjIFrames
{
    public enum BreakableObjectType
    {
        Crate = 0,
        Barrel = 1,
    }
    public BreakableObjectType Type;
    public SpriteRenderer SpriteRenderer;
    public Rigidbody2D RB;
    public int HitsRequiredToKill { get; set; } = 3;
    public void OnCollisionEnter2D(Collision2D collision) => OnTriggerStay2D(collision.collider);
    public void OnCollisionStay2D(Collision2D collision) => OnTriggerStay2D(collision.collider);
    public void OnTriggerEnter2D(Collider2D collision) => OnTriggerStay2D(collision);
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Proj") && collision.gameObject.TryGetComponent(out Projectile p))
            ReceiveProjectileImpact(p);
    }
    public void ReceiveProjectileImpact(Projectile p)
    {
        if (AlreadyBroken)
            return;
        if (!p.SpecializedImmuneFrames.Contains(this) && ((p.Damage > 0 && p.Friendly) || p.Hostile))
        {
            if (p.Penetrate != -1 && --p.Penetrate == 0)
                p.Kill();
            else
                p.SpecializedImmuneFrames.Add(new Projectile.ImmunityData(this, p.immunityFrames));
            HitsRequiredToKill -= (int)Mathf.Max(1, p.Damage);
            if (HitsRequiredToKill <= 0 || p is MeleeHitbox)
            {
                Break();
            }
            else
            {
                if (Type == BreakableObjectType.Crate || Type == BreakableObjectType.Barrel)
                {
                    AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 0.6f, 1.6f);
                    SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.red, 0.5f);
                }
            }
        }
    }
    public bool AlreadyBroken { get; set; } = false;
    public CapsuleCollider2D Collider;
    public void Break()
    {
        AlreadyBroken = true;
        Color c = Color.white;
        if(Type == BreakableObjectType.Crate)
        {
            c = ColorHelper.WoodColor;
            AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 1, Utils.RandFloat(0.875f, 0.925f));
            float dropRand = Utils.RandFloat();
            if (dropRand < 0.02f)
                CoinManager.SpawnShield(transform.position, .5f);
            else if (dropRand < 0.04f)
                CoinManager.SpawnHeart(transform.position, .5f);
            else if (dropRand < 0.12f)
                CoinManager.SpawnKey(transform.position, .5f);
            else if (dropRand < 0.25f)
                CoinManager.SpawnGem(transform.position, .5f, Utils.RandInt(1, 2 + WaveDirector.WaveNum / 3));
            else if (dropRand < 0.75f)
                CoinManager.SpawnCoin(transform.position, Utils.RandInt(1, 6 + WaveDirector.WaveNum), .5f, true);
        }
        else if(Type == BreakableObjectType.Barrel)
        {
            c = ColorHelper.WoodColor;
            AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 1, Utils.RandFloat(0.9f, 0.975f));
            float dropRand = Utils.RandFloat();
            if (dropRand < 0.025f)
                CoinManager.SpawnHeart(transform.position, .5f);
            else if (dropRand < 0.1f)
                CoinManager.SpawnGem(transform.position, .5f, Utils.RandInt(1, 2 + WaveDirector.WaveNum / 3));
            else
                CoinManager.SpawnCoin(transform.position, Utils.RandInt(1, 4 + WaveDirector.WaveNum), .5f, true);
        }
        for (int i = 0; i < 30; ++i)
        {
            Vector2 randPos = Collider.bounds.min + new Vector3(Collider.bounds.extents.x * Utils.RandFloat(2f), Collider.bounds.extents.y * Utils.RandFloat(2f));
            ParticleManager.NewParticle(randPos, 0.7f * Utils.RandFloat(0.8f, 1.0f), Utils.RandCircle(6) + Vector2.up * Utils.RandFloat(5, 10), 5, Utils.RandFloat(1, 1.2f), 1,
                Color.Lerp(c, Color.black, Utils.RandFloat(0.2f)));
        }
        Destroy(gameObject);
    }
    public void FixedUpdate()
    {
        if(Type == BreakableObjectType.Crate)
        {
            RB.velocity *= 0.94f;
            SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.white, 0.07f);
        }
        else if(Type == BreakableObjectType.Barrel)
        {
            RB.velocity *= 0.95f;
            SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.white, 0.07f);
        }
    }
}
