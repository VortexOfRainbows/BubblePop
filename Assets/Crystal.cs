using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Crystal : Accessory
{
    public override void ModifyLifeStats(ref int MaxLife, ref int Life, ref int MaxShield, ref int Shield)
    {
        MaxLife += 1;
        Life += 1;
    }
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = Vector2.zero;
        scale = 2.4f;
    }
    protected override string Name()
    {
        return "Wishing Emerald";
    }
    protected override string Description()
    {
        return "Grants an additional heart";
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<GachaponUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Magnet>();
    }
    protected override void ReducePowerPool(List<PowerUp> powerPool)
    {
        base.ReducePowerPool(powerPool);
    }
    protected override void AnimationUpdate()
    {
        transform.localScale = new Vector3(p.Body.transform.localScale.x * (p.Body.Flipped ? -1 : 1), p.Body.transform.localScale.y, p.Body.transform.localScale.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, p.Body.transform.localPosition + new Vector3(0, .06f, 0), 0.3f);
        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.zero, 0.05f);
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
        float bounceArea = -0.2f;
        if (toBody < bounceArea)
        {
            velocity *= -bounceCount;
            transform.localPosition = (Vector2)transform.localPosition + new Vector2(0, bounceArea - toBody);
            bounceCount *= 0.5f;
        }
        else
        {
            velocity.x *= 0.998f;
            velocity.y -= 0.005f;
        }
        transform.localPosition = (Vector2)transform.localPosition + velocity;
    }
}
