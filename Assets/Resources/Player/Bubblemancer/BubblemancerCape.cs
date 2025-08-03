using System.Collections.Generic;
using UnityEngine;

public class BubblemancerCape : Accessory
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubblemancerUnlock>();
    public override void Init()
    {
        RightCapeDefaultPos = CapeR.transform.localPosition;
        LeftCapeDefaultPos = CapeL.transform.localPosition;
    }
    protected Vector3 RightCapeDefaultPos;
    protected Vector3 LeftCapeDefaultPos;
    protected virtual Vector2 CapeScale => new Vector2(0.73f, 0.67f);
    protected float RightInwardAnimDuringDash = 0.3f;
    protected float LeftInwardAnimDuringDash = 0.425f;
    protected float DashStretchAmt = 0.875f;
    protected float LookingAtMouseScale = 0.5f;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Magnet>();
        powerPool.Add<BubbleShield>();
    }
    protected override string Description()
    {
        return "A stylish robe fashioned with wool from his precious sheep";
    }
    public GameObject CapeB;
    public GameObject CapeL;
    public GameObject CapeR;
    public SpriteRenderer CapeBRend => spriteRender;
    public SpriteRenderer CapeLRend;
    public SpriteRenderer CapeRRend;
    protected override void AnimationUpdate()
    {
        //Time.timeScale = 0.2f;
        Vector2 toMouse = Utils.MouseWorld - (Vector2)p.Body.transform.position;
        float facingDir = p.Direction;
        toMouse = toMouse.normalized * LookingAtMouseScale * facingDir;

        float dashFactorL = 0.8f - p.squash + p.Bobbing * 0.2f;
        float dashFactorR = 0.8f - p.squash + p.Bobbing * 0.3f;
        CapeR.transform.localPosition = new Vector3((RightCapeDefaultPos.x + RightInwardAnimDuringDash * dashFactorR) * facingDir, RightCapeDefaultPos.y);// + toMouse.x, 0, 0);
        CapeR.transform.localScale = new Vector3(0.9f + toMouse.x * 0.75f + 0.1f * toMouse.x * facingDir, 1, 1);
        CapeL.transform.localPosition = new Vector3((LeftCapeDefaultPos.x - LeftInwardAnimDuringDash * dashFactorL) * facingDir, LeftCapeDefaultPos.y);// + toMouse.x + facingDir, 0, 0);
        CapeL.transform.localScale = new Vector3(0.9f - toMouse.x * 0.9f + 0.1f * toMouse.x * facingDir, 1, 1);

        //float addedXOffset = 0.4f * Mathf.Sin(MimicHatEulerZ * Mathf.Deg2Rad);
        CapeL.transform.eulerAngles = new Vector3(0, 0, 15f * dashFactorL * facingDir);
        CapeR.transform.eulerAngles = new Vector3(0, 0, -25 * dashFactorR * facingDir);
        CapeB.transform.localPosition = new Vector2(Mathf.Lerp(-0.2f, 0.6f, 1 - p.squash) * toMouse.x * facingDir, -.25f);
        //Debug.Log(CapeB.transform.localPosition.x);
        Vector2 scale = new Vector2((1 - p.squash) * 2.5f + 0.1f * (1 - p.Bobbing), p.Bobbing * p.squash);
        //if its 90% or 270%, we want the x scale reduced
        scale.x *= 0.2f + DashStretchAmt * Mathf.Cos(new Vector2(Mathf.Abs(p.lastVelo.x), p.lastVelo.y).ToRotation());
        scale.x += 1;
        CapeB.transform.localScale = Vector2.Lerp(CapeB.transform.localScale, scale * CapeScale, 0.5f);
        CapeB.transform.eulerAngles = new Vector3(0, 0, 0);

        if (facingDir < 0)
        {
            CapeRRend.flipX = false;
            CapeLRend.flipX = false;
            CapeBRend.flipX = false;
        }
        else
        {
            CapeRRend.flipX = true;
            CapeLRend.flipX = true;
            CapeBRend.flipX = true;
        }
    }
    protected override void DeathAnimation()
    {
        float facingDir = p.Direction;
        CapeL.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(CapeL.transform.eulerAngles.z, 15 * facingDir, 0.1f));
        CapeR.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(CapeR.transform.eulerAngles.z, -25 * facingDir, 0.1f));
        CapeR.transform.localPosition = Vector3.Lerp(CapeR.transform.localPosition, new Vector3(-0.8f * facingDir, 0), 0.1f);
        CapeL.transform.localPosition = Vector3.Lerp(CapeL.transform.localPosition, new Vector3(1f * facingDir, 0), 0.1f);
        CapeL.transform.localScale = Vector3.Lerp(CapeL.transform.localScale, new Vector3(1f, .9f, 1), 0.1f);
        CapeR.transform.localScale = Vector3.Lerp(CapeR.transform.localScale, new Vector3(1f, .9f, 1), 0.1f);
        CapeB.transform.localScale = Vector3.Lerp(CapeB.transform.localScale, new Vector3(1.3f, .4f, 1) * 0.9f, 0.1f);
        CapeB.transform.localPosition = Vector3.Lerp(CapeB.transform.localPosition, new Vector3(0f, CapeB.transform.localPosition.y, 0), 0.1f);

        if (facingDir < 0)
        {
            CapeRRend.flipX = false;
            CapeLRend.flipX = false;
            CapeBRend.flipX = false;
        }
        else
        {
            CapeRRend.flipX = true;
            CapeLRend.flipX = true;
            CapeBRend.flipX = true;
        }
    }
}
