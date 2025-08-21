using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class JumpMotion : Animator
{
    public Transform[] LegAnchors;
    public Transform[] ArmAnchors;
    public Transform BodyAnchor;
    public Transform Visual;
    public float JumpPercent { get; set; }
    public override void UpdateAnimation()
    {
        if(JumpPercent > 0)
        {
            BodyAnchor.localPosition = new Vector3(0, -JumpPercent * 0.05f, 0);
            Visual.transform.localPosition = new Vector3(0, 0, 0);
            int i = 1;
            foreach (Transform t in LegAnchors)
            {
                t.LerpLocalEulerZ(-10f * JumpPercent * i, 0.1f);
                i = -1;
            }
            foreach (Transform t in ArmAnchors)
            {
                t.LerpLocalEulerZ(5 * JumpPercent, 0.1f);
            }
            BodyAnchor.LerpLocalEulerZ(0f, 0.1f);
        }
        else if(JumpPercent < 0)
        {
            float sin = Mathf.Abs(Mathf.Sin(-JumpPercent * Mathf.PI));
            sin = Mathf.Pow(sin, 0.8f) * 1.2f;
            BodyAnchor.LerpLocalEulerZ(JumpPercent * 15.5f, 0.1f);
            BodyAnchor.localPosition = new Vector3(0, JumpPercent * 0.025f, 0);
            Visual.transform.localPosition = new Vector3(0, sin, 0);
            foreach (Transform t in LegAnchors)
            {
                t.LerpLocalEulerZ(35 * JumpPercent, 0.1f);
            }
            foreach (Transform t in ArmAnchors)
            {
                t.LerpLocalEulerZ(25 * JumpPercent, 0.1f);
            }
        }
        Visual.transform.LerpLocalScale(new Vector2((1 + JumpPercent * 0.08f) * Utils.SignNoZero(Visual.transform.localScale.x), 1 - JumpPercent * 0.08f), 0.1f);
    }
}