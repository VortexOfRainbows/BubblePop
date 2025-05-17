using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngineInternal;

public class Gachapon : Body
{
    public Sprite[] altFaces;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<GachaponUnlock>();
    protected override UnlockCondition CategoryUnlockCondition => UnlockCondition.Get<GachaponUnlock>();
    public override void Init()
    {
        PrimaryColor = new Color(0.95f, 1f, 0.6f);
    }
    //public SpriteRenderer MouthR;
    protected override float AngleMultiplier => 0.4f;
    protected override float RotationSpeed => 1f;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Choice>();
        powerPool.Add<BubbleBirb>();
        powerPool.Add<Overclock>();
    }
    protected override string Description()
    {
        return "A greedy shopkeeper? No! I'm the most honest gal around!";
    }
    public override void FaceUpdate()
    {
        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && Player.Instance.Weapon.IsAttacking())
            FaceR.sprite = altFaces[1];
        else
            FaceR.sprite = altFaces[0];
        Vector2 toMouse = Utils.MouseWorld - (Vector2)transform.position;
        Vector2 toMouse2 = toMouse.normalized;
        toMouse2.x += Mathf.Sign(toMouse2.x) * 4;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.16f, 0).RotatedBy(toMouseR);
        looking.y *= 0.8f;
        if (looking.x < 0)
            toMouseR += Mathf.PI;
        Vector2 pos = new Vector2(0.08f * p.Direction, 0.3f) + looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouseR * Mathf.Rad2Deg);
        FaceR.flipX = toMouse.x > 0;
        //MouthR.flipX = FaceR.flipX;
    }
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {

    }
}
