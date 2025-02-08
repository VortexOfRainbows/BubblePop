using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public abstract class UnlockCondition
{
    #region UnlockCondition Datastructure Related Stuff
    private static int typeCounter = 0;
    private static int maximumTypes = 0;
    public static UnlockCondition Get(string typeName)
    {
        if (Reverses == null)
            InitDict();
        return Get(Reverses[typeName]);
    }
    public static UnlockCondition Get<T>() where T : UnlockCondition => Get(typeof(T).Name);
    public static UnlockCondition Get(int unlockID)
    {
        if (Unlocks == null)
            InitDict();
        return Unlocks[unlockID];
    }
    private static void AddToDictionary(UnlockCondition unlock)
    {
        string typeName = unlock.GetType().Name;
        if (Reverses.ContainsKey(typeName))
            return;
        unlock.MyID = typeCounter;
        Unlocks[typeCounter] = unlock;
        Reverses[typeName] = typeCounter;
        Debug.Log($"Added: {unlock.LockedText()} to the dictionary at index {typeCounter}");
        typeCounter++;
        maximumTypes++;
    }
    private static Dictionary<int, UnlockCondition> Unlocks;
    private static Dictionary<string, int> Reverses;
    public static void InitDict()
    {
        Reverses = new();
        Unlocks = new Dictionary<int, UnlockCondition>();
        ReflectiveEnumerator.AssembleInstances<UnlockCondition>();
    }
    public int MyID = -1;
    #endregion
    protected UnlockCondition()
    {
        AddToDictionary(this);
    }
    public virtual void SaveData()
    {
        throw new System.Exception("Saving data for unlocks not yet implemented");
    }
    public virtual void LoadData()
    {
        throw new System.Exception("Saving data for unlocks not yet implemented");
    }
    public virtual bool IsUnlocked => false;
    public virtual string LockedText()
    {
        return "Unlocked by accomplishing *some* objective";
    }
}
