using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
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
    public override bool IsAttacking()
    {
        return base.IsAttacking() || Open != WantsOpen;
    }
    public override bool IsPrimaryAttacking()
    {
        return (base.IsPrimaryAttacking() || Open != WantsOpen) && !IsSecondaryAttacking();
    }
    public override bool IsSecondaryAttacking()
    {
        return base.IsSecondaryAttacking();
    }
    private float AttackCooldown => 0;
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft < -AttackCooldown && AttackRight < 0)
        {
            if (!alternate)
            {
                AttackLeft = 80;
                player.bonusBubbles = player.ShotgunPower;
            }
        }
        if (AttackRight < -AttackCooldown && AttackLeft < 0)
        {
            if (alternate)
            {
                AttackRight = 80;
            }
        }
    }
    public bool WantsOpen = false;
    public bool Open = false;
    public float OpeningAnimationTimer = 0;
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
        Vector2 attemptedPosition = new Vector2(1.1f, -0.15f * dir).RotatedBy(toMouse.ToRotation()) + p.rb.velocity.normalized * 0.1f;
        attemptedPosition.y *= 0.6f;
        attemptedPosition.y -= 0.15f;
        attemptedPosition.x += 0.75f * dir;
        //Debug.Log(attemptedPosition.ToRotation() * Mathf.Rad2Deg);

        Vector3 targetScale = Vector3.one;
        float bonusPDirOffset = 0;
        p.PointDirOffset = -20f * dir * p.squash;
        p.MoveOffset = -5 * bodyDir * p.squash;
        p.DashOffset = 20 * dir * (1 - p.squash);

        WantsOpen = AttackLeft >= 0 || AttackRight >= 0;
        Vector2 awayFromWand = new Vector2(-0.05f, 0.2f * dir).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad);
        Vector2 shootSpot = (Vector2)transform.position + awayFromWand;
        if(WantsOpen == Open)
        {
            if (AttackLeft >= 0)
            {
                float percent = AttackLeft / 80f;
                float sin = Mathf.Abs(MathF.Sin(percent * Mathf.PI));
                sin *= sin;
                bool canAttack = AttackLeft == 75;
                if (canAttack)
                {
                    AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1f, 1f);

                    for (int i = 0; i < 10; ++i)
                        ParticleManager.NewParticle(shootSpot, 1, Utils.RandCircle(5), 0.2f, 0.5f, 2, Color.blue);

                    float speed = 24;
                    Projectile.NewProjectile<SmallBubble>(shootSpot, toMouse.normalized * speed + awayFromWand);
                }
                bonusPDirOffset += 5 * dir * p.squash - 15 * sin * dir;
                targetScale = new Vector3(1 + sin * 0.1f, 1 - sin * 0.2f, 1);
                attemptedPosition.y += sin * 0.1f;
            }
            AttackRight--;
            AttackLeft--;
        }
        else
        {
            OpeningAnimationTimer++;
            float dir2 = Open ? -1 : 1;
            float percent = OpeningAnimationTimer / 80f;
            float iPer = 1 - percent;
            float sin = MathF.Sin(percent * MathF.PI) * dir2;
            targetScale = new Vector3(1 + percent * 0.1f * iPer * dir2, 1 + sin * 0.4f * dir2 - percent * 0.4f * iPer * dir2, 1);
            attemptedPosition.y += sin * (Open ? .5f : 1.5f);
            bonusPDirOffset += 52 * Mathf.Min(1, 2 * percent) * dir * p.squash;
            if (OpeningAnimationTimer >= 80)
            {
                for (int i = 0; i < 36; ++i)
                {
                    Vector2 circular = new Vector2(Utils.RandFloat(6, 12), 0).RotatedBy(i / 18f * Mathf.PI);
                    circular.y *= Open ? 0.8f : 0.5f;
                    circular = circular.RotatedBy(25 * dir * Mathf.Deg2Rad);
                    Vector2 pos = (Vector2)transform.position + (Open ? circular * 0.3f + awayFromWand * 1.5f: Vector2.zero );
                    ParticleManager.NewParticle(pos, 0.5f, dir2 * (circular + awayFromWand * 5) + player.rb.velocity, 0.05f, Utils.RandFloat(0.1f, 0.5f), 2, Color.gray * 0.4f);
                }
                Open = WantsOpen;
                OpeningAnimationTimer = 0;
            }
        }
        spriteRender.sprite = Open ? OpenBook : ClosedBook;

        float r = (new Vector2(attemptedPosition.x, attemptedPosition.y * 0.5f).ToRotation() * Mathf.Rad2Deg - bonusPDirOffset - p.MoveOffset + p.DashOffset) - p.PointDirOffset;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition, 0.08f);
        float lerpAmt = spriteRender.flipY != p.PointDirOffset > 0 ? 1 : 0.125f;
        spriteRender.flipY = p.PointDirOffset > 0;
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, lerpAmt);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        transform.LerpLocalScale(targetScale, 0.5f);
    }
}