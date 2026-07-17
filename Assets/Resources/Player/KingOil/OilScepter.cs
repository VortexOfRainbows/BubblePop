using System.Collections.Generic;
using UnityEngine;

public class OilScepter : Weapon
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        rotation -= 45f;
        offset.y -= 0.375f;
        offset.x -= 0.375f;
        scale *= 1.675f;
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
    protected virtual float RightAttackSpeed => 50;
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
    public Vector2 recoil = Vector2.zero;
    public float bonusPointDirOffset = 0;
    protected virtual int AttackRate => 10;
    protected virtual float SpreadDegrees => 180;
    private void WandUpdate()
    {
        Vector2 toMouse = Player.Control.MousePosition - (Vector2)p.transform.position;
        float dir = Mathf.Sign(toMouse.x);
        float bodyDir = Mathf.Sign(p.rb.velocity.x);
        Vector2 attemptedPosition = new Vector2(1.5f, 0).RotatedBy(toMouse.ToRotation());
        Vector2 clampedMousePos = toMouse.magnitude < 3 ? (Vector2)p.transform.position + toMouse.normalized * 3 : Player.Control.MousePosition;

        p.PointDirOffset = 0;
        p.MoveOffset = 0;
        p.DashOffset = 100 * dir * (1 - p.Squash);

        Vector2 awayFromWand = attemptedPosition;
        bonusPointDirOffset = 0;
        if (AttackLeft > 0)
        {
            bool canAttack = AttackLeft == AttackRate;
            if (canAttack)
            {
                AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1f, 1.2f);
                Vector2 randomAddition = awayFromWand * Utils.RandFloat(2, 4) + Utils.RandCircle(2f);
                float speed = Utils.RandFloat(15, 16);
                float spread = 5;
                Vector2 shotDirection = attemptedPosition.normalized.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad) * speed + randomAddition;
                Projectile.NewProjectile<SmallBubble>(Player.Position + awayFromWand * 2, shotDirection, 1, Player);
                recoil -= awayFromWand * .1f;
            }
            //float percent = AttackLeft / (50f + Player.ShotgunPower * 10f);
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
        float direction = -Utils.SignNoZero(attemptedPosition.x);
        Vector2 tiltAugment = toMouse.normalized;
        tiltAugment.x = Mathf.Abs(tiltAugment.x) + 2;
        tiltAugment.y *= -direction;
        float tiltRotation = tiltAugment.ToRotation() * Mathf.Rad2Deg;
        float r = tiltRotation + p.PointDirOffset - p.MoveOffset + p.DashOffset;
        attemptedPosition.y -= 0.75f;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition + recoil, 0.25f);
        if(Utils.SignNoZero(transform.localScale.x) != direction)
            transform.localScale = new Vector3(direction, 1, 1);
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.18f);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        AttackLeft--;
        recoil *= 0.925f;
    }
}
