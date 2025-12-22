using System.Collections.Generic;
using UnityEngine;

public class BunceHat : BubblemancerHat
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<PlayerDeathUnlock10>();
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        scale *= 1.35f;
        offset.y -= 0.35f;
        offset.x += 0.1f;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bunce Cap").WithDescription("Grants an additional heart\nYou stupid!");
    }
    public override void ModifyLifeStats(ref int MaxLife, ref int Life, ref int MaxShield, ref int Shield)
    {
        ++Life;
        ++MaxLife;
    }
    protected override void AnimationUpdate()
    {
        float bonus = 0.12f;
        if (player.Body is Bubblemancer)
        {
            if (player.abilityTimer > 0)
            {
                float sin = Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Mathf.PI * Mathf.Max(0, player.abilityTimer / player.AbilityCD)))) * player.abilityTimer / player.AbilityCD;
                velocity = new Vector2(0, p.Direction * 0.65f * sin).RotatedBy(p.lastVelo.ToRotation());
            }
            else if (player.AbilityReady)
                velocity = Vector2.Lerp(velocity, Vector2.zero, 0.15f);
        }
        else if (player.Body is Gachapon)
            bonus = 0.16f;
        bonus -= 0.23f;
        spriteRender.flipX = !p.Body.Flipped;
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, p.MoveDashRotation() - 12 * p.Direction, 0.2f));
        velocity = Vector2.Lerp(velocity, Vector2.zero, 0.2f);
        transform.localPosition = Vector2.Lerp((Vector2)transform.localPosition,
            new Vector2(-0.12f * p.Direction, (bonus + 0.6f * p.Bobbing * p.squash - 0.2f * (1 - p.squash))).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad),
            0.2f) + velocity;
    }
    protected override void DeathAnimation()
    {
        if (p.DeathKillTimer <= 0)
            velocity *= 0.1f;
        base.DeathAnimation();
    }
    public override int GetRarity()
    {
        return 2;
    }
}
