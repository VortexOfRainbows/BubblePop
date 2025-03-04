using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class ThoughtBubble : Body
{
    public override void Init()
    {
        PrimaryColor = new Color(1.00f, 1.05f, 1.1f);
    }
    public SpriteRenderer MouthR;
    protected override float AngleMultiplier => 0.1f;
    protected override float RotationSpeed => 1f;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Choice>();
        powerPool.Add<Choice>();
        powerPool.Add<BubbleBirb>();
    }
    protected override string Description()
    {
        return "A mysterious Bubbletech Scientist. His origin is unknown; he appeared to Bubblemancer shortly after the first waves... why was he there?";
    }
    public override void FaceUpdate()
    {
        Vector2 toMouse = Utils.MouseWorld - (Vector2)transform.position;
        toMouse *= Mathf.Sign(p.lastVelo.x);
        Vector2 toMouse2 = toMouse.normalized;
        toMouse2.x += Mathf.Sign(toMouse2.x) * 4;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.2f, 0).RotatedBy(toMouseR);
        looking.y *= 0.8f;
        if (looking.x < 0)
            toMouseR += Mathf.PI;
        Vector2 pos = new Vector2(0.1f, 0.16f * p.Direction) + looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouseR * Mathf.Rad2Deg);
        FaceR.flipX = !p.BodyR.flipY;
        MouthR.flipX = p.BodyR.flipY;
    }
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {

    }
}
