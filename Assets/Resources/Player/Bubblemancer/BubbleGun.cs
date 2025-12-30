using System.Collections.Generic;
using UnityEngine;
public class BubbleGun : BubblemancerWand
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ShotSpeed10>();
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        offset.x -= 0.05f;
        offset.y -= 0.05f;
        scale *= 1.025f;
    }
    public SpriteRenderer Upper;
    public SpriteRenderer Lower;
    private float spinSpeed = 0;
    private float spin = 0;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        base.ModifyPowerPool(powerPool);
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubble Gun").WithDescription("The right to bare bubble shall not be infringed");
    }
    public override void EquipUpdate()
    {
        player.PrimaryAttackSpeedModifier += 0.2f;
    }
    protected override void AnimationUpdate()
    {
        GunUpdate();
    }
    private float AttackCooldownLeft => Mathf.Max(0, 14f - (Player.Instance.PrimaryAttackSpeedModifier - 1) * 2f);
    private float AttackCooldownRight => Mathf.Max(0, 20f - (Player.Instance.SecondaryAttackSpeedModifier - 1) * 20f);
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft < -AttackCooldownLeft && AttackRight < 0)
        {
            if (!alternate)
            {
                AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1f, 1f);
                AttackLeft = 10;
                player.bonusBubbles += 5 + player.ShotgunPower;
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
        float chance = 0.1f + 0.1f * starshotNum;
        if (Utils.RandFloat(1f) < chance)
        {
            Vector2 toMouse = Utils.MouseWorld - (Vector2)p.gameObject.transform.position;
            Vector2 awayFromWand = new Vector2(1, 0).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad);
            float spread = Mathf.Max(60 - player.FasterBulletSpeed * 4, 0);
            float speed = Utils.RandFloat(16, 17) + 2.4f * player.FasterBulletSpeed;
            Vector2 velocity = toMouse.normalized * speed + awayFromWand * 4;
            Vector2 norm = velocity.normalized * (12 + player.FasterBulletSpeed * 0.5f) + Utils.RandCircle(4) * (10f / (10f + player.FasterBulletSpeed));
            Projectile.NewProjectile<StarProj>((Vector2)transform.position + awayFromWand * 2, velocity.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad), 2, transform.position.x + norm.x, transform.position.y + norm.y, Utils.RandInt(2) * 2 - 1);
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

        Vector2 playerToMouse = Utils.MouseWorld - (Vector2)p.transform.position;
        Vector2 mouseAdjustedFromPlayer = playerToMouse.magnitude < 4 ? playerToMouse.normalized * 4 + (Vector2)p.transform.position : Utils.MouseWorld;
        float dir = Mathf.Sign(playerToMouse.x);
        Vector2 awayFromWand = new Vector2(2, 0.1f * dir).RotatedBy(playerToMouse.ToRotation());
        Vector2 toMouse = mouseAdjustedFromPlayer - (Vector2)transform.position - awayFromWand;
        float bodyDir = Mathf.Sign(p.rb.velocity.x);
        Vector2 attemptedPosition = playerToMouse.normalized * 1.1f + p.rb.velocity.normalized * 0.1f;

        //Debug.Log(attemptedPosition.ToRotation() * Mathf.Rad2Deg);

        p.PointDirOffset = 2 * dir * p.squash;
        p.MoveOffset = -0 * bodyDir * p.squash;
        p.DashOffset = 100 * dir * (1 - p.squash);

        if (AttackLeft > 0)
        {
            bool canAttack = AttackLeft == 10;
            bool bonusBubble = player.bonusBubbles > 0 && AttackLeft >= 41;
            if (!canAttack && bonusBubble)
            {
                canAttack = true;
                player.bonusBubbles--;
            }
            if (canAttack)
            {
                int starshotNum = player.Starshot;
                int shotCount = player.bonusBubbles / 5;
                float spreadAmt = (25f + shotCount * 0.5f) / shotCount;
                for(int i = 0; i < shotCount; ++i)
                {
                    float speed = Utils.RandFloat(25f, 27f);
                    float spread = spreadAmt * (i - (shotCount - 1) * 0.5f);
                    Projectile.NewProjectile<SmallBubble>((Vector2)transform.position + awayFromWand,
                        toMouse.normalized.RotatedBy(spread * Mathf.Deg2Rad)
                        * speed + Utils.RandCircle(0.15f), 1);
                    TryDoingStarShot(ref starshotNum);
                }
                player.bonusBubbles %= 5;
            }
            float percent = AttackLeft / 10f;
            p.PointDirOffset -= 20 * percent * dir * p.squash;
        }
        if (AttackRight > 0)
        {
            if ((Input.GetMouseButton(1) || AttackRight < 100) && AttackRight >= 50)
            {
                int maxCharge = 250 + 100 * player.OldCoalescence;
                if (AttackRight == 50)
                {
                    AudioManager.PlaySound(SoundID.ChargeWindup, Player.Position, 0.3f, 1.5f);
                    AudioManager.PlaySound(SoundID.ChargePoint.GetVariation(0), Player.Position, 0.6f, 1f);
                    Projectile.NewProjectile<BigBubble>((Vector2)transform.position + awayFromWand, Vector2.zero, 1, 149, 0);
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
                        AudioManager.PlaySound(SoundID.ChargePoint.GetVariation(2), Player.Position, 0.7f, 1f + Mathf.Sqrt(scale) * 0.05f);
                    }
                }
            }
            else
            {
                if (AttackRight > 50)
                    AttackRight = 50;
                AttackRight--;
                float percent = AttackRight / 50f;
                p.PointDirOffset -= 40 * percent * dir * p.squash;
            }
        }
        else
            AttackRight--;

        //Final Stuff
        float r = attemptedPosition.ToRotation() * Mathf.Rad2Deg - p.PointDirOffset - p.MoveOffset + p.DashOffset;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition, 0.08f);
        spriteRender.flipY = Upper.flipY = Lower.flipY = dir < 0;
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.15f);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        AttackLeft--;

        bool notAttacking = false;
        if (-AttackCooldownLeft <= AttackLeft || -AttackCooldownRight <= AttackRight || Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            spinSpeed += 0.275f;
        }
        else
            notAttacking = true;
        float cos = 1.1f * Mathf.Abs(Mathf.Cos(spin * Mathf.Deg2Rad));
        float angleScale = Mathf.Min(1.0f, cos);
        if(notAttacking)
        {
            if(cos - 1.0f > 0.01f)
            {
                spinSpeed += 0.3f;
            }
            else
            {
                spinSpeed *= 0.9f;
                spin = Mathf.LerpAngle(spin, 0, 0.05f);
            }
        }

        Lower.transform.localScale = new Vector3(1, Mathf.Lerp(Lower.transform.localScale.y, angleScale, 0.25f), 1);
        spinSpeed *= 0.96f;
        spin += spinSpeed;
        spin %= 360;
    }
    protected override void DeathAnimation()
    {
        AttackLeft = 0;
        AttackRight = 0;
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, -0.5f), 0.1f);
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.transform.eulerAngles.z,
             spriteRender.flipY ? 190 : - 10, 0.1f));
    }
    public override int GetRarity()
    {
        return 3;
    }
}
