using System.Collections.Generic;
using UnityEngine;

public class BunceHat : BubblemancerHat
{
    public override UnlockCondition UnlockCondition => UnlockCondition.Get<PlayerDeathUnlock100>();
    public override void ModifyUIOffsets(ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(ref offset, ref rotation, ref scale);
        scale *= 1.33f;
        offset.y -= 0.1f;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        base.ModifyPowerPool(powerPool);
        powerPool.Add<WeaponUpgrade>();
    }
    protected override void ReducePowerPool(List<PowerUp> powerPool)
    {
        powerPool.Remove<BinaryStars>();
    }
    protected override string Description()
    {
        return "You stupid!";
    }
    public override void OnStartWith()
    {

    }
    protected override void AnimationUpdate()
    {
        float r = new Vector2(Mathf.Abs(p.lastVelo.x), p.lastVelo.y * p.Direction).ToRotation() * Mathf.Rad2Deg * (0.3f + 1f * Mathf.Max(0, p.abilityTimer / p.abilityCD));
        if (spriteRender.flipX == p.BodyR.flipY)
        {
            spriteRender.flipX = !p.BodyR.flipY;
        }
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, r - 12 * p.Direction, 0.2f));
        if (p.abilityTimer > 0)
        {
            float sin = Mathf.Sqrt(Mathf.Abs( Mathf.Sin(Mathf.PI * Mathf.Max(0, p.abilityTimer / p.abilityCD)))) * p.abilityTimer / p.abilityCD;
            velocity = new Vector2(0, p.Direction * 2.5f * sin).RotatedBy(p.lastVelo.ToRotation());
        }
        else if (p.abilityTimer <= 0)
            velocity = Vector2.Lerp(velocity, Vector2.zero, 0.15f);
        transform.localPosition = Vector2.Lerp((Vector2)transform.localPosition,
            new Vector2(-0.1f * p.Direction, 0.2f + 0.5f * p.Bobbing * p.squash - 0.2f * (1 - p.squash)).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad) + velocity,
            0.25f);
    }
    protected override void DeathAnimation()
    {
        if (p.DeathKillTimer <= 0)
            velocity *= 0.1f;
        base.DeathAnimation();
    }
}
