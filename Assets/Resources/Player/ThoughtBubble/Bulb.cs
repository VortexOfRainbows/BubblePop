using System.Collections.Generic;
using UnityEngine;
public class Bulb : Hat
{
    public Sprite OnBulb;
    public Sprite OffBulb;
    public SpriteRenderer light2d;
    protected float bounceCount = 0.7f;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ThoughtBubbleUnlock>();
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        scale *= 2.0f;
        offset.y -= 0.4f;
        offset.x = 0.085f;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Eureka>();
        powerPool.Add<SpearOfLight>();
        powerPool.Add<NeuronActivation>();
        powerPool.Add<Refraction>();
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Light Bulb").WithDescription("Brighten your day with this technological marvel!");
    }
    public override void OnStartWith()
    {

    }
    protected override void AnimationUpdate()
    {
        float r = p.MoveDashRotation();
        spriteRender.flipX = !p.Body.Flipped;
        spriteRender.sprite = OnBulb;
        light2d.color = light2d.color.WithAlpha(Mathf.Lerp(light2d.color.a, 0.2f, 0.08f));
        light2d.gameObject.SetActive(true);
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, r - 10 * p.Direction, 0.2f));
        velocity = Vector2.Lerp(velocity, Vector2.zero, 0.15f);
        transform.localPosition = Vector2.Lerp((Vector2)transform.localPosition,
            new Vector2(-0.5f * p.Direction, 0.5f + 0.5f * p.Bobbing * p.squash - 0.2f * (1 - p.squash)).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad) + velocity,
            0.25f);
        bounceCount = 0.7f;
    }
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
            light2d.color = light2d.color.WithAlpha(Mathf.Lerp(light2d.color.a, 0.2f, 0.08f));
        if (!on)
            light2d.color = light2d.color.WithAlpha(Mathf.Lerp(light2d.color.a, 0.0f, 0.08f));
        spriteRender.sprite = on ? OnBulb : OffBulb;
        light2d.gameObject.SetActive(on);
        transform.localPosition = (Vector2)transform.localPosition + velocity;
        float deathAngle = 90 * Mathf.Min(p.DeathKillTimer / 100f, 1);
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, (deathAngle - 20) * p.Direction, 0.2f));
    }
    public static readonly float DefaultShotSpeed = 2.2f;
    public static readonly float MaxRange = 48;
    private float lightSpearCounter = 0;
    public static float SpeedModifier(Player p) => p.PassiveAttackSpeedModifier; //temporarily use instance
    public override void PostEquipUpdate()
    {
        if(Player.LightSpear > 0)
        {
            float Damage = 2.0f + Player.LightSpear * 0.5f;
            Vector2 shootFromPos = (Vector2)transform.position + new Vector2(0, 0.6f).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad) * transform.lossyScale.x;
            float shotTime = DefaultShotSpeed;
            lightSpearCounter += Time.fixedDeltaTime * SpeedModifier(Player);
            while(lightSpearCounter > shotTime)
            {
                if(LaunchSpear(shootFromPos, out Vector2 norm, new(), Player, Player.LightChainReact, Damage: Damage))
                {
                    velocity -= norm;
                    lightSpearCounter -= shotTime;
                }
                else
                    lightSpearCounter -= shotTime * 0.1f;
            }
        }
    }
    public static bool LaunchSpear(Vector2 shootFromPos, out Vector2 norm, List<Enemy> ignore, Player player, int BounceNum = 0, float bonusRange = 0, float defaultRangeMult = 1f, float Damage = 2.0f)
    {
        float speedMod = SpeedModifier(player);
        float spearSpeed = 5 + speedMod * 0.015f; // this only matters for visuals as the spear is hitscan
        float spearRange = ((player.LightSpear + 3) * 2.25f) * defaultRangeMult + bonusRange; //starts at 3 * 3 = 9, +2.25 range per stack
        if (spearRange > MaxRange)
            spearRange = MaxRange;
        norm = Vector2.zero;
        Vector2 searchPosition = shootFromPos;
        bool hitTarget = false;
        int totalHits = Mathf.Max(1, 1 + player.Refraction);
        int enemiesFound = 0;
        for (int i = 0; i < totalHits; ++i)
        {
            Enemy target = Enemy.FindClosest(searchPosition, spearRange, out Vector2 norm2, ignore, true);
            if (target != null)
            {
                if (++enemiesFound == 1) //refraction targetting
                {
                    norm = norm2;
                    if (Damage < 0.5f)
                        return true;
                    Projectile.NewProjectile<LightSpear>(shootFromPos, norm * spearSpeed, Damage, player, target.transform.position.x, target.transform.position.y, BounceNum, -1);
                    searchPosition = target.transform.position;
                    spearRange = 7 + player.Refraction * 2; //Starts at 7 + 2 = 9, scales by + 2 per stack
                }
                ignore.Add(target);
                hitTarget = true;
            }
        }
        for(int i = 1; i < enemiesFound; i++) //Only activates if enemies found is >= 2
        {
            int index = ignore.Count - i;
            float percent = i / (float)(enemiesFound - 1);
            float radians = Mathf.PI * percent * 2f;
            Enemy target = ignore[index];
            Vector2 newNorm = (Vector2)target.transform.position - shootFromPos;
            Damage *= 0.8f;
            if (Damage < 0.5f)
                return hitTarget;
            Projectile.NewProjectile<LightSpear>(shootFromPos, newNorm.normalized * spearSpeed, Damage, player, target.transform.position.x, target.transform.position.y, BounceNum, radians);
        }
        return hitTarget;
    }
}
