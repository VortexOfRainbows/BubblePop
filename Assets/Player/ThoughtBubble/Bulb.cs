using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bulb : Hat
{
    public Sprite OnBulb;
    public Sprite OffBulb;
    public Light2D light2d;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ChoiceUnlock200>();
    protected override UnlockCondition CategoryUnlockCondition => UnlockCondition.Get<ThoughtBubbleUnlock>();
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        scale *= 2.2f;
        offset.y -= 0.25f;
        offset.x = 0.075f;
        light2d.gameObject.SetActive(false);
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<WeaponUpgrade>();
        powerPool.Add<SpearOfLight>();
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
        float r = new Vector2(Mathf.Abs(p.lastVelo.x), p.lastVelo.y * p.Direction).ToRotation() * Mathf.Rad2Deg * (0.3f + 1f * Mathf.Max(0, p.abilityTimer / p.abilityCD));
        spriteRender.flipX = p.BodyR.flipY;
        spriteRender.sprite = OnBulb;
        light2d.intensity = Mathf.Lerp(light2d.intensity, 1, 0.08f);
        light2d.gameObject.SetActive(true);
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, r - 10 * p.Direction, 0.2f));
        velocity = Vector2.Lerp(velocity, Vector2.zero, 0.15f);
        transform.localPosition = Vector2.Lerp((Vector2)transform.localPosition,
            new Vector2(-0.5f * p.Direction, 0.5f + 0.5f * p.Bobbing * p.squash - 0.2f * (1 - p.squash)).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad) + velocity,
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
            velocity.x += 0.05f * p.Direction;
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
        bool on = Mathf.Abs(velocity.y) > 0.032f;
        if (on)
            light2d.intensity = Mathf.Lerp(light2d.intensity, 1, 0.08f);
        if (!on)
            light2d.intensity = Mathf.Lerp(light2d.intensity, 0, 0.08f);
        spriteRender.sprite = on ? OnBulb : OffBulb;
        light2d.gameObject.SetActive(on);
        transform.localPosition = (Vector2)transform.localPosition + velocity;
        float deathAngle = 90 * Mathf.Min(p.DeathKillTimer / 100f, 1);
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, (deathAngle - 20) * p.Direction, 0.2f));
    }
    private float lightSpearCounter = 0;
    public override void EquipUpdate()
    {
        if(p.LightSpear > 0)
        {
            Vector2 shootFromPos = (Vector2)transform.position + new Vector2(0, 0.4f).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad);
            float shotTime = 2;
            lightSpearCounter += Time.fixedDeltaTime * (1 + Mathf.Sqrt(p.LightSpear));
            while(lightSpearCounter > shotTime)
            {
                float spearSpeed = 10 + p.LightSpear;
                float spearRange = 4 + p.LightSpear * 2;
                Entity target = Entity.FindClosest(shootFromPos, spearRange, out Vector2 norm);
                if (target != null)
                {
                    Projectile.NewProjectile<SmallBubble>(shootFromPos, norm * spearSpeed, 0, 0);
                }
                lightSpearCounter -= shotTime;
            }
        }
    }
}
