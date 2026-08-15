using UnityEngine;
using static Quest;

public class BreakableObject : MonoBehaviour, IImpactedByProjIFrames
{
    public enum BreakableObjectType
    {
        Crate = 0,
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
        if (collision.CompareTag("Proj") && collision.gameObject.TryGetComponent(out Projectile p) && !p.SpecializedImmuneFrames.Contains(this) && ((p.Damage > 0 && p.Friendly) || p.Hostile))
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
                if(Type == BreakableObjectType.Crate)
                {
                    AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 0.6f, 1.6f);
                    SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.red, 0.5f);
                }
            }
        }
    }
    public CapsuleCollider2D Collider;
    public void Break()
    {
        Color c = Color.white;
        if(Type == BreakableObjectType.Crate)
        {
            c = ColorHelper.WoodColor;
        }
        for (int i = 0; i < 30; ++i)
        {
            Vector2 randPos = Collider.bounds.min + new Vector3(Collider.bounds.extents.x * Utils.RandFloat(0.1f, 0.9f), Collider.bounds.extents.y * Utils.RandFloat(0.1f, 0.9f));
            ParticleManager.NewParticle(randPos, 0.7f * Utils.RandFloat(0.8f, 1.0f), Utils.RandCircle(6) + Vector2.up * Utils.RandFloat(5, 10), 5, Utils.RandFloat(1, 1.2f), 1,
                Color.Lerp(c, Color.black, Utils.RandFloat(0.2f)));
        }
        AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 1, 0.9f);
        Destroy(gameObject);
    }
    public void FixedUpdate()
    {
        if(Type == BreakableObjectType.Crate)
        {
            RB.velocity *= 0.94f;
            SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.white, 0.07f);
        }
    }
}
