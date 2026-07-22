using Steamworks;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class OilScepter : Weapon
{
    public int AllowedDiamonds => (Player != null ? 1 + Player.BonusBlackDiamond : 1);
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        rotation -= 45f;
        offset.y -= 0.4125f;
        offset.x -= 0.4125f;
        scale *= 1.6f;
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<KingOilUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Corrode>();
        powerPool.Add<Contaminate>();
        powerPool.Add<Combust>();
        powerPool.Add<Concoct>();
        powerPool.Add<Gasoline>();
        powerPool.Add<OilSpill>();
        powerPool.Add<BlackDiamond>();
    }
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Primary, Ability.ID.Secondary, Ability.ID.Passive);
    }
    public override void EquipUpdate()
    {
        Player.TarShots += 1;
    }
    public Vector2 DeathVelocity = Vector2.zero;
    protected override void AnimationUpdate()
    {
        spriteRender.enabled = true;
        DeathVelocity = Vector2.zero;
        WandUpdate();
    }
    protected override void DeathAnimation()
    {
        ActiveDiamondProjectile = 0;
        AttackLeft = 0;
        AttackRight = 0;
        if (p.DeathKillTimer <= 0)
        {
            Vector2 toMouse = (Player.Control.MousePosition - (Vector2)p.transform.position).normalized;
            DeathVelocity = new Vector2(toMouse.x * 4 + Utils.SignNoZero(toMouse.x) * 2, 12);
        }
        else
        {
            DeathVelocity *= 0.985f;
            DeathVelocity.y -= 0.1f;
        }
        transform.position += (Vector3)DeathVelocity * Time.fixedDeltaTime;
        transform.SetLocalEulerZ(transform.localEulerAngles.z + DeathVelocity.x * 240f * Time.fixedDeltaTime);
        if(p.DeathKillTimer > 110 && spriteRender.enabled)
        {
            spriteRender.enabled = false;
            Gem.gameObject.SetActive(false);
            for(int i = 0; i < 40; ++i)
            {
                float scale = Utils.RandFloat(0.2f, 1.0f);
                ParticleManager.NewParticle(Gem.transform.position, scale, Utils.RandCircle(12 - scale * 8), 1.0f, Utils.RandFloat(0.4f, 0.8f), ParticleManager.ID.Circle, Color.red * 0.7f);
            }
        }
    }
    public override void Init()
    {

    }
    protected virtual float AttackCooldown => 30;
    protected virtual int LeftAttackSpeed => 80;
    protected virtual float RightAttackSpeed => 90;
    protected virtual float SpreadDegrees => 30;
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft <= 0 && AttackRight < -AttackCooldown && ActiveDiamondProjectile < AllowedDiamonds)
        {
            if (!alternate)
            {
                //AudioManager.PlaySound(SoundID.BathBombSizzle, transform.position, 1, 2);
                AttackLeft = LeftAttackSpeed;
            }
        }
        if (AttackRight < -AttackCooldown && AttackLeft < 0 && ActiveDiamondProjectile < AllowedDiamonds)
        {
            if (alternate)
            {
                AttackRight = RightAttackSpeed;
            }
        }
    }
    public SpecialTrail Trail { get; private set; }
    public Transform Gem;
    public Vector2 recoil = Vector2.zero;
    public float bonusPointDirOffset = 0;
    public float bonusPointDirOffsetRight = 0;
    public int WandCounter = 0;
    public float WalkMovementAnimation = 0;
    public int ActiveDiamondProjectile { get; set; } = 0;
    private void WandUpdate()
    {
        Vector2 toMouse = Player.Control.MousePosition - (Vector2)p.transform.position;
        float dir = Mathf.Sign(toMouse.x);
        Vector2 attemptedPosition = new Vector2(1.5f, 0).RotatedBy(toMouse.ToRotation());

        p.DashOffset = 100 * dir * (1 - p.Squash);

        Vector2 awayFromWand = attemptedPosition.normalized;
        if (AttackLeft > 0)
        {
            ++WandCounter;
            bonusPointDirOffset = Mathf.Lerp(bonusPointDirOffset, 90, 0.25f);
            attemptedPosition *= 0.9f;
            bool canAttack = AttackLeft <= 1;
            if (canAttack)
            {
                AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1.0f, 1.3f);
                Vector2 randomAddition = awayFromWand * Utils.RandFloat(2, 4) + Utils.RandCircle(2f);
                float speed = Utils.RandFloat(15, 16);
                float spread = SpreadDegrees;
                int shotCount = 3;
                Vector2 spawnPos = (Vector2)Gem.transform.position + awayFromWand * 1.1f;
                for (int i = 0; i < 10; ++i)
                    ParticleManager.NewParticle(spawnPos, Utils.RandFloat(0.5f, 0.75f), Utils.RandCircle(4) + awayFromWand * Utils.RandFloat(3, 5), 1.0f, Utils.RandFloat(0.3f, 0.5f), ParticleManager.ID.Circle, Color.red.WithAlpha(0.5f));
                for (int i = 0; i < shotCount; ++i)
                {
                    float spreadPercent = (i + 0.5f) / shotCount;
                    Vector2 shotDirection = attemptedPosition.normalized.RotatedBy(Mathf.Lerp(-spread, spread, spreadPercent) * Mathf.Deg2Rad) * speed + randomAddition;
                    Projectile.NewProjectile<SmallBubble>(spawnPos, shotDirection, 1, Player);
                }
                recoil -= awayFromWand * .1f;
            }
            //float percent = AttackLeft / (50f + Player.ShotgunPower * 10f);
        }
        else if (AttackRight <= 0)
            bonusPointDirOffset = Mathf.Lerp(bonusPointDirOffset, 0, 0.08f);
        if (AttackRight > 0)
        {
            if(AttackRight == RightAttackSpeed)
                AudioManager.PlaySound(SoundID.ChargePoint, transform.position, 0.7f, Utils.RandFloat(0.67f, 0.73f), 0);
            bonusPointDirOffset = Mathf.Lerp(bonusPointDirOffset, 90, 0.1f);
            float percent = 1 - AttackRight / RightAttackSpeed;
            float sin = Mathf.Sin(percent * percent * Mathf.PI * 1.25f);
            bonusPointDirOffsetRight = Mathf.Lerp(sin * (-120 - 45 * percent) - bonusPointDirOffset, 0, 0.2f);
            attemptedPosition *= 1f - 0.5f * sin + .2f * percent;
            attemptedPosition.y += 0.5f * percent * sin;
            if (AttackRight == 1)
            {
                AudioManager.PlaySound(SoundID.Teleport, transform.position, 1.8f, 0.4f, 0);
                AudioManager.PlaySound(SoundID.Infect, transform.position, 0.8f, 0.8f, 0);
                ++ActiveDiamondProjectile;
                float distance = 16;
                Vector2 final = Utils.RaycastWithTileSupport(Gem.transform.position, awayFromWand, ref distance, 0.1f);
                Projectile.NewProjectile<KingOilDiamondProj>(Gem.transform.position, awayFromWand * 5, 1, Player, final.x, final.y);

                if(Player.Control.SecondaryAttackHold && ActiveDiamondProjectile < AllowedDiamonds)
                    AttackRight = RightAttackSpeed + 1;
            }
            AttackRight--;
        }
        else
        {
            bonusPointDirOffsetRight = Mathf.Lerp(bonusPointDirOffsetRight, 0, 0.08f);
            if (ActiveDiamondProjectile < AllowedDiamonds)
                AttackRight--;
        }
        WalkMovementAnimation += Mathf.Sqrt(Player.RB.velocity.magnitude);

        //Final Stuff
        float leftClickPercent = bonusPointDirOffset / 90f;
        float pointPercent = 1 - leftClickPercent;
        float direction = -Utils.SignNoZero(attemptedPosition.x);

        Vector2 tiltAugment = toMouse.normalized;
        tiltAugment.x = Mathf.Abs(tiltAugment.x) + 2 * pointPercent;
        tiltAugment.y *= -direction;
        float tiltRotation = tiltAugment.ToRotation() * Mathf.Rad2Deg;
        float r = tiltRotation + p.DashOffset + bonusPointDirOffset * direction + bonusPointDirOffsetRight * direction;
        attemptedPosition.y *= 1 - 0.5f * pointPercent;
        attemptedPosition.y -= 0.75f * pointPercent;

        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition + recoil, 0.125f);
        if (Utils.SignNoZero(transform.localScale.x) != direction)
            transform.localScale = new Vector3(direction, 1, 1);
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.15f);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        AttackLeft--;
        recoil *= 0.925f;


        if (Trail == null)
        {
            Trail = SpecialTrail.NewTrail(Gem, Color.red.WithAlpha(0.5f), .4f, .4f, 0.2f, manuallyUpdated: false, orderInLayer: 3);
            Trail.Trail.minVertexDistance *= 10;
        }
        else
        {
            Trail.Trail.startColor = Color.red.WithAlpha(0.5f * (1 - ActiveDiamondProjectile / (float)AllowedDiamonds));
            Trail.gameObject.SetActive(Gem.gameObject.activeSelf);
            if (!Trail.gameObject.activeSelf)
            {
                Trail.Trail.Clear();
                Trail.transform.position = Gem.transform.position;
            }
        }
        Gem.gameObject.GetComponent<SpriteRenderer>().color = Color.black.Lerp(Color.white, 1 - ActiveDiamondProjectile / (float)AllowedDiamonds);
    }
    public void Update()
    {
        if (!spriteRender.enabled)
        {
            Trail.gameObject.SetActive(Gem.gameObject.activeSelf);
            return;
        }
        Gem.gameObject.SetActive(ActiveDiamondProjectile < AllowedDiamonds);
        if (Trail != null)
        {
            if (AttackLeft >= 0)
            {
                Vector2 toMouse = transform.localPosition;
                toMouse = toMouse.normalized;
                float attackPercent = 1 - AttackLeft / LeftAttackSpeed;
                float iPer = 1 - attackPercent;
                float sin = Mathf.Sin(attackPercent * Mathf.PI);
                sin = Mathf.Sqrt(sin);
                int shotCount = 3;
                for (int i = 1; i <= shotCount; ++i)
                {
                    Vector2 circular = new Vector2(1.5f - 1.75f * attackPercent * attackPercent, 0).RotatedBy(WandCounter / (float)LeftAttackSpeed * Utils.TwoPI / 2f + i * Utils.TwoPI / shotCount);
                    circular.x *= 0.5f;
                    circular = circular.RotatedBy(toMouse.ToRotation());
                    circular += (Vector2)Gem.transform.position;
                    circular += toMouse;
                    Vector2 scale = Vector2.one * (0.5f + attackPercent * iPer);
                    SpriteBatch.Draw(Main.TextureAssets.Shadow, circular, scale, 0, Color.red * sin, 3, Main.TextureAssets.AdditiveShader);
                    SpriteBatch.Draw(Main.TextureAssets.Shadow, circular, scale * 0.75f, 0, Color.white * sin, 3, Main.TextureAssets.AdditiveShader);
                    //ParticleManager.NewParticle(circular, 2f, Vector2.zero + Player.RB.velocity * 0.75f, 0.1f, 0.4f, ParticleManager.ID.Pixel, Color.red);
                    //Trails[i].transform.position = circular;
                }
            }
        }
    }
}
