using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class NewControls
{
    public static Dictionary<KeyCode, string> LocalizedValue = GenLocals();
    public static Dictionary<KeyCode, string> GenLocals()
    {
        Dictionary<KeyCode, string> dict = new()
        {
            [KeyCode.LeftArrow] = "LArrow",
            [KeyCode.RightArrow] = "RArrow",
            [KeyCode.DownArrow] = "DArrow",
            [KeyCode.UpArrow] = "UArrow",
            [KeyCode.LeftShift] = "LShift",
            [KeyCode.RightShift] = "RShift",
            [KeyCode.LeftControl] = "LCtrl",
            [KeyCode.RightControl] = "RCtrl"
        };
        return dict;
    }
    public static string KeyCodeToString(KeyCode code)
    {
        if(LocalizedValue.TryGetValue(code, out string val))
            return val;
        return code.ToString();
    }
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
                if (Up.ToString() == "UArrow" && Left.ToString() == "LArrow" && Down.ToString() == "DArrow" && Right.ToString() == "RArrow")
                    return "Arrows";
                if (Up.ToString() == "W" && Left.ToString() == "A" && Down.ToString() == "S" && Right.ToString() == "D")
                    return "WASD";
                else if (Up.ToString() == "I" && Left.ToString() == "J" && Down.ToString() == "K" && Right.ToString() == "L")
                    return "IJKL";
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
            return KeyCodeToString(code);
            //if (secondCode != KeyCode.None)
            //    return $"{KeyCodeToString(code)} or {KeyCodeToString(secondCode)}";
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
                t += KeyCodeToString(codes[i]) + (i < codes.Length - 1 ? "/" : "");
            return t;
        }
    }
    public class MouseHold : ControlBinding
    {
        public MouseHold(int bind) : base(bind) { }
        public override bool Get() => Input.GetMouseButton(Bind);
        public override string ToString()
        {
            return "Mouse" + (Bind + 1);
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
                t += "/" + Controls[i].shortDisplayName;
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
                ControlSchemeType = 3;
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
            currentAngle = Utils.LerpAngleRadians(currentAngle, newAngle, Utils.DeltaTimeLerpFactor(0.4f));
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
    public bool PendingBlock { get; internal set; }
    public bool BlockAttackState { get; internal set; }
    public bool BlockAttack 
    { 
        get => BlockAttackState; 
        set => PendingBlock = value;
    }
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
    public bool CheckIfControlsAreCorrect()
    {
        if (Player.AllPlayers.Count <= 1)
            return true;
        var gamePad = Gamepad.current;
        bool controllerConnected = gamePad != null;
        if(controllerConnected)
            return ControlSchemeType == 0 || ControlSchemeType == 3;
        else
            return ControlSchemeType == 1 || ControlSchemeType == 2;
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
    public TextMeshPro PlayerNumber;
    public static readonly List<Player> AllPlayers = new();
    //TODO: Make this output the distance so it can be used in situations requiring distance easily
    public static Player FindClosest(Vector3 position, out Vector2 norm, out float distanceToPlayer, float searchDistance = 10000)
    {
        searchDistance *= searchDistance;
        norm = Vector2.zero;
        Player best = null;
        foreach (Player e in AllPlayers)
        {
            if (e == null)
                continue;
            Vector2 toDest = e.transform.position - position;
            float dist = toDest.sqrMagnitude;
            if (dist <= searchDistance)
            {
                best = e;
                searchDistance = dist;
                norm = toDest;
            }
        }
        distanceToPlayer = norm.magnitude;
        norm /= distanceToPlayer;
        return best;
    }
    public static Player FindFarthest(Vector3 position, out Vector2 norm, out float distanceToPlayer)
    {
        float searchDistance = 0;
        norm = Vector2.zero;
        Player best = null;
        foreach (Player e in AllPlayers)
        {
            if (e == null)
                continue;
            Vector2 toDest = e.transform.position - position;
            float dist = toDest.sqrMagnitude;
            if (dist >= searchDistance)
            {
                best = e;
                searchDistance = dist;
                norm = toDest;
            }
        }
        distanceToPlayer = norm.magnitude;
        norm /= distanceToPlayer;
        return best;
    }
    public NewControls Control { get; private set; } //For some reason this crashes when assigned = new NewControls(0);
    public SpriteRenderer AimIndicator;
    public int InstanceID = 0;
    public static Player GetInstance(int instanceID = 0)
    {
        return AllPlayers.Count <= instanceID ? null : AllPlayers[instanceID];
    }
    private int Shield = 0;
    public new int Life = 0;
    public new int MaxLife = 0;
    public int GetShield() => Shield;
    public PowerUp MostRecentPower = null;
    #region Animator References
    public ref Body Body => ref Animator.Body;
    public ref Weapon Weapon => ref Animator.Weapon;
    public ref Hat Hat => ref Animator.Hat;
    public ref Accessory Accessory => ref Animator.Accessory;
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
    private readonly float speed = 2.5f;
    public readonly float MovementDeacceleration = 0.9f;
    public const float DashDefault = 25f;
    public static bool HasAttacked = false;
    public static bool PickedLowerDifficultyWaveCard = false;
    public static bool HasBeenHit => TimesHitThisRun > 0;
    public static int TimesHitThisRun = 0;
    public static int GoldSpentTotal = 0;
    public static List<Type> DifferentTypesOfSkullEnemiesZappedThisRun { get; set; } = new();
    /// <summary>
    /// Shorthand for abilityTimer <= 0;
    /// </summary>
    public bool AbilityReady => abilityTimer <= 0;
    public bool AbilityOnCooldown => abilityTimer > 0;
    private bool HasRunStartingGear = false;
    public float MaxSpeed => MoveSpeedMod * 6f;
    public int bonusBubbles = 0;
    public bool IsDead => DeathKillTimer > 0;
    //public void Awake()
    //{
    //    if (!HasCalledInit)
    //    {
    //        Init();
    //        HasCalledInit = true;
    //    }
    //}
    public override void Init()
    {
        PowerInit();
        WaveDirector.Reset();
        CameraManager.SetCameraOrthographicSize(12);
        DeathKillTimer = 0;
        PickedUpPhoenixLivesThisRound = SpentBonusLives = 0;
        HasRunStartingGear = HasWonThisCycle = false;
        Life = MaxLife = 3;
        if(InstanceID == 0)
            PlayerStatUI.ResetLife();
        Control = new(AllPlayers.Count > 1 ? InstanceID + 1 : 0);
        Debug.Log($"Initialized Player With Control Scheme: [{Control.ControlSchemeType}]");
        if (Main.UIManager.MPControls1 != null && Main.UIManager.MPControls2 != null)
        {
            var target = InstanceID == 0 ? Main.UIManager.MPControls1 : Main.UIManager.MPControls2;
            string s = Control.AllBindings();
            target.text = s;
            if (InstanceID == 0 && Main.UIManager.SPControls != null)
            {
                Main.UIManager.SPControls.text = s;
            }
        }
        PlayerNumber.color = InstanceID == 0 ? ColorHelper.Player1Color : ColorHelper.Player2Color;
        PlayerNumber.text = InstanceID == 0 ? "P1" : "P2";
        PlayerNumber.gameObject.SetActive(AllPlayers.Count > 1);
        PowerCountIncludingStacks = 0;
        BestPowerCountIncludingStacks = 0;
        TotalBlackMarketPowersPickedUp = 0;
        TotalNonBlackMarketPowersPickedUp = 0;
    }
    public float abilityTimer = 0;
    private void MovementUpdate()
    {
        Vector2 velocity = RB.velocity;
        float cSpeed = speed * MoveSpeedMod;
        float cDeaccel = MovementDeacceleration;
        float cMaxSpeed = MaxSpeed;
        if (Body is Fizzy f)
        {
            if (f.SkateboardPercent > 0)
            {
                if (!f.OnSkateboard || f.SkateboardPercent >= 1)
                {
                    cDeaccel = Mathf.Lerp(cDeaccel, 0.9985f, f.SkateboardPercent);
                    cSpeed *= Mathf.Lerp(1, 0.225f, f.SkateboardPercent);
                    cMaxSpeed *= Mathf.Lerp(1, 3.75f, f.SkateboardPercent);
                }
                else
                {
                    float iPer = 1 - f.SkateboardPercent;
                    cSpeed *= 0.1f + 0.9f * iPer * iPer;
                    velocity *= 0.98f;
                }
                if (!f.OnSkateboard || f.SkateboardPercent < 1)
                {
                    float bonusSkateboardSpeed = 1.0f + Kickflip * (f.OnSkateboard ? 0.3f : 0.15f);
                    cSpeed *= bonusSkateboardSpeed;
                }
            }
        }
        Vector2 movespeed = Vector2.zero;
        if (Control.Up && !Control.Down)
        {
            if (velocity.y < 0)
                velocity.y *= cDeaccel;
            movespeed.y += cSpeed * Control.Movement.y;
        }
        else if (!Control.Up && Control.Down)
        {
            if (velocity.y > 0)
                velocity.y *= cDeaccel;
            movespeed.y += cSpeed * Control.Movement.y;
        }
        else
            velocity.y *= cDeaccel;
        if (!Control.Right && Control.Left)
        {
            if (velocity.x > 0)
                velocity.x *= cDeaccel;
            movespeed.x += cSpeed * Control.Movement.x;
        }
        else if (Control.Right && !Control.Left)
        {
            if (velocity.x < 0)
                velocity.x *= cDeaccel;
            movespeed.x += cSpeed * Control.Movement.x;
        }
        else
            velocity.x *= cDeaccel;
        movespeed = movespeed.normalized;

        abilityTimer -= Time.fixedDeltaTime * AbilityRecoverySpeed;
        if (abilityTimer < 0)
            abilityTimer = 0;

        Body.AbilityUpdate(ref velocity, movespeed);

        //Final stuff
        velocity += Control.Movement.GetVector2().magnitude * cSpeed * movespeed;
        float currentSpeed = velocity.magnitude;
        if (currentSpeed > cMaxSpeed)
        {
            Vector2 norm = velocity.normalized;
            velocity = norm * (cMaxSpeed + (currentSpeed - cMaxSpeed) * 0.8f);
            if (Body is Bubblemancer)
            {
                if (currentSpeed > cMaxSpeed + 15f * MoveSpeedMod)
                {
                    for (float i = 0; i < 1; i += 0.5f)
                        ParticleManager.NewParticle((Vector2)transform.position + i * Time.fixedDeltaTime * velocity + Utils.RandCircle(i * 2) - norm * .5f, .5f, norm * -Utils.RandFloat(15f), 1.0f, 0.6f, 0, Body.PrimaryColor);
                }
            }
        }
        if (Body is Fizzy fiz && fiz.OnSkateboard)
        {
            Vector2 norm = velocity.normalized;
            if (currentSpeed > cMaxSpeed || Utils.RandFloat() < 0.15f)
                ParticleManager.NewParticle((Vector2)fiz.Skateboard.transform.position + new Vector2(0, .3f) + Utils.RandCircle(0.25f) - norm * .5f, .5f, norm * -Utils.RandFloat(15f), 1.0f, 0.6f, 0, Body.PrimaryColor);
        }
        RB.velocity = velocity;
        Control.LastAbility = Control.Ability;
        Body.AliveUpdate();
        Animator.PostFixedUpdate();
    }
    private float AttackUpdateTimer = 0;
    public override void OnFixedUpdate()
    {
        Body.Player = Accessory.Player = Weapon.Player = Hat.Player = this; //There's probably a better way to do this
        if (!HasRunStartingGear && Main.WavesUnleashed)
        {
            foreach (Equipment e in Equips)
            {
                e.OnStartWith();
                e.TotalTimesUsed = e.TotalTimesUsed + 1;
            }
            HasRunStartingGear = true;
            HasAttacked = false;
            TimesHitThisRun = 0;
            DifferentTypesOfSkullEnemiesZappedThisRun.Clear();
        }
        WaveDirector.FixedUpdate();
        if (!HasRunStartingGear)
        {
            MaxLife = Life = Shield = 0;
            //These must go in a certain order, since body establishes the health first, so dont change this to a for loop
            Body.ModifyLifeStats(ref MaxLife, ref Life, ref Shield);
            Hat.ModifyLifeStats(ref MaxLife, ref Life, ref Shield);
            Accessory.ModifyLifeStats(ref MaxLife, ref Life, ref Shield);
            Weapon.ModifyLifeStats(ref MaxLife, ref Life, ref Shield);
            if (AllPlayers.Count > 1)
            {
                MaxLife += 2;
                Life += 2;
            }
            if(InstanceID == 0)
                PlayerStatUI.SetHeartsToPlayerLife();
            ModifyAscensionLevel(0); //Might be better to move this to the place where body is set or changed, but this works for now.
        }
        UpdatePowerUps();
        UpdateBuffs();
        HomingRangeSqrt = Mathf.Sqrt(HomingRange);
        bool dead = DeathKillTimer > 0;
        CreateRoadblockBarriers();
        if (!World.WithinBorders(transform.position, true))
            Entity.PushIntoClosestPossibleTile(transform, base.RB, includeProgressionBounds: true);
        CheckForClosestObjects();
        if (dead)
        {
            Pop();
            //Main.MouseHoveringOverButton = false;
        }
        else
        {
            if (HasRunStartingGear)
            {
                //Debug.Log($"{Shield}, {MaxShield}, {TotalMaxShield}");
                if (TotalMaxLife < Life)
                    Life = TotalMaxLife;
                if (TotalMaxShield < Shield)
                    Shield = TotalMaxShield;
                if(InstanceID == 0)
                    PlayerStatUI.SetHeartsToPlayerLife();
            }
            BonusChoices = false;
            CoinsOnPowerPickup = 0; //This update needs to happen here so the reset works for mitosis
            foreach (Equipment e in Equips)
                e.EquipUpdate();
            PostEquipUpdate();
            foreach (Equipment e in Equips)
                e.PostEquipUpdate();
            MovementUpdate();
            if (!Control.BlockAttack)
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
            //if (InstanceID == 0 && Main.GameUpdateCount % 200 == 0)
            //    Debug.Log($"Has Attacked: {HasAttacked}, Picked Lower Card: {PickedLowerDifficultyWaveCard}, Has Taken Damage: {HasBeenHit}:{TimesHitThisRun}");
            if (Weapon.IsAttacking())
            {
                if (Weapon.IsPrimaryAttacking())
                    AttackUpdateTimer += PrimaryAttackSpeedModifier;
                if (Weapon.IsSecondaryAttacking())
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
        if (InstanceID == 0)
        {
            int playerCount = 0;
            Vector2 average = Vector2.zero;
            foreach (Player p in AllPlayers)
            {
                average += (Vector2)p.transform.position;
                playerCount++;
            }
            if (playerCount <= 0)
                playerCount = 1;
            average /= playerCount;
            float size = 15f;
            if (playerCount > 1)
            {
                float biggestDistToCenter = 0;
                foreach (Player p in AllPlayers)
                    biggestDistToCenter = Mathf.Max(biggestDistToCenter, Mathf.Max(Mathf.Abs(p.Position.x - average.x), Mathf.Abs(p.Position.y - average.y)));
                size = Mathf.Max(size, Mathf.Min(30, biggestDistToCenter * 1.16f + 6));
            }
            CameraManager.LerpCameraOrthographicSize(size, 0.03f);
            CameraManager.LerpCameraPosition(new Vector2(average.x, average.y), 0.1f);
        }
        ImmuneFlashing();
        //if(!dead)
        //{
        //    Roadblock.DoRoadblockVisual(ref RoadblockCounter, transform.position, Main.PylonProgressionNumber + 1, 1);
        //}
    }
    public GameObject RoadblockBarrier;
    public void CreateRoadblockBarriers()
    {
        bool hasBarriersAround = false;
        bool left = World.IsRoadblocked(Position + new Vector2(-2, 0));
        bool right = World.IsRoadblocked(Position + new Vector2(2, 0));
        bool top = World.IsRoadblocked(Position + new Vector2(0, 2));
        bool bot = World.IsRoadblocked(Position + new Vector2(0, -2));
        bool topLeft = World.IsRoadblocked(Position + new Vector2(-2, 2));
        bool botRight = World.IsRoadblocked(Position + new Vector2(2, -2));
        bool topRight = World.IsRoadblocked(Position + new Vector2(2, 2));
        bool botLeft = World.IsRoadblocked(Position + new Vector2(-2, -2));
        if (left || right || bot || top || topLeft || botRight || topRight || botLeft)
            hasBarriersAround = true;

        if (hasBarriersAround)
        {
            if (RoadblockBarrier == null)
                RoadblockBarrier = GameObject.Instantiate(Main.PrefabAssets.Roadblocker, Position, Quaternion.identity);
            RoadblockBarrier.transform.GetChild(0).gameObject.SetActive(left);
            RoadblockBarrier.transform.GetChild(1).gameObject.SetActive(right);
            RoadblockBarrier.transform.GetChild(2).gameObject.SetActive(top);
            RoadblockBarrier.transform.GetChild(3).gameObject.SetActive(bot);
            RoadblockBarrier.transform.GetChild(4).gameObject.SetActive(topLeft);
            RoadblockBarrier.transform.GetChild(5).gameObject.SetActive(botRight);
            RoadblockBarrier.transform.GetChild(6).gameObject.SetActive(topRight);
            RoadblockBarrier.transform.GetChild(7).gameObject.SetActive(botLeft);
            RoadblockBarrier.transform.position = World.RealTileMap.Map.GetCellCenterWorld(World.RealTileMap.Map.WorldToCell(Position));
        }
        else
        {
            if (RoadblockBarrier != null)
                Destroy(RoadblockBarrier);
        }
    }
    public bool RunOnce { get; set; } = true;
    public new void Update()
    {
        if (RunOnce && InstanceID == 0 && Main.UIManager.MultiplayerMenu != null)
        {
            Main.UIManager.OpenMultiplayerMenu(false);
            RunOnce = false;
        }
        if (!Control.CheckIfControlsAreCorrect())
        {
            Control = new NewControls(InstanceID + 1);
            if (Main.UIManager.MPControls1 != null && Main.UIManager.MPControls2 != null)
            {
                var target = InstanceID == 0 ? Main.UIManager.MPControls1 : Main.UIManager.MPControls2;
                string s = Control.AllBindings();
                target.text = s;
            }
            Main.UIManager.OpenMultiplayerMenu(true);
        }
        base.Update();
    }
    public void LateUpdate()
    {
        Control.UpdateMouse(transform.position);
        if (Control.ControlSchemeType <= 1)
        {
            AimIndicator.gameObject.SetActive(false);
        }
        else
        {
            if (InstanceID == 0)
                AimIndicator.color = new Color(0.5f, 0.5f, 1f);
            else
                AimIndicator.color = new Color(1f, 0.5f, 0.5f);
            AimIndicator.gameObject.SetActive(true);
            Vector2 toMouse = Control.MousePosition - (Vector2)transform.position;
            AimIndicator.transform.localPosition = toMouse.normalized * 2.5f;
            AimIndicator.transform.SetLocalEulerZ(toMouse.ToRotation() * Mathf.Rad2Deg);
        }
        if (Control.PrimaryAttackHold || Control.SecondaryAttackHold)
        {

        }
        else
        {
            if (Control.PendingBlock) //pending block
                Control.BlockAttackState = true; //block
            else
                Control.BlockAttackState = false;
            Control.PendingBlock = false;
        }
        if (AllPlayers.Count > 1)
        {
            float distToOtherPlayer = Distance(AllPlayers[InstanceID == 0 ? 1 : 0].gameObject);
            PlayerNumber.color = PlayerNumber.color.WithAlpha(Mathf.Lerp(PlayerNumber.color.a, distToOtherPlayer < 12 ? 1 : 0, Utils.DeltaTimeLerpFactor(0.15f)));
        }
    }
    public void SetLife(int num)
    {
        if (num > TotalMaxLife)
            num = TotalMaxLife;
        if(num > Life)
        {
            if(ChoiceOnHeal > 0)
                for (int i = 0; i < ChoiceOnHeal; i++)
                    PowerUp.Spawn<Choice>(transform.position);
        }
        Life = num;
        OnSetLife(Life);
    }
    public void Hurt(int damage = 1)
    {
        OnHurtEffects();
        if (damage > 0)
        {
            for (int i = 0; i < GlassShards; ++i)
                if (Utils.RollWithLuckRaw() > 0.5f)
                    ++damage;
            TimesHitThisRun += damage;
            if (TimesHitThisRun >= 15 && this.Body is Gachapon)
            {
                UnlockCondition.Get<GachaponHealer>().SetComplete();
            }
            if (AllPlayers.Count > 1)
            {
                if (InstanceID == 0)
                    AllPlayers[1].Hurt(-damage);
                else
                    AllPlayers[0].Hurt(-damage);
            }
        }
        else
            damage = -damage;
        bool skipDamageStep = false;
        if (DodgeStat > 0)
        {
            bool dodge = Utils.RandFloat() < DodgeStat / Mathf.Pow(2, ConsecutiveDodges);
            if (dodge)
                skipDamageStep = true;
        }
        if (!skipDamageStep)
            ConsecutiveDodges = 0;
        else
        {
            PopupText.NewPopupText(transform.position, Vector2.up * 8 + Utils.RandCircle(4), Color.gray, "DODGE", true, 0.8f, 100);
            ConsecutiveDodges++;
            if (ConsecutiveDodges >= 3)
                if (Body is Fizzy)
                    UnlockCondition.Get<FizzyThinkImJustGonnaStandThereAndTakeIt>().SetComplete();
        }
        float defaultImmuneFrames = 100;
        float immuneMult = ImmunityFrameMultiplier;
        if (skipDamageStep)
            immuneMult *= 0.75f;
        int shieldDamage = Mathf.Min(damage, Shield);
        int lifeDamage = damage - shieldDamage;
        if (shieldDamage > 0)
        {
            if (!skipDamageStep)
                SetShield(Shield - shieldDamage);
            OnShieldHurtEffects();
            immuneMult *= ShieldImmunityFrameMultiplier;
        }
        if (lifeDamage > 0)
        {
            if (!skipDamageStep)
                SetLife(Life - lifeDamage);
        }
        UniversalImmuneFrames = defaultImmuneFrames * immuneMult;
        if (Life <= 0)
            Pop();
        else
            Body.ModifyHurtAnimation();
    }
    public void OnHurtEffects()
    {
        if (RetaliatoryBomb > 0)
        {
            Vector2 velo = Utils.RandCircle(3);
            Vector2 endPos = velo + (Vector2)transform.position;
            Projectile.NewProjectile<BathBomb>(transform.position, velo, RetaliatoryBomb * 10, this, endPos.x, endPos.y);
        }
    }
    public void OnShieldHurtEffects()
    {
        if (BubbleShields > 0)
        {
            float velocity = 2;
            int amt = 16 + 8 * BubbleShields;
            if (amt > 1024)
                amt = 1024;
            for (int i = 0; i < amt; ++i)
                Projectile.NewProjectile<SmallBubble>(transform.position, Utils.RandCircle(0, 1) * Utils.RandFloat(0.5f + i * 0.2f, velocity + i * 0.4f), 1, this);
        }
        if (GladiatorDuration > 0)
        {
            AddBuff<OmniBoost>(GladiatorDuration);
        }
        if (CatalystJellies > 0)
        {
            int stack = CatalystJellies;
            List<int> types = new() { PowerUp.Get<Dash>().Type, PowerUp.Get<BinaryStars>().Type, PowerUp.Get<Starbarbs>().Type, PowerUp.Get<LuckyStar>().Type, PowerUp.Get<Supernova>().Type };
            int type = PowerUp.PickRandomPower(types, 0, 0, false, -1);
            PowerUp.Get(type).PickUp(this, stack);
        }
    }
    public void Pop()
    {
        //Time.timeScale = 0.5f + 0.5f * Mathf.Sqrt(Mathf.Max(0, 1 - DeathKillTimer / 200f));
        if (InstanceID == 0)
        {
            if (DeathKillTimer > 100)
                CameraManager.LerpCameraOrthographicSize(6, 0.03f);
            else
                CameraManager.LerpCameraOrthographicSize(17, 0.03f);
        }
        RB.velocity *= 0.9f;
        Body.DeadUpdate();
        Hat.DeadUpdate();
        Weapon.DeadUpdate();
        Accessory.DeadUpdate();
        DeathKillTimer++;
        if (Input.GetKey(KeyCode.R) && Main.DebugCheats)
        {
            DeathKillTimer = 0;
            //Body.SetActive(true);
        }
        if (DeathKillTimer > 200)
            RegisterDeath();
    }
    public void RegisterDeath()
    {
        if (InstanceID == 1 || Player.AllPlayers.Count <= 1)
            PlayerData.PlayerDeaths++;
        if (BonusPhoenixLives > 0)
        {
            if (InstanceID == 1 || Player.AllPlayers.Count <= 1)
                RemovePower(PowerUp.Get<BubbleBirb>().Type, 1);
            Rebirth();
            return;
        }
        CoinManager.AfterDeathReset();
        Main.CanvasManager.GameOver();
    }
    public void Rebirth()
    {
        if (Accessory is RedCape)
            PowerUp.Spawn<Contract>(transform.position);
        for (int i = 0; i < 30; i++)
        {
            Projectile.NewProjectile<PhoenixFire>(transform.position, new Vector2(32, 0).RotatedBy(i / 15f * Mathf.PI), 15, this);
        }
        UniversalImmuneFrames = 200 * ImmunityFrameMultiplier;
        SpentBonusLives++;
        DeathKillTimer = 0;
        SetLife(TotalMaxLife);
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
        if (InstanceID == 0)
            PlayerStatUI.SetHeartsToPlayerLife();
    }
    public void SetShield(int num)
    {
        Shield = num;
        if (Shield > TotalMaxShield)
            Shield = TotalMaxShield;
        if(Shield < 0)
            Shield = 0;
        if (InstanceID == 0)
            PlayerStatUI.SetHeartsToPlayerLife();
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
                foreach (Buff b in buffs)
                    if (b is SpeedBoost)
                        existingSpeedBoosts++;
                if (existingSpeedBoosts < allowedBoosts)
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
        if (InstanceID == 0)
        {
            if (PowerUp.Get<EatenCake>().Stack > 0)
            {
                PowerUp.Get<QuantumCake>().PickUp(this);
                RemovePower(PowerUp.Get<EatenCake>().MyID);
            }
        }
        if (InstakillsOnWaveStart > 0)
            AddBuff<LightningBottle>(-1, InstakillsOnWaveStart);
        LuckyStarItemsAcquiredThisWave = 0;
    }
    public void OnDestroy()
    {
        AllPlayers.Remove(this);
    }
    public static List<GameObject> ObjectsConsideredForUIInteraction = new();
    public GameObject ClosestInteractable { get; private set; }
    public void CheckForClosestObjects()
    {
        float bestPos = ClosestInteractable == null ? float.MaxValue : Utils.DistanceSquared(ClosestInteractable.transform.position, Position);
        for (int i = 0; i < ObjectsConsideredForUIInteraction.Count; ++i)
        {
            var obj = ObjectsConsideredForUIInteraction[i];
            if (obj == null)
                ObjectsConsideredForUIInteraction.RemoveAt(i--);
            Vector2 toPlayer = Position - (Vector2)obj.transform.position;
            float sqrMag = toPlayer.sqrMagnitude;
            if (sqrMag < bestPos)
            {
                bestPos = sqrMag;
                ClosestInteractable = obj;
            }
        }
    }
    public bool ThisIsPlayerClosestInteractable(GameObject self)
    {
        return ClosestInteractable == self;
    }
    public void OnWorldReset()
    {
        //This is just to reset animation and weapon related stuff
        Weapon.DeadUpdate();
        Body.DeadUpdate();
        Hat.DeadUpdate();
        Accessory.DeadUpdate();
    }
    public static void GameWin()
    {
        //Only need to do this for 1 player for now, as the other player will use the same equipment (for now)
        Player.Instance.OnGameWin();
    }
    public bool HasWonThisCycle = false;
    public void OnGameWin()
    {
        Body body = Instance.Body;
        if (WaveDirector.WaveNum >= 15)
        {
            if (body is ThoughtBubble)
            {
                if(!!HasAttacked)
                    UnlockCondition.Get<ThoughtBubbleWave15NoAttack>().SetComplete();
            }
            else if (body is Gachapon)
            {
                if (!PickedLowerDifficultyWaveCard)
                    UnlockCondition.Get<GachaponWave15AllSkullWaves>().SetComplete();
                if (Instance.BestPowerCountIncludingStacks <= 21)
                    UnlockCondition.Get<GachaponBlackjack>().SetComplete();
            }
            else if (body is Bubblemancer)
            {
                if(!HasBeenHit)
                    UnlockCondition.Get<BubblemancerPerfection>().SetComplete();
                if(AscensionLevel >= 3)
                    UnlockCondition.Get<BubblemancerCatalyst>().SetComplete();
            }
            else if(body is Fizzy)
            {
                int tbImitation = 0;
                if(Accessory is LabCoat || (Accessory.IsSubEquip && Accessory.SubEquipParent is LabCoat))
                    tbImitation++;
                if (Weapon is Book || (Weapon.IsSubEquip && Weapon.SubEquipParent is Book))
                    tbImitation++;
                if (Hat is Bulb || (Hat.IsSubEquip && Hat.SubEquipParent is Bulb))
                    tbImitation++;
                if(tbImitation >= 2)
                    UnlockCondition.Get<FizzyFakeDoctor>().SetComplete();
                if (AscensionLevel >= 1 && PowerUp.Get<FocusFizz>().Stack >= 10)
                    UnlockCondition.Get<FizzyFocus>().SetComplete();
                if(TotalBlackMarketPowersPickedUp >= TotalNonBlackMarketPowersPickedUp)
                    UnlockCondition.Get<FizzyStuffed>().SetComplete();
            }
            UnlockCondition.Get<ThoughtBubbleUnlock>().SetComplete();
        }
        if (WaveDirector.WaveNum >= 20 && body is Gachapon && HasWonThisCycle)
            UnlockCondition.Get<GachaponAddicted>().SetComplete();
        if (HasWonThisCycle)
            return;
        HasWonThisCycle = true;
        foreach(Equipment e in Equips)
        {
            e.VictoryCount += 1;
            e.HighestDifficultyUnlocked = Math.Max(e.HighestDifficultyUnlocked, AscensionLevel + 1);
        }
    }
    public static int AscensionLevel { get; set; } = 3;
    public static int PlayerHighestAscensionAvailable()
    {
        int lowest = 3; //Max ascension available
        foreach (Player p in AllPlayers)
            lowest = Math.Min(lowest, p.Body.HighestDifficultyUnlocked);
        return lowest;
    }
    public static void ModifyAscensionLevel(int change)
    {
        int max = PlayerHighestAscensionAvailable() + 1;
        if(change == 0)
        {
            AscensionLevel = Mathf.Clamp(AscensionLevel, 0, max - 1);
            return;
        }
        AscensionLevel += change;
        if (AscensionLevel < 0)
            AscensionLevel += max;
        AscensionLevel %= max;
    }
    public static class AscensionModifiers
    { 
        public static bool AllEnemiesPermanent => AscensionLevel >= 1;
        public static bool AllEnemiesTagTeam => AscensionLevel >= 2;
        public static bool Pandemic => AscensionLevel >= 3;
    }
}
