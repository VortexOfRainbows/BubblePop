using System.Collections.Generic;
using UnityEngine;

public abstract class UnlockCondition
{
    public virtual string SaveString => GetType().Name;
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
    public static Dictionary<int, UnlockCondition> Unlocks { get; private set; }
    private static Dictionary<string, int> Reverses;
    public static void InitDict()
    {
        Reverses = new();
        Unlocks = new Dictionary<int, UnlockCondition>();
        ReflectiveEnumerator.AssembleInstances<UnlockCondition>();
    }
    public int MyID = -1;
    #endregion
    public static bool ForceUnlockAll = false; //For the playtest, we will have everything unlocked
    public virtual PowerUp Power => null;
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
    protected UnlockCondition()
    {
        AssociatedUnlocks = new();
        Description = new(Rarity, SaveString);
        InitializeDescription(ref Description);
        AddToDictionary(this);
    }
    public void SaveData()
    {
        PlayerData.SaveBool(SaveString, Completed);
    }
    public void LoadData()
    {
        Completed = PlayerData.GetBool(SaveString);
    }
    public bool Unlocked => ForceUnlockAll || TryUnlock();
    protected virtual bool TryUnlockCondition => false;
    public string LockedText()
    {
        return Description.BriefDescription();
    }
    public bool Completed { get; set; } = false;
    public void SetComplete()
    {
        Completed = true;
        SaveData();
    }
    public bool TryUnlock()
    {
        if(TryUnlockCondition)
            SetComplete();
        return Completed;
    }
    protected DetailedDescription Description;
    public string GetName()
    {
        return DetailedDescription.TextBoundedByRarityColor(Rarity - 1, Description.GetName(true), false);
    }
    public string GetDescription(bool brief = false)
    {
        return brief ? Description.BriefDescription() : Description.FullDescription();
    }
    public virtual int Rarity => AssociatedUnlocks.Count > 0 ? FrontPageUnlock().GetRarity() : 1;
    public virtual void InitializeDescription(ref DetailedDescription description)
    {

    }
    public void AddAssociatedEquip(Equipment e)
    {
        AssociatedUnlocks.Add(e);
    }
    public Equipment FrontPageUnlock()
    {
        foreach(Equipment e in AssociatedUnlocks)
        {
            if (e is Body)
                return e;
        }
        return AssociatedUnlocks[0];
    }
    public List<Equipment> AssociatedUnlocks;
}
