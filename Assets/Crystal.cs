using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Accessory
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = Vector2.zero;
        scale = 2f;
    }
    protected override string Name()
    {
        return "Chaotic Emeralds";
    }
    protected override string Description()
    {
        return "They are said to hold a moderate amount of power";
    }
    protected override UnlockCondition CategoryUnlockCondition => base.CategoryUnlockCondition;
    protected override UnlockCondition UnlockCondition => base.UnlockCondition;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<BubbleBirb>();
        powerPool.Add<Magnet>();
    }
    protected override void ReducePowerPool(List<PowerUp> powerPool)
    {
        base.ReducePowerPool(powerPool);
    }
    protected override void AnimationUpdate()
    {
        transform.localScale = new Vector3(p.Body.transform.localScale.x * (p.Body.Flipped ? -1 : 1), p.Body.transform.localScale.y, p.Body.transform.localScale.z);
        transform.localPosition = p.Body.transform.localPosition;
    }
    protected override void DeathAnimation()
    {
        base.DeathAnimation();
    }
}
