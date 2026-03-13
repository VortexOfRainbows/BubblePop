using System;
using UnityEngine;

public abstract class Animator : MonoBehaviour
{
    public Entity Parent;
    public void Start()
    {
        if(Parent != null && Parent is not Player)
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
    public bool FlipWithDirection = false;
    public Vector2 ReAnchorOffset { get; set; } = Vector2.zero;
    public float walkSpeedMultiplier;
    public Vector2 ParentVelocity { get; set; } = Vector2.zero;
    public void Animate()
    {
        if(Parent is Player p && p.Body is Fizzy f)
        {
            if(f.OnSkateboard)
                ParentVelocity *= p.MovementDeacceleration;
            else
                ParentVelocity = Parent.RB.velocity * (1 - f.SkateboardPercent);
        }
        else
            ParentVelocity = Parent.RB.velocity;
        float rRate = 1 - ResetRate;
        float walkMotion = WalkSize;
        float walkDirection = 1f;
        if (FlipWithDirection)
            walkDirection = -Utils.SignNoZero(ParentVelocity.x);
        float magnitude = ParentVelocity.magnitude;
        float velocity = (SqrtSpeed ? Mathf.Sqrt(magnitude) : magnitude) * 2f * SpeedMult;
        walkSpeedMultiplier = Mathf.Clamp(Math.Abs(velocity), 0, 1f);
        WalkCounter += walkDirection * velocity * Mathf.Deg2Rad * Mathf.Clamp(walkSpeedMultiplier * 9, 0, 1);
        WalkCounter = WalkCounter.WrapAngle();

        if (rRate != 1)
        {
            WalkMultiplier += Mathf.Abs(velocity) * ResetRate * 2.5f;
            WalkMultiplier *= rRate;
            WalkMultiplier = Math.Clamp(WalkMultiplier, 0, 1);
        }
        if (walkSpeedMultiplier == 0)
            WalkCounter = 0;

        Vector2 circularMotion = new Vector2(walkMotion * WalkMultiplier, 0).RotatedBy(-WalkCounter + AngleOffset * Mathf.Deg2Rad) * walkSpeedMultiplier;
        circularMotion.x *= WalkSizeXMult;
        Vector2 inverse = -circularMotion;
        float mult = 1.0f;
        if (circularMotion.y < 0)
            circularMotion.y *= 0.1f;
        if (inverse.y < 0)
        {
            mult = 0.2f;
            inverse.y *= 0.1f;
        }
        inverse += ReAnchorOffset * walkSpeedMultiplier;

        transform.localPosition = transform.localPosition.SetXY(inverse);
        float angle = WalkTiltAngle;
        float leftAngle = Mathf.Sin(WalkCounter) * -angle * walkSpeedMultiplier * mult;
        transform.localEulerAngles = new Vector3(0, 0, leftAngle);
    }
}
