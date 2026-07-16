using System.Collections.Generic;
using UnityEngine;

public class OilHat : BubblemancerHat
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(0.08f, -0.9f);
        scale *= 0.9425f;
        rotation = 5f;
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<KingOilUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Burger>();
        powerPool.Add<Burger>();
        powerPool.Add<Burger>();
        powerPool.Add<Burger>();
        powerPool.Add<Burger>();
    }
    protected override void AnimationUpdate()
    {
        float r = p.MoveDashRotation() * 0.4f;
        transform.localScale = new Vector3(p.Body.transform.localScale.x * (p.Body.Flipped ? -1 : 1), 0.8f + 0.2f * p.Body.transform.localScale.y, p.Body.transform.localScale.z);
        transform.localEulerAngles = Mathf.LerpAngle(transform.localEulerAngles.z, r, 0.1f) * Vector3.forward;
        transform.SetLocalXY(Vector2.Lerp((Vector2)transform.localPosition, new Vector2(0, 0.05f + 0.65f * p.Bobbing * p.Squash).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad), 0.1f) + velocity);
        velocity *= 0.9f;
    }
}
