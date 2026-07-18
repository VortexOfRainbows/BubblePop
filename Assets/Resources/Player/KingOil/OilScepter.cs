using System.Collections.Generic;
using UnityEngine;

public class OilScepter : Weapon
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        rotation -= 45f;
        offset.y -= 0.4125f;
        offset.x -= 0.4125f;
        scale *= 1.6f;
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<KingOilUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<WeaponUpgrade>();
        powerPool.Add<WeaponUpgrade>();
        powerPool.Add<WeaponUpgrade>();
        powerPool.Add<WeaponUpgrade>();
        powerPool.Add<WeaponUpgrade>();
        powerPool.Add<WeaponUpgrade>();
        powerPool.Add<WeaponUpgrade>();
    }
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Primary, Ability.ID.Secondary);
    }
    protected override void AnimationUpdate()
    {
        WandUpdate();
    }
    protected override void DeathAnimation()
    {

    }
    public override void Init()
    {

    }
    protected virtual float AttackCooldown => 10;
    protected virtual int AttackRate => 45;
    protected virtual float RightAttackSpeed => 50;
    protected virtual float SpreadDegrees => 10;
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft <= 0 && AttackRight < -AttackCooldown)
        {
            if (!alternate)
            {
                //AudioManager.PlaySound(SoundID.BathBombSizzle, transform.position, 1, 2);
                AttackLeft = AttackRate;
            }
        }
        if (AttackRight < -AttackCooldown && AttackLeft < 0)
        {
            if (alternate)
            {
                AttackRight = RightAttackSpeed;
            }
        }
    }
    public SpecialTrail Trail { get; private set; }
    public Transform Gem;
    public Vector2 recoil = Vector2.zero;
    public float bonusPointDirOffset = 0;
    public int WandCounter = 0;
    private void WandUpdate()
    {
        Vector2 toMouse = Player.Control.MousePosition - (Vector2)p.transform.position;
        float dir = Mathf.Sign(toMouse.x);
        float bodyDir = Mathf.Sign(p.rb.velocity.x);
        Vector2 attemptedPosition = new Vector2(1.5f, 0).RotatedBy(toMouse.ToRotation());
        Vector2 clampedMousePos = toMouse.magnitude < 3 ? (Vector2)p.transform.position + toMouse.normalized * 3 : Player.Control.MousePosition;

        p.DashOffset = 100 * dir * (1 - p.Squash);

        Vector2 awayFromWand = attemptedPosition.normalized;
        float attackPercent = 1 - AttackLeft / AttackRate;
        if (AttackLeft > 0)
        {
            ++WandCounter;
            bonusPointDirOffset = Mathf.Lerp(bonusPointDirOffset, 90, 0.25f);
            bool canAttack = AttackLeft <= 1;

            //ParticleManager.NewParticle((Vector2)Gem.position + awayFromWand + circular, new Vector2(1, 1), Vector2.zero, 0, 0.5f, ParticleManager.ID.LineForeground, Color.red, 0);

            if (canAttack)
            {
                AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1f, 1.2f);
                Vector2 randomAddition = awayFromWand * Utils.RandFloat(2, 4) + Utils.RandCircle(2f);
                float speed = Utils.RandFloat(15, 16);
                float spread = SpreadDegrees;
                Vector2 shotDirection = attemptedPosition.normalized.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad) * speed + randomAddition;
                Projectile.NewProjectile<SmallBubble>(Player.Position + awayFromWand * 2, shotDirection, 1, Player);
                recoil -= awayFromWand * .1f;
            }
            //float percent = AttackLeft / (50f + Player.ShotgunPower * 10f);
        }
        else
        {
            bonusPointDirOffset = Mathf.Lerp(bonusPointDirOffset, 0, 0.2f);
        }
        if (AttackRight > 0)
        {
            AttackRight--;
        }
        else
        {
            AttackRight--;
        }
        //Final Stuff
        float leftClickPercent = bonusPointDirOffset / 90f;
        float pointPercent = 1 - leftClickPercent;
        float direction = -Utils.SignNoZero(attemptedPosition.x);
        Vector2 tiltAugment = toMouse.normalized;
        tiltAugment.x = Mathf.Abs(tiltAugment.x) + 2 * pointPercent;
        tiltAugment.y *= -direction;
        float tiltRotation = tiltAugment.ToRotation() * Mathf.Rad2Deg;
        float r = tiltRotation + p.DashOffset + bonusPointDirOffset * direction;
        attemptedPosition.y -= 0.75f * pointPercent;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition + recoil, 0.25f);
        if (Utils.SignNoZero(transform.localScale.x) != direction)
            transform.localScale = new Vector3(direction, 1, 1);
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.18f);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        AttackLeft--;
        recoil *= 0.925f;

        if(AttackLeft > 0)
        {
            for (int i = 1; i <= 3; ++i)
            {
                Vector2 circular = new Vector2(1 - attackPercent, 0).RotatedBy(WandCounter / (float)AttackRate * Utils.TwoPI / 2f + i * Utils.TwoPI / 3f);
                circular.x *= 0.5f;
                circular = circular.RotatedBy(awayFromWand.ToRotation());
                circular += (Vector2)Gem.transform.position;
                circular += awayFromWand;
                //ParticleManager.NewParticle(circular, 2f, Vector2.zero + Player.RB.velocity * 0.75f, 0.1f, 0.4f, ParticleManager.ID.Pixel, Color.red);
                //Trails[i].transform.position = circular;
            }
        }
        
        if(Trail == null)
        {
            Trail = SpecialTrail.NewTrail(Gem, Color.red.WithAlpha(0.4f), .5f, .4f, 0.2f, manuallyUpdated: false, orderInLayer: 3);
            Trail.Trail.minVertexDistance *= 10;
        }
    }
}
