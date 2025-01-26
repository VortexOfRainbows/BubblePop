using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Entity : MonoBehaviour
{
    public float PointWorth = 0;
    public float IFrame = 0;
    public int Life = 10;
    public static readonly string ProjTag = "Proj";
    public static readonly string EnemyTag = "Enemy";
    public void OnTriggerStay2D(Collider2D collision)
    {
        TriggerCollision(collision);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerCollision(collision);
    }
    public void TriggerCollision(Collider2D collision)
    {
        if (collision.tag == ProjTag && collision.GetComponentInParent<Projectile>() is Projectile proj)
        {
            if (this is Player p)
            {
                if (proj.Hostile && p.DeathKillTimer <= 0)
                {
                    p.Pop();
                }
            }
            else
            {
                if (proj.Friendly && (IFrame <= 0 || proj.Type != 3))
                {
                    Life -= proj.Damage;
                    if (proj.Type == 0)
                        proj.Kill();
                    if (proj.Type == 3)
                    {
                        IFrame = 100;
                        proj.gameObject.transform.localScale *= 0.75f;
                        proj.Damage--;
                        if (proj.Damage < 0)
                            proj.Kill();
                    }
                }
                if (Life <= 0)
                {
                    OnKill();
                    EventManager.Point += (int)PointWorth;
                    Destroy(gameObject);
                }
            }
        }
        if (collision.tag == EnemyTag)
        {
            if (this is Player p && p.DeathKillTimer <= 0)
            {
                p.Pop();
            }
        }
    }
    public void DeathParticles(int count = 10, float size = 0, Color c = default)
    {
        BoxCollider2D c2D = GetComponent<BoxCollider2D>();
        for(int i = 0; i < count; i++)
        {
            Vector2 randPos = c2D.bounds.min + new Vector3(c2D.bounds.extents.x * Utils.RandFloat(1), c2D.bounds.extents.y * Utils.RandFloat(1));
            ParticleManager.NewParticle(randPos, size * Utils.RandFloat(0.9f, 1.1f), Utils.RandCircle(1) * Utils.RandFloat(4, 12) + Vector2.up * Utils.RandFloat(3), 3, .75f, 1, c);
        }
    }
    public virtual void OnKill()
    {
    }
}
