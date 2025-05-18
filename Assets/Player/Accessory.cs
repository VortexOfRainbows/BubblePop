using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accessory : Equipment
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(0.05f, 0.9f);
    }
    protected float MimicHatEulerZ = 0;
    new public void AliveUpdate()
    {
        float r = new Vector2(p.Direction, p.lastVelo.y * p.Direction).ToRotation() * Mathf.Rad2Deg * (0.3f + 1f * Mathf.Max(0, player.abilityTimer / player.AbilityCD));
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
