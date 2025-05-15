using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : Hat
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(0.52f, -0.6f);
        scale = 1.4f;
        rotation = 15f;
    }
    protected override string Name()
    {
        return "Dice Bow";
    }
    protected override string Description()
    {
        return "Strategically superglued dice";
    }
    protected override UnlockCondition CategoryUnlockCondition => base.CategoryUnlockCondition;
    protected override UnlockCondition UnlockCondition => base.UnlockCondition;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<BubbleBirb>();
        powerPool.Add<Choice>();
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
