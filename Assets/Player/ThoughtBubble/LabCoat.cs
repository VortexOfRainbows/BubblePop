using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class LabCoat : BubblemancerCape
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        scale *= 1.05f;
        offset.x += 0.05f;
        offset.y -= 0.1f;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Magnet>();
        powerPool.Add<Calculator>();
    }
    public SpriteRenderer ArmL;
    public SpriteRenderer ArmR;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ThoughtBubbleUnlock>();
    protected override Vector2 CapeScale => new Vector2(0.86f, 0.8f);
    protected override string Name()
    {
        return "Bubblechem Lab Coat";
    }
    protected override string Description()
    {
        return "Ready to do science!";
    }
    public override void Init()
    {
        base.Init();
        RightInwardAnimDuringDash = 0;
        LeftInwardAnimDuringDash = 0;
        DashStretchAmt *= 0.55f;
        LookingAtMouseScale *= 0.8f;
    }
    public override void OnStartWith()
    {
        //Player.Instance.DashMult = 0.6f;
    }
    protected override void AnimationUpdate()
    {
        base.AnimationUpdate();
        Vector2 toMouse = Utils.MouseWorld - (Vector2)p.Body.transform.position;
        float facingDir = p.Direction;
        toMouse = toMouse.normalized * LookingAtMouseScale * facingDir;
        CapeB.transform.localPosition = CapeB.transform.localPosition + new Vector3((toMouse.x * 0.2f - 0.08f) * facingDir, 0);
        ArmL.flipX = CapeLRend.flipX;
        ArmR.flipX = CapeRRend.flipX;
        MoveArm(ArmL);
        MoveArm(ArmR);
        //ArmL.transform.localScale = new Vector3(1f / CapeL.transform.localScale.x, ArmL.transform.localScale.y);
        //ArmR.transform.localScale = new Vector3(1f / CapeR.transform.localScale.x, ArmR.transform.localScale.y);
    }
    public void MoveArm(SpriteRenderer arm)
    {
        float veloX = 8 * Mathf.Sqrt(Mathf.Abs(p.rb.velocity.x));
        float veloY = 4 * Mathf.Sqrt(Mathf.Abs(p.rb.velocity.y)) * Mathf.Sign(p.rb.velocity.y);
        veloX -= veloY;
        arm.transform.localPosition = new Vector3(Mathf.Abs(arm.transform.localPosition.x) * -p.Direction, arm.transform.localPosition.y);
        arm.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(arm.transform.eulerAngles.z, veloX * -p.Direction, 0.2f));
    }
}
