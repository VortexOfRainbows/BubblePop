using System.Collections.Generic;
using UnityEngine;

public abstract class UnlockCondition
{
    public virtual PowerUp Power => null;
    public static void SaveInt(string tag, int value)
    {
        PlayerData.SaveInt(tag, value);
    }
    public static int LoadInt(string tag)
    {
        return PlayerData.GetInt(tag);
    }
    public static void SaveAllData()
    {
        if (Unlocks == null)
            InitDict();
        for (int i = 0; i < maximumTypes; ++i)
        {
            Unlocks[i].SaveData();
        }
    }
    public static void LoadAllData()
    {
        if (Unlocks == null)
            InitDict();
        for (int i = 0; i < maximumTypes; ++i)
        {
            Unlocks[i].LoadData();
        }
    }
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

    }
    public virtual void LoadData()
    {

    }
    public virtual bool IsUnlocked => false;
    public virtual string LockedText()
    {
        return "Unlocked by accomplishing *some* objective";
    }
}
