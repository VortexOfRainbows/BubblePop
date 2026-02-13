using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleStaff : BubblemancerWand
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ChargeShot10>();
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        offset.y -= 0.01f;
        scale *= 0.99f;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        base.ModifyPowerPool(powerPool);
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubble Staff").WithDescription("An expertly crafted bubble-tech weapon");
    }
    public override int GetRarity()
    {
        return 2;
    }
    protected override void AnimationUpdate()
    {
        base.AnimationUpdate();
        spriteRender.flipY = !spriteRender.flipY;
    }
    public override void EquipUpdate()
    {
        Player.OldCoalescence += 1;
        Player.SecondaryAttackSpeedModifier += 0.2f;
    }
}
