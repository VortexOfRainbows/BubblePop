using System;
using System.Collections.Generic;
using UnityEngine;
public class Book : Weapon
{
    public int AllowedBalls => Player.AllowedThunderBalls;
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(0, 0);
        rotation = 0;
        scale = 1.5f;
    }
    public Sprite OpenBook => Resources.Load<Sprite>("Player/ThoughtBubble/BookOpen");
    public Sprite ClosedBook => Resources.Load<Sprite>("Player/ThoughtBubble/BookClosed");
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ThoughtBubbleUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<ResearchNotes>();
        powerPool.Add<Boomerang>();
        powerPool.Add<ZapRadius>();
        powerPool.Add<ResearchGrants>();
        powerPool.Add<BonusBatteries>();
        powerPool.Add<ThunderBubbles>();
        powerPool.Add<Electroluminescence>();
    }
    public override void Init()
    {
        Player.TotalBookBalls = 0;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Electrodynamics Grimoire").WithDescription("Thought Bubble's trusty tome, 'only used for self defense'");
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
        if (InClosingAnimation || !hasDoneSelectAnimation)
            return;
        if (AttackLeft < -AttackCooldown && AttackRight < 0)
        {
            if (!alternate)
            {
                AttackLeft = 80;
                //player.bonusBubbles = player.ShotgunPower;
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
    public static bool InClosingAnimation = false;
    public static float ClosingPercent = 0.0f;
    private bool WantsOpen = false;
    private bool Open = true;
    public float OpeningAnimationTimer = 0;
    public bool JustOpened = false;
    private bool hasDoneSelectAnimation = false;
    private void AttackUpdate()
    {
        //if(Input.GetKeyDown(KeyCode.V))
        //{
        //    DamagePower++;
        //    ShotgunPower++;
        //}

        Vector2 toMouse = Player.Control.MousePosition - (Vector2)p.gameObject.transform.position;
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

        WantsOpen = (AttackLeft >= 0 || AttackRight >= 0 || (!InClosingAnimation && (Player.Control.PrimaryAttackHold || Player.Control.SecondaryAttackHold) && !Main.MouseHoveringOverButton)) && hasDoneSelectAnimation;
        Vector2 awayFromWand = new Vector2(-0.05f, 0.2f * dir).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad);
        Vector2 shootSpot = (Vector2)transform.position + awayFromWand;
        if(WantsOpen == Open && hasDoneSelectAnimation)
        {
            float percent = 0f;
            if (AttackLeft >= 0)
            {
                percent = AttackLeft / 80f;
                bool canAttack = AttackLeft == 75 && Player.TotalBookBalls < AllowedBalls;
                if (canAttack)
                {
                    AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1f, 1f);

                    //for (int i = 0; i < 10; ++i)
                    //    ParticleManager.NewParticle(shootSpot, 1, Utils.RandCircle(5), 0.2f, 0.5f, 2, Color.blue);

                    if (!JustOpened)
                    {
                        Color c = Color.Lerp(Player.Body.PrimaryColor, Color.blue, 0.75f);
                        for (int i = 0; i < 20; ++i)
                        {
                            Vector2 circular = new Vector2(Utils.RandFloat(7, 8), 0).RotatedBy(i / 10f * Mathf.PI);
                            circular.y *= 0.5f;
                            circular = circular.RotatedBy(25 * dir * Mathf.Deg2Rad);
                            Vector2 pos = (Vector2)transform.position;
                            ParticleManager.NewParticle(pos, 0.5f, circular + awayFromWand * 5 + Player.RB.velocity, 0.05f, Utils.RandFloat(0.1f, 0.5f), 2, c * 0.6f);
                        }
                        //for (int i = 0; i < 20; ++i)
                        //{
                        //    Vector2 circular = new Vector2(Utils.RandFloat(7, 8), 0).RotatedBy(i / 10f * Mathf.PI);
                        //    circular.y *= 0.5f;
                        //    circular = circular.RotatedBy(25 * dir * Mathf.Deg2Rad);
                        //    Vector2 pos = (Vector2)transform.position;
                        //    ParticleManager.NewParticle(pos, 0.5f, circular + awayFromWand * 5 + player.rb.velocity, 0.05f, Utils.RandFloat(0.1f, 0.5f), 2, Color.gray * 0.5f);
                        //}
                    }
                    else
                        JustOpened = false;
                    ++Player.TotalBookBalls;
                    float speed = 3.3f + Player.FasterBulletSpeed * 0.33f;
                    Projectile.NewProjectile<ThunderBubble>(shootSpot, toMouse.normalized * speed + awayFromWand, 1);
                }
            }
            else if (AttackRight >= 0)
            {
                percent = AttackRight / 80f;
                bool canAttack = AttackRight == 75 && Player.TotalBookBalls < AllowedBalls;
                if (canAttack)
                {
                    AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1f, 1f);

                    //for (int i = 0; i < 10; ++i)
                    //    ParticleManager.NewParticle(shootSpot, 1, Utils.RandCircle(5), 0.2f, 0.5f, 2, Color.blue);
                    if (!JustOpened)
                    {
                        Color c = Color.Lerp(Player.Body.PrimaryColor, Color.blue, 0.75f);
                        for (int i = 0; i < 20; ++i)
                        {
                            Vector2 circular = new Vector2(Utils.RandFloat(7, 8), 0).RotatedBy(i / 10f * Mathf.PI);
                            circular.y *= 0.5f;
                            circular = circular.RotatedBy(25 * dir * Mathf.Deg2Rad);
                            Vector2 pos = (Vector2)transform.position;
                            ParticleManager.NewParticle(pos, 0.5f, circular + awayFromWand * 5 + Player.RB.velocity, 0.05f, Utils.RandFloat(0.1f, 0.5f), 2, c * 0.6f);
                        }
                    }
                    else
                        JustOpened = false;
                    ++Player.TotalBookBalls;
                    float speed = 12.0f + Player.FasterBulletSpeed * 1.2f;
                    Projectile.NewProjectile<ThunderBubble>(shootSpot, toMouse.normalized * speed + awayFromWand, 1);
                }
            }
            float sin = Mathf.Abs(MathF.Sin(percent * Mathf.PI));
            sin *= sin;
            bonusPDirOffset += 5 * dir * p.squash - 15 * sin * dir;
            targetScale = new Vector3(1 + sin * 0.1f, 1.1f - sin * 0.1f, 1);
            attemptedPosition.y += sin * 0.1f;
            AttackRight--;
            AttackLeft--;
        }
        else
        {
            if (Open)
                InClosingAnimation = true;
            OpeningAnimationTimer++;
            float timeToOpenClose = 80f * (0.6f + 0.4f * Player.AttackSpeedModifier);
            float dir2 = Open ? -1 : 1;
            float percent = ClosingPercent = OpeningAnimationTimer / timeToOpenClose;
            float iPer = 1 - percent;
            float sin = MathF.Sin(percent * MathF.PI) * dir2;
            targetScale = new Vector3(1 + percent * 0.1f * iPer * dir2, 1 + sin * 0.4f * dir2 - percent * 0.4f * iPer * dir2, 1);
            attemptedPosition.y += sin * (Open ? .5f : 1.5f);
            bonusPDirOffset += 52 * Mathf.Min(1, 2 * percent) * dir * p.squash;
            if (OpeningAnimationTimer >= timeToOpenClose)
            {
                for (int i = 0; i < 36; ++i)
                {
                    Vector2 circular = new Vector2(Utils.RandFloat(6, 12), 0).RotatedBy(i / 18f * Mathf.PI);
                    circular.y *= Open ? 0.8f : 0.5f;
                    circular = circular.RotatedBy(25 * dir * Mathf.Deg2Rad);
                    Vector2 pos = (Vector2)transform.position + (Open ? circular * 0.3f + awayFromWand * 1.5f: Vector2.zero );
                    ParticleManager.NewParticle(pos, 0.5f, dir2 * (circular + awayFromWand * 5) + Player.RB.velocity, 0.05f, Utils.RandFloat(0.1f, 0.5f), 2, Color.gray * 0.4f);
                }
                Open = WantsOpen;
                OpeningAnimationTimer = 0;
                InClosingAnimation = false;
                ClosingPercent = 0;
                if (!Open)
                {
                    AttackLeft = AttackRight = -1;
                    AudioManager.PlaySound(SoundID.BubblePop, transform.position, 1f, 0.9f);
                }
                else
                    JustOpened = true;
                Player.TotalBookBalls = 0;
                hasDoneSelectAnimation = true;
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
    protected override void DeathAnimation()
    {
        transform.position += (Vector3)velocity;
        float toBody = transform.localPosition.y - p.Body.transform.localPosition.y;
        if (p.DeathKillTimer <= 0)
        {
            Open = false;
            velocity *= 0.0f;
            velocity.y += !Open ? 0.1f : 0.05f;
            velocity.x += 0.01f * p.Direction;
        }
        else if (toBody < -0.25f && p.DeathKillTimer > 10)
        {
            spriteRender.sprite = OpenBook;
            velocity *= 0.1f;
            transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.transform.eulerAngles.z, spriteRender.flipY ? 200 : -20, 0.05f));
        }
        else
        {
            velocity.x *= 0.96f;
            velocity.y -= 0.005f;
        }
        AttackLeft = 0;
        AttackRight = 0;
        transform.LerpLocalScale(Vector2.one, 0.1f);
    }
}