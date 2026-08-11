using UnityEngine;
using static Quest;

public class BreakableObject : MonoBehaviour, IImpactedByProjIFrames
{
    public SpriteRenderer SpriteRenderer;
    public int HitsRequiredToKill { get; set; } = -1;
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
                AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 0.6f, 1.6f);
                SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.red, 0.5f);
            }
        }
    }
    public CapsuleCollider2D Collider;
    public void Break()
    {
        for (int i = 0; i < 30; ++i)
        {
            Vector2 randPos = Collider.bounds.min + new Vector3(Collider.bounds.extents.x * Utils.RandFloat(1), Collider.bounds.extents.y * Utils.RandFloat(1));
            ParticleManager.NewParticle(randPos, 0.7f * Utils.RandFloat(0.9f, 1.1f), Utils.RandCircle(8) + Vector2.up * Utils.RandFloat(6, 12), 5, Utils.RandFloat(1, 1.2f), 1,
                Color.Lerp(Color.white, Color.black, Utils.RandFloat(0.8f, 1)));
        }
        AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 1, 0.9f);
        Destroy(gameObject);
    }
}
