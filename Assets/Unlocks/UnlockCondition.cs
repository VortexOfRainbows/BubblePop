using System.Collections.Generic;
using UnityEngine;

public abstract class UnlockCondition
{
    public const int RegularAchievement = 0;
    public const int Meadows = 1;
    public const int City = 2;
    public const int Lab = 3;
    public const int Completionist = 4;
    public const int Challenge = 5;
    public const int Secret = 6;
    public int AchievementZone = RegularAchievement;
    public int AchievementCategory = Completionist;
    public virtual void SetAchievementCategories(ref int zone, ref int category)
    {

    }
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
    public static void PrepareStatics()
    {
        PlayerData.MetaProgression.TotalAchievementStars = 0;
        for (int i = 0; i < maximumTypes; ++i)
        {
            UnlockCondition u = Unlocks[i];
            if(u.AchievementZone == Meadows)
                PlayerData.MetaProgression.TotalMeadowsStars += 1;
            PlayerData.MetaProgression.TotalAchievementStars += 1;
        }
        foreach(PowerUp p in PowerUp.PowerUps.Values)
        {
            p.BlackMarketVariantUnlockCondition?.AddAssociatedPower(p);
        }
    }
    protected UnlockCondition()
    {
        AssociatedUnlocks = new();
        AssociatedBlackMarketUnlocks = new();
        Description = new(Rarity, SaveString);
        Description.WithoutSizeAugments();
        InitializeDescription(ref Description);
        SetAchievementCategories(ref AchievementZone, ref AchievementCategory);
        AddToDictionary(this);
    }
    public void SaveData()
    {
        PlayerData.SaveBool(SaveString, Completed);
    }
    public void LoadData()
    {
        bool complete = PlayerData.GetBool(SaveString);
        if(complete)
            SetComplete(true);
    }
    public bool Unlocked => FakeComplete || TryUnlock();
    protected virtual bool TryUnlockCondition => false;
    public string LockedText()
    {
        return Description.BriefDescription();
    }
    protected bool Completed { get; set; } = false;
    private bool FakeComplete = false;
    public bool ForceUnlock()
    {
        if(!FakeComplete && !Completed && ForceUnlockAll)
        {
            UpdatePowerPool();
            FakeComplete = true;
        }
        return FakeComplete;
    }
    public void SetComplete(bool skipSave = false, bool completeStatus = true)
    {
        if(!Completed && completeStatus)
        {
            Completed = true;
            if(!skipSave)
                SaveData();
            if (AchievementZone == Meadows)
                PlayerData.MetaProgression.MeadowsStars += 1;
            PlayerData.MetaProgression.AchievementStars += 1;
            UpdatePowerPool();
            PopUpTextUI.UnlockQueue.Enqueue(this);
        }
        else if(!completeStatus)
        {
            Completed = false;
            if (!skipSave)
                SaveData();
        }
    }
    public void UpdatePowerPool()
    {
        if (AssociatedBlackMarketUnlocks.Count > 0) //This will make it so completing an achievement mid run will add the power to the pool so you can get it the same run
            foreach (PowerUp p in AssociatedBlackMarketUnlocks)
            {
                Debug.Log($"Unlock: Adding to black market pool! {DetailedDescription.TextBoundedByColor("#ff0000", p.UnlockedName)}");
                PowerUp.AddBlackMarketPowerToPool(p);
            }
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
    public void AddAssociatedPower(PowerUp p)
    {
        AssociatedBlackMarketUnlocks.Add(p);
    }
    public virtual Equipment FrontPageUnlock()
    {
        foreach(Equipment e in AssociatedUnlocks)
        {
            if (e is Body)
                return e;
        }
        return AssociatedUnlocks[0];
    }
    public List<Equipment> AssociatedUnlocks { get; private set; }
    public List<PowerUp> AssociatedBlackMarketUnlocks { get; private set; }
}
