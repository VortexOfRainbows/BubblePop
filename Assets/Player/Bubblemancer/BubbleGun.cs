using System.Collections.Generic;
using UnityEngine;
public class BubbleGun : BubblemancerWand
{
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        base.ModifyPowerPool(powerPool);
        powerPool.Add<WeaponUpgrade>();
    }
    protected override string Name()
    {
        return "Bubble Gun";
    }
    protected override string Description()
    {
        return "The Right to Bubble shall not be infringed";
    }
    protected override void AnimationUpdate()
    {
        GunUpdate();
    }
    private float AttackCooldownLeft => 10f / Player.Instance.AttackSpeedModifier;
    private float AttackCooldownRight => 20f / Player.Instance.AttackSpeedModifier;
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft < -AttackCooldownLeft && AttackRight < 0)
        {
            if (!alternate)
            {
                AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1f, 1f);
                AttackLeft = 10;
                p.bonusBubbles += 5 + p.ShotgunPower;
            }
        }
        if (AttackRight < -AttackCooldownRight && AttackLeft < 0)
        {
            if (alternate)
            {
                AttackRight = 50;
            }
        }
    }
    private void TryDoingStarShot(ref int starshotNum)
    {
        if (starshotNum <= 0)
            return;
        float chance = 0.1f + 0.05f * starshotNum;
        if (Utils.RandFloat(1f) < chance)
        {
            Vector2 toMouse = Utils.MouseWorld - (Vector2)p.gameObject.transform.position;
            Vector2 awayFromWand = new Vector2(1, 0).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad);
            float spread = Mathf.Max(90 - p.FasterBulletSpeed * 5, 0);
            float speed = Utils.RandFloat(16, 19) + 1.6f * p.FasterBulletSpeed;
            Vector2 velocity = toMouse.normalized * speed + awayFromWand * 4;
            Vector2 norm = velocity.normalized * (12 + p.FasterBulletSpeed * 0.5f) + Utils.RandCircle(4) * (10f / (10f + p.FasterBulletSpeed));
            Projectile.LegacyNewProjectile((Vector2)transform.position + awayFromWand * 2, velocity.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad), type: 4, transform.position.x + norm.x, transform.position.y + norm.y);

            float chanceOfLosingAStarProc = 0.6f * (10f / (9f + p.ShotgunPower)); //0.45 chance when you have one extra power, .409f when you have another, etc
            if (Utils.RandFloat(1) < chanceOfLosingAStarProc)
                --starshotNum;
        }
    }
    private void GunUpdate()
    {
        //if(Input.GetKeyDown(KeyCode.V))
        //{
        //    DamagePower++;
        //    ShotgunPower++;
        //}

        Vector2 awayFromWand = new Vector2(2, 0).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad);
        Vector2 toMouse = Utils.MouseWorld - (Vector2)p.gameObject.transform.position - awayFromWand;
        float dir = Mathf.Sign(toMouse.x);
        float bodyDir = Mathf.Sign(p.rb.velocity.x);
        Vector2 attemptedPosition = new Vector2(0.8f, 0.2f * dir).RotatedBy(toMouse.ToRotation()) + p.rb.velocity.normalized * 0.1f;

        //Debug.Log(attemptedPosition.ToRotation() * Mathf.Rad2Deg);

        p.PointDirOffset = 2 * dir * p.squash;
        p.MoveOffset = -5 * bodyDir * p.squash;
        p.DashOffset = 100 * dir * (1 - p.squash);

        if (AttackLeft > 0)
        {
            bool canAttack = AttackLeft == 10;
            bool bonusBubble = p.bonusBubbles > 0 && AttackLeft >= 41;
            if (!canAttack && bonusBubble)
            {
                canAttack = true;
                p.bonusBubbles--;
            }
            if (canAttack)
            {
                int starshotNum = p.Starshot;
                int shotCount = p.bonusBubbles / 5;
                for(int i = 0; i < shotCount; ++i)
                {
                    float speed = Utils.RandFloat(12, 15) + 1.75f * p.FasterBulletSpeed;
                    float spread = 15 * (i - (shotCount - 1) * 0.5f);
                    Projectile.LegacyNewProjectile((Vector2)transform.position + awayFromWand,
                        toMouse.normalized.RotatedBy(spread * Mathf.Deg2Rad)
                        * speed + new Vector2(Utils.RandFloat(-0.7f, 0.7f), Utils.RandFloat(-0.7f, 0.7f)));
                    TryDoingStarShot(ref starshotNum);
                }
                p.bonusBubbles %= 5;
            }
            float percent = AttackLeft / 50f;
            p.PointDirOffset += 165 * percent * dir * p.squash;
        }
        if (AttackRight > 0)
        {
            if ((Input.GetMouseButton(1) || AttackRight < 100) && AttackRight >= 50)
            {
                int maxCharge = 250 + 100 * p.Coalescence;
                if (AttackRight == 50)
                {
                    AudioManager.PlaySound(SoundID.ChargeWindup, Player.Position, 0.3f, 1.5f);
                    AudioManager.PlaySound(SoundID.ChargePoint.GetVariation(0), Player.Position, 0.6f, 1f);
                    Projectile.LegacyNewProjectile((Vector2)transform.position + awayFromWand, Vector2.zero, 3, 149, 0);
                }
                if (AttackRight < maxCharge)
                {
                    AttackRight++;
                    if (AttackRight == 150)
                    {
                        AudioManager.PlaySound(SoundID.ChargePoint.GetVariation(1), Player.Position, 0.65f, 1f);
                    }
                    if (AttackRight == 250)
                    {
                        AudioManager.PlaySound(SoundID.ChargePoint.GetVariation(2), Player.Position, 0.7f, 1f);
                    }
                    if(AttackRight > 250 && (AttackRight - 50) % 100 == 0)
                    {
                        float scale = (AttackRight - 250) / 100;
                        AudioManager.PlaySound(SoundID.ChargePoint.GetVariation((int)scale % 3), Player.Position, 0.7f + Mathf.Sqrt(scale) * 0.1f, 1f);
                    }
                }
                p.PointDirOffset += -Mathf.Min(45f, (AttackRight - 50f) / 200f * 45f) * dir * p.squash;
            }
            else
            {
                if (AttackRight > 50)
                    AttackRight = 50;
                AttackRight--;
                float percent = AttackRight / 50f;
                p.PointDirOffset += 125 * percent * dir * p.squash;
            }
        }
        else
            AttackRight--;

        //Final Stuff
        float r = attemptedPosition.ToRotation() * Mathf.Rad2Deg - p.PointDirOffset - p.MoveOffset + p.DashOffset;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition, 0.08f);
        spriteRender.flipY = dir < 0;
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.15f);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        AttackLeft--;
    }
}
