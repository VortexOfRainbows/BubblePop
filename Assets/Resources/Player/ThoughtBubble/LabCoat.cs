using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LabCoat : BubblemancerCape
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        scale *= 1.1f;
        offset.x -= 0.06f;
        offset.y -= 0.21f;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Calculator>();
    }
    public SpriteRenderer ArmL;
    public SpriteRenderer ArmR;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ThoughtBubbleUnlock>();
    protected override Vector2 CapeScale => new Vector2(1f, 1f);
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Lab Coat").WithDescription("Comes with Thought Bubble's favorite tools");
    }
    public override void Init()
    {
        base.Init();
        RightInwardAnimDuringDash = 0;
        LeftInwardAnimDuringDash = 0;
        DashStretchAmt *= 0.55f;
        LookingAtMouseScale *= 0.5f;
        DeathLeftMult = 1.0f;
        DeathRightMult = 1.0f;
    }
    public override void OnStartWith()
    {
        //Player.Instance.DashMult = 0.6f;
    }
    protected override void AnimationUpdate()
    {
        base.AnimationUpdate();
        if (this is ShadyCoat)
            return;
        Vector2 toMouse = Utils.MouseWorld - (Vector2)p.Body.transform.position;
        float facingDir = p.Direction;
        toMouse = facingDir * LookingAtMouseScale * toMouse.normalized;
        CapeB.transform.localPosition = CapeB.transform.localPosition + new Vector3((toMouse.x * 0.08f + 0.02f) * facingDir, -0.06f);
        ArmL.flipX = CapeLRend.flipX;
        ArmR.flipX = CapeRRend.flipX;
        MoveArm(ArmL, -1);
        MoveArm(ArmR, 1);
        //ArmL.transform.localScale = new Vector3(1f / CapeL.transform.localScale.x, ArmL.transform.localScale.y);
        //ArmR.transform.localScale = new Vector3(1f / CapeR.transform.localScale.x, ArmR.transform.localScale.y);
    }
    public void MoveArm(SpriteRenderer arm, float dir = 1, float bonus = 0)
    {
        float veloX = 8 * Mathf.Sqrt(Mathf.Abs(p.rb.velocity.x));
        float veloY = 4 * Mathf.Sqrt(Mathf.Abs(p.rb.velocity.y)) * Mathf.Sign(p.rb.velocity.y) * dir;
        veloX -= veloY;
        bonus *= p.Direction;
        arm.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(arm.transform.eulerAngles.z - bonus, veloX * -p.Direction, 0.2f) + bonus);
    }
}
