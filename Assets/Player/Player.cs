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
    public float Direction => Mathf.Abs(lastVelo.x);
    public int bonusBubbles = 0;
    public static Player Instance;
    public static Vector2 Position => Instance == null ? Vector2.zero : (Vector2)Instance.transform.position;
    [SerializeField]
    private Camera MainCamera;
    public GameObject Body;
    [SerializeField] public Weapon Wand;
    [SerializeField] protected Hat Hat;
    [SerializeField] protected Accessory Cape;
    public GameObject Face;
    public SpriteRenderer BodyR;
    public SpriteRenderer FaceR;
    public Rigidbody2D rb;

    private float speed = 2.5f;
    private float MovementDeacceleration = 0.9f;
    private float MaxSpeed = 6f;
    public float squash { get; private set; } = 1f;
    private float walkTimer = 0;
    public Vector2 lastVelo;
    public float SquashAmt { get; private set; } = 0.45f;
    public float Bobbing { get; private set; }
    void Start()
    {
        PowerInit();
        EventManager.Restart();
        MainCamera.orthographicSize = 12;
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        DamagePower = 0;
        ShotgunPower = 0;
        DeathKillTimer = 0;
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
        if(Control.Dash && !Control.LastDash && movespeed.magnitude > 0 && dashTimer <= 0)
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
                for(float i = 0; i < 1; i += 0.5f)
                    ParticleManager.NewParticle((Vector2)transform.position + velocity * i * Time.fixedDeltaTime + Utils.RandCircle(i * 2) - norm * .5f, .5f, norm * -Utils.RandFloat(15f), 1.0f, 0.6f);
            }
        }

        rb.velocity = velocity;
        Control.LastDash = Control.Dash;
        float angleMult = 0.5f + (squash < 0.9f ? 0.5f : 0);
        Bobbing = BobbingUpdate();
        if (lastVelo.sqrMagnitude > 0.10f)
        {
            float r = Body.transform.eulerAngles.z;
            float angle = lastVelo.ToRotation() * Mathf.Rad2Deg;
            if (angle < 0)
                angle += 360;
            if (angle > 90 && angle <= 270)
            {
                angle = angle - 180;
                angle = 180 + angle * angleMult;
            }
            else
            {
                if (angle >= 270)
                    angle -= 360;
                angle *= angleMult;
            }
            r = Mathf.LerpAngle(r, angle, 0.12f);
            BodyR.flipY = r >= 90 && r < 270;
            bool flip = !BodyR.flipY;
            Body.transform.eulerAngles = new Vector3(0, 0, r);
        }
        Body.transform.localScale = new Vector3(1 + (1 - squash) * 2.5f + 0.1f * (1 - Bobbing), Bobbing * squash, 1);
        Body.transform.localPosition = new Vector2(0, Mathf.Sign(lastVelo.x) * ((Bobbing * squash) - 1)).RotatedBy(lastVelo.ToRotation());
        if (squash < 1)
            squash += 0.005f;
        squash = Mathf.Lerp(squash, 1, 0.025f);
        if (Mathf.Abs(velocity.x) > 0.1f)
            lastVelo.x = velocity.x;
        lastVelo.y = velocity.y;
        FaceUpdate();
    }
    public void FaceUpdate()
    {
        Vector2 toMouse = Utils.MouseWorld - (Vector2)Body.transform.position;
        toMouse *= Mathf.Sign(lastVelo.x);
        Vector2 pos = new Vector2(0.15f, 0) + toMouse.normalized * 0.25f;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        FaceR.flipY = BodyR.flipY;
    }
    public float PointDirOffset;
    public float MoveOffset;
    public float DashOffset;
    public float AttackLeft = 0;
    public float AttackRight = 0;
    public float BobbingUpdate()
    { 
        float abs = Mathf.Sqrt(Mathf.Abs(rb.velocity.magnitude)) * 0.5f;
        walkTimer += abs + 1;
        walkTimer %= 100f;
        float sin = Mathf.Sin(walkTimer / 50f * Mathf.PI);
        float bobbing = 0.9f + 0.1f * sin;
        return bobbing;
    }
    private void FixedUpdate()
    {
        Instance = this;
        EventManager.Update();
        if (DeathKillTimer > 0)
            Pop();
        else
        {
            UpdatePowerUps();
            MovementUpdate();
            Wand.AliveUpdate();
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
        Time.timeScale = 0.5f + 0.5f * Mathf.Sqrt(Mathf.Max(0, 1 - DeathKillTimer / 200f));
        if (DeathKillTimer > 100)
            MainCamera.orthographicSize = Mathf.Lerp(MainCamera.orthographicSize, 6f, 0.03f);
        else
            MainCamera.orthographicSize = Mathf.Lerp(MainCamera.orthographicSize, 17f, 0.03f);
        AttackLeft = 0;
        AttackRight = 0;
        rb.velocity *= 0.9f;
        if(DeathKillTimer <= 0)
        {
            AudioManager.PlaySound(GlobalDefinitions.audioClips[23], Body.transform.position, 0.21f, 0.4f);
            for (int i = 0; i < 100; i++)
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * i / 25f);
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), 
                    Utils.RandFloat(0.5f, 1.1f), circular * Utils.RandFloat(0, 24) + new Vector2(0, Utils.RandFloat(-2, 4)), 4f, Utils.RandFloat(1, 3));
            }
            Body.SetActive(false);
        }
        Hat.DeadUpdate();
        Wand.DeadUpdate();
        Cape.DeadUpdate();
        DeathKillTimer++;
        //if(Input.GetKey(KeyCode.R))
        //{
        //    DeathKillTimer = 0;
        //    Body.SetActive(true);
        //}
        if(DeathKillTimer > 200)
        {
            UIManager.Instance.GameOver();
        }
    }
    public void Dash(ref Vector2 velocity, Vector2 moveSpeed)
    {
        dashTimer = dashCD;
        velocity = velocity * MaxSpeed + moveSpeed * speed * 25f;
        squash = SquashAmt;
        Body.transform.eulerAngles = new Vector3(0, 0, velocity.ToRotation() * Mathf.Rad2Deg);
        AudioManager.PlaySound(GlobalDefinitions.audioClips[12], Wand.transform.position, 1f, Utils.RandFloat(1.2f, 1.3f));

        OnDash(velocity);
    }
}
