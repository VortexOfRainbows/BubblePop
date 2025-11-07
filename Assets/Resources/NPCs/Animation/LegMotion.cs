using System;
using UnityEngine;

public abstract class Animator : MonoBehaviour
{
    public Entity Parent;
    public void Start()
    {
        Parent.AddAnim(this);
    }
    public virtual void UpdateAnimation()
    {

    }
}
public class LegMotion : Animator
{
    //public float Anim => Parent.Anim;
    public override void UpdateAnimation()
    {
        Animate();
    }
    public float SpeedMult = 1.0f;
    public float WalkCounter;
    public float AngleOffset = 180;
    public float WalkSize = 0.25f;
    public float WalkSizeXMult = 0.25f;
    public float WalkTiltAngle = 10;
    public bool SqrtSpeed = true;
    public float ResetRate = 0.0f;
    private float WalkMultiplier = 1.0f;
    public void Animate()
    {
        float rRate = 1 - ResetRate;
        float walkMotion = WalkSize;
        float walkDirection = 1f;
        //if (Entity.Velocity.y < -0.0 && MathF.Abs(Entity.Velocity.y) > 0.001f && MathF.Abs(Entity.Velocity.x) < 0.001f)
        //    walkDirection = -1;
        float velocity = (SqrtSpeed ? Mathf.Sqrt(Parent.RB.velocity.magnitude) : Parent.RB.velocity.magnitude) * 2f * SpeedMult;
        float walkSpeedMultiplier = Mathf.Clamp(Math.Abs(velocity), 0, 1f);
        WalkCounter += walkDirection * velocity * Mathf.Deg2Rad * Mathf.Clamp(walkSpeedMultiplier * 9, 0, 1);
        WalkCounter = WalkCounter.WrapAngle();

        if(rRate != 1)
        {
            WalkMultiplier += Mathf.Abs(velocity) * ResetRate * 2.5f;
            WalkMultiplier *= rRate;
            WalkMultiplier = Math.Clamp(WalkMultiplier, 0, 1);
        }

        Vector2 circularMotion = new Vector2(walkMotion * WalkMultiplier, 0).RotatedBy(-WalkCounter + AngleOffset * Mathf.Deg2Rad) * walkSpeedMultiplier;
        circularMotion.x *= WalkSizeXMult;
        Vector2 inverse = -circularMotion;
        if (circularMotion.y < 0)
            circularMotion.y *= 0.1f;
        if (inverse.y < 0)
            inverse.y *= 0.1f;

        transform.localPosition = transform.localPosition.SetXY(inverse);
        float angle = WalkTiltAngle;
        float leftAngle = Mathf.Sin(WalkCounter) * -angle * walkSpeedMultiplier;
        transform.localEulerAngles = new Vector3(0, 0, leftAngle);
    }
}
