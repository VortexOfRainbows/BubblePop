using System.Dynamic;
using UnityEngine;
using UnityEngine.TextCore;

public static class Control
{
    public static bool Dash => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.Space);
    public static bool LastDash = false;
    public static bool Up => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
    public static bool Down => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
    public static bool Left => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
    public static bool Right => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
}
public partial class Player : Entity
{
    public float Direction => Mathf.Sign(lastVelo.x);
    public int bonusBubbles = 0;
    public static Player Instance;
    public static Vector2 Position => Instance == null ? Vector2.zero : (Vector2)Instance.transform.position;
    [SerializeField]
    private Camera MainCamera;
    [SerializeField] public Body Body;
    [SerializeField] public Weapon Wand;
    [SerializeField] public Hat Hat;
    [SerializeField] public Accessory Cape;
    public SpriteRenderer BodyR => Body.spriteRender;
    public Rigidbody2D rb;

    private float speed = 2.5f;
    private float MovementDeacceleration = 0.9f;
    private float MaxSpeed = 6f;
    public float squash { get; private set; } = 1f;
    private float walkTimer = 0;
    public Vector2 lastVelo;
    public float SquashAmt { get; private set; } = 0.45f;
    public float Bobbing { get; private set; }
    private bool HasRunStartingGear = false;
    public const float DashDefault = 25f;
    public float DashMult = 1f;
    void Start()
    {
        PowerInit();
        EventManager.Restart();
        MainCamera.orthographicSize = 12;
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        ChargeShotDamage = 0;
        ShotgunPower = 0;
        DeathKillTimer = 0;
        DashMult = 1;
        HasRunStartingGear = false;
        PickedUpPhoenixLivesThisRound = SpentBonusLives = 0;
    }
    public float dashCD { get; private set; } = 0.5f;
    public float dashTimer = 0;
    private void MovementUpdate()
    {
        Vector2 velocity = rb.velocity;
        Vector2 movespeed = Vector2.zero;
        if (Control.Up && !Control.Down)
        {
            if (velocity.y < 0)
                velocity.y *= MovementDeacceleration;
            movespeed.y += speed;
        }
        else if (!Control.Up && Control.Down)
        {
            if (velocity.y > 0)
                velocity.y *= MovementDeacceleration;
            movespeed.y -= speed;
        }
        else
            velocity.y *= MovementDeacceleration;
        if (!Control.Right && Control.Left)
        {
            if (velocity.x > 0)
                velocity.x *= MovementDeacceleration;
            movespeed.x -= speed;
        }
        else if (Control.Right && !Control.Left)
        {
            if (velocity.x < 0)
                velocity.x *= MovementDeacceleration;
            movespeed.x += speed;
        }
        else
            velocity.x *= MovementDeacceleration;
        movespeed = movespeed.normalized;

        dashTimer -= Time.fixedDeltaTime;
        if (Control.Dash && !Control.LastDash && movespeed.magnitude > 0 && dashTimer <= 0)
        {
            Dash(ref velocity, movespeed);
        }

        //Final stuff
        velocity += movespeed * speed;
        float currentSpeed = velocity.magnitude;
        if (currentSpeed > MaxSpeed)
        {
            Vector2 norm = velocity.normalized;
            velocity = norm * (MaxSpeed + (currentSpeed - MaxSpeed) * 0.8f);
            if (currentSpeed > MaxSpeed + 15f)
            {
                for (float i = 0; i < 1; i += 0.5f)
                    ParticleManager.NewParticle((Vector2)transform.position + velocity * i * Time.fixedDeltaTime + Utils.RandCircle(i * 2) - norm * .5f, .5f, norm * -Utils.RandFloat(15f), 1.0f, 0.6f);
            }
        }

        rb.velocity = velocity;
        Control.LastDash = Control.Dash;
        Bobbing = BobbingUpdate();
        Body.AliveUpdate();
        if (squash < 1)
            squash += 0.005f;
        squash = Mathf.Lerp(squash, 1, 0.025f);
        if (Mathf.Abs(velocity.x) > 0.1f)
            lastVelo.x = velocity.x;
        lastVelo.y = velocity.y;
    }
    public float PointDirOffset;
    public float MoveOffset;
    public float DashOffset;
    public float BobbingUpdate()
    {
        float abs = Mathf.Sqrt(Mathf.Abs(rb.velocity.magnitude)) * 0.5f;
        walkTimer += abs + 1;
        walkTimer %= 100f;
        float sin = Mathf.Sin(walkTimer / 50f * Mathf.PI);
        float bobbing = 0.9f + 0.1f * sin;
        return bobbing;
    }
    private float AttackUpdateTimer = 0;
    new private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.N))
            UnlockCondition.ForceUnlockAll = true;
        if (!HasRunStartingGear && !UIManager.StartingScreen)
        {
            Hat.OnStartWith();
            Cape.OnStartWith();
            Wand.OnStartWith();
            Body.OnStartWith();
            HasRunStartingGear = true;
        }
        Instance = this;
        EventManager.Update();
        UpdatePowerUps();
        if (DeathKillTimer > 0)
            Pop();
        else
        {
            base.FixedUpdate(); //Reduce I frames. Will reorganize later
            MainCamera.orthographicSize = Mathf.Lerp(MainCamera.orthographicSize, 15f, 0.03f);
            MovementUpdate();
            if (Input.GetMouseButton(0))
                Wand.StartAttack(false);
            else if (Input.GetMouseButton(1))
                Wand.StartAttack(true);
            AttackUpdateTimer += AttackSpeedModifier;
            while (AttackUpdateTimer >= 1)
            {
                Wand.AliveUpdate();
                AttackUpdateTimer -= 1;
            }
            Hat.AliveUpdate();
            Cape.AliveUpdate();
        }
        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, new Vector3(transform.position.x, transform.position.y, MainCamera.transform.position.z), 0.1f);
    }
    new void Update()
    {
        base.Update();
        Instance = this;
    }
    public float DeathKillTimer = 0;
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
        Wand.DeadUpdate();
        Cape.DeadUpdate();
        DeathKillTimer++;
        if(Input.GetKey(KeyCode.R))
        {
            DeathKillTimer = 0;
            //Body.SetActive(true);
        }
        if (DeathKillTimer > 200)
        {
            RegisterDeath();
        }
    }
    public void Dash(ref Vector2 velocity, Vector2 moveSpeed)
    {
        float speed = DashDefault * DashMult;
        dashTimer = dashCD * DashMult;
        velocity = velocity * MaxSpeed + moveSpeed * speed;
        squash = SquashAmt;
        Body.transform.eulerAngles = new Vector3(0, 0, velocity.ToRotation() * Mathf.Rad2Deg);
        AudioManager.PlaySound(SoundID.Dash.GetVariation(3), Wand.transform.position, 1f, Utils.RandFloat(1.2f, 1.3f));

        OnDash(velocity);
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
        UIManager.Instance.GameOver();
    }
    public void Rebirth()
    {
        for(int i = 0; i < 30; i++)
        {
            Projectile.NewProjectile(transform.position, new Vector2(32, 0).RotatedBy(i / 15f * Mathf.PI), 7, 0, 0);
        }
        IFrame = 200;
        SpentBonusLives++;
        DeathKillTimer = 0;
    }
}
