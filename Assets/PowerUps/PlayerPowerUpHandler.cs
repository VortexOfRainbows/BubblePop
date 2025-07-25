using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class Player : Entity
{
    public int PowerCount => powers.Count;
    /// <summary>
    /// Returns the PowerUpID of the power at the given index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int GetPower(int index) => powers[index];
    public void PickUpPower(int Type)
    {
        if (powers.Contains(Type))
            return;
        else
            powers.Add(Type);
    }
    public void RemovePower(int Type, int num = 1)
    {
        int index= powers.IndexOf(Type);
        if (index != -1)
        {
            PowerUp p = PowerUp.Get(GetPower(index));
            p.Stack -= num;
            if(p.Stack <= 0)
            {
                powers.RemoveAt(index);
                //if (powers.Count < index)
                //{
                //    
                //}
            }
        }
    }
    public float AbilityCD => Body.AbilityCD;
    public int ChargeShotDamage = 0;
    public int ShotgunPower = 0;
    public int DashSparkle = 0;
    public int FasterBulletSpeed = 0;
    public int Starbarbs = 0;
    public int SoapySoap = 0;
    public int BubbleBlast = 0;
    public int Starshot = 0;
    public float AttackSpeedModifier = 1.0f;
    public float PrimaryAttackSpeedModifier = 1.0f, SecondaryAttackSpeedModifier = 1.0f, PassiveAttackSpeedModifier = 1.0f;
    public int BinaryStars = 0;
    public int EternalBubbles = 0;

    public int BonusPhoenixLives = 0;
    public int PickedUpPhoenixLivesThisRound = 0;
    public int SpentBonusLives = 0;

    public int BubbleTrail = 0;
    public int Coalescence = 0;
    public int LuckyStar = 0;

    public int TrailOfThoughts = 0, Magnet = 0, LightSpear = 0, LightChainReact = 0, BrainBlast = 0, SnakeEyes = 0, Refraction = 0;
    public int RollDex = 0, RollInit = 0, RollChar = 0, RollPerc = 0;
    public float TrailOfThoughtsRecoverySpeed => AbilityRecoverySpeed;
    public float AbilityRecoverySpeed = 1.0f, AbilityRecoverySpeedMult = 1.0f, MoveSpeedMod = 1.0f;
    public float BlueChipChance = 0.0f;
    public float HomingRange = 0;
    public float HomingRangeSqrt = 0;
    public int ChipHeight = 5;
    public int ChipStacks = 2;
    public bool HasBubbleShield => BubbleShields > 0;
    public int BubbleShields = 0;
    public int TotalMaxShield { get; set; }
    public int TotalMaxLife { get; set; }
    public float ImmunityFrameMultiplier = 1.0f;
    public float ShieldImmunityFrameMultiplier = 1.0f;

    private List<int> powers;
    private void PowerInit()
    {
        powers = new List<int>();
        PowerUp.ResetAll();
    }
    private void ResetPowers()
    {
        powers.Clear();
    }
    private void ClearPowerBonuses()
    {
        ChargeShotDamage = ShotgunPower = DashSparkle = FasterBulletSpeed = Starbarbs = SoapySoap = BubbleBlast = Starshot = BinaryStars = EternalBubbles = BonusPhoenixLives = BubbleTrail = Coalescence = Magnet = LightSpear = 0;
        AttackSpeedModifier = AbilityRecoverySpeed = AbilityRecoverySpeedMult = MoveSpeedMod = ImmunityFrameMultiplier = ShieldImmunityFrameMultiplier = 1.0f;
        LuckyStar = TrailOfThoughts = LightChainReact = BrainBlast = SnakeEyes = Refraction = 0;
        PrimaryAttackSpeedModifier = SecondaryAttackSpeedModifier = PassiveAttackSpeedModifier = 0;
        BlueChipChance = HomingRange = 0.0f;
        ChipHeight = 5;
        ChipStacks = 2;
        TotalMaxShield = MaxShield;
        TotalMaxLife = MaxLife;
        BubbleShields = 0;
        RollDex = RollInit = RollChar = RollPerc = 0;
    }
    private void UpdatePowerUps()
    {
        ClearPowerBonuses();
        for(int i = 0; i < powers.Count; i++)
        {
            PowerUp power = PowerUp.Get(powers[i]);
            if(power.Stack > 0)
            {
                power.HeldEffect(this);
                //Debug.Log($"Doing held effect for {power.Stack}");
            }
        }
        AbilityRecoverySpeed = AbilityRecoverySpeed * AbilityRecoverySpeedMult;
    }
    public void PostEquipUpdate()
    {
        PrimaryAttackSpeedModifier += AttackSpeedModifier;
        SecondaryAttackSpeedModifier += AttackSpeedModifier;
        PassiveAttackSpeedModifier += AttackSpeedModifier;
        UpdateFixed();
    }
    private float BinaryStarTimer = 0.0f;
    private float BubbleTrailTimer = 0.0f;
    private void UpdateFixed()
    {
        if(BinaryStars > 0)
        {
            BinaryStarTimer -= Time.fixedDeltaTime;
            while(BinaryStarTimer <= 0)
            {
                BinaryStarTimer += 1.5f / PassiveAttackSpeedModifier; //1.0, 1.25, 1.5, 1.75, 2.0
                Vector2 circular = Utils.RandCircle(1).normalized;
                float speedMax = 18 + FasterBulletSpeed;
                int c = BinaryStars + 1;
                float spreadAmt = Mathf.PI * 2f / (float)c;
                for (int i = 0; i < c; i++)
                {
                    circular = circular.RotatedBy(spreadAmt);
                    Vector2 target = (Vector2)transform.position + circular * (16 + FasterBulletSpeed);
                    Projectile.NewProjectile<StarProj>(transform.position, circular.RotatedBy(Mathf.PI * 0.55f) * speedMax, target.x, target.y);
                }
            }
        }
        else
        {
            BinaryStarTimer = 0;
        }
        if(BubbleTrail > 0)
        {
            BubbleTrailTimer -= Time.fixedDeltaTime;
            while (BubbleTrailTimer <= 0)
            {
                BubbleTrailTimer += 2f / (PassiveAttackSpeedModifier * (BubbleTrail + 2f)); //.5, 2/5, 2/6, 2/7, 1/4
                Vector2 circular = (Utils.RandCircle(1.3f) - Animator.lastVelo * 0.4f).normalized;
                float speedMax = 2 + FasterBulletSpeed * 0.2f;
                Projectile.NewProjectile<SmallBubble>(transform.position, circular * speedMax);
            }
        }
        else
        {
            BubbleTrailTimer = 0;
        }
    }
    public void OnDash(Vector2 velo)
    {
        OnUseAbility();
        if (DashSparkle > 0)
        {
            Vector2 norm = velo.normalized;
            int stars = 1 + DashSparkle;
            for (; stars > 0; --stars)
            {
                Vector2 target = (Vector2)transform.position + norm * 14 + Utils.RandCircle(6);
                Projectile.NewProjectile<StarProj>(transform.position, norm.RotatedBy(Utils.RandFloat(-135, 135) * Mathf.Deg2Rad) * -Utils.RandFloat(16f, 24f), target.x, target.y);
            }
        }
    }
}

