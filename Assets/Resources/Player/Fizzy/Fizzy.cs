using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Fizzy : Body
{
    public static readonly string[] CoolWords = new string[] { "BASED", "AWESOME", "TUBULAR", "COOL", "RADICAL", "EPIC", "RAD", "SWAG", "GNARLY", "KICKFLIP" };
    public static readonly string[] CoolWords2 = new string[] { "WACK", "SLICK", "YOLO", "DISMOUNT", "RADICAL", "SUPER", "RAD", "RELEASE", "KICKFLIP" };
    public GameObject Skateboard;
    public Transform Wheel1, Wheel2, Board;
    public bool OnSkateboard { get; private set; } = false;
    public bool FinishedSkateAnim = false;
    public float SkateboardMountCounter = 0;
    public float SkateboardMountTime => 54;
    public override float AbilityCD => 6;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyUnlock>();
    public override void Init()
    {
        Player.abilityTimer = 0;
        PrimaryColor = new Color(0.8f, 0.7f, 0.56f, 0.7f);
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add(PowerUp.Get<BonusBoards>());
        powerPool.Add(PowerUp.Get<WeaponUpgrade>());
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Fizzy").WithDescription("He's a pretty cool bubble");
    }
    public override void InitializeAbilities(ref List<Ability> abilities)
    {
        abilities.Add(new Ability(Ability.ID.Ability, "Do a Y:[Cool Kickflip] and Y:mount a Y:Skateboard"));
    }
    public float SkateboardPercent => SkateboardMountCounter / SkateboardMountTime;
    public float starTimer = 1.0f;
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {
        if (Player.Control.Ability && !Player.Control.LastAbility && (moveSpeed.magnitude > 0 || playerVelo.magnitude > 1))
            SwapSkateboard();
        if(OnSkateboard)
        {
            if(SkateboardMountCounter < SkateboardMountTime)
                SkateboardMountCounter++;
            else if(SkateboardMountCounter == SkateboardMountTime && !FinishedSkateAnim)
            {
                AudioManager.PlaySound(SoundID.BathBombBurst, transform.position, 1.1f, 0.7f, 0);
                FinishedSkateAnim = true;
                Vector2 combind = playerVelo + moveSpeed;
                playerVelo += combind.normalized * 40;
                PopupText.NewPopupText(transform.position, new Vector2(0, 10) + Utils.RandCircle(5) + playerVelo * 0.8f, Utils.PastelRainbow(Utils.RandFloat(Mathf.PI * -0.75f, Mathf.PI * 0.25f), 0.55f, default), CoolWords[Utils.RandInt(CoolWords.Length)], true, 1.1f, 80);
            }
        }
        else
        {
            if (SkateboardMountCounter > 0)
                SkateboardMountCounter--;
        }
        float percent = SkateboardPercent;
        float sin = Mathf.Sin(percent * Mathf.PI);
        float scale = Mathf.Sqrt(percent) + sin * 0.5f;
        float xReduce = Player.Accessory is BubblemancerCape || Player.Accessory is LabCoat ? 0.7f : 1.0f;
        Skateboard.transform.LerpLocalScale(new Vector2(xReduce, 1) * scale, 0.1f);
        float jump = sin * 0.5f + 0.5f * Mathf.Sqrt(Mathf.Abs(sin));
        Player.Visual.transform.localPosition = new Vector3(0, jump, 0);
        Skateboard.transform.localPosition = new Vector3(0, (Player.Accessory is Kicks ? -1.75f : -1.3f) - jump, 0);
        float speed = playerVelo.magnitude * 0.9f * -p.Direction;
        Wheel1.transform.SetLocalEulerZ(Wheel1.transform.localEulerAngles.z + speed);
        Wheel2.transform.SetLocalEulerZ(Wheel2.transform.localEulerAngles.z + speed);
        Vector2 norm = playerVelo.normalized;
        Board.transform.localEulerAngles = new Vector3((1 - percent) * 540, 0, norm.x * 5 + norm.y * 5);

        if(OnSkateboard)
        {
            int trail = Player.DashSparkle;
            if (trail > 0)
            {
                starTimer -= Time.fixedDeltaTime;
                while (starTimer <= 0)
                {
                    starTimer += 4f / (Player.PassiveAttackSpeedModifier * (trail + 3f)); //4/4, 4/5, 4/6, 4/7, etc
                    Vector2 circular = (Utils.RandCircle(1.3f) - Player.RB.velocity).normalized;
                    Vector2 randPos = (Vector2)transform.position + Utils.RandCircleEdge(3);
                    Projectile.NewProjectile<StarProj>((Vector2)Skateboard.transform.position + circular * 2 + new Vector2(0, 0.3f), circular * 8, 2, Player, randPos.x, randPos.y);
                }
            }
        }
    }
    public void SwapSkateboard()
    {
        bool ready = OnSkateboard || Player.AbilityReady;
        if (!ready)
            return;
        if(SkateboardMountCounter >= SkateboardMountTime || SkateboardMountCounter <= 0)
        {
            FinishedSkateAnim = false;
            if (Player.UniversalImmuneFrames < SkateboardMountTime + 4)
                Player.UniversalImmuneFrames = SkateboardMountTime + 4;
            if(OnSkateboard)
            {
                AudioManager.PlaySound(SoundID.Starbarbs, transform.position, 1, 1.2f, 0);
                AudioManager.PlaySound(SoundID.BathBombBurst, transform.position, 1.1f, 0.7f, 0);
                int c = 1 + Player.BonusBoards;
                for(int i = 0; i < c; ++i)
                    Projectile.NewProjectile<SkateboardProj>(Skateboard.transform.position, Player.RB.velocity, 10, Player, Utils.RandFloat(-1, 1) * (0.5f + i * 0.1f));
                PopupText.NewPopupText(transform.position, new Vector2(0, 10) + Utils.RandCircle(5) + Player.RB.velocity * 0.5f, Utils.PastelRainbow(Utils.RandFloat(Mathf.PI * -0.75f, Mathf.PI * 0.25f), 0.55f, default), CoolWords2[Utils.RandInt(CoolWords2.Length)], true, 1.1f, 80);
            }
            else
            {
                AudioManager.PlaySound(SoundID.Teleport, transform.position, 1, 1.7f, 0);
                AudioManager.PlaySound(SoundID.Starbarbs, transform.position, 1, 1.5f, 0);
            }
            OnSkateboard = !OnSkateboard;
            if (OnSkateboard)
                Player.abilityTimer = AbilityCD;
            Skateboard.SetActive(OnSkateboard);
            Player.OnUseAbility();
        }
    }

    public override void FaceUpdate()
    {
        Vector2 toMouse = p.LookPosition - (Vector2)transform.position;
        Vector2 toMouse2 = toMouse.normalized;
        toMouse2.x += Mathf.Sign(toMouse2.x) * 4;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.08f, 0).RotatedBy(toMouseR);
        looking.x += 0.04f * p.Direction;
        Vector2 pos = looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouse2.y * 12.5f * p.Direction);
        Face.transform.localScale = new Vector3(p.Direction, 1, 1);
    }
}
