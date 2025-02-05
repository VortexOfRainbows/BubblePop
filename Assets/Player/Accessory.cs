using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accessory : Equipment
{
    protected float MimicHatEulerZ = 0;
    new public void AliveUpdate()
    {
        float r = new Vector2(p.Direction, p.lastVelo.y * p.Direction).ToRotation() * Mathf.Rad2Deg * (0.3f + 1f * Mathf.Max(0, p.dashTimer / p.dashCD));
        MimicHatEulerZ = Mathf.LerpAngle(transform.eulerAngles.z, r, 0.2f);
        base.AliveUpdate();
    }
    protected override void AnimationUpdate()
    {

    }
    protected override void DeathAnimation()
    {

    }
}
