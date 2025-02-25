using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bulb : Hat
{
    public Sprite OnBulb;
    public Sprite OffBulb;
    public override UnlockCondition UnlockCondition => UnlockCondition.Get<StartsUnlocked>();
    public override void ModifyUIOffsets(ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(ref offset, ref rotation, ref scale);
        scale *= 2.2f;
        offset.y -= 0.3f;
        offset.x = 0.1f;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<WeaponUpgrade>();
    }
    protected override string Description()
    {
        return "Brighten your day with this technological marvel!";
    }
    public override void OnStartWith()
    {

    }
    protected override void AnimationUpdate()
    {
        float r = new Vector2(Mathf.Abs(p.lastVelo.x), p.lastVelo.y * p.Direction).ToRotation() * Mathf.Rad2Deg * (0.3f + 1f * Mathf.Max(0, p.dashTimer / p.dashCD));
        if (spriteRender.flipX == p.BodyR.flipY)
        {
            spriteRender.flipX = !p.BodyR.flipY;
        }
        spriteRender.sprite = OnBulb; 
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, r - 15 * p.Direction, 0.2f));
        if (p.dashTimer > 0)
        {
            float sin = Mathf.Sqrt(Mathf.Abs( Mathf.Sin(Mathf.PI * Mathf.Max(0, p.dashTimer / p.dashCD)))) * p.dashTimer / p.dashCD;
            velocity = new Vector2(0, p.Direction * 2.5f * sin).RotatedBy(p.lastVelo.ToRotation());
        }
        else if (p.dashTimer <= 0)
            velocity = Vector2.Lerp(velocity, Vector2.zero, 0.15f);
        transform.localPosition = Vector2.Lerp((Vector2)transform.localPosition,
            new Vector2(-0.2f * p.Direction, 0.5f + 0.5f * p.Bobbing * p.squash - 0.2f * (1 - p.squash)).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad) + velocity,
            0.25f);
        bounceCount = 0.7f;
    }
    private float bounceCount = 0.7f;
    protected override void DeathAnimation()
    {
        float toBody = transform.localPosition.y - p.Body.transform.localPosition.y;
        if (p.DeathKillTimer <= 0)
        {
            velocity *= 0.0f;
            velocity.y += 0.03f;
            velocity.x += 0.03f * p.Direction;
        }
        if (toBody < -0.5f)
        {
            velocity *= -bounceCount;
            transform.localPosition = (Vector2)transform.localPosition + new Vector2(0, -0.5f -toBody);
            bounceCount *= 0.5f;
        }
        else
        {
            velocity.x *= 0.998f;
            velocity.y -= 0.005f;
        }
        spriteRender.sprite = Mathf.Abs(velocity.y) > 0.036f ? OnBulb : OffBulb;
        transform.localPosition = (Vector2)transform.localPosition + velocity;
        float deathAngle = 0; // 90 * Mathf.Min(p.DeathKillTimer / 100f, 1) * (spriteRender.flipX ? -1 : 1);
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, (deathAngle - 20) * p.Direction, 0.2f));
    }
}
