using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    #region Buff Datastructure Related Stuff
    private static int typeCounter = 0;
    private static int maximumTypes = 0;
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
    private static void AddToDictionary(Buff unlock)
    {
        string typeName = unlock.GetType().Name;
        if (Reverses.ContainsKey(typeName))
            return;
        unlock.MyID = typeCounter;
        Buffs[typeCounter] = unlock;
        Reverses[typeName] = typeCounter;
        typeCounter++;
        maximumTypes++;
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
    public Entity owner { get; set; }
    public float initiallyAppliedDuration;
    public float timeLeft;
    public bool active = true;
    public void Update()
    {
        UpdateDuration();
        if(active)
            Apply(owner);
    }
    public void UpdateDuration()
    {
        timeLeft -= Time.fixedDeltaTime;
        if(timeLeft < 0)
            active = false;
    }
    public virtual void Apply(Entity e)
    {

    }
    public int GetCurrentStacks(Entity e)
    {
        return e.UniqueBuffTypes.GetValueOrDefault(GetType());
    }
    public virtual Sprite GetSprite()
    {
        return Resources.Load<Sprite>($"Buffs/{GetType().Name}");
    }
    public virtual Color BackgroundColor()
    {
        return new Color(200 / 255f, 255 / 255f, 255 / 255f, 150 / 255f);
    }
}
public class SpeedBoost : Buff
{
    public override void Apply(Entity e)
    {
        float boost = 0.15f * timeLeft / initiallyAppliedDuration;
        if(e is Player p)
        {
            p.MoveSpeedMod += boost;
        }
    }
}
public class Poison : Buff
{
    public float dot = 0;
    public override Color BackgroundColor()
    {
        return new Color(0.9f, 0.7f, 0.6f);
    }
    public override void Apply(Entity e)
    {
        if (e is Enemy enemy)
        {
            float damage = 3 + e.MaxLife * 0.04f + Player.Instance.SnakeEyes;
            float tickRate = Mathf.Min(1, Mathf.Max(0.25f, 20f / damage));
            dot += Time.fixedDeltaTime;
            while (dot >= tickRate / 2f)
            {
                enemy.Injure(damage / initiallyAppliedDuration * tickRate, -1, new Color(0.8f, 0.27f, 0.9f));
                dot -= tickRate;
            }
        }
    }
}