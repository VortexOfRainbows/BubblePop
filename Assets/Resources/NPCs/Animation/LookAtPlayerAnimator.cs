using System;
using UnityEngine;
using UnityEngine.VFX;

public class LookAtPlayerAnimator : Animator
{
    public float LerpRate = 0.1f;
    public Vector2 ScaleFactor = new(1, 1);
    public bool Rotate = false;
    public float RotationXAugment = 1;
    public override void UpdateAnimation()
    {
        if(Parent is Enemy e)
        {
            Player target = e.Target;
            Vector2 toTarget = target.Position - (Vector2)Parent.transform.position;
            Vector2 norm = toTarget.normalized;
            norm.x *= ScaleFactor.x * e.Visual.transform.localScale.x;
            norm.y *= ScaleFactor.y * e.Visual.transform.localScale.y;
            transform.LerpLocalPosition(norm, LerpRate);

            if(Rotate)
            {
                norm.x *= Utils.SignNoZero(e.Visual.transform.localScale.x);
                norm.x += RotationXAugment;
                float angleToPlayer = norm.ToRotation();

                transform.LerpLocalEulerZ(angleToPlayer * Mathf.Rad2Deg, LerpRate);
            }
        }
    }
}
