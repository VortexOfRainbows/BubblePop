using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NewControls
{
    //private string[] joystickButtons = {
    //    "JoystickButton0", // Typically A (Xbox)/Cross (PS)
    //    "JoystickButton1", // B / Circle
    //    "JoystickButton2", // X / Square
    //    "JoystickButton3", // Y / Triangle
    //    "JoystickButton4", // LB / L1
    //    "JoystickButton5", // RB / R1
    //    "JoystickButton6", // Back / Share
    //    "JoystickButton7", // Start / Options
    //    "JoystickButton8", // L3
    //    "JoystickButton9"  // R3
    //};
    #region control classes
    public abstract class ControlBinding
    {
        public ControlBinding(int bind)
        {
            Bind = bind;
        }
        protected int Bind;
        public virtual bool Get() => false;
        public static implicit operator bool(ControlBinding c)
        {
            return c.Get();
        }
        public new virtual string ToString()
        {
            return Bind.ToString();
        }
    }
    public class EmptyBinding : ControlBinding
    {
        public EmptyBinding(int bind) : base(bind) { }
    }
    public class KeyHold : ControlBinding
    {
        protected readonly KeyCode code;
        protected readonly KeyCode secondCode;
        public KeyHold(KeyCode bind, KeyCode bind2 = KeyCode.None) : base((int)bind) { code = bind; secondCode = bind2; }
        public override bool Get() => Input.GetKey(code) || Input.GetKey(secondCode);
        public override string ToString()
        {
            if (secondCode != KeyCode.None)
                return $"{code}, {secondCode}";
            return code.ToString();
        }
    }
    public class KeyDown : KeyHold
    {
        public KeyDown(KeyCode bind, KeyCode bind2 = KeyCode.None) : base(bind, bind2) { }
        public override bool Get() => Input.GetKeyDown(code) || Input.GetKeyDown(secondCode);
    }
    public class VerboseKeyHold : ControlBinding
    {
        protected readonly KeyCode anti;
        protected readonly KeyCode[] codes;
        public VerboseKeyHold(KeyCode antiKey = KeyCode.None, params KeyCode[] code) : base(0) { anti = antiKey; codes = code; }
        public override bool Get()
        {
            if (Input.GetKey(anti))
                return false;
            foreach(KeyCode k in codes)
                if (Input.GetKey(k))
                    return true;
            return false;
        }
        public override string ToString()
        {
            string t = string.Empty;
            for (int i = 0; i < codes.Length; ++i)
                t += codes[i].ToString() + (i < codes.Length - 1 ? ", " : "");
            return t;
        }
    }
    public class VerboseKeyDown : VerboseKeyHold
    {
        public VerboseKeyDown(KeyCode antiKey = KeyCode.None, params KeyCode[] code) : base(antiKey, code) { }
        public override bool Get()
        {
            if (Input.GetKey(anti))
                return false;
            foreach (KeyCode k in codes)
                if (Input.GetKeyDown(k))
                    return true;
            return false;
        }
    }
    public class MouseHold : ControlBinding
    {
        public MouseHold(int bind) : base(bind) { }
        public override bool Get() => Input.GetMouseButton(Bind);
        public override string ToString()
        {
            return "Mouse" + base.ToString();
        }
    }
    public class MouseDown : ControlBinding
    {
        public MouseDown(int bind) : base(bind) { }
        public override bool Get() => Input.GetMouseButtonDown(Bind);

    }
    #endregion
    public NewControls(int ControlScheme)
    {
        bool controllerConnected = false;
        var connectedControllers = Input.GetJoystickNames();
        if (connectedControllers.Length > 0)
        {
            foreach (string s in connectedControllers)
                Debug.Log(("Controller Connected: " + s).WithColor(ColorHelper.SentinelBlue.ToHexString()));
            controllerConnected = true;
        }
        else
        {
            Debug.Log("No Controllers Connected".WithColor(ColorHelper.SentinelGreen.ToHexString()));
        }
        ControlSchemeType = ControlScheme;
        if(ControlSchemeType == 0) //Default
        {
            PrimaryAttackHold = new MouseHold(0);
            PrimaryAttackStart = new MouseDown(0);
            SecondaryAttackHold = new MouseHold(1);
            SecondaryAttackStart = new MouseDown(1);
            Up = new KeyHold(KeyCode.W, KeyCode.UpArrow);
            Left = new KeyHold(KeyCode.A, KeyCode.LeftArrow);
            Down = new KeyHold(KeyCode.S, KeyCode.DownArrow);
            Right = new KeyHold(KeyCode.D, KeyCode.RightArrow);
            Ability = new KeyHold(KeyCode.LeftShift, KeyCode.Space);
        }
        else if(ControlSchemeType == 1) //Player 1
        {
            PrimaryAttackHold = new MouseHold(0);
            PrimaryAttackStart = new MouseDown(0);
            SecondaryAttackHold = new MouseHold(1);
            SecondaryAttackStart = new MouseDown(1);
            Up = new KeyHold(KeyCode.UpArrow);
            Left = new KeyHold(KeyCode.LeftArrow);
            Down = new KeyHold(KeyCode.DownArrow);
            Right = new KeyHold(KeyCode.RightArrow);
            Ability = new KeyHold(KeyCode.RightControl);
        }  
        else //Player 2
        {
            PrimaryAttackHold = new VerboseKeyHold(KeyCode.Space, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L);
            PrimaryAttackStart = new VerboseKeyDown(KeyCode.Space, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L);
            SecondaryAttackHold = new KeyHold(KeyCode.Space);
            SecondaryAttackStart = new KeyDown(KeyCode.Space);
            Up = new KeyHold(KeyCode.W);
            Left = new KeyHold(KeyCode.A);
            Down = new KeyHold(KeyCode.S);
            Right = new KeyHold(KeyCode.D);
            Ability = new KeyHold(KeyCode.LeftShift);
            AimUp = new KeyHold(KeyCode.I);
            AimLeft = new KeyHold(KeyCode.J);
            AimDown = new KeyHold(KeyCode.K);
            AimRight = new KeyHold(KeyCode.L);
            if(controllerConnected) //Player 2/Controller
            {
                SecondaryAttackHold = new KeyHold(KeyCode.JoystickButton4);
                SecondaryAttackStart = new KeyDown(KeyCode.JoystickButton4);
            }
        }
    }
    public void UpdateMouse(Vector2 PlayerPosition, float playerDirection)
    {
        if(ControlSchemeType == 0 || ControlSchemeType == 1)
        {
            MousePosition = Utils.MouseWorld;
            toMouse = MousePosition - PlayerPosition;
        }
        else if (ControlSchemeType == 2)
        {
            float currentAngle = toMouse.ToRotation();
            float newAngle;
            Vector2 direction = Vector2.zero;
            if(AimUp)
                direction += Vector2.up;
            if(AimDown)
                direction += Vector2.down;
            if (AimLeft)
                direction += Vector2.left;
            if (AimRight)
                direction += Vector2.right;
            if (direction != Vector2.zero)
                newAngle = direction.ToRotation();
            else
                newAngle = Mathf.Round(currentAngle * 4 / Mathf.PI) * Mathf.PI / 4f;
            currentAngle = Utils.LerpAngleRadians(currentAngle, newAngle, Utils.DeltaTimeLerpFactor(0.1f));
            toMouse = new Vector2(1, 0).RotatedBy(currentAngle);
            MousePosition = PlayerPosition + toMouse.normalized * 10f;
        }
    }
    public int ControlSchemeType = 0;
    public readonly ControlBinding PrimaryAttackHold;
    public readonly ControlBinding PrimaryAttackStart;
    public readonly ControlBinding SecondaryAttackHold;
    public readonly ControlBinding SecondaryAttackStart;
    public readonly ControlBinding AimLeft, AimRight, AimDown, AimUp;
    public readonly ControlBinding Up;
    public readonly ControlBinding Left;
    public readonly ControlBinding Down;
    public readonly ControlBinding Right;
    public readonly ControlBinding Ability;
    public bool LastAbility { get; internal set; }
    public Vector2 MousePosition;
    private Vector2 toMouse = Vector2.up;
    public void PrintAllBindings()
    {
        string concat = string.Empty;
        concat += "Primary: " + PrimaryAttackHold.ToString();
        concat += "\nSecondary: " + SecondaryAttackHold.ToString();
        concat += "\nUp: " + Up.ToString();
        concat += "\nLeft: " + Left.ToString();
        concat += "\nDown: " + Down.ToString();
        concat += "\nRight: " + Right.ToString();
        concat += "\nAbility: " + Ability.ToString();
        Debug.Log(concat);
    }
}
public static class Control
{
    public static bool Interact => Input.GetKeyDown(KeyCode.E);
    public static bool Tab => Input.GetKey(KeyCode.Tab);
    public static bool LeftMouseClick => Input.GetMouseButtonDown(0);
    public static bool RightMouseClick => Input.GetMouseButtonDown(1);
}
public partial class Player : Entity
{
    public static readonly List<Player> AllPlayers = new();
    public NewControls Control { get; private set; }
    public SpriteRenderer AimIndicator;
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
    public new Rigidbody2D RB => Animator.rb;
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
        Control = new(AllPlayers.Count > 1 ? InstanceID + 1 : 0);
        Debug.Log($"Initialized Player With Control Scheme: [{Control.ControlSchemeType}]");
        Control.PrintAllBindings();
    }
    public float abilityTimer = 0;
    private void MovementUpdate()
    {
        Vector2 velocity = RB.velocity;
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
        RB.velocity = velocity;
        Control.LastAbility = Control.Ability;
        Body.AliveUpdate();
        Animator.PostUpdate();
    }
    private float AttackUpdateTimer = 0;
    public override void OnFixedUpdate()
    {
        Body.Player = Accessory.Player = Weapon.Player = Hat.Player = this; //There's probably a better way to do this
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
            Entity.PushIntoClosestPossibleTile(transform, base.RB, includeProgressionBounds: true);
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
                if (Control.PrimaryAttackHold)
                {
                    HasAttacked = true;
                    Weapon.StartAttack(false);
                }
                else if (Control.SecondaryAttackHold)
                {
                    HasAttacked = true;
                    Weapon.StartAttack(true);
                }
            }
            if (InstanceID == 0 && Main.GameUpdateCount % 200 == 0)
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
    public void LateUpdate()
    {
        if(Body != null)
            Control.UpdateMouse(transform.position, Body.FlipDir);
        if(Control.ControlSchemeType == 0)
        {
            AimIndicator.gameObject.SetActive(false);
        }
        else
        {
            if(InstanceID == 0)
                AimIndicator.color = new Color(0.5f, 0.5f, 1f);
            else
                AimIndicator.color = new Color(1f, 0.5f, 0.5f);
            AimIndicator.gameObject.SetActive(true);
            Vector2 toMouse = Control.MousePosition - (Vector2)transform.position;
            AimIndicator.transform.localPosition = toMouse.normalized * 2.5f;
            AimIndicator.transform.SetEulerZ(toMouse.ToRotation() * Mathf.Rad2Deg);
        }
        //THIS IS FOR DEBUG AN SHOULD BE REMOVED AFTER CONTROLS ARE FIGURED OUT
        if(InstanceID == 1)
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    Debug.Log("Key pressed: " + key.ToString());
                    break; // Stop after finding the first pressed key
                }
            }
    }
    public void SetLife(int num)
    {
        if (num > TotalMaxLife)
            num = TotalMaxLife;
        Life = num;
        OnSetLife(Life);
    }
    public void Hurt(int damage = 1)
    {
        for(int i = 0; i < GlassShards; ++i)
            if(Utils.RandFloat() < 0.25f)
                ++damage;
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
                    Projectile.NewProjectile<SmallBubble>(transform.position, Utils.RandCircle(0, 1) * Utils.RandFloat(0.5f + i * 0.2f, velocity + i * 0.4f), 1);
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
        RB.velocity *= 0.9f;
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
            PowerUp.Get<QuantumCake>().PickUp(this);
            RemovePower(PowerUp.Get<EatenCake>().MyID);
        }
        LuckyStarItemsAcquiredThisWave = 0;
    }
}
