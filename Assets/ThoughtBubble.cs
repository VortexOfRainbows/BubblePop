using System.Collections.Generic;
using UnityEngine;

public class ThoughtBubble : Body
{
    public SpriteRenderer MouthR;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Choice>();
        powerPool.Add<Choice>();
    }
    protected override string Description()
    {
        return "A mysterious Bubbletech Scientist. His origin is unknown; he appeared before Bubblemancer shortly after the first waves... why was he there?";
    }
    public override void FaceUpdate()
    {
        Vector2 toMouse = Utils.MouseWorld - (Vector2)transform.position;
        toMouse *= Mathf.Sign(p.lastVelo.x);
        Vector2 toMouse2 = toMouse.normalized;
        toMouse2.x += Mathf.Sign(toMouse2.x) * 5;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.2f, 0).RotatedBy(toMouseR);
        looking.y *= 0.8f;
        if (looking.x < 0)
            toMouseR += Mathf.PI;
        toMouseR *= (spriteRender.flipY ? -1 : 1);
        Vector2 pos = new Vector2(0.1f, 0.04f) + looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouseR * Mathf.Rad2Deg);
        FaceR.flipY = MouthR.flipY = false;
    }
}
