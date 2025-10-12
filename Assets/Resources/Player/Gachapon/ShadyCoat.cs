using System.Collections.Generic;
using UnityEngine;
public class ShadyCoat : LabCoat
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        scale *= 1.1f;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Magnet>();
        powerPool.Add<ResonanceRuby>();
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<GachaponUnlock>();
    protected override Vector2 CapeScale => new Vector2(1f, 1f);
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Shady Coat").WithDescription("Gachapon must know something about the black market dealer! She has his coat!");
    }
    public override void Init()
    {
        base.Init();
    }
    protected override void AnimationUpdate()
    {
        base.AnimationUpdate();
        Vector2 toMouse = Utils.MouseWorld - (Vector2)p.Body.transform.position;
        float facingDir = p.Direction;
        toMouse = facingDir * LookingAtMouseScale * toMouse.normalized;
        float offset = player.Body is Gachapon ? 0.35f : 0.1f;
        CapeB.transform.localPosition = CapeB.transform.localPosition
            + new Vector3((toMouse.x * 0.06f) * facingDir, offset);
        if (player.Body is ThoughtBubble)
            CapeB.transform.localScale = new Vector3(1.1f, CapeB.transform.localScale.y, CapeB.transform.localScale.z);
        ArmL.flipX = !CapeLRend.flipX;
        ArmR.flipX = CapeRRend.flipX;
        MoveArm(ArmL, -1, 7);
        MoveArm(ArmR, 1);
    }
}