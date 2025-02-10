using System.Collections.Generic;
using UnityEngine;

public class BubblemancerHat : Hat
{
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Starbarbs>();
    }
    protected override string Description()
    {
        return "A stylish hat fashioned with wool from his precious sheep";
    }
    protected override void AnimationUpdate()
    {
        float r = new Vector2(Mathf.Abs(p.lastVelo.x), p.lastVelo.y * p.Direction).ToRotation() * Mathf.Rad2Deg * (0.3f + 1f * Mathf.Max(0, p.dashTimer / p.dashCD));
        if (spriteRender.flipX == p.BodyR.flipY)
        {
            spriteRender.flipX = !p.BodyR.flipY;
        }
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, r, 0.2f));
        //if (dashTimer > 0)
        //{
        //    float sin = Mathf.Sqrt(Mathf.Abs( Mathf.Sin(Mathf.PI * Mathf.Max(0, dashTimer / dashCD)))) * dashTimer / dashCD;
        //    posOffset = new Vector2(-sin, Mathf.Sign(lastVelo.x) * 1.1f * sin).RotatedBy(lastVelo.ToRotation());
        //}
        //else if (dashTimer <= 0)
        velocity = Vector2.Lerp(velocity, Vector2.zero, 0.2f);
        transform.localPosition = Vector2.Lerp((Vector2)transform.localPosition, new Vector2(0, -0.3f + 0.8f * p.Bobbing * p.squash - 1f * (1 - p.squash)), 0.05f) + velocity;
    }
    protected override void DeathAnimation()
    {
        if (p.DeathKillTimer <= 0)
            velocity.y += 0.25f;
        float toBody = transform.localPosition.y - p.Body.transform.localPosition.y;
        float sinusoid1 = Mathf.Sin(p.DeathKillTimer * Mathf.PI / 60f);
        float sinusoid2 = Mathf.Sin(p.DeathKillTimer * Mathf.PI / 40f);
        if (toBody < 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.localEulerAngles.z, 0, 0.1f));
            transform.localPosition = (Vector2)transform.localPosition + velocity;
            velocity *= 0.6f;
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.localEulerAngles.z, sinusoid1 * 25f, 0.1f));
            transform.localPosition = (Vector2)transform.localPosition + velocity;
            velocity.x = sinusoid2 * 0.019f * toBody;
            velocity.y -= 0.003f;
            velocity *= 0.97f;
        }
    }
}
