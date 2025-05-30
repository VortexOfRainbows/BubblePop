using System.Collections.Generic;
using UnityEngine;

public class BubblemancerHat : Hat
{
    protected override UnlockCondition CategoryUnlockCondition => UnlockCondition.Get<BubblemancerUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Dash>();
        powerPool.Add<LuckyStar>();
        powerPool.Add<BinaryStars>();
        powerPool.Add<Starbarbs>();
    }
    protected override string Description()
    {
        return "A stylish hat fashioned with wool from his precious sheep";
    }
    protected override void AnimationUpdate()
    {
        spriteRender.flipX = !p.Body.Flipped;
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, p.MoveDashRotation(), 0.2f));
        velocity = Vector2.Lerp(velocity, Vector2.zero, 0.2f);
        transform.localPosition = Vector2.Lerp((Vector2)transform.localPosition, 
            new Vector2(0, (-0.3f + 0.8f * p.Bobbing * p.squash - 0.2f * (1 - p.squash))).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad),
            0.06f) + velocity;
    }
    protected override void DeathAnimation()
    {
        if (p.DeathKillTimer <= 0)
            velocity.y += 0.25f;
        float toBody = transform.localPosition.y - p.Body.transform.localPosition.y;
        float sinusoid1 = Mathf.Sin(p.DeathKillTimer * Mathf.PI / 60f);
        float sinusoid2 = Mathf.Sin(p.DeathKillTimer * Mathf.PI / 40f);
        if (toBody < 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.localEulerAngles.z, 0, 0.1f));
            transform.localPosition = (Vector2)transform.localPosition + velocity;
            velocity *= 0.6f;
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.localEulerAngles.z, sinusoid1 * 25f, 0.1f));
            transform.localPosition = (Vector2)transform.localPosition + velocity;
            velocity.x = sinusoid2 * 0.019f * toBody;
            velocity.y -= 0.003f;
            velocity *= 0.97f;
        }
        transform.localPosition = Vector2.Lerp(transform.localPosition, new Vector2(0, transform.localPosition.y), 0.03f);
    }
}
