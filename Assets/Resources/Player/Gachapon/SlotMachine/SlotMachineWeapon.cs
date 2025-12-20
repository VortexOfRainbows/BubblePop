using System;
using System.Collections.Generic;
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
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gacha Slots").WithDescription("99% of bubbles pop before hitting it big");
    }
    public override void EquipUpdate()
    {

    }
    public override bool IsPrimaryAttacking()
    {
        return base.IsPrimaryAttacking() || GambleAttackFrames > 0;
    }
    public Transform LeverArm;
    public Transform Coin;
    public float AttackGamble = 0;
    public int GambleOutcome = 1;
    protected override void AnimationUpdate()
    {
        Vector2 playerToMouse = Utils.MouseWorld - (Vector2)p.transform.position;
        Vector2 mouseAdjustedFromPlayer = playerToMouse.magnitude < 4 ? playerToMouse.normalized * 4 + (Vector2)p.transform.position : Utils.MouseWorld;
        float dir = Mathf.Sign(playerToMouse.x);
        float scaleUp = 0.95f;

        Vector2 awayFromWand = new Vector2(1.5f, 0.25f * dir).RotatedBy(playerToMouse.ToRotation());
        Vector2 toMouse = mouseAdjustedFromPlayer - (Vector2)transform.position - awayFromWand;
        Vector2 norm = toMouse.normalized;
        float bodyDir = Mathf.Sign(p.rb.velocity.x);
        Vector2 attemptedPosition = playerToMouse.normalized * 1.05f + p.rb.velocity.normalized * 0.1f;
        attemptedPosition.y *= 0.95f;
        Vector2 originalPosition = attemptedPosition;

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
                            PopupText.NewPopupText(Player.Position + new Vector2(0, 2), Vector3.up * 5f, ColorHelper.RarityColors[GambleOutcome - 1], GambleText[GambleOutcome - 1], true, 0.66f, 130);
                        }
                        if (AttackGamble == 50 || AttackGamble == 30 || AttackGamble == 10)
                        {
                            velocity -= norm * 1f;
                            int num = GambleOutcome == 5 ? 3 : 0;
                            int type = GambleOutcome != 5 ? GambleOutcome - 1 : AttackGamble == 50 ? 3 : AttackGamble == 30 ? 2 : 1;
                            for (int i = -num; i <= num; i += 1)
                            {
                                Projectile.NewProjectile<GachaProj>((Vector2)transform.position + awayFromWand, norm.RotatedBy(Mathf.Deg2Rad * 3.5f * i) * (23 - Mathf.Abs(i) * 1f), 2, type);
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
                        FakeAttack = false;
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
        if (AttackRight > 0)
        {
            float percent = (AttackRight - WindUpTime) / (AttackCooldownRight - WindUpTime);
            percent = Mathf.Clamp(percent, 0, 1);
            float iPer = 1 - percent * percent * percent * percent;
            iPer = iPer * iPer;
            float angleOffset = 120 * iPer * Mathf.Deg2Rad * dir;
            attemptedPosition *= 1 + iPer * 0.6f;
            float lerp = 0;
            if(AttackRight <= WindUpTime && AttackRight > RightClickCooldown)
            {
                percent = (AttackRight - RightClickCooldown) / (WindUpTime - RightClickCooldown);
                iPer = 1 - percent * percent;
                angleOffset += -220 * iPer * Mathf.Deg2Rad * dir;
                float sin = Mathf.Sin((iPer * 0.33f + 0.67f * iPer * iPer) * Mathf.PI);
                attemptedPosition *= 1.0f + 1.15f * sin * sin;
                scaleUp += sin * 0.3f;
            }
            else if(AttackRight <= RightClickCooldown)
            {
                percent = AttackRight / RightClickCooldown;
                iPer = 1 - percent;
                angleOffset += -220 * Mathf.Deg2Rad * dir;
                lerp = 0.5f * iPer + 0.5f * iPer * iPer;
            }
            else
            {
                attemptedPosition += Utils.RandCircle() * iPer * 0.2f;
            }
            attemptedPosition = attemptedPosition.RotatedBy(angleOffset);
            if(lerp > 0)
                attemptedPosition = Vector2.Lerp(attemptedPosition, originalPosition, lerp);
            AttackRight--;
        }
        else
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
    }
    private float AttackCooldownLeft => 80;
    private float AttackCooldownRight => 245;
    private float GambleAnimationFrames => 112;
    private float GambleAttackFrames => 55;
    private float RightClickCooldown => 50;
    private float WindUpTime => 100;
    public bool FakeAttack = false;
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft <= 0 && AttackGamble <= 0 && AttackRight < 0 && !alternate)
        {
            bool hasMoney = CoinManager.Current > 0 || !Main.WavesUnleashed;
            if(!hasMoney)
            {
                AudioManager.PlaySound(SoundID.SoapDie, transform.position, 1.5f, 1.0f, 1);
                AttackLeft = AttackCooldownLeft - 60;
                FakeAttack = true;
            }
            else
            {
                if (Main.WavesUnleashed)
                    CoinManager.ModifyCurrent(-1);
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
        //transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, -0.5f), 0.1f);
        //transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.transform.eulerAngles.z,
        //     spriteRender.flipY ? 190 : - 10, 0.1f));
    }
    public override int GetRarity()
    {
        return 1;
    }
    public static readonly string[] GambleText = { "Try Again!", "Small Win!", "Medium Win!", "Huge Win!", "Jackpot!"};
    public int DetermineGambleOutcome()
    {
        float n = Utils.RollWithLuckRaw();
        //Best Match
        //JACKPOT! Should be EXTREMELY rare ~0.6%
        if ((n -= 0.006f) < 0) //Can give a lot of coins back
            return 5;

        //Good Match
        //Huge Win! Should be very rare ~2.7%
        if ((n -= 0.027f) < 0) //should give ~15 coins back MAX (0.027 * 15 ~= 0.405)
            return 4;

        //Variant Match
        //Small Win! ~6.7%
        if ((n -= 0.067f) < 0) //should give ~9 coins back MAX (0.067 * 9 ~= 0.603)
            return 3;

        //Weak Variant Match
        //Break Even! ~20%
        if ((n -= 0.2f) < 0) //should give ~3 coins back MAX (0.2 * 3 = 0.6)
            return 2;

        //No Matches
        //Lose Money! Should be most cases ~65%
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
