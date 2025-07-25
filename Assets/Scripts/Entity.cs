using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Entity : MonoBehaviour
{
    public GameObject Visual;
    //public static int NextUniqueID = 0;
    //public int UniqueID = NextUniqueID++;
    public float UniversalImmuneFrames = 0;
    public int Life = 10;
    public int MaxLife = -1;
    public float DamageTaken = 0;
    public bool SpawnedIn = false;
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
        if (collision.CompareTag(ProjTag) && collision.GetComponentInParent<Projectile>() is Projectile proj)
            HurtByProjectile(proj);
        if (collision.CompareTag(EnemyTag))
            HurtByNPC();
    }
    public void HurtByProjectile(Projectile proj)
    {
        OnHurtByProjectile(proj);
    }
    public virtual void OnHurtByProjectile(Projectile proj)
    {

    }
    public void InjureNPC(int damage, bool specialColor = false)
    {
        Life -= damage;
        DamageTaken += damage;
        BoxCollider2D c2D = GetComponent<BoxCollider2D>();
        Vector2 randPos = c2D.bounds.min + new Vector3(c2D.bounds.extents.x * Utils.RandFloat(1), c2D.bounds.extents.y * Utils.RandFloat(1));
        Color c = !specialColor ? new Color(1f, 0.5f, 0.4f) : new Color(1f, 0.9f, 0.3f);
        PopupText.NewPopupText(randPos, Utils.RandCircle(3) + Vector2.up * 2, c, damage.ToString(), specialColor);
    }
    public void HurtByNPC()
    {
        if (this is Player p && !p.IsDead && p.UniversalImmuneFrames <= 0)
        {
            p.Hurt(1);
        }
    }
    public Vector2 lastPos = Vector2.zero;
    public void FixedUpdate()
    {
        UniversalImmuneFrames--;
        OnFixedUpdate();
        Animate();
    }
    public virtual void OnFixedUpdate()
    {

    }
    public void Start()
    {
        Init();
        if(this is Enemy)
            EnemyHealthScaling();
        if (MaxLife < Life)
            MaxLife = Life;
        SpawnedIn = true;
    }
    public virtual void Init()
    {

    }
    public void EnemyHealthScaling()
    {
        Life = (int)(Life * WaveDirector.EnemyScalingFactor + 0.5f);
    }
    public void Update()
    {
        if (this is not Player)
        {
            if (DamageTaken > 0)
            {
                UpdateRendererColor(Color.Lerp(Color.white, Color.red, 0.8f), 0.05f + DamageTaken / 500f); 
                DamageTaken -= 20f * Time.deltaTime;
            }
            else
            {
                DamageTaken = 0;
                UpdateRendererColor(Color.white, 0.15f);
            }
        }
        //if (Utils.RandFloat(1) < 0.5f && lastPos != (Vector2)transform.position)
        //{
        //    Vector2 toLastPos = lastPos - (Vector2)transform.position;
        //    if(toLastPos.sqrMagnitude > 0.002f)
        //        ParticleManager.NewParticle(transform.position + new Vector3(Utils.RandFloat(-transform.lossyScale.x, transform.lossyScale.x) * 1.1f, -transform.lossyScale.y * 0.7f), Utils.RandFloat(0.35f, 0.45f), toLastPos * Utils.RandFloat(0.9f, 1.1f), .5f, Utils.RandFloat(0.4f, 0.5f), 0, ParticleManager.BathColor);
        //}
        lastPos = transform.position;
    }
    private SpriteRenderer[] childrenRenderers = null;
    public void UpdateRendererColor(Color c, float lerp)
    {
        childrenRenderers ??= Visual.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer r in childrenRenderers)
        {
            r.color = Color.Lerp(r.color, c, lerp);
        }
    }
    public virtual void Kill()
    {

    }
    public float Distance(GameObject other)
    {
        return (other.transform.position - transform.position).magnitude;
    }
    public Rigidbody2D RB;
    //public float Anim = 0;
    protected List<Animator> ChildAnimators = new();
    public void AddAnim(Animator a)
    {
        ChildAnimators.Add(a);
    }
    public void Animate()
    {
        foreach (Animator a in ChildAnimators)
            a.UpdateAnimation();
    }
}
