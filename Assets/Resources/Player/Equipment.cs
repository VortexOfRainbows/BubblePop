using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public virtual void ModifyLifeStats(ref int MaxLife, ref int Life, ref int MaxShield, ref int Shield)
    {

    }
    public void Start()
    {
        if(p == null && Player.Instance != null)
            p = Player.Instance.Animator;
    }
    public static Player player => Player.Instance;
    public PlayerAnimator p;
    private static List<PowerUp> PowerPool = new();
    private string InternalName = null;
    public List<GameObject> SubEquipment = new();
    /// <summary>
    /// Only used for equipment UI boxes to help match their values to their original prefab after the original prefabs generates some values during runtime.
    /// The most obvious example of this is the index in the all-equipment pool
    /// If this is changed for default prefabs, equips may have trouble saving.
    /// </summary>
    public SpriteRenderer spriteRender;
    public Vector2 velocity;
    public bool hasInit = false;
    public int IndexInAllEquipPool
    {
        get
        {
            return Main.GlobalEquipData.EquipTypeToIndex[GetType()];
        }
    }
    public Equipment OriginalPrefab => Main.GlobalEquipData.AllEquipList[IndexInAllEquipPool].GetComponent<Equipment>();
    private int m_LocalIndexInAllEquipPool;
    public static void ModifyPowerPoolAll()
    {
        PowerUp.ResetPowerAvailability();
        PowerPool.Clear();
        Player.Instance.Hat.ModifyPowerPool(PowerPool);
        Player.Instance.Accessory.ModifyPowerPool(PowerPool);
        Player.Instance.Weapon.ModifyPowerPool(PowerPool);
        Player.Instance.Body.ModifyPowerPool(PowerPool);
        Player.Instance.Hat.ReducePowerPool(PowerPool);
        Player.Instance.Accessory.ReducePowerPool(PowerPool);
        Player.Instance.Weapon.ReducePowerPool(PowerPool);
        Player.Instance.Body.ReducePowerPool(PowerPool);
        for (int i = 0; i < PowerPool.Count; ++i)
        {
            PowerUp.AddPowerUpToAvailability(PowerPool[i]);
        }
        //This imports all black market powers, but it would be better to do this elsewhere most likely
        for (int i = 0; i < PowerUp.Reverses.Count; ++i)
        {
            PowerUp p = PowerUp.Get(i);
            if (p.IsBlackMarket())
                PowerUp.AddPowerUpToAvailability(p);
        }
        PowerPool.Clear();
    }
    protected virtual UnlockCondition UnlockCondition => UnlockCondition.Get<StartsUnlocked>();
    public UnlockCondition GetUnlockCondition() => UnlockCondition;
    /// <summary>
    /// Essentially which character this equipment should be unlocked by default under. For bodies, this unlock condition should be the same as the normal unlock condition
    /// </summary>
    public bool SameUnlockAsBody(Body b)
    {
        return b.UnlockCondition == UnlockCondition && this is not Body;
    }
    //protected virtual UnlockCondition CategoryUnlockCondition => UnlockCondition.Get<StartsUnlocked>();
    //public bool CategoryUnlocked => CategoryUnlockCondition.Unlocked || this is Body || isSubEquipment;
    public bool IsUnlocked => UnlockCondition.Unlocked /*&& CategoryUnlocked) || (SameUnlockAsBody(Player.Instance.Body) && !isSubEquipment)*/;
    public virtual void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {

    }
    public void AliveUpdate()
    {
        if(!hasInit)
        {
            Init();
            hasInit = true;
        }
        AnimationUpdate();
    }
    public void DeadUpdate()
    {
        DeathAnimation();
    }
    protected virtual void AnimationUpdate()
    {

    }
    protected virtual void DeathAnimation()
    {

    }
    public virtual void OnStartWith()
    {

    }
    protected virtual void ModifyPowerPool(List<PowerUp> powerPool)
    {

    }
    protected virtual void ReducePowerPool(List<PowerUp> powerPool)
    {

    }
    public virtual void Init()
    {

    }
    //[Obsolete]
    //public int GetPrice() => 0;
    /// <summary>
    /// Ran while the player has this equipment equipped.
    /// Runs after the powerup-reset code, meaning that any adjustments to powerup related states will take effect.
    /// Not called on non-player equipment.
    /// </summary>
    public virtual void EquipUpdate()
    {

    }
    /// <summary>
    /// Ran while the player has this equipment equipped, but after the equips have been processed.
    /// Runs after the powerup-reset code, meaning that any adjustments to powerup related states will take effect.
    /// Not called on non-player equipment.
    /// Useful for attack-like effects that rely on powerups and other equipment modifications before activating.
    /// </summary>
    public virtual void PostEquipUpdate()
    {

    }
    #region DescriptionStuff
    public string GetName(bool noColor = false)
    {
        return GetMyDescription().GetName(noColor);
    }
    public string GetDescription()
    {
        return GetMyDescription().FullDescription();
    }
    public string GetUnlockReq()
    {
        return "Unlock by:\n" + UnlockCondition.LockedText();
    }
    public string TypeName
    {
        get
        {
            if(InternalName == null || InternalName.Length <= 0)
                InternalName = GetType().Name;
            return InternalName;
        }
        set
        {
            InternalName = value;
        }
    }
    public void SetUpData(int index)
    {
        Main.GlobalEquipData.EquipTypeToIndex.Add(GetType(), index);
        DetailedDescription descData = new(GetRarity() - 1, TypeName.ToSpacedString());
        InitializeDescription(ref descData);
        Main.GlobalEquipData.DescriptionData.Add(descData);
        Main.GlobalEquipData.TimesUsedList.Add(0);
        LoadGlobalData();

        UnlockCondition.AddAssociatedEquip(this);
        //LoadGlobalData();
    }
    public int TotalTimesUsed
    {
        get
        {
            //Debug.Log($"Fetched {IndexInAllEquipPool}: {Main.GlobalEquipData.TimesUsedList[IndexInAllEquipPool]}");
            return Main.GlobalEquipData.TimesUsedList[IndexInAllEquipPool];
        }
        set
        {
            Main.GlobalEquipData.TimesUsedList[IndexInAllEquipPool] = value;
            SaveGlobalData();
            //Debug.Log($"Saved {IndexInAllEquipPool}: {TotalTimesUsed}");
        }
    }
    public void LoadGlobalData()
    {
        //Debug.Log("Load Tag: " + $"{TypeName}UsedTotal");
        TotalTimesUsed = PlayerData.GetInt($"{TypeName}UsedTotal", 0);
    }
    public void SaveGlobalData()
    {
        //Debug.Log("Save Tag: " + $"{TypeName}UsedTotal");
        PlayerData.SaveInt($"{TypeName}UsedTotal", TotalTimesUsed);
    }
    public virtual int GetRarity()
    {
        return 1;
    }
    public DetailedDescription GetMyDescription()
    {
        return Main.GlobalEquipData.DescriptionData[IndexInAllEquipPool];
    }
    public virtual void InitializeDescription(ref DetailedDescription description)
    {

    }
    #endregion
}
