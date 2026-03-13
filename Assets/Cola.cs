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
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyUnlock>();
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
    protected override void DeathAnimation()
    {
        if(transform.localScale.x > 0.01f)
        {
            transform.localScale = Vector3.zero;
            Vector2 endpos = Player.Position + Utils.RandCircle(6f);
            Projectile.NewProjectile<ColaProj>(transform.position, Vector2.up * 400, 3, Player, endpos.x, endpos.y, p.Direction);
        }
    }
    public override void Init()
    {
        transform.localScale = Vector3.zero;
    }
    private float AttackCooldown => Mathf.Max(20, 52f - Player.AttackSpeedModifier * 2);
    private float RightAttackSpeed => 70 + Player.SecondaryAttackSpeedModifier * 50;
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft < -AttackCooldown && AttackRight < -AttackCooldown)
        {
            if (!alternate)
            {
                //AudioManager.PlaySound(SoundID.BathBombSizzle, transform.position, 1, 2);
                AttackLeft = 50 + Player.ShotgunPower * 10;
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
    private void WandUpdate()
    {
        Vector2 toMouse = Player.Control.MousePosition - (Vector2)p.transform.position;
        float dir = Mathf.Sign(toMouse.x);
        float bodyDir = Mathf.Sign(p.rb.velocity.x);
        Vector2 attemptedPosition = new Vector2(1.1f, -0.25f * dir).RotatedBy(toMouse.ToRotation()) + p.rb.velocity.normalized * 0.1f;
        attemptedPosition.x *= 1.25f;
        attemptedPosition *= 1.25f;
        Vector2 clampedMousePos = toMouse.magnitude < 3 ? (Vector2)p.transform.position + toMouse.normalized * 3 : Player.Control.MousePosition;

        p.PointDirOffset = 0;
        p.MoveOffset = -5 * bodyDir * p.squash;
        p.DashOffset = 100 * dir * (1 - p.squash);

        Vector2 awayFromWand = new Vector2(0.8f, 0).RotatedBy((transform.eulerAngles.z - bonusPointDirOffset) * Mathf.Deg2Rad);
        bonusPointDirOffset = 0;
        Vector2 SODAtoMouse = clampedMousePos - (Vector2)transform.position - awayFromWand;
        if (AttackLeft > 0)
        {
            bool canAttack = AttackLeft % 10 == 0;
            bool bonusBubble = Player.bonusBubbles > 0;
            if (!canAttack && bonusBubble)
            {
                canAttack = true;
                Player.bonusBubbles--;
            }
            if (canAttack)
            {
                AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1f, 1.2f);
                float speed = Utils.RandFloat(15, 16);
                float spread = 5;
                Vector2 shotDirection = toMouse.normalized.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad)
                    * speed + awayFromWand * Utils.RandFloat(2, 4) + Utils.RandCircle(2f);
                Projectile.NewProjectile<SmallBubble>((Vector2)transform.position + awayFromWand * 2,
                   shotDirection, 1, Player);
                recoil -= awayFromWand * 0.8f;
            }
            float percent = AttackLeft / (50f + Player.ShotgunPower * 10f);
        }
        if (AttackRight > 0)
        {
            if (AttackRight == RightAttackSpeed)
                AudioManager.PlaySound(SoundID.ChargePoint, transform.position, 0.75f, 0.75f * RightAttackSpeed / 120f, 0);
            float percent = 1 - AttackRight / RightAttackSpeed;
            float firstPercent = Mathf.Min(percent * 2, 1);
            float secondPercent = Mathf.Clamp(percent * 1.5f - 0.5f, 0, 1);
            float sin = Mathf.Sin(firstPercent * Mathf.PI / 2);
            float sin2 = Mathf.Sin(secondPercent * secondPercent * Mathf.PI * 1.2f);
            bonusPointDirOffset -= (180 * dir) * sin;
            attemptedPosition *= 1 - sin;
            attemptedPosition.y += 2.2f * sin - 0.5f * sin2;
            float d = 80 * sin2 * dir;
            attemptedPosition = attemptedPosition.RotatedBy(Mathf.Deg2Rad * d);
            bonusPointDirOffset -= d;
            if (--AttackRight <= 0)
            {
                Vector2 velo = toMouse.normalized;
                float dist = toMouse.magnitude;
                dist = Mathf.Clamp(dist, 7, 16);
                Vector2 targetPosition = (Vector2)p.transform.position + velo * dist;
                int explodeDamage = 3;
                Projectile.NewProjectile<ColaProj>(transform.position, velo * 24, explodeDamage, Player, targetPosition.x, targetPosition.y, dir);
                AudioManager.PlaySound(SoundID.Teleport, transform.position, 1, 1.7f, 0);
                transform.localScale = Vector3.zero;
            }
            spriteRender.enabled = true;
        }
        else
        {
            float regenRate = Mathf.Min(1, -AttackRight / AttackCooldown);
            transform.LerpLocalScale(Vector3.one * (Mathf.Sin(Mathf.PI * regenRate) * 0.7f + regenRate), 0.24f);
            AttackRight--;
        }

        //Final Stuff
        float r = SODAtoMouse.ToRotation() * Mathf.Rad2Deg - p.PointDirOffset - bonusPointDirOffset - p.MoveOffset + p.DashOffset;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition + recoil, 0.25f);
        gameObject.GetComponent<SpriteRenderer>().flipY = p.PointDirOffset < 0;
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.18f);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        AttackLeft--;
        recoil *= 0.925f;
    }
}
