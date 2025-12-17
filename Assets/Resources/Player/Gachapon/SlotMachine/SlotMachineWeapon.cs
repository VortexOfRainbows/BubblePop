using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

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
    public Transform LeverArm;
    public float AttackGamble = 0;
    public int GambleOutcome = 1;
    protected override void AnimationUpdate()
    {
        Vector2 playerToMouse = Utils.MouseWorld - (Vector2)p.transform.position;
        Vector2 mouseAdjustedFromPlayer = playerToMouse.magnitude < 4 ? playerToMouse.normalized * 4 + (Vector2)p.transform.position : Utils.MouseWorld;
        float dir = Mathf.Sign(playerToMouse.x);

        transform.localScale = transform.localScale.SetXY(-dir, 1);

        Vector2 awayFromWand = new Vector2(1.5f, 0.25f * dir).RotatedBy(playerToMouse.ToRotation());
        Vector2 toMouse = mouseAdjustedFromPlayer - (Vector2)transform.position - awayFromWand;
        Vector2 norm = toMouse.normalized;
        float bodyDir = Mathf.Sign(p.rb.velocity.x);
        Vector2 attemptedPosition = playerToMouse.normalized * 1.0f + p.rb.velocity.normalized * 0.1f;
        attemptedPosition.y *= 0.8f;

        p.PointDirOffset = 2 * dir * p.squash;
        p.MoveOffset = -0 * bodyDir * p.squash;
        p.DashOffset = 100 * dir * (1 - p.squash);

        if (AttackLeft > 0 || AttackGamble > 0)
        {
            if (AttackGamble > 0)
            {
                AttackGamble--;
                AttackLeft = 0;
                if (AttackGamble > 100) //Spin Slots animation
                {

                }
                else //Actual attack portion
                {
                    if (AttackGamble == 100)
                        PopupText.NewPopupText(Player.Position + new Vector2(0, 2), Vector3.up * 5f, ColorHelper.RarityColors[GambleOutcome - 1], GambleText[GambleOutcome - 1], true, 0.66f, 130);
                    if(GambleOutcome == 5)
                    {

                    }
                    else
                    {
                        if(AttackGamble == 80 || AttackGamble == 50 || AttackGamble == 20)
                        {
                            velocity -= norm * 1f;
                            for(int i = -1; i <= 1; i += 1)
                            {
                                Projectile.NewProjectile<SmallBubble>((Vector2)transform.position + awayFromWand, norm.RotatedBy(Mathf.Deg2Rad * 15 * i) * (20 - Mathf.Abs(i) * 4), 1);
                            }
                        }
                    }
                }
            }
            float percent = AttackLeft / AttackCooldownLeft;
            float armDefaultRotation = 130f * percent;
            Vector2 armDefaultPosition = new Vector2(0, 0.16f).RotatedBy(armDefaultRotation * Mathf.Deg2Rad);
            armDefaultPosition.x *= 0.4f;
            LeverArm.transform.LerpLocalPosition(armDefaultPosition, 0.2f);
            LeverArm.transform.LerpLocalEulerZ(armDefaultRotation, 0.2f);
            if(AttackLeft == 1)
            {
                AttackGamble = 200;
                GambleOutcome = DetermineGambleOutcome();
            }
        }
        if (AttackRight > 0)
        {

        }
        else
            AttackRight--;

        //Final Stuff
        float r = attemptedPosition.ToRotation() * Mathf.Rad2Deg - p.PointDirOffset - p.MoveOffset + p.DashOffset;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition + velocity, 0.08f);
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.15f);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z + (dir == -1 ? 180 : 0));
        AttackLeft--;

        velocity *= 0.8f;
    }
    private float AttackCooldownLeft => 60;
    private float AttackCooldownRight => 60;
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft <= 0 && AttackGamble <= 0 && AttackRight < 0 && !alternate)
        {
            AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1f, 1f);
            AttackLeft = AttackCooldownLeft;
        }
        if (AttackRight <= 0 && AttackLeft < 0 && AttackGamble < 0 && alternate)
        {
            AttackRight = AttackCooldownRight;
        }
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
        return 1;
    }
    public static readonly string[] GambleText = { "Try Again!", "Small Win!", "Medium Win!", "Huge Win!", "Jackpot!"};
    public int DetermineGambleOutcome()
    {
        float n = Utils.RollWithLuckRaw();
        //Best Match
        //JACKPOT! Should be EXTREMELY rare ~1%
        if (n < 0.01f)
            return 5;

        //Good Match
        //Huge Win! Should be very rare ~4%
        if (n < 0.05f)
            return 4;

        //Variant Match
        //Small Win! ~15%
        if (n < 0.2f)
            return 3;

        //Weak Variant Match
        //Break Even! ~25%
        if (n < 0.45f)
            return 2;

        //No Matches
        //Lose Money! Should be most cases ~55%
        return 1;
    }
}
