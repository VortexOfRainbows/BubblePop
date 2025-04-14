using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    //public static int NextUniqueID = 0;
    //public int UniqueID = NextUniqueID++;
    public static Entity FindClosest(Vector3 position, float searchDistance, out Vector2 norm, string tag = "Enemy", bool requireNonImmune = true, params Entity[] ignore)
    {
        norm = Vector2.zero;
        int best = -1;
        Entity[] e = FindObjectsByType<Entity>(FindObjectsSortMode.None);
        for(int i = 0; i < e.Length; ++i)
        {
            Entity e1 = e[i];
            Vector2 toDest = e1.transform.position - position;
            float dist = toDest.magnitude;
            Debug.Log(e1.tag);
            if (dist <= searchDistance && (!requireNonImmune || e1.UniversalImmuneFrames <= 0) && e1.CompareTag(tag))
            {
                bool blackListed = false;
                foreach(Entity e2 in ignore)
                {
                    if(/*e2 != null &&*/e2 == e1)
                    {
                        //Debug.Log("Failed Blacklist");
                        blackListed = true;
                        break;
                    }
                }
                if (!blackListed)
                {
                    best = i;
                    searchDistance = dist;
                    norm = toDest;
                }
            }
        }
        norm = norm.normalized;
        return best == -1 ? null : e[best];
    }
    public SpriteRenderer baseRenderer;
    public float UniversalImmuneFrames = 0;
    public int Life = 10;
    public float DamageTaken = 0;
    public static readonly string PlayerTag = "Player";
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
        OnHurtByProjectile(proj);
    }
    public virtual void OnHurtByProjectile(Projectile proj)
    {

    }
    public void InjureNPC(int damage)
    {
        Life -= damage;
        DamageTaken += damage;
        BoxCollider2D c2D = GetComponent<BoxCollider2D>();
        Vector2 randPos = c2D.bounds.min + new Vector3(c2D.bounds.extents.x * Utils.RandFloat(1), c2D.bounds.extents.y * Utils.RandFloat(1));
        PopupText.NewPopupText(randPos, Utils.RandCircle(3) + Vector2.up * 2, new Color(1f, 0.5f, 0.4f), damage.ToString());
    }
    public void HurtByNPC()
    {
        if (this is Player p && p.DeathKillTimer <= 0 && p.UniversalImmuneFrames <= 0)
        {
            p.Pop();
        }
    }
    public Vector2 lastPos = Vector2.zero;
    public void FixedUpdate()
    {
        UniversalImmuneFrames--;
        OnFixedUpdate();
    }
    public virtual void OnFixedUpdate()
    {

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
    public virtual void Kill()
    {

    }
    public float Distance(GameObject other)
    {
        return (other.transform.position - transform.position).magnitude;
    }
}
