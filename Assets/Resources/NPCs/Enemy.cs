using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Entity
{
    public class ImmunityData
    {
        public ImmunityData(Projectile attacker, int frames)
        {
            this.attacker = attacker;
            immuneFrames = frames;
        }
        public Projectile attacker;
        public int immuneFrames;
    }
    public List<ImmunityData> SpecializedImmuneFrames = new List<ImmunityData>();
    public void DeathParticles(int count = 10, float size = 0, Color c = default)
    {
        BoxCollider2D c2D = GetComponent<BoxCollider2D>();
        for (int i = 0; i < count; i++)
        {
            Vector2 randPos = c2D.bounds.min + new Vector3(c2D.bounds.extents.x * Utils.RandFloat(1), c2D.bounds.extents.y * Utils.RandFloat(1));
            ParticleManager.NewParticle(randPos, size * Utils.RandFloat(0.9f, 1.1f), Utils.RandCircle(1) * Utils.RandFloat(4, 12) + Vector2.up * Utils.RandFloat(3), 3, .75f, 1, c);
        }
    }
    public sealed override void OnFixedUpdate()
    {
        if (SpecializedImmuneFrames.Count > 0)
        {
            for (int i = SpecializedImmuneFrames.Count - 1; i >= 0; --i)
                if (--SpecializedImmuneFrames[i].immuneFrames <= 0)
                    SpecializedImmuneFrames.RemoveAt(i);
        }
        UniversalImmuneFrames--;
        AI();
    }
    public bool AlreadyDead => Life <= -50;
    private void SetDead() => Life = -50;
    public sealed override void OnHurtByProjectile(Projectile proj)
    {
        if (SpecializedImmuneFrames.Contains(proj))
            return;
        bool piercingProjectile = proj.Penetrate > 1 || proj.Penetrate == -1;
        if (piercingProjectile && UniversalImmuneFrames > 0)
            return;
        if (proj.Friendly && !AlreadyDead)
        {
            InjureNPC(proj.Damage);
            proj.OnHitTarget(this);
            if (proj.Penetrate != -1)
            {
                --proj.Penetrate;
                if (proj.Penetrate <= 0)
                    proj.Kill();
            }
            if (Life < 0)
                Life = 0;
            if (piercingProjectile)
            {
                SpecializedImmuneFrames.Add(new ImmunityData(proj, proj.immunityFrames));
            }
        }
        if (Life <= 0 && !AlreadyDead)
        {
            SetDead();
            Kill();
        }
    }
    public virtual float PowerDropChance => 0.04f;
    public float PointWorth = 0;
    public sealed override void Kill()
    {
        OnKill();
        CoinManager.SpawnCoin(transform.position, (int)(1 + 30 * Utils.RandFloat(1) * Utils.RandFloat(1) * Utils.RandFloat(1) * Utils.RandFloat(1)));
        bool LuckyDrop = Utils.RandFloat(1) < PowerDropChance;
        EventManager.Point += (int)PointWorth;
        if (EventManager.CanSpawnPower() || LuckyDrop)
            PowerUp.Spawn(PowerUp.RandomFromPool(), transform.position, LuckyDrop ? 0 : 100);
        Destroy(gameObject);
    }
    public virtual void AI()
    {

    }
    public virtual void OnKill()
    {

    }
}
