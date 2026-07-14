using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public virtual void ModifyLifeStats(ref int MaxLife, ref int Life, ref int Shield)
    {

    }
    public void Start()
    {
        if(Player != null && myAnim == null)
            p = Player.Animator;
    }
    public Player Player { get; set; } = null;
    private PlayerAnimator myAnim;
    public PlayerAnimator p { 
        get => myAnim != null 
            ? myAnim :
            Player.Animator; 
        set => myAnim = value; 
    }
    private static readonly List<PowerUp> PowerPool = new();
    private string InternalName = null;
    public readonly List<GameObject> SubEquipment = new();
    public Equipment SubEquipParent { get; set; }
    public bool IsSubEquip { get; set; }
    /// <summary>
    /// Only used for equipment UI boxes to help match their values to their original prefab after the original prefabs generates some values during runtime.
    /// The most obvious example of this is the index in the all-equipment pool
    /// If this is changed for default prefabs, equips may have trouble saving.
    /// </summary>
    public SpriteRenderer spriteRender;
    public Vector2 velocity;
    public bool HasInit { get; protected set; } = false;
    public int IndexInAllEquipPool
    {
        get
        {
            return Main.GlobalEquipData.EquipTypeToIndex[GetType()];
        }
    }
    public Equipment OriginalPrefab => Main.GlobalEquipData.AllEquipmentsList[IndexInAllEquipPool].GetComponent<Equipment>();
    //private int m_LocalIndexInAllEquipPool;
    public static void ModifyPowerPoolAll()
    {
        PowerUp.ResetPowerAvailability();
        PowerPool.Clear();
        Player.Instance.Hat.ModifyPowerPool(PowerPool);
        Player.Instance.Accessory.ModifyPowerPool(PowerPool);
        Player.Instance.Weapon.ModifyPowerPool(PowerPool);
        Player.Instance.Body.ModifyPowerPool(PowerPool);
        //Player.Instance.Hat.ReducePowerPool(PowerPool);
        //Player.Instance.Accessory.ReducePowerPool(PowerPool);
        //Player.Instance.Weapon.ReducePowerPool(PowerPool);
        //Player.Instance.Body.ReducePowerPool(PowerPool);
        for (int i = 0; i < PowerPool.Count; ++i)
        {
            PowerUp.AddPowerUpToAvailability(PowerPool[i]);
        }
        //This imports all black market powers, but it would be better to do this elsewhere most likely
        for (int i = 0; i < PowerUp.Reverses.Count; ++i)
        {
            PowerUp p = PowerUp.Get(i);
            if (p.IsBlackMarket() && p.Weighting > 0 && (!p.HasBlackMarketAlternate || p.BlackMarketVariantUnlockCondition.IsComplete))
                PowerUp.AddBlackMarketPowerToPool(p);
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
    public bool IsUnlocked => UnlockCondition.IsComplete /*&& CategoryUnlocked) || (SameUnlockAsBody(Player.Instance.Body) && !isSubEquipment)*/;
    public virtual void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {

    }
    public void AliveUpdate()
    {
        if (Player != null && Player.Control == null)
            return;
        if(!HasInit)
        {
            Init();
            HasInit = true;
            if (this is Body b)
            {
                List<Ability> abilities = GetAbility();
                if(abilities.Count > 0)
                {
                    abilities[0].SetProgressDisplayFunc(b.AbilityProgess);
                    abilities[0].SetNumberDisplayFunc(b.AbilityCount);
                }
            }
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
    public List<PowerUp> GetPowerPoolForDisplay()
    {
        List<PowerUp> powerPool = new();
        ModifyPowerPool(powerPool);
        powerPool.Sort((PowerUp first, PowerUp second) => first.Rarity - second.Rarity);
        return powerPool;
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
    public string GetName(bool noColor = false) => noColor ? GetMyDescription().Name : GetMyDescription().Name.WithRarityColor(GetRarity() - 1, false);
    public string GetDescription() => GetMyDescription().Full;
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
        EquipDescription newDescription = new(this);
        ModifyDescription(ref newDescription);

        //LocalizationBuilder.CopyOldEquipmentDescriptionToNewSystem(this, descData);
        //InitializeAbilities(ref MyAbilities);

        Main.GlobalEquipData.DescriptionData.Add(newDescription);
        Main.GlobalEquipData.EquipDataList.Add(new Main.GlobalEquipData.EquipData(0, 0, 0));
        LoadGlobalData();
        UnlockCondition.AddAssociatedEquip(this);
        //LoadGlobalData();
    }
    public Main.GlobalEquipData.EquipData MyStaticData()
    {
        return Main.GlobalEquipData.EquipDataList[IndexInAllEquipPool];
    }
    public int TotalTimesUsed
    {
        get => MyStaticData().TimesUsed;
        set
        {
            MyStaticData().TimesUsed = value;
            SaveGlobalData(0);
            //Debug.Log($"Saved {IndexInAllEquipPool}: {TotalTimesUsed}");
        }
    }
    public int VictoryCount
    {
        get => MyStaticData().VictoryCount;
        set
        {
            MyStaticData().VictoryCount = value;
            SaveGlobalData(1);
        }
    }
    public int HighestDifficultyUnlocked
    {
        get => MyStaticData().HighestDifficultyCleared;
        set
        {
            MyStaticData().HighestDifficultyCleared = value;
            SaveGlobalData(2);
        }
    }
    public void LoadGlobalData()
    {
        //Debug.Log("Load Tag: " + $"{TypeName}UsedTotal");
        TotalTimesUsed = PlayerData.GetInt($"{TypeName}Total", 0);
        VictoryCount = PlayerData.GetInt($"{TypeName}Wins", 0);
        HighestDifficultyUnlocked = PlayerData.GetInt($"{TypeName}LVL", 0);
    }
    public void SaveGlobalData(int num = -1)
    {
        //Debug.Log("Save Tag: " + $"{TypeName}UsedTotal");
        if(num == -1 || num == 0)
            PlayerData.SaveInt($"{TypeName}Total", TotalTimesUsed);
        if(num == -1 || num == 1)
            PlayerData.SaveInt($"{TypeName}Wins", VictoryCount);
        if(num == -1 || num == 2)
            PlayerData.SaveInt($"{TypeName}LVL", HighestDifficultyUnlocked);
    }
    public virtual int GetRarity()
    {
        return 1;
    }
    public EquipDescription GetMyDescription()
    {
        return Main.GlobalEquipData.DescriptionData[IndexInAllEquipPool];
    }
    public List<Ability> GetAbility()
    {
        return GetMyDescription().Abilities;
    }
    public virtual void ModifyDescription(ref EquipDescription description)
    {

    }
    #endregion
}
