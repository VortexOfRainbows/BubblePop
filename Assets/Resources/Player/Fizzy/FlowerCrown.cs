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
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Passive);
    }
    public override void EquipUpdate() => Player.HasFlowerCrownRecursiveHeal = true;
    public override int GetRarity() => 3;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyTouchGrass>();
    protected override void AnimationUpdate()
    {
        float r = p.MoveDashRotation() * 0.5f;
        transform.localScale = new Vector3(p.Body.transform.localScale.x * (p.Body.Flipped ? -1 : 1), p.Body.transform.localScale.y, p.Body.transform.localScale.z);
        transform.localEulerAngles = Mathf.LerpAngle(transform.localEulerAngles.z, r, 0.1f) * Vector3.forward;
        transform.SetLocalXY(Vector2.Lerp((Vector2)transform.localPosition,
            new Vector2(0, (-1.15f + 1.1f * p.Bobbing * p.Squash)).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad),
            0.1f) + velocity);
        velocity *= 0.9f;
    }
}
