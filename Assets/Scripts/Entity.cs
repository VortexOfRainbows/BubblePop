using UnityEngine;

public class Entity : MonoBehaviour
{
    public SpriteRenderer baseRenderer;
    public float PointWorth = 0;
    public float IFrame = 0;
    public int Life = 10;
    public float DamageTaken = 0;
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
            HurtByProjectile(proj);
        }
        if (collision.tag == EnemyTag)
        {
            HurtByNPC();
        }
    }
    public void HurtByProjectile(Projectile proj)
    {
        if (this is Player p)
        {
            if (proj.Hostile && p.DeathKillTimer <= 0 && p.IFrame <= 0)
            {
                p.Pop();
            }
        }
        else
        {
            if (proj.Friendly && (IFrame <= 0 || proj is not BigBubble) && Life > -50)
            {
                Life -= proj.Damage;
                DamageTaken += proj.Damage;
                proj.OnHitTarget(this);
                if (proj is SmallBubble || proj is StarProj)
                    proj.Kill();
                if (Life < 0)
                    Life = 0;
            }
            if (Life <= 0 && Life > -50)
            {
                Kill();
                Life = -50;
                bool LuckyDrop = Utils.RandFloat(1) < 0.04f || this is EnemyBossDuck;
                EventManager.Point += (int)PointWorth;
                if (EventManager.CanSpawnPower() || LuckyDrop)
                    PowerUp.Spawn(PowerUp.RandomFromPool(), transform.position, LuckyDrop ? 0 : 100);
                Destroy(gameObject);
            }
        }
    }
    public void HurtByNPC()
    {
        if (this is Player p && p.DeathKillTimer <= 0 && p.IFrame <= 0)
        {
            p.Pop();
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
    public Vector2 lastPos = Vector2.zero;
    public void FixedUpdate()
    {
        IFrame--;
    }
    public void Update()
    {
        if (this is not Player)
        {
            if (baseRenderer == null)
                baseRenderer = GetComponent<SpriteRenderer>();
            if (DamageTaken > 0)
            {
                baseRenderer.color = Color.Lerp(baseRenderer.color, Color.Lerp(Color.white, Color.red, 0.8f), 0.05f + DamageTaken / 500f);
                DamageTaken -= 20f * Time.deltaTime;
            }
            else
            {
                DamageTaken = 0;
                baseRenderer.color = Color.Lerp(baseRenderer.color, Color.white, 0.15f);
            }
        }
        if (Utils.RandFloat(1) < 0.5f && lastPos != (Vector2)transform.position)
        {
            Vector2 toLastPos = lastPos - (Vector2)transform.position;
            if(toLastPos.sqrMagnitude > 0.002f)
                ParticleManager.NewParticle(transform.position + new Vector3(Utils.RandFloat(-transform.lossyScale.x, transform.lossyScale.x) * 1.1f, -transform.lossyScale.y * 0.7f), Utils.RandFloat(0.35f, 0.45f), toLastPos * Utils.RandFloat(0.9f, 1.1f), .5f, Utils.RandFloat(0.4f, 0.5f), 0, ParticleManager.BathColor);
        }
        lastPos = transform.position;
    }
    private void Kill()
    {
        OnKill();
        CoinManager.SpawnCoin(transform.position, (int)(1 + 30 * Utils.RandFloat(1) * Utils.RandFloat(1) * Utils.RandFloat(1) * Utils.RandFloat(1)));
    }
    public virtual void OnKill()
    {

    }
    public float Distance(GameObject other)
    {
        return (other.transform.position - transform.position).magnitude;
    }
}
