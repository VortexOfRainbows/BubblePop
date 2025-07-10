using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public void Start()
    {
        if(p == null)
            p = Player.Instance.Animator;
    }
    public static Player player => Player.Instance;
    public PlayerAnimator p;
    private static List<PowerUp> PowerPool = new();
    private string InternalName = null;
    public List<GameObject> SubEquipment = new();
    public Equipment OriginalPrefab = null;
    public SpriteRenderer spriteRender;
    public Vector2 velocity;
    public bool hasInit = false;
    public bool isSubEquipment = false; 
    public int IndexInTheAllEquipPool
    {
        get
        {
            return OriginalPrefab == null ? myIndexInAllEquipPool : OriginalPrefab.IndexInTheAllEquipPool;
        }
        set
        {
            myIndexInAllEquipPool = value;
        }
    }
    private int myIndexInAllEquipPool;
    public string TypeName { 
        get
        {
            if (InternalName == null)
                InternalName = this.GetType().Name;
            return InternalName;
        }
        set
        {
            InternalName = value;
        }
    }
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
        PowerPool.Clear();
    }
    protected virtual UnlockCondition UnlockCondition => UnlockCondition.Get<StartsUnlocked>();
    /// <summary>
    /// Essentially which character this equipment should be unlocked by default under. For bodies, this unlock condition should be the same as the normal unlock condition
    /// </summary>
    protected virtual UnlockCondition CategoryUnlockCondition => UnlockCondition.Get<StartsUnlocked>();
    public bool SameUnlockAsBody(Body b)
    {
        return b.CategoryUnlockCondition == CategoryUnlockCondition && this is not Body;
    }
    public bool CategoryUnlocked => CategoryUnlockCondition.Unlocked || this is Body || isSubEquipment;
    public bool IsUnlocked => (UnlockCondition.Unlocked && CategoryUnlocked) || (SameUnlockAsBody(Player.Instance.Body) && !isSubEquipment);
    public virtual void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {

    }
    public string GetName()
    {
        return IsUnlocked ? Name() : "???";
    }
    public string GetDescription()
    {
        if (IsUnlocked) //If fully unlocked
            return Description();
        if (CategoryUnlocked && !UnlockCondition.Unlocked) //If only the character is unlocked
            return "Unlock by:\n" + UnlockCondition.LockedText();
        if (!CategoryUnlocked && UnlockCondition.Unlocked) //If only the equipment is unlocked
            return "Unlock by:\n" + CategoryUnlockCondition.LockedText();
        return "Play more to discover this equipment";
    }
    protected virtual string Name()
    {
        return TypeName.ToSpacedString();
    }
    protected virtual string Description()
    {
        return "Temporary Equipment Description";
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
    /// <summary>
    /// Equipment is at 0
    /// </summary>
    /// <returns></returns>
    [Obsolete]
    public int GetPrice() => 0;
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
}
