using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Book : Weapon
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(0.05f, 0);
        rotation = 45;
        scale = 1.4f;
    }
    public Sprite OpenBook => Resources.Load<Sprite>("Player/ThoughtBubble/BookOpen");
    public Sprite ClosedBook => Resources.Load<Sprite>("Player/ThoughtBubble/BookClosed");
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ThoughtBubbleUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        //powerPool.Add<EternalBubbles>();
        //powerPool.Add<Shotgun>();
        //powerPool.Add<ChargeShot>();
        //powerPool.Add<BubbleBlast>();
        //powerPool.Add<SoapySoap>();
        //powerPool.Add<ShotSpeed>();
        //powerPool.Add<Starshot>();
    }
    protected override string Name()
    {
        return "Guide to Bubble Physics";
    }
    protected override string Description()
    {
        return "A grimoire detailing the wonderful world of bubble sorcery";
    }
    protected override void AnimationUpdate()
    {
        AttackUpdate();
    }
    private float AttackCooldown => 0;
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft < -AttackCooldown && AttackRight < 0)
        {
            if (!alternate)
            {
                AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1f, 1f);
                AttackLeft = 50;
                player.bonusBubbles = player.ShotgunPower;
            }
        }
        if (AttackRight < -AttackCooldown && AttackLeft < 0)
        {
            if (alternate)
            {
                AttackRight = 50;
            }
        }
    }
    private void AttackUpdate()
    {
        //if(Input.GetKeyDown(KeyCode.V))
        //{
        //    DamagePower++;
        //    ShotgunPower++;
        //}

        Vector2 toMouse = Utils.MouseWorld - (Vector2)p.gameObject.transform.position;
        float dir = Mathf.Sign(toMouse.x);
        float bodyDir = Mathf.Sign(p.rb.velocity.x);
        Vector2 attemptedPosition = new Vector2(0.5f, -0.1f * dir).RotatedBy(toMouse.ToRotation()) + p.rb.velocity.normalized * 0.1f;
        attemptedPosition.y *= 0.5f;
        attemptedPosition.y -= 0.1f;
        attemptedPosition.x += 1.5f * dir;
        //Debug.Log(attemptedPosition.ToRotation() * Mathf.Rad2Deg);

        float bonusPDirOffset = 0;
        p.PointDirOffset = -20f * dir * p.squash;
        p.MoveOffset = -5 * bodyDir * p.squash;
        p.DashOffset = 20 * dir * (1 - p.squash);

        Vector2 awayFromWand = new Vector2(1, 0).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad);
        if (AttackLeft > 0)
        {
            bool canAttack = AttackLeft % 2 == 1 && AttackLeft >= 41;
            bool bonusBubble = player.bonusBubbles > 0 && AttackLeft >= 41;
            if (!canAttack && bonusBubble)
            {
                canAttack = true;
                player.bonusBubbles--;
            }
            if (canAttack)
            {
                int starshotNum = player.Starshot;
                float speed = Utils.RandFloat(9, 15) + 1.5f * player.FasterBulletSpeed;
                float spread = 12 + Mathf.Sqrt(player.ShotgunPower) * (10f / (10f + player.FasterBulletSpeed));
                Projectile.NewProjectile<SmallBubble>((Vector2)transform.position + awayFromWand * 2,
                    toMouse.normalized.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad)
                    * speed + awayFromWand * Utils.RandFloat(2, 4) + new Vector2(Utils.RandFloat(-0.7f, 0.7f), Utils.RandFloat(-0.7f, 0.7f)));
                float odds = Mathf.Sqrt(1f / (AttackLeft - 40f));
                int attempts = player.bonusBubbles;
                while (attempts >= AttackLeft - 40)
                {
                    if (Utils.RandFloat(1) <= odds)
                    {
                        speed = Utils.RandFloat(9, 15) + 1.5f * player.FasterBulletSpeed;
                        Projectile.NewProjectile<SmallBubble>((Vector2)transform.position + awayFromWand * 2,
                            toMouse.normalized.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad)
                            * speed + awayFromWand * Utils.RandFloat(2, 4) + new Vector2(Utils.RandFloat(-0.7f, 0.7f), Utils.RandFloat(-0.7f, 0.7f)));
                        player.bonusBubbles--;
                    }
                    attempts--;
                }
            }
            float percent = AttackLeft / 50f;
            bonusPDirOffset += 45 * percent * dir * p.squash;
        }
        if (AttackRight > 0)
        {
            AttackRight--;
        }
        else
            AttackRight--;
        if(AttackLeft < 0 && AttackRight < 0)
        {
            spriteRender.sprite = ClosedBook;
        }
        else
            spriteRender.sprite = OpenBook;

        float r = (attemptedPosition.ToRotation() * Mathf.Rad2Deg - bonusPDirOffset - p.MoveOffset + p.DashOffset) - p.PointDirOffset;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition, 0.08f);
        float lerpAmt = spriteRender.flipY != p.PointDirOffset > 0 ? 1 : 0.125f;
        spriteRender.flipY = p.PointDirOffset > 0;
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, lerpAmt);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        AttackLeft--;
    }
}