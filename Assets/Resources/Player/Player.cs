using System.Collections.Generic;
using UnityEngine;
public static class Control
{
    public static bool Interact => Input.GetKeyDown(KeyCode.E);
    public static bool Ability => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.Space);
    public static bool LastAbility = false;
    public static bool Up => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
    public static bool Down => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
    public static bool Left => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
    public static bool Right => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
    public static bool Tab => Input.GetKey(KeyCode.Tab);
    public static bool LeftMouseHold => Input.GetMouseButton(0);
    public static bool LeftMouseClick => Input.GetMouseButtonDown(0);
    public static bool RightMouseHold => Input.GetMouseButton(1);
    public static bool RightMouseClick => Input.GetMouseButtonDown(1);
}
public partial class Player : Entity
{
    public static readonly List<Player> AllPlayers = new();
    public int InstanceID = 0;
    public static Player GetInstance(int instanceID = 0)
    {
        return AllPlayers.Count <= instanceID ? null : AllPlayers[instanceID];
    }
    private int Shield = 0;
    private int MaxShield = 0;
    public new int Life = 0;
    public new int MaxLife = 0;
    public int GetShield() => Shield;
    public PowerUp MostRecentPower = null;
    #region Animator References
    public Body Body { get => Animator.Body; set => Animator.Body = value; }
    public Weapon Weapon { get => Animator.Weapon; set => Animator.Weapon = value; }
    public Hat Hat { get => Animator.Hat; set => Animator.Hat = value; }
    public Accessory Accessory { get => Animator.Accessory; set => Animator.Accessory = value; }
    public Equipment[] Equips => new Equipment[] { Hat, Accessory, Weapon, Body };
    public PlayerAnimator Animator;
    public Rigidbody2D rb => Animator.rb;
    public float SquashAmt { get; private set; } = 0.6f;
    private float DeathKillTimer { get => Animator.DeathKillTimer; set => Animator.DeathKillTimer = value; }
    #endregion
    public static Color ProjectileColor => Instance.Body.PrimaryColor;
    public static Player Instance => GetInstance(0);
    public static Vector2 Position => Instance == null ? Vector2.zero : (Vector2)Instance.transform.position;
    public Camera MainCamera => Camera.main;
    private readonly float speed = 2.5f;
    private readonly float MovementDeacceleration = 0.9f;
    public const float DashDefault = 25f;
    public static bool HasAttacked = false;
    public static bool PickedLowerDifficultyWaveCard = false;
    public static bool HasBeenHit => TimesHitThisRun > 0;
    public static int TimesHitThisRun = 0;
    public static int GoldSpentTotal = 0;
    /// <summary>
    /// Shorthand for abilityTimer <= 0;
    /// </summary>
    public bool AbilityReady => abilityTimer <= 0;
    public bool AbilityOnCooldown => abilityTimer > 0;
    private bool HasRunStartingGear = false;
    public float MaxSpeed => MoveSpeedMod * 6f;
    public int bonusBubbles = 0;
    public bool IsDead => DeathKillTimer > 0;
    public override void Init()
    {
        PowerInit();
        WaveDirector.Reset();
        MainCamera.orthographicSize = 12;
        DeathKillTimer = 0;
        PickedUpPhoenixLivesThisRound = SpentBonusLives = 0;
        HasRunStartingGear = false;
        Life = MaxLife = 3;
        PlayerStatUI.SetHeartsToPlayerLife();
    }
    public float abilityTimer = 0;
    private void MovementUpdate()
    {
        Vector2 velocity = rb.velocity;
        float cSpeed = speed * MoveSpeedMod;
        Vector2 movespeed = Vector2.zero;
        if (Control.Up && !Control.Down)
        {
            if (velocity.y < 0)
                velocity.y *= MovementDeacceleration;
            movespeed.y += cSpeed;
        }
        else if (!Control.Up && Control.Down)
        {
            if (velocity.y > 0)
                velocity.y *= MovementDeacceleration;
            movespeed.y -= cSpeed;
        }
        else
            velocity.y *= MovementDeacceleration;
        if (!Control.Right && Control.Left)
        {
            if (velocity.x > 0)
                velocity.x *= MovementDeacceleration;
            movespeed.x -= cSpeed;
        }
        else if (Control.Right && !Control.Left)
        {
            if (velocity.x < 0)
                velocity.x *= MovementDeacceleration;
            movespeed.x += cSpeed;
        }
        else
            velocity.x *= MovementDeacceleration;
        movespeed = movespeed.normalized;

        abilityTimer -= Time.fixedDeltaTime * AbilityRecoverySpeed;
        if (abilityTimer < 0)
            abilityTimer = 0;

        Body.AbilityUpdate(ref velocity, movespeed);

        //Final stuff
        velocity += movespeed * cSpeed;
        float currentSpeed = velocity.magnitude;
        if (currentSpeed > MaxSpeed)
        {
            Vector2 norm = velocity.normalized;
            velocity = norm * (MaxSpeed + (currentSpeed - MaxSpeed) * 0.8f);
            if (currentSpeed > MaxSpeed + 15f * MoveSpeedMod)
            {
                for (float i = 0; i < 1; i += 0.5f)
                    ParticleManager.NewParticle((Vector2)transform.position + velocity * i * Time.fixedDeltaTime + Utils.RandCircle(i * 2) - norm * .5f, .5f, norm * -Utils.RandFloat(15f), 1.0f, 0.6f, 0, Body.PrimaryColor);
            }
        }
        rb.velocity = velocity;
        Control.LastAbility = Control.Ability;
        Body.AliveUpdate();
        Animator.PostUpdate();
    }
    private float AttackUpdateTimer = 0;
    public override void OnFixedUpdate()
    {
        if (!HasRunStartingGear && Main.WavesUnleashed)
        {
            foreach(Equipment e in Equips)
            {
                e.OnStartWith();
                e.TotalTimesUsed = e.TotalTimesUsed + 1;
            }
            HasRunStartingGear = true;
            HasAttacked = false;
            TimesHitThisRun = 0;
        }
        WaveDirector.FixedUpdate();
        if (!HasRunStartingGear)
        {
            MaxLife = Life = Shield = MaxShield = 0;
            foreach (Equipment e in Equips)
                e.ModifyLifeStats(ref MaxLife, ref Life, ref Shield, ref MaxShield);
            PlayerStatUI.SetHeartsToPlayerLife();
        }
        UpdatePowerUps();
        UpdateBuffs();
        HomingRangeSqrt = Mathf.Sqrt(HomingRange);
        bool dead = DeathKillTimer > 0;
        if (!World.WithinBorders(transform.position, true))
            Entity.PushIntoClosestPossibleTile(transform, RB, includeProgressionBounds: true);
        if (dead)
            Pop();
        else
        {
            if (HasRunStartingGear)
            {
                //Debug.Log($"{Shield}, {MaxShield}, {TotalMaxShield}");
                if (TotalMaxLife < Life)
                    Life = TotalMaxLife;
                if (TotalMaxShield < Shield)
                    Shield = TotalMaxShield;
                PlayerStatUI.UpdateHearts(this);
            }
            BonusChoices = false;
            foreach (Equipment e in Equips)
                e.EquipUpdate();
            PostEquipUpdate();
            foreach (Equipment e in Equips)
                e.PostEquipUpdate();
            MainCamera.orthographicSize = Mathf.Lerp(MainCamera.orthographicSize, 15f, 0.03f);
            MovementUpdate();
            if(!Main.MouseHoveringOverButton)
            {
                if (Input.GetMouseButton(0))
                {
                    HasAttacked = true;
                    Weapon.StartAttack(false);
                }
                else if (Input.GetMouseButton(1))
                {
                    HasAttacked = true;
                    Weapon.StartAttack(true);
                }
            }
            if (Main.GameUpdateCount % 200 == 0)
                Debug.Log($"Has Attacked: {HasAttacked}, Picked Lower Card: {PickedLowerDifficultyWaveCard}, Has Taken Damage: {HasBeenHit}:{TimesHitThisRun}");
            if (Weapon.IsAttacking())
            {
                if(Weapon.IsPrimaryAttacking())
                    AttackUpdateTimer += PrimaryAttackSpeedModifier;
                if(Weapon.IsSecondaryAttacking())
                    AttackUpdateTimer += SecondaryAttackSpeedModifier;
            }
            else
            {
                AttackUpdateTimer += 1;
            }
            while (AttackUpdateTimer >= 1)
            {
                Weapon.AliveUpdate();
                AttackUpdateTimer -= 1;
            }
            Hat.AliveUpdate();
            Accessory.AliveUpdate();
        }
        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, new Vector3(transform.position.x, transform.position.y, MainCamera.transform.position.z), 0.1f);
        ImmuneFlashing();
        Main.MouseHoveringOverButton = false;
    }
    new public void Update() => base.Update(); //why did I do tis i don't know
    public void SetLife(int num)
    {
        if (num > TotalMaxLife)
            num = TotalMaxLife;
        Life = num;
        OnSetLife(Life);
    }
    public void Hurt(int damage = 1)
    {
        TimesHitThisRun++;
        if(TimesHitThisRun>= 15 && this.Body is Gachapon)
        {
            UnlockCondition.Get<GachaponHealer>().SetComplete();
        }
        float defaultImmuneFrames = 100;
        float immunityFrameMultiplier = ImmunityFrameMultiplier;
        if(Shield > 0)
        {
            SetShield(Shield - damage);
            immunityFrameMultiplier *= ShieldImmunityFrameMultiplier;
            if(BubbleShields > 0)
            {
                float velocity = 2;
                int amt = 16 + 8 * BubbleShields;
                for (int i = 0; i < amt; ++i)
                    Projectile.NewProjectile<SmallBubble>(transform.position, Random.insideUnitCircle * Utils.RandFloat(0.5f + i * 0.2f, velocity + i * 0.4f), 1);
            }
        }
        else
        {
            SetLife(Life - damage);
        }
        UniversalImmuneFrames = defaultImmuneFrames * immunityFrameMultiplier;
        if (Life <= 0)
            Pop();
        else
        {
            Body.ModifyHurtAnimation();
        }
    }
    public void Pop()
    {
        //Time.timeScale = 0.5f + 0.5f * Mathf.Sqrt(Mathf.Max(0, 1 - DeathKillTimer / 200f));
        if (DeathKillTimer > 100)
            MainCamera.orthographicSize = Mathf.Lerp(MainCamera.orthographicSize, 6f, 0.03f);
        else
            MainCamera.orthographicSize = Mathf.Lerp(MainCamera.orthographicSize, 17f, 0.03f);
        rb.velocity *= 0.9f;
        Body.DeadUpdate();
        Hat.DeadUpdate();
        Weapon.DeadUpdate();
        Accessory.DeadUpdate();
        DeathKillTimer++;
        if(Input.GetKey(KeyCode.R) && Main.DebugCheats)
        {
            DeathKillTimer = 0;
            //Body.SetActive(true);
        }
        if (DeathKillTimer > 200)
            RegisterDeath();
    }
    public void RegisterDeath()
    {
        PlayerData.PlayerDeaths++;
        if(BonusPhoenixLives > 0)
        {
            RemovePower(PowerUp.Get<BubbleBirb>().Type, 1);
            Rebirth();
            return;
        }
        CoinManager.AfterDeathReset();
        Main.UIManager.GameOver();
    }
    public void Rebirth()
    {
        if(Accessory is RedCape)
            PowerUp.Spawn<Contract>(transform.position);
        for(int i = 0; i < 30; i++)
        {
            Projectile.NewProjectile<PhoenixFire>(transform.position, new Vector2(32, 0).RotatedBy(i / 15f * Mathf.PI), 15);
        }
        UniversalImmuneFrames = 200 * ImmunityFrameMultiplier;
        SpentBonusLives++;
        DeathKillTimer = 0;
        Life = TotalMaxLife;
        OnSetLife(Life);
    }
    public override void OnHurtByProjectile(Projectile proj)
    {
        if (UniversalImmuneFrames > 0)
            return;
        if (proj.Hostile && DeathKillTimer <= 0)
        {
            Hurt(1);
        }
    }
    public void OnSetLife(int value)
    {
        PlayerStatUI.SetHearts(value, Shield);
    }
    public void SetShield(int num)
    {
        Shield = num;
        PlayerStatUI.SetHearts(Life, num);
    }
    public void ImmuneFlashing()
    {
        if (UniversalImmuneFrames > 0 && !IsDead)
        {
            float percent = UniversalImmuneFrames / 100f;
            float totalFlashes = 8;
            float sin = Mathf.Cos(Mathf.PI * percent * 2 * totalFlashes);
            Visual.SetActive(sin <= 0);
        }
        else if (UniversalImmuneFrames <= 0 || IsDead)
        {
            UniversalImmuneFrames = 0;
            Visual.SetActive(true);
        }
    }
    public void OnUseAbility()
    {
        int dex = RollDex;
        //Debug.Log(dex);
        if (dex > 0)
        {
            if (dex >= 81 || Utils.RandFloat(1) < 0.19f + dex * 0.01f)
            {
                int allowedBoosts = 1 + dex;
                int existingSpeedBoosts = 0;
                foreach(Buff b in buffs)
                    if (b is SpeedBoost)
                        existingSpeedBoosts++;
                if(existingSpeedBoosts < allowedBoosts)
                    AddBuff<SpeedBoost>(5);
            }
        }
    }
    public void OnWaveEnd(int oldWaveNumber)
    {

    }
    public void OnWaveStart(int newWaveNumber)
    {
        if (newWaveNumber % 2 == 0)
        {
            if (HasBubbleShield && Shield < TotalMaxShield)
                SetShield(Shield + 1);
        }
        if(PowerUp.Get<EatenCake>().Stack > 0)
        {
            PowerUp.Get<QuantumCake>().PickUp();
            RemovePower(PowerUp.Get<EatenCake>().MyID);
        }
        LuckyStarItemsAcquiredThisWave = 0;
    }
}
