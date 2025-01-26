using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int Life = 10;
    public readonly string ProjTag = "Proj";
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
                if(proj.Hostile && p.DeathKillTimer <= 0)
                {
                    p.Pop();
                }
            }
            else
            {
                if(proj.Friendly)
                {
                    Life -= proj.Damage;
                    if (proj.Type == 0 || proj.Type == 3)
                        Destroy(proj.gameObject);
                }
                if(Life <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
