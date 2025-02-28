using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public static Player p => Player.Instance;
    private static List<PowerUp> PowerPool = new();
    private string InternalName = null;
    public List<GameObject> SubEquipment = new();
    public Equipment OriginalPrefab = null;
    public SpriteRenderer spriteRender;
    public Vector2 velocity;
    public bool hasInit = false;
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
        Player.Instance.Cape.ModifyPowerPool(PowerPool);
        Player.Instance.Wand.ModifyPowerPool(PowerPool);
        Player.Instance.Body.ModifyPowerPool(PowerPool);
        Player.Instance.Hat.ReducePowerPool(PowerPool);
        Player.Instance.Cape.ReducePowerPool(PowerPool);
        Player.Instance.Wand.ReducePowerPool(PowerPool);
        Player.Instance.Body.ReducePowerPool(PowerPool);
        for (int i = 0; i < PowerPool.Count; ++i)
        {
            PowerUp.AddPowerUpToAvailability(PowerPool[i]);
        }
        PowerPool.Clear();
    }
    public virtual UnlockCondition UnlockCondition => UnlockCondition.Get<StartsUnlocked>();
    public virtual void ModifyUIOffsets(ref Vector2 offset, ref float rotation, ref float scale)
    {

    }
    public string GetName()
    {
        return UnlockCondition.Unlocked ? Name() : "???";
    }
    public string GetDescription()
    {
        return UnlockCondition.Unlocked ? Description() : UnlockCondition.LockedText();
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
}
