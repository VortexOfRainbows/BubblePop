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
