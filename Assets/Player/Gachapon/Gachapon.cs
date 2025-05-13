using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngineInternal;

public class Gachapon : Body
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ThoughtBubbleUnlock>();
    protected override UnlockCondition CategoryUnlockCondition => UnlockCondition.Get<ThoughtBubbleUnlock>();
    public override void Init()
    {
        PrimaryColor = new Color(0.6f, 0.9f, 0.3f);
    }
    //public SpriteRenderer MouthR;
    protected override float AngleMultiplier => 0.1f;
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
        Vector2 toMouse = Utils.MouseWorld - (Vector2)transform.position;
        Vector2 toMouse2 = toMouse.normalized;
        toMouse2.x += Mathf.Sign(toMouse2.x) * 4;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.2f, 0).RotatedBy(toMouseR);
        looking.y *= 0.8f;
        if (looking.x < 0)
            toMouseR += Mathf.PI;
        Vector2 pos = new Vector2(0.1f * p.Direction, 0.16f) + looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouseR * Mathf.Rad2Deg);
        FaceR.flipX = toMouse.x < 0;
        //MouthR.flipX = FaceR.flipX;
    }
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {

    }
}
