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
    protected override UnlockCondition CategoryUnlockCondition => UnlockCondition.Get<GachaponUnlock>();
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
        float bonusR = p.Body is Bubblemancer ? 1f * Mathf.Max(0, p.abilityTimer / p.abilityCD) : 0;
        float r = new Vector2(Mathf.Abs(p.lastVelo.x), p.lastVelo.y * p.Direction).ToRotation() * Mathf.Rad2Deg * (0.3f + bonusR);
        transform.localScale = new Vector3(p.Body.transform.localScale.x * (p.Body.Flipped ? -1 : 1), p.Body.transform.localScale.y, p.Body.transform.localScale.z);
        transform.localEulerAngles = Mathf.LerpAngle(transform.localEulerAngles.z, r, 0.1f) * Vector3.forward;
        transform.localPosition = Vector2.Lerp((Vector2)transform.localPosition,
            new Vector2(0, (-1.5f + 1.5f * p.Bobbing * p.squash)).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad),
            0.25f) + velocity;
    }
    protected override void DeathAnimation()
    {
        base.DeathAnimation();
    }
}
