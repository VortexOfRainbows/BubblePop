using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    private static List<PowerUp> PowerPool = new();
    public List<Equipment> SubEquipment = new();
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
        return UnlockCondition.IsUnlocked ? Name() : "???";
    }
    public string GetDescription()
    {
        return UnlockCondition.IsUnlocked ? Description() : UnlockCondition.LockedText();
    }
    protected virtual string Name()
    {
        return this.GetType().Name.ToSpacedString();
    }
    protected virtual string Description()
    {
        return "Temporary Equipment Description";
    }
    public static Player p => Player.Instance;
    public SpriteRenderer spriteRender;
    public Vector2 velocity;
    public void AliveUpdate()
    {
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
}
