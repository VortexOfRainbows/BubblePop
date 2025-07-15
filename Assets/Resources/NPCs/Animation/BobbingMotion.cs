using System;
using UnityEngine;

public class BobbingMotion : Animator
{
    public override void UpdateAnimation()
    {
        Animate();
    }
    public float SpeedMult = 1.0f;
    public float Counter;
    public float AngleOffset = 90;
    public float sizeMult = 0.1f;
    public float angularMult = 0.0f;
    public float angularSpeedMult = 1.0f;
    public bool SqrtSpeed = true;
    public void Animate()
    {
        float velocity = (SqrtSpeed ? Mathf.Sqrt(Parent.RB.velocity.magnitude) : Parent.RB.velocity.magnitude) * 2f * SpeedMult;
        float walkSpeedMultiplier = Mathf.Clamp(Math.Abs(velocity), 0, 1f);
        Counter += velocity * Mathf.Deg2Rad * Mathf.Clamp(walkSpeedMultiplier * 9, 0, 1);
        Counter = Counter.WrapAngle();
        float sin = Mathf.Sin(Counter * 2 + AngleOffset * Mathf.Deg2Rad);
        float sin2 = Mathf.Sin(Counter * 2 * angularSpeedMult + AngleOffset * Mathf.Deg2Rad);
        transform.localPosition = new Vector3(0, sin * sizeMult, transform.localPosition.z);
        transform.localEulerAngles = new Vector3(0, 0, sin2 * angularMult);
    }
}
