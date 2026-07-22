using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    #region Buff Datastructure Related Stuff
    public static Buff Get(string typeName)
    {
        if (Reverses == null)
            InitDict();
        return Get(Reverses[typeName]);
    }
    public static Buff Get<T>() where T : Buff => Get(typeof(T).Name);
    public static Buff Get(int unlockID)
    {
        if (Buffs == null)
            InitDict();
        return Buffs[unlockID];
    }
    private static Dictionary<int, Buff> Buffs;
    private static Dictionary<string, int> Reverses;
    public static void InitDict()
    {
        Reverses = new();
        Buffs = new Dictionary<int, Buff>();
        ReflectiveEnumerator.AssembleInstances<Buff>();
    }
    public int MyID = -1;
    #endregion
    public Entity Owner { get; private set; }
    public readonly List<Vector2> BuffStack = new(); //Vector where x is the counter and y is the original time applied
    public bool Active { get; private set; } = true;
    public bool SkipTimeUpdate { get; protected set; } = false;
    public void Update()
    {
        UpdateDuration();
        if(Active)
            Update(Owner);
    }
    public void UpdateDuration()
    {
        if (!SkipTimeUpdate)
        {
            for (int i = BuffStack.Count - 1; i >= 0; --i)
            {
                Vector2 orig = BuffStack[i];
                orig.x -= Time.fixedDeltaTime;
                BuffStack[i] = orig;
                if (orig.x < 0)
                {
                    Stacks -= 1;
                    BuffStack.RemoveAt(i);
                }
            }
        }
        if(BuffStack.Count <= 0 || Stacks <= 0)
            Active = false;
    }
    public virtual void Apply(Entity e, float duration, int addStacks = 1)
    {
        if (StackSeparately || BuffStack.Count <= 0)
        {
            if (duration == -1)
                SkipTimeUpdate = true;
            for(int i = addStacks; i > 0; --i)
                BuffStack.Insert(0, new Vector2(duration, duration));
            if(MaxStackSize != -1) //Clear out older stacks when going over the max size rather than preventing new application
            {
                while (BuffStack.Count > MaxStackSize)
                {
                    Stacks -= 1;
                    BuffStack.RemoveAt(BuffStack.Count - 1);
                }
            }
            Owner = e;
        }
        else if(duration > BuffStack[0].x)
            BuffStack[0] = new Vector2(duration, duration);
        Stacks += addStacks;
    }
    public virtual void RemoveStack(int stacksToRemove = 1)
    {
        if(StackSeparately) //Completely remove the buff if it stacks separately
        {
            Stacks = -1000;
        }
        else
            Stacks -= stacksToRemove;
        if (Stacks <= 0)
            Active = false;
    }
    public float Detonate(Entity e)
    {
        Active = false;
        return OnDetonate(e);
    }
    /// <summary>
    /// Called when a debuff is forcefully removed by a detonate trigger, such as King Oil's ignition
    /// </summary>
    /// <param name="e"></param>
    public virtual float OnDetonate(Entity e)
    {
        return 0;
    }
    public virtual void Update(Entity e)
    {

    }
    public virtual Sprite GetSprite()
    {
        return Resources.Load<Sprite>($"Buffs/{GetType().Name}");
    }
    public int Stacks { get; set; } = 0;
    public virtual bool StackSeparately => false;
    public virtual int MaxStackSize => -1;
}
public class LightningBottle : Buff
{
    public override void Update(Entity e)
    {

    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>($"Buffs/LightningBuff");
    }
}
public class SpeedBoost : Buff
{
    public override void Update(Entity e)
    {
        if (e is Player p)
        {
            foreach (Vector2 v in BuffStack)
            {
                float boost = 0.15f * v.x / v.y;
                p.TrueMoveModifier += boost;
            }
        }
    }
    public override bool StackSeparately => true;
}
public class Poison : Buff
{
    public float DamageOverTime = 0;
    public override void Update(Entity e)
    {
        if (e is Enemy enemy)
        {
            foreach (Vector2 v in BuffStack)
            {
                float damage = 3 + e.MaxLife * 0.04f + Player.Instance.SnakeEyes;
                float tickRate = Mathf.Min(1, Mathf.Max(0.25f, 20f / damage));
                DamageOverTime += Time.fixedDeltaTime;
                while (DamageOverTime >= tickRate / 2f)
                {
                    enemy.Injure(damage / v.y * tickRate, -1, new Color(0.8f, 0.27f, 0.9f), 1);
                    DamageOverTime -= tickRate;
                }
            }
        }
    }
    public override float OnDetonate(Entity e)
    {
        float totalDamage = 0;
        foreach (Vector2 v in BuffStack)
            totalDamage += (3 + e.MaxLife * 0.04f + Player.Instance.SnakeEyes) * v.x / v.y;
        totalDamage += Player.Instance.CombustBonusDamage;
        if (e is Enemy enemy)
            enemy.Injure(totalDamage, -2, new Color(0.8f, 0.27f, 0.9f), 1);
        return totalDamage;
    }
    public override bool StackSeparately => true;
}
public class Tarred : Buff
{
    public float DamageOverTime = 0;
    public override void Update(Entity e)
    {
        if (e is Enemy enemy)
        {
            enemy.TarStacks += Stacks;
            float damage = Player.Instance.CorrodeDamage * Stacks;
            if(damage > 0)
            {
                float tickRate = Mathf.Min(1, Mathf.Max(0.25f, 20f / damage));
                DamageOverTime += Time.fixedDeltaTime;
                while (DamageOverTime >= tickRate / 2f)
                {
                    enemy.Injure(damage * tickRate, -1, ColorHelper.KingOilColor, 1);
                    DamageOverTime -= tickRate;
                }
            }
        }
    }
    public override float OnDetonate(Entity e)
    {
        AudioManager.PlaySound(SoundID.BathBombBurst, e.transform.position, 1.0f, 0.7f);
        for (int i = 0; i < 9; ++i)
            ParticleManager.NewParticle(e.transform.position, Utils.RandFloat(0.5f, 1.2f), Utils.RandCircle(5), 1, Utils.RandFloat(0.6f, 1.2f), ParticleManager.ID.Fire);
        float damage = 4 + Player.Instance.CombustBonusDamage;
        damage *= Stacks;
        if (e is Enemy enemy)
            enemy.Injure(damage, -2, ColorHelper.KingOilColor, 1);
        return damage;
    }
    public override bool StackSeparately => false;
}
public class Chill : Buff
{
    public override int MaxStackSize => 10;
    public override bool StackSeparately => true;
    public override void Update(Entity e)
    {
        if (e is Enemy enemy)
            enemy.FreezeMultiplier -= Stacks * 0.1f;
    }
    public override float OnDetonate(Entity e)
    {
        //AudioManager.PlaySound(SoundID.WoodBreak, e.transform.position, 1.2f, 1.7f);
        float damage = Stacks + Player.Instance.CombustBonusDamage;
        if (e is Enemy enemy)
            enemy.Injure(damage, -2, ColorHelper.RarityColors[2], 1);
        return damage;
    }
}
public class OmniBoost : Buff
{
    public override void Update(Entity e)
    {
        if (e is Player p)
        {
            float boost = 0.5f * Stacks;
            p.TrueMoveModifier += boost;
            p.DamageMultiplier += boost;
            p.AttackSpeedModifier += boost;
        }
    }
    public override bool StackSeparately => true;
}