using UnityEngine;

public class Entity : MonoBehaviour
{
    public int Life = 10;
    public readonly string ProjTag = "Proj";
    public readonly string EnemyTag = "Enemy";
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
                if (proj.Friendly)
                {
                    Life -= proj.Damage;
                    if (proj.Type == 0 || proj.Type == 3)
                        proj.Kill();
                }
                if (Life <= 0)
                {
                    if (this is EnemySoap soap && this is not EnemySoapTiny)
                    {
                        soap.Kill();
                    }
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
}
