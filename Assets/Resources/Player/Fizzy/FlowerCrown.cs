using System.Collections.Generic;
using UnityEngine;

public class FlowerCrown : Cap
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(0.1675f, -0.62f);
        scale = 1.3f;
        rotation = 12f;
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyUnlock>();
    protected override void AnimationUpdate()
    {
        float r = p.MoveDashRotation() * 0.5f;
        transform.localScale = new Vector3(p.Body.transform.localScale.x * (p.Body.Flipped ? -1 : 1), p.Body.transform.localScale.y, p.Body.transform.localScale.z);
        transform.localEulerAngles = Mathf.LerpAngle(transform.localEulerAngles.z, r, 0.1f) * Vector3.forward;
        transform.SetLocalXY(Vector2.Lerp((Vector2)transform.localPosition,
            new Vector2(0, (-1.15f + 1.1f * p.Bobbing * p.Squash)).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad),
            0.1f) + velocity);
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
            transform.SetLocalXY(transform.localPosition.x, transform.localPosition.y - 0.95f - toBody);
            bounceCount *= 0.5f;
        }
        else
        {
            velocity.x *= 0.998f;
            velocity.y -= 0.005f;
        }
        transform.SetLocalXY((Vector2)transform.localPosition + velocity);
        transform.localEulerAngles = Mathf.LerpAngle(transform.localEulerAngles.z, 0, 0.1f) * Vector3.forward;
    }
}
