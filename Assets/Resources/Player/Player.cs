using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class NewControls
{
    #region control classes
    public class DirectionalBinding
    {
        public StickControl Stick;
        public ControlBinding Up, Left, Down, Right;
        public DirectionalBinding(StickControl stick)
        {
            Stick = stick;
        }
        public DirectionalBinding(ControlBinding up, ControlBinding left, ControlBinding down, ControlBinding right)
        {
            Up = up;
            Left = left;
            Down = down;
            Right = right;
        }
        public Vector2 GetVector2()
        {
            if(Stick != null)
                return Stick.ReadValue();
            return new Vector2(Right - Left, Up - Down).normalized;
        }
        public static implicit operator Vector2(DirectionalBinding c) => c.GetVector2();
        public float x => GetVector2().x;
        public float y => GetVector2().y;
        public new string ToString()
        {
            if(Stick == null)
            {
                return $"{Up.ToString()}, {Left.ToString()}, {Down.ToString()}, {Right.ToString()}";
            }
            return Stick.shortDisplayName;
        }
    }
    public abstract class ControlBinding
    {
        public ControlBinding(int bind)
        {
            Bind = bind;
        }
        protected int Bind;
        public virtual bool Get() => false;
        public virtual float GetContinuous() => Get() ? 1 : 0;
        public static implicit operator bool(ControlBinding c) => c.Get();
        public static implicit operator float(ControlBinding c) => c.GetContinuous();
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
                return $"{code}/{secondCode}";
            return code.ToString();
        }
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
    public class ButtonBinding : ControlBinding
    {
        public ButtonControl Control { get; set; }
        public ButtonBinding(ButtonControl control) : base(0)
        {
            Control = control;
        }
        public override bool Get() => Control.isPressed;
        public override string ToString()
        {
            return Control.shortDisplayName;
        }
        public override float GetContinuous()
        {
            return Control.ReadValue();
        }
    }
    public class VerboseButtonBinding : ButtonBinding
    {
        public ButtonControl[] Controls;
        public VerboseButtonBinding(ButtonControl control, params ButtonControl[] extras) : base(control)
        {
            Controls = extras;
        }
        public override bool Get()
        {
            if (base.Get())
                return true;
            foreach (ButtonControl k in Controls)
                if (k.isPressed)
                    return true;
            return false;
        }
        public override string ToString()
        {
            string t = Control.shortDisplayName;
            for (int i = 0; i < Controls.Length; ++i)
                t += ", " + Controls[i].shortDisplayName;
            return t;
        }
    }
    #endregion
    public NewControls(int ControlScheme)
    {
        var gamePad = Gamepad.current;
        bool controllerConnected = gamePad != null;
        //Gamepad.all;
        if (controllerConnected)
            Debug.Log(("Controller Connected: " + gamePad).WithColor(ColorHelper.SentinelBlue.ToHexString()));
        else
            Debug.Log("No Controllers Connected".WithColor(ColorHelper.SentinelGreen.ToHexString()));
        ControlSchemeType = ControlScheme;
        if (ControlSchemeType == 1 && controllerConnected)
            ControlSchemeType = 0;
        if(ControlSchemeType == 0) //Default
        {
            PrimaryAttackHold = new MouseHold(0);
            SecondaryAttackHold = new MouseHold(1);
            Movement = new DirectionalBinding(
                new KeyHold(KeyCode.W, KeyCode.UpArrow), 
                new KeyHold(KeyCode.A, KeyCode.LeftArrow),
                new KeyHold(KeyCode.S, KeyCode.DownArrow), 
                new KeyHold(KeyCode.D, KeyCode.RightArrow));
            Ability = new KeyHold(KeyCode.LeftShift, KeyCode.Space);
        }
        else if(ControlSchemeType == 1) //Player 1
        {
            PrimaryAttackHold = new MouseHold(0);
            SecondaryAttackHold = new MouseHold(1);
            Movement = new DirectionalBinding(
                new KeyHold(KeyCode.UpArrow),
                new KeyHold(KeyCode.LeftArrow),
                new KeyHold(KeyCode.DownArrow),
                new KeyHold(KeyCode.RightArrow));
            Ability = new KeyHold(KeyCode.RightControl);
        }  
        else //Player 2
        {
            if(controllerConnected) //Player 2/Controller
            {
                Movement = new DirectionalBinding(gamePad.leftStick);
                Aim = new DirectionalBinding(gamePad.rightStick);
                Ability = new VerboseButtonBinding(gamePad.xButton, gamePad.yButton, gamePad.aButton, gamePad.bButton, /*gamePad.leftStickButton, gamePad.rightStickButton,*/ gamePad.leftTrigger, gamePad.leftShoulder);
                PrimaryAttackHold = new ButtonBinding(gamePad.rightTrigger);
                SecondaryAttackHold = new ButtonBinding(gamePad.rightShoulder);
            }
            else
            {
                PrimaryAttackHold = new VerboseKeyHold(KeyCode.Space, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L);
                SecondaryAttackHold = new KeyHold(KeyCode.Space);
                Movement = new DirectionalBinding(
                    new KeyHold(KeyCode.W),
                    new KeyHold(KeyCode.A),
                    new KeyHold(KeyCode.S),
                    new KeyHold(KeyCode.D));
                Ability = new KeyHold(KeyCode.LeftShift);
                Aim = new DirectionalBinding(
                    new KeyHold(KeyCode.I),
                    new KeyHold(KeyCode.J),
                    new KeyHold(KeyCode.K),
                    new KeyHold(KeyCode.L));
            }
        }
    }
    public void UpdateMouse(Vector2 PlayerPosition)
    {
        if(ControlSchemeType == 0 || ControlSchemeType == 1 || Aim == null)
        {
            MousePosition = Utils.MouseWorld;
            AimingVector = MousePosition - PlayerPosition;
        }
        else
        {
            float currentAngle = AimingVector.ToRotation();
            float newAngle = currentAngle;
            Vector2 direction = Aim;
            if (direction != Vector2.zero)
                newAngle = direction.ToRotation();
            else if(Aim.Stick == null)
                newAngle = Mathf.Round(currentAngle * 4 / Mathf.PI) * Mathf.PI / 4f;
            currentAngle = Utils.LerpAngleRadians(currentAngle, newAngle, Utils.DeltaTimeLerpFactor(0.1f));
            AimingVector = new Vector2(1, 0).RotatedBy(currentAngle);
            MousePosition = AimingVector.normalized * 10f;
            float snapDist = 0.25f;
            if(MousePosition.x < snapDist && MousePosition.x > -snapDist)
                MousePosition.x = MousePosition.x > 0 ? snapDist : -snapDist;
            MousePosition += PlayerPosition;
        }
    }
    public int ControlSchemeType = 0;
    public readonly ControlBinding PrimaryAttackHold;
    public readonly ControlBinding SecondaryAttackHold;
    public readonly ControlBinding AimLeft, AimRight, AimDown, AimUp;
    public readonly DirectionalBinding Movement;
    public readonly DirectionalBinding Aim;
    public bool Up => Movement.y > .001f;
    public bool Left => Movement.x < -.001f;
    public bool Down => Movement.y < -.001f;
    public bool Right => Movement.x > .001f;
    public readonly ControlBinding Ability;
    public bool LastAbility { get; internal set; }
    public Vector2 MovementVector = Vector2.zero;
    private Vector2 AimingVector = Vector2.up;
    public Vector2 MousePosition;
    public string AllBindings()
    {
        string concat = string.Empty;
        string h = ColorHelper.SentinelGreen.ToHexString();
        concat += "Primary: ".WithColor(h) + PrimaryAttackHold.ToString();
        concat += "\nSecondary: ".WithColor(h) + SecondaryAttackHold.ToString();
        concat += "\nMovement: ".WithColor(h) + Movement.ToString();
        concat += "\nAim: ".WithColor(h) + (Aim != null ? Aim.ToString() : "Mouse");
        concat += "\nAbility: ".WithColor(h) + Ability.ToString();
        return concat;
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
    //TODO: Make this output the distance so it can be used in situations requiring distance easily
    public static Player FindClosest(Vector3 position, out Vector2 norm, float searchDistance = 10000)
    {
        searchDistance *= searchDistance;
        norm = Vector2.zero;
        Player best = null;
        foreach (Player e in AllPlayers)
        {
            Vector2 toDest = e.transform.position - position;
            float dist = toDest.sqrMagnitude;
            if (dist <= searchDistance)
            {
                best = e;
                searchDistance = dist;
                norm = toDest;
            }
        }
        norm = norm.normalized;
        return best;
    }
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
    public Vector2 Position => (Vector2)transform.position;
    public static Vector2 Instance1Pos => Instance != null ? (Vector2)Instance.transform.position : Vector2.zero;
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
        if(Main.UIManager.MPControls1 != null && Main.UIManager.MPControls2 != null)
        {
            var target = InstanceID == 0 ? Main.UIManager.MPControls1 : Main.UIManager.MPControls2;
            string s = Control.AllBindings();
            target.text = s;
        }
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
            movespeed.y += cSpeed * Control.Movement.y;
        }
        else if (!Control.Up && Control.Down)
        {
            if (velocity.y > 0)
                velocity.y *= MovementDeacceleration;
            movespeed.y += cSpeed * Control.Movement.y;
        }
        else
            velocity.y *= MovementDeacceleration;
        if (!Control.Right && Control.Left)
        {
            if (velocity.x > 0)
                velocity.x *= MovementDeacceleration;
            movespeed.x += cSpeed * Control.Movement.x;
        }
        else if (Control.Right && !Control.Left)
        {
            if (velocity.x < 0)
                velocity.x *= MovementDeacceleration;
            movespeed.x += cSpeed * Control.Movement.x;
        }
        else
            velocity.x *= MovementDeacceleration;
        movespeed = movespeed.normalized;
            
        abilityTimer -= Time.fixedDeltaTime * AbilityRecoverySpeed;
        if (abilityTimer < 0)
            abilityTimer = 0;

        Body.AbilityUpdate(ref velocity, movespeed);

        //Final stuff
        velocity += Control.Movement.GetVector2().magnitude * cSpeed * movespeed;
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
        Control.UpdateMouse(transform.position);
        if(Control.ControlSchemeType != 2)
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
                    Projectile.NewProjectile<SmallBubble>(transform.position, Utils.RandCircle(0, 1) * Utils.RandFloat(0.5f + i * 0.2f, velocity + i * 0.4f), 1, this);
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
            Projectile.NewProjectile<PhoenixFire>(transform.position, new Vector2(32, 0).RotatedBy(i / 15f * Mathf.PI), 15, this);
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
