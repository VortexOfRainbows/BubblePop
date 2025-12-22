using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Entity : MonoBehaviour
{
    public GameObject Visual;
    //public static int NextUniqueID = 0;
    //public int UniqueID = NextUniqueID++;
    public float UniversalImmuneFrames = 0;
    public float Life { get; set; } = 10f;
    public float MaxLife { get; set; } = -1;
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
        if(this is not Enemy e)
            Animate();
    }
    public virtual void OnFixedUpdate()
    {

    }
    public void Start()
    {
        Init();
        if(this is Enemy e)
        {
            EnemyHealthScaling(e);
        }
        if (MaxLife < Life)
            MaxLife = Life;
        SpawnedIn = true;
    }
    public virtual void Init()
    {

    }
    public void EnemyHealthScaling(Enemy e)
    {
        Life = (int)(Life * WaveDirector.EnemyScalingFactor + 0.5f);
        if(e.IsSkull)
        {
            Life *= 2;
        }
    }
    public void Update()
    {
        if (this is not Player && (this is not Enemy e || !e.IsDummy))
        {
            if (DamageTaken > 0)
            {
                UpdateRendererColor(Color.Lerp(Color.white, Color.red, 0.8f), Utils.DeltaTimeLerpFactor(0.05f + DamageTaken / 500f)); 
                DamageTaken -= 20f * Time.deltaTime;
            }
            else
            {
                DamageTaken = 0;
                UpdateRendererColorToDefault(Utils.DeltaTimeLerpFactor(0.1f));
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
    public SpriteRenderer[] ChildrenRenders()
    {
        return childrenRenderers ??= Visual.GetComponentsInChildren<SpriteRenderer>();
    }
    protected SpriteRenderer[] childrenRenderers = null;
    private Color[] defaultColors = null;
    public void UpdateRendererColor(Color c, float lerp)
    {
        ChildrenRenders();
        if (defaultColors == null)
        {
            defaultColors = new Color[childrenRenderers.Length];
            for (int i = 0; i < defaultColors.Length; ++i)
                defaultColors[i] = childrenRenderers[i].color;
        }
        foreach (SpriteRenderer r in childrenRenderers)
            r.color = Color.Lerp(r.color, c, lerp);
    }
    public void UpdateRendererColorToDefault(float lerp)
    {
        ChildrenRenders();
        if (defaultColors == null)
        {
            defaultColors = new Color[childrenRenderers.Length];
            for (int i = 0; i < defaultColors.Length; ++i)
                defaultColors[i] = childrenRenderers[i].color;
        }
        for (int i = 0; i < defaultColors.Length; ++i)
            childrenRenderers[i].color = Color.Lerp(childrenRenderers[i].color, defaultColors[i], lerp);
    }
    public void AdjustRenderColorFromDefault(Color other, float lerp)
    {
        ChildrenRenders();
        if (defaultColors == null)
        {
            defaultColors = new Color[childrenRenderers.Length];
            for (int i = 0; i < defaultColors.Length; ++i)
                defaultColors[i] = childrenRenderers[i].color;
        }
        for (int i = 0; i < defaultColors.Length; ++i)
            childrenRenderers[i].color = Color.Lerp(defaultColors[i], other, lerp);
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
