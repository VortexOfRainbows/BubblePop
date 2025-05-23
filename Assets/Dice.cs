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
        powerPool.Add<SnakeEyes>();
        powerPool.Add<Choice>();
    }
    protected override void ReducePowerPool(List<PowerUp> powerPool)
    {
        base.ReducePowerPool(powerPool);
    }
    protected override void AnimationUpdate()
    {
        float r = p.MoveDashRotation();
        transform.localScale = new Vector3(p.Body.transform.localScale.x * (p.Body.Flipped ? -1 : 1), p.Body.transform.localScale.y, p.Body.transform.localScale.z);
        transform.localEulerAngles = Mathf.LerpAngle(transform.localEulerAngles.z, r, 0.1f) * Vector3.forward;
        transform.localPosition = Vector2.Lerp((Vector2)transform.localPosition,
            new Vector2(0, (-1.5f + 1.5f * p.Bobbing * p.squash)).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad),
            0.25f) + velocity;
        bounceCount = 0.7f;
        velocity *= 0.9f;
    }
    private float bounceCount = 0.7f;
    protected override void DeathAnimation()
    {
        float toBody = transform.localPosition.y - p.Body.transform.localPosition.y;
        if (p.DeathKillTimer <= 0)
        {
            velocity *= 0.1f;
            velocity.x += Utils.RandFloat(-1, 1) * 0.05f;
            velocity.y += 0.05f;
        }
        if (toBody < -0.95f)
        {
            velocity *= -bounceCount;
            transform.localPosition = (Vector2)transform.localPosition + new Vector2(0, -0.95f - toBody);
            bounceCount *= 0.5f;
        }
        else
        {
            velocity.x *= 0.998f;
            velocity.y -= 0.005f;
        }
        transform.localPosition = (Vector2)transform.localPosition + velocity;
        transform.localEulerAngles = Mathf.LerpAngle(transform.localEulerAngles.z, 0, 0.1f) * Vector3.forward;
    }
}
