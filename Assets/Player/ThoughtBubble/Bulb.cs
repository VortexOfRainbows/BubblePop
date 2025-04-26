using System.Collections.Generic;
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
        powerPool.Add<NeuronActivation>();
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
    public static readonly float DefaultShotSpeed = 2.2f;
    public static readonly float MaxRange = 48;
    private float lightSpearCounter = 0;
    public static float SpeedModifier => (1 + Mathf.Sqrt(p.LightSpear));
    public override void EquipUpdate()
    {
        if(p.LightSpear > 0)
        {
            Vector2 shootFromPos = (Vector2)transform.position + new Vector2(0, 0.6f).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad) * transform.lossyScale.x;
            float shotTime = DefaultShotSpeed;
            lightSpearCounter += Time.fixedDeltaTime * SpeedModifier;
            while(lightSpearCounter > shotTime)
            {
                if(LaunchSpear(shootFromPos, out Vector2 norm, null, p.LightChainReact))
                {
                    velocity -= norm;
                    lightSpearCounter -= shotTime;
                }
                else
                    lightSpearCounter -= shotTime * 0.1f;
            }
        }
    }
    public static bool LaunchSpear(Vector2 shootFromPos, out Vector2 norm, Entity ignore, int BounceNum = 0, float bonusRange = 0)
    {
        float speedMod = SpeedModifier;
        float spearSpeed = 5 + speedMod * 0.015f; // this only matters for visuals as the spear is hitscan
        float spearRange = speedMod * 4.5f + bonusRange; //starts at 2 * 4.5 = 9
        if (spearRange > MaxRange)
            spearRange = MaxRange;
        Entity target = Entity.FindClosest(shootFromPos, spearRange, out Vector2 norm2, "Enemy", true, ignore);
        norm = norm2;
        if (target != null)
        {
            Projectile.NewProjectile<LightSpear>(shootFromPos, norm * spearSpeed, target.transform.position.x, target.transform.position.y, BounceNum);
            return true;
        }
        return false;
    }
}
