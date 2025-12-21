using System;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SlotMachineWeapon : Weapon
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<GachaponUnlock>();
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset.x -= 0.42f;
        offset.y -= 0.6f;
        scale *= 0.94f;
        rotation += 25;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        //base.ModifyPowerPool(powerPool);
        powerPool.Add<Pity>();
        powerPool.Add<ConsolationPrize>();
        //powerPool.Add<ChargeShot>();
        //powerPool.Add<BubbleBlast>();
        //powerPool.Add<SoapySoap>();
        //powerPool.Add<ShotSpeed>();
        //powerPool.Add<Starshot>();
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gacha Slots").WithDescription("99% of bubbles pop before hitting it big");
    }
    public override void EquipUpdate()
    {
        if(runOnce)
        {
            runOnce = false;
            SwapSlotSprite(GambleSlots[0], 0, false);
            SwapSlotSprite(GambleSlots[1], 0, false);
            SwapSlotSprite(GambleSlots[2], 0, false);
        }
    }
    public override bool IsPrimaryAttacking()
    {
        return base.IsPrimaryAttacking() || GambleAttackFrames > 0;
    }
    public MeleeHitbox Hitbox { get; set; }
    public SpecialTrail Trail { get; set; }
    public Transform LeverArm;
    public Transform Coin, GeoCenter;
    public float AttackGamble = 0;
    public int GambleOutcome = 1;
    public Vector3 previousAttemptedPosition = Vector3.zero;
    float dir = 1;
    private bool runOnce = true;
    private bool hasDoneSelectAnimation = false;
    private float PityBonus = 0.0f;
    private int PityCount = 0;
    protected override void AnimationUpdate()
    {
        Vector2 playerToMouse = Utils.MouseWorld - (Vector2)p.transform.position;
        Vector2 mouseAdjustedFromPlayer = playerToMouse.magnitude < 4 ? playerToMouse.normalized * 4 + (Vector2)p.transform.position : Utils.MouseWorld;
        float scaleUp = 0.95f;
        if (!IsSecondaryAttacking() || AttackRight > WindUpTime + 20)
            dir = Mathf.Sign(playerToMouse.x);
        Vector2 awayFromWand = new Vector2(1.5f, 0.25f * dir).RotatedBy(playerToMouse.ToRotation());
        Vector2 toMouse = mouseAdjustedFromPlayer - (Vector2)transform.position - awayFromWand;
        Vector2 norm = toMouse.normalized;
        float bodyDir = Mathf.Sign(p.rb.velocity.x);

        Vector2 attemptedPosition = previousAttemptedPosition;
        if (!IsSecondaryAttacking() || AttackRight > WindUpTime + 20)
        {
            attemptedPosition = playerToMouse.normalized * 1.05f + p.rb.velocity.normalized * 0.1f;
            attemptedPosition.y *= 0.95f;
        }
        Vector2 originalPosition = attemptedPosition;
        previousAttemptedPosition = attemptedPosition;
        p.PointDirOffset = 2 * dir * p.squash;
        p.DashOffset = 100 * dir * (1 - p.squash);
        float armDefaultRotation = 0;
        Vector2 armDefaultPosition = new Vector2(0, 0.16f);

        if (AttackLeft > 0 || AttackGamble > 0)
        {
            if (AttackGamble > 0)
            {
                AttackGamble--;
                AttackLeft = 0;
                if (AttackGamble >= GambleAttackFrames) //Spin Slots animation
                {
                    if(FakeAttack)
                        AttackGamble = GambleAnimationFrames;
                    else
                        GambleAnimation();
                }
                else //Actual attack portion
                {
                    if(!FakeAttack)
                    {
                        if (AttackGamble == GambleAttackFrames - 1)
                        {
                            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 2.4f, 1.5f - 0.25f * GambleOutcome, 2);
                            string text = GambleText[GambleOutcome - 1];
                            if(PityCount != 0)
                            {
                                text = $"({PityCount}) {text}";
                            }
                            PopupText.NewPopupText(Player.Position + new Vector2(0, 2), Vector3.up * 5f, ColorHelper.RarityColors[GambleOutcome - 1], text, true, 0.6f + GambleOutcome * 0.06f, 130);
                        }
                        if ((AttackGamble == 50 || AttackGamble == 30 || AttackGamble == 10) && hasDoneSelectAnimation)
                        {
                            velocity -= norm * 1f;
                            int num = GambleOutcome == 5 ? 3 : 1;
                            int type = GambleOutcome != 5 ? GambleOutcome - 1 : AttackGamble == 50 ? 3 : AttackGamble == 30 ? 2 : 1;
                            float separation = GambleOutcome == 5 ? 4.5f : 9f;
                            for (int i = -num; i <= num; i += 1)
                            {
                                if(num == 1 && i != 0)
                                    Projectile.NewProjectile<SmallBubble>((Vector2)transform.position + awayFromWand, norm.RotatedBy(Mathf.Deg2Rad * separation * i) * (23 - Mathf.Abs(i) * 1f), 1);
                                else
                                    Projectile.NewProjectile<GachaProj>((Vector2)transform.position + awayFromWand, norm.RotatedBy(Mathf.Deg2Rad * separation * i) * (23 - Mathf.Abs(i) * 1f), 2, type);
                                if(i == 0)
                                {
                                    if(type == 0 || GambleOutcome == 5)
                                        AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1.0f, 1.0f, 0);
                                    if (type == 1 || GambleOutcome == 5)
                                        AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1.0f, 0.5f, 0);
                                    if (type == 2 || GambleOutcome == 5)
                                        AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1.0f, 0.4f, 1);
                                    if (type == 3 || GambleOutcome == 5)
                                        AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1.0f, 0.3f, 2);
                                }
                            }
                        }
                    }
                    if (AttackGamble <= 1)
                    {
                        hasDoneSelectAnimation = true;
                        FakeAttack = false;
                    }
                }
            }
            float percent = AttackLeft / AttackCooldownLeft;
            float iPer = 1 - percent;
            percent = AttackLeft / (AttackCooldownLeft - 40);
            if (percent >= 0 && percent <= 1)
            {
                if ((int)AttackLeft == (int)AttackCooldownLeft - 40)
                {
                    AudioManager.PlaySound(SoundID.SoapDie, transform.position, 1.5f, 1.2f, 0);
                }
                armDefaultRotation = 120f * percent;
                armDefaultPosition = armDefaultPosition.RotatedBy(armDefaultRotation * Mathf.Deg2Rad);
                armDefaultPosition.x *= 0.4f;
            }

            if(!FakeAttack)
            {
                iPer *= 2.3f;
                if (iPer > 1)
                    iPer = 1;
                Coin.gameObject.SetActive(true);
                Coin.localScale = new Vector3(1, 1, 1) * (Mathf.Sin(iPer * Mathf.PI) * 0.5f + iPer);
                Coin.transform.localPosition = new Vector2(0, MathF.Sin(Math.Min(MathF.PI, iPer * MathF.PI)) * 2f - iPer * 1.5f - 1);
            }

            if(AttackLeft == 1)
            {
                if (!FakeAttack)
                {
                    AttackGamble = GambleAnimationFrames + GambleAttackFrames;
                    GambleOutcome = DetermineGambleOutcome();
                }
                else AttackGamble = GambleAttackFrames / 2;
                Coin.gameObject.SetActive(false);
            }
        }
        else if(!hasDoneSelectAnimation)
        {
            StartAttack(false);
        }
        else if (AttackRight > 0)
        {
            if (Hitbox == null && AttackRight == AttackCooldownRight)
            {
                Hitbox = Projectile.NewProjectile<MeleeHitbox>(transform.position, Vector2.zero, 10 * (1 + 0.2f * Player.Instance.ConsolationPrize), 0).GetComponent<MeleeHitbox>();
                AudioManager.PlaySound(SoundID.ChargePoint, transform.position, 0.75f, 0.85f, 0);
            }
            float percent = (AttackRight - WindUpTime) / (AttackCooldownRight - WindUpTime);
            percent = Mathf.Clamp(percent, 0, 1);
            float iPer = 1 - percent * percent * percent * percent;
            iPer = iPer * iPer;
            float angleOffset = 120 * iPer * Mathf.Deg2Rad * dir;
            attemptedPosition *= 1 + iPer * 0.6f;
            float lerp = 0;
            if(AttackRight <= WindUpTime && AttackRight > RightClickEndLag)
            {
                percent = (AttackRight - RightClickEndLag) / (WindUpTime - RightClickEndLag);
                iPer = 1 - percent * percent;
                angleOffset += -225 * iPer * Mathf.Deg2Rad * dir;
                float sin = Mathf.Sin((iPer * 0.33f + 0.67f * iPer * iPer) * Mathf.PI);
                attemptedPosition *= 1.0f + 1.25f * sin * sin;
                scaleUp += sin * 0.3f;
                if(Hitbox != null && !Hitbox.Friendly)
                {
                    AudioManager.PlaySound(SoundID.Teleport, transform.position, 1, 1.6f, 0);
                    //AudioManager.PlaySound(SoundID.SoapDie, transform.position, 2, 2f * player.SecondaryAttackSpeedModifier, 0);
                    Hitbox.Friendly = true;
                    if (Trail == null)
                    {
                        Trail = SpecialTrail.NewTrail(Hitbox.transform, ColorHelper.RarityColors[4] * 0.6f, 1.9f, 0.25f * (WindUpTime - RightClickEndLag) * Time.fixedDeltaTime / MathF.Sqrt(player.SecondaryAttackSpeedModifier), 0.1f, true);
                        Trail.Trail.sortingOrder = 2;
                    }
                }
            }
            else if(AttackRight <= RightClickEndLag)
            {
                percent = AttackRight / RightClickEndLag;
                iPer = 1 - percent;
                angleOffset += -225 * Mathf.Deg2Rad * dir;
                lerp = 0.5f * iPer + 0.5f * iPer * iPer;
                if (Hitbox != null)
                {
                    Hitbox.Kill();
                    Hitbox = null;
                }
                if (Trail != null)
                {
                    Trail.FakeParent = null;
                    Trail = null;
                }
            }
            else
            {
                attemptedPosition += Utils.RandCircle() * iPer * 0.26f;
            }
            attemptedPosition = attemptedPosition.RotatedBy(angleOffset);
            if(lerp > 0)
                attemptedPosition = Vector2.Lerp(attemptedPosition, originalPosition, lerp);
            if (Hitbox != null)
            {
                Hitbox.RB.velocity = attemptedPosition.normalized * 10;
                if(Hitbox.Friendly && Utils.RandFloat(1) < 0.5f)
                    ParticleManager.NewParticle(Hitbox.transform.position + new Vector3(Utils.RandFloat(-0.3f, 0.3f), Utils.RandFloat(-0.3f, 0.3f)), Utils.RandFloat(2f, 2.25f), Hitbox.RB.velocity * Utils.RandFloat(0.25f, 0.5f), 2, Utils.RandFloat(0.7f, 0.8f), 3, ColorHelper.RarityColors[4] * 0.85f);
                Hitbox.transform.position = GeoCenter.position;
                Hitbox.transform.localScale = transform.localScale;
                Hitbox.transform.SetEulerZ(attemptedPosition.ToRotation() * Mathf.Rad2Deg);
                Hitbox.c2D.radius = 1.65f;
            }
            if (Trail != null)
            {
                Trail.AIUpdate();
            }
        }
        AttackRight--;

        float lerpFactor = FakeAttack ? 0.2f : 0.145f;
        LeverArm.transform.LerpLocalPosition(armDefaultPosition, lerpFactor);
        LeverArm.transform.LerpLocalEulerZ(armDefaultRotation, lerpFactor);
        lerpFactor = IsSecondaryAttacking() ? 0.35f : 0.1f;
        //Final Stuff
        float r = attemptedPosition.ToRotation() * Mathf.Rad2Deg - p.PointDirOffset - p.MoveOffset + p.DashOffset;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition + velocity, lerpFactor);
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.15f);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z + (dir == -1 ? 180 : 0));
        transform.localScale = transform.localScale.SetXY(-dir * scaleUp, scaleUp);
        AttackLeft--;

        velocity *= 0.8f;
        bounceCount = 0.7f;
    }
    private float AttackCooldownLeft => 80;
    private float AttackCooldownRight => 100 + 20 * Mathf.Sqrt(player.SecondaryAttackSpeedModifier) + WindUpTime + RightClickEndLag;
    private float GambleAnimationFrames => 112;
    private float GambleAttackFrames => 55;
    private float RightClickEndLag => 70;
    private float WindUpTime => (int)(RightClickEndLag + 50 * Mathf.Sqrt(player.SecondaryAttackSpeedModifier));
    public bool FakeAttack = false;
    protected float bounceCount = 0.7f;
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft <= 0 && AttackGamble <= 0 && AttackRight < 0 && !alternate)
        {
            bool hasMoney = CoinManager.Current > 4 || !Main.WavesUnleashed;
            if(!hasMoney)
            {
                AudioManager.PlaySound(SoundID.SoapDie, transform.position, 1.5f, 1.0f, 1);
                AttackLeft = AttackCooldownLeft - 60;
                FakeAttack = true;
            }
            else
            {
                if (Main.WavesUnleashed)
                    CoinManager.ModifyCurrent(-5);
                AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1.5f, 1.2f, 1);
                AttackLeft = AttackCooldownLeft;
            }
        }
        if (AttackRight <= 0 && AttackLeft < 0 && AttackGamble <= 0 && alternate)
        {
            AttackRight = AttackCooldownRight;
        }
    }
    protected override void DeathAnimation()
    {
        AttackLeft = 0;
        AttackRight = 0;
        AttackGamble = 0;
        float toBody = transform.localPosition.y - p.Body.transform.localPosition.y;
        if (p.DeathKillTimer <= 0)
        {
            velocity *= 0.0f;
            velocity.y += 0.075f;
            velocity.x += 0.06f * p.Direction;
        }
        if (toBody < -0.4f)
        {
            velocity *= -bounceCount;
            transform.localPosition = (Vector2)transform.localPosition + new Vector2(0, -0.4f - toBody);
            bounceCount *= 0.6f;
        }
        else
        {
            velocity.x *= 0.998f;
            velocity.y -= 0.006f;
        }
        transform.localPosition = (Vector2)transform.localPosition + velocity;
        //transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, -0.5f), 0.1f);
        transform.LerpLocalEulerZ(90 * dir, 0.05f);
    }
    public override int GetRarity()
    {
        return 1;
    }
    public static readonly string[] GambleText = { "Try Again!", "Small Win!", "Medium Win!", "Huge Win!", "Jackpot!"};
    public int DetermineGambleOutcome()
    {
        if(!hasDoneSelectAnimation)
        {
            return 5;
        }
        float n = Utils.RollWithLuckRaw();
        //Best Match
        //JACKPOT! ~0.6%
        float chanceForJackpot = 0.006f;
        if(player.PityGrowthAmount != 0)
        {
            chanceForJackpot *= 1 + PityBonus;
        }
        if ((n -= chanceForJackpot) < 0)
        {
            PityCount = 0;
            PityBonus = 0;
            return 5;
        }
        if(player.PityGrowthAmount != 0)
        {
            PityCount++;
            PityBonus += player.PityGrowthAmount;
        }
        else
        {
            PityCount = 0;
            PityBonus = 0;
        }


        //Good Match
        //Huge Win! ~2.7%
        if ((n -= 0.027f) < 0) 
            return 4;

        //Variant Match
        //Small Win! ~6.7%
        if ((n -= 0.067f) < 0) 
            return 3;

        //Weak Variant Match
        //Break Even! ~20%
        if ((n -= 0.2f) < 0) 
            return 2;

        //No Matches
        //Lose Money! Should be most cases ~70%
        return 1;
    }
    public Slot[] GambleSlots = { };
    [Serializable]
    public class Slot
    {
        public float DefaultX;
        public Transform Transform;
        public SpriteRenderer Renderer;
        public int SpriteNum1;
        public SpriteRenderer RendererSecond;
        public int SpriteNum2;
        public bool ReadyToSwitchSprite1 { get; set; } = false;
        public bool ReadyToSwitchSprite2 { get; set; } = false;
    }
    public void GambleAnimation()
    {
        if(AttackGamble == GambleAnimationFrames + GambleAttackFrames - 1)
        {
            AudioManager.PlaySound(SoundID.Starbarbs, transform.position, 1.5f, 0.65f * player.PrimaryAttackSpeedModifier, 0);
        }
        int offsetAmt = 20;
        float totalFrames = GambleAnimationFrames - offsetAmt * 2;
        for(int i = 0; i < 3; ++i)
        {
            float offset = offsetAmt * i;
            float counter = AttackGamble - GambleAttackFrames - offset;
            float percent = 1 - counter / totalFrames;
            if(percent >= 0 && percent <= 1)
            {
                float intensity = MathF.Sin(percent * MathF.PI);
                Slot s = GambleSlots[i];

                float jiggle = Utils.RandFloat(-intensity, intensity);
                s.Transform.localPosition = new Vector3(s.DefaultX + 0.008f * jiggle, 0.025f * jiggle, s.Transform.localPosition.z);
                SlotAnimation(s, percent);
            }
        }
    }
    public void SlotAnimation(Slot s, float percent)
    {
        for (int i = 0; i < 2; ++i)
        {
            float slotPercent = (MathF.Sin(percent * percent * MathF.PI * 0.5f) * 3 + i * 0.5f);
            bool final = slotPercent > 3;
            slotPercent %= 1;
            float yPos = -slotPercent;
            if (yPos < -0.5f)
                yPos += 1;
            SpriteRenderer r = i == 0 ? s.Renderer : s.RendererSecond;
            r.transform.localPosition = new Vector3(0.09f - 0.09f * MathF.Cos(yPos * MathF.PI), yPos, -0.1f);
            r.transform.localScale = new Vector3(1, 1 - Math.Abs(yPos) * 1.25f, 1);

            bool outOfScreen = yPos > 0.45f || yPos < -0.45f;
            if (outOfScreen)
            {
                if (s.ReadyToSwitchSprite1 && i == 0)
                {
                    SwapSlotSprite(s, i, true);
                    s.ReadyToSwitchSprite1 = false;
                }
                else if (s.ReadyToSwitchSprite2 && i == 1)
                {
                    SwapSlotSprite(s, i);
                    s.ReadyToSwitchSprite2 = false;
                }
            }
            else if (i == 0)
                s.ReadyToSwitchSprite1 = true;
            else if (i == 1)
                s.ReadyToSwitchSprite2 = true;
        }
    }
    public void SwapSlotSprite(Slot s, int i, bool final = false)
    {
        if (final && i == 0)
        {
            if(GambleOutcome == 5)
                s.SpriteNum1 = 0;
            else if(GambleOutcome == 4)
                s.SpriteNum1 = 3;
            else if (GambleOutcome == 3)
                s.SpriteNum1 = 1;
            else if (GambleOutcome == 2)
                s.SpriteNum1 = 2;
            else
            {
                if(s == GambleSlots[1])
                {
                    while (s.SpriteNum1 == GambleSlots[0].SpriteNum1)
                        s.SpriteNum1 = Utils.RandInt(4);
                }
                else if(s == GambleSlots[2])
                {
                    while (s.SpriteNum1 == GambleSlots[1].SpriteNum1 || s.SpriteNum1 == GambleSlots[0].SpriteNum1)
                        s.SpriteNum1 = Utils.RandInt(4);
                }
            }
        }
        else
        {
            ref int num = ref s.SpriteNum2;
            if(i == 0)
                num = ref s.SpriteNum1;
            int previousSprite = num;
            while(num == previousSprite)
                num = Utils.RandInt(4);
            if (i == 1 && s.SpriteNum1 == num)
                num = Utils.RandInt(4);
        }
        s.Renderer.sprite = Main.TextureAssets.SlotSymbol[s.SpriteNum1];
        s.RendererSecond.sprite = Main.TextureAssets.SlotSymbol[s.SpriteNum2];
    }
}
