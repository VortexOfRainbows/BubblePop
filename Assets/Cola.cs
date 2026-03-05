using System.Collections.Generic;
using UnityEngine;

public class Cola : Weapon
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        offset.y += 0.5f;
        offset.x += 0.5f;
        scale *= 1.25f;
    }
    // protected override UnlockCondition CategoryUnlockCondition => UnlockCondition.Get<BubblemancerUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<EternalBubbles>();
        powerPool.Add<Shotgun>();
        powerPool.Add<ChargeShot>();
        powerPool.Add<BubbleBlast>();
        powerPool.Add<SoapySoap>();
        powerPool.Add<ShotSpeed>();
        powerPool.Add<Starshot>();
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Cola Pop").WithDescription("A popular bubble beverage that tastes sweet with a hint of vanilla, caramel, and citrus");
    }
    public override void InitializeAbilities(ref List<Ability> abilities)
    {
        abilities.Add(new Ability(Ability.ID.Primary, "Unleash a Y:[stream of bubbles] in a burst"));
        abilities.Add(new Ability(Ability.ID.Secondary, "Y:Throw an Y:[explosive bottle of pop]"));
    }
    protected override void AnimationUpdate()
    {
        WandUpdate();
    }
    private float AttackCooldown => Mathf.Max(0, 30f - (Player.AttackSpeedModifier - 1) * 5);
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft < -AttackCooldown && AttackRight < 0)
        {
            if (!alternate)
            {
                AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1f, 1f);
                AttackLeft = 50;
                Player.bonusBubbles = Player.ShotgunPower;
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
    private void WandUpdate()
    {
        //if(Input.GetKeyDown(KeyCode.V))
        //{
        //    DamagePower++;
        //    ShotgunPower++;
        //}

        Vector2 toMouse = Player.Control.MousePosition - (Vector2)p.gameObject.transform.position;
        float dir = Mathf.Sign(toMouse.x);
        float bodyDir = Mathf.Sign(p.rb.velocity.x);
        Vector2 attemptedPosition = new Vector2(0.8f, 0.2f * dir).RotatedBy(toMouse.ToRotation()) + p.rb.velocity.normalized * 0.1f;
        attemptedPosition.y -= 0.1f;
        //Debug.Log(attemptedPosition.ToRotation() * Mathf.Rad2Deg);

        p.PointDirOffset = -5f * dir * p.squash;
        p.MoveOffset = -5 * bodyDir * p.squash;
        p.DashOffset = 100 * dir * (1 - p.squash);

        float bonusPointDirOffset = 0;
        Vector2 awayFromWand = new Vector2(1, 0).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad);
        if (AttackLeft > 0)
        {
            bool canAttack = AttackLeft % 10 == 1;
            bool bonusBubble = Player.bonusBubbles > 0;
            if (!canAttack && bonusBubble)
            {
                canAttack = true;
                Player.bonusBubbles--;
            }
            if (canAttack)
            {
                float speed = Utils.RandFloat(12, 14);
                float spread = 6 + Mathf.Sqrt(Player.ShotgunPower) * 0.5f;
                Projectile.NewProjectile<SmallBubble>((Vector2)transform.position + awayFromWand * 2,
                    toMouse.normalized.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad)
                    * speed + awayFromWand * Utils.RandFloat(1, 3) + Utils.RandCircle(1f), 1, Player);
                float odds = Mathf.Sqrt(1f / (AttackLeft - 40f));
                int attempts = Player.bonusBubbles;
            }
            float percent = AttackLeft / 50f;
            bonusPointDirOffset += 5f * percent * dir * p.squash;
        }
        if (AttackRight > 0)
        {
            if ((Player.Control.SecondaryAttackHold || AttackRight < 100) && AttackRight >= 50)
            {
                int maxCharge = 250 + 100 * Player.OldCoalescence;
                if (AttackRight == 50)
                {
                    AudioManager.PlaySound(SoundID.ChargeWindup, Player.Position, 0.3f, 1.5f);
                    AudioManager.PlaySound(SoundID.ChargePoint.GetVariation(0), Player.Position, 0.6f, 1f);
                    Projectile.NewProjectile<BigBubble>((Vector2)transform.position + awayFromWand, Vector2.zero, 1, Player, 149, 0);
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
                    if (AttackRight > 250 && (AttackRight - 50) % 100 == 0)
                    {
                        float scale = (AttackRight - 250) / 100;
                        AudioManager.PlaySound(SoundID.ChargePoint.GetVariation(2), Player.Position, 0.7f, 1f + scale * 0.05f);
                    }
                }
                bonusPointDirOffset += -Mathf.Min(45f, (AttackRight - 50f) / 200f * 45f) * dir * p.squash;
            }
            else
            {
                if (AttackRight > 50)
                    AttackRight = 50;
                AttackRight--;
                float percent = AttackRight / 50f;
                bonusPointDirOffset += 125 * percent * dir * p.squash;
            }
        }
        else
            AttackRight--;

        //Final Stuff
        float r = attemptedPosition.ToRotation() * Mathf.Rad2Deg - p.PointDirOffset - bonusPointDirOffset - p.MoveOffset + p.DashOffset;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition, 0.08f);
        gameObject.GetComponent<SpriteRenderer>().flipY = p.PointDirOffset < 0;
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.18f);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        AttackLeft--;
    }
}
