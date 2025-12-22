using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ShadyCoat : LabCoat
{
    public override int GetRarity()
    {
        return 2;
    }
    public GameObject Mask;
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        scale *= 1.1f;
        offset.y -= 0.15f;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<ResonanceRuby>();
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<GachaponBurger>();
    protected override Vector2 CapeScale => new Vector2(1f, 1f);
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Suspicious Disguise").WithDescription("Increases the rate at which black market items appear in the shop by 200%\n\nGachapon must know something about the black market dealer... why else would she have his coat?");
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
        float offset = player.Body is Gachapon ? 0.37f : 0.1f;
        CapeB.transform.localPosition = CapeB.transform.localPosition
            + new Vector3((toMouse.x * 0.06f) * facingDir, offset);
        if (player.Body is ThoughtBubble)
        {
            CapeB.transform.localScale = new Vector3(1.1f * facingDir, CapeB.transform.localScale.y, CapeB.transform.localScale.z);
            Mask.SetActive(false);
        }
        Vector2 voffset = toMouse.normalized * 1f * facingDir;
        voffset.y *= 0.2f;

        Vector3 targetMaskPos = (Vector2)player.Body.transform.position + voffset * 0.08f;
        Mask.transform.position = Vector3.Lerp(Mask.transform.position, targetMaskPos, 0.1f);
        Mask.transform.localPosition = new Vector3(Mask.transform.localPosition.x, Mask.transform.localPosition.y, 1);
        Mask.transform.LerpLocalEulerZ(voffset.y * 35, 0.1f);
        //Mask.GetComponent<SpriteRenderer>().flipX = player.Body.FaceR.flipX;
        ArmL.flipX = !CapeLRend.flipX;
        ArmR.flipX = CapeRRend.flipX;
        MoveArm(ArmL, -0.75f, 7);
        MoveArm(ArmR, 0.75f);
    }
    public override void EquipUpdate()
    {
        player.BlackmarketMult += 2;
    }
}