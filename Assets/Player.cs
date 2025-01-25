using UnityEngine;

public static class Control
{
    public static bool Dash => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    public static bool LastDash = false;
    public static bool Up => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
    public static bool Down => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
    public static bool Left => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
    public static bool Right => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
}
public class Player : Entity
{
    public static Player Instance;
    public static Vector2 Position => (Vector2)Instance.transform.position;
    [SerializeField]
    private Camera MainCamera;
    public GameObject Wand;
    public GameObject Body;
    public Rigidbody2D rb;
    private float speed = 2.5f;
    private float MovementDeacceleration = 0.9f;
    private float MaxSpeed = 6f;
    private float squash = 1f;
    private float walkTimer = 0;
    private Vector2 lastVelo;
    private float SquashAmt = 0.45f;
    void Start()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }
    private float dashCD = 0.5f;
    private float dashTimer = 0;
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

        if(Control.Dash && !Control.LastDash && movespeed.magnitude > 0)
            if(dashTimer <= 0)
            {
                dashTimer = dashCD;
                velocity = velocity * MaxSpeed + movespeed * speed * 25f;
                squash = SquashAmt;
                Body.transform.eulerAngles = new Vector3(0, 0, velocity.ToRotation() * Mathf.Rad2Deg);
            }
        dashTimer -= Time.fixedDeltaTime;

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
                    ParticleManager.NewParticle((Vector2)transform.position + velocity * i * Time.fixedDeltaTime + Utils.RandCircle(i * 2) - norm * .5f, .5f, norm * Utils.RandFloat(10f, 15f), 1.0f, 0.6f);
            }
        }

        rb.velocity = velocity;
        Control.LastDash = Control.Dash;
        float angleMult = 0.5f + (squash < 0.9f ? 0.5f : 0);
        if(velocity.sqrMagnitude > 0.5f)
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
            Body.GetComponent<SpriteRenderer>().flipY = r >= 90 && r < 270;
            Body.transform.eulerAngles = new Vector3(0, 0, r);
        }
        float bobbing = BobbingUpdate();
        Body.transform.localScale = new Vector3(1 + (1 - squash) * 2.5f + 0.1f * (1 - bobbing), bobbing * squash, 1);
        Body.transform.localPosition = new Vector2(0, Mathf.Sign(lastVelo.x) * ((bobbing * squash) - 1)).RotatedBy(lastVelo.ToRotation());
        if (squash < 1)
            squash += 0.005f;
        squash = Mathf.Lerp(squash, 1, 0.025f);
        if (Mathf.Abs(velocity.x) > 0.1f)
            lastVelo.x = velocity.x;
        if (Mathf.Abs(velocity.y) > 0.1f)
            lastVelo.y = velocity.y;
    }
    private Vector3 WandEulerAngles = new Vector3(0, 0, 0);
    public float PointDirOffset;
    private float MoveOffset;
    private float DashOffset;
    private float AttackLeft = 0;
    public float AttackRight = 0;
    private void WandUpdate()
    {
        if(AttackLeft < -20 && AttackRight < 0)
        {
            if (Input.GetMouseButton(0))
            {
                AttackLeft = 50;
            }
        }
        if (AttackRight < -20 && AttackLeft < 0)
        {
            if (Input.GetMouseButton(1))
            {
                AttackRight = 50;
            }
        }

        Vector2 toMouse = Utils.MouseWorld - (Vector2)transform.position;
        float dir = Mathf.Sign(toMouse.x);
        float bodyDir = Mathf.Sign(rb.velocity.x);
        Vector2 attemptedPosition = new Vector2(0.8f, 0.3f * dir).RotatedBy(toMouse.ToRotation()) + rb.velocity.normalized * 0.1f;

        //Debug.Log(attemptedPosition.ToRotation() * Mathf.Rad2Deg);

        PointDirOffset = -45 * dir * squash;
        MoveOffset = -5 * bodyDir * squash;
        DashOffset = 100 * dir * (1 - squash);

        Vector2 awayFromWand = new Vector2(1, 0).RotatedBy(Wand.transform.eulerAngles.z * Mathf.Deg2Rad);
        if (AttackLeft > 0)
        {
            if(AttackLeft % 2 == 1 && AttackLeft >= 41)
            {
                Projectile.NewProjectile((Vector2)Wand.transform.position + awayFromWand * 2,
                    toMouse.normalized.RotatedBy(Utils.RandFloat(-12, 12) * Mathf.Deg2Rad)
                    * Utils.RandFloat(9, 15) + awayFromWand * Utils.RandFloat(2, 4) + new Vector2(Utils.RandFloat(-0.7f, 0.7f), Utils.RandFloat(-0.7f, 0.7f)));
            }
            float percent = AttackLeft / 50f;
            PointDirOffset += 165 * percent * dir * squash;
        }
        if(AttackRight > 0)
        {
            if((Input.GetMouseButton(1) || AttackRight < 100) && AttackRight >= 50 )
            {
                if(AttackRight == 50)
                {
                    Projectile.NewProjectile((Vector2)Wand.transform.position + awayFromWand, Vector2.zero, 3, 0);
                }
                if(AttackRight < 350)
                    AttackRight++;
                PointDirOffset += -Mathf.Min(45f, (AttackRight - 50f) / 300f * 45f) * dir * squash;
            }
            else
            {
                if (AttackRight > 50)
                    AttackRight = 50;
                AttackRight--;
                float percent = AttackRight / 50f;
                PointDirOffset += 125 * percent * dir * squash;
            }
        }
        else
            AttackRight--;

        //Final Stuff
        float r = attemptedPosition.ToRotation() * Mathf.Rad2Deg - PointDirOffset - MoveOffset + DashOffset;
        Wand.transform.localPosition = Vector2.Lerp(Wand.transform.localPosition, attemptedPosition, 0.08f);
        Wand.GetComponent<SpriteRenderer>().flipY = PointDirOffset < 0;
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.15f);
        Wand.transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        AttackLeft--;
    }
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
        MovementUpdate();
        WandUpdate();
        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, new Vector3(transform.position.x, transform.position.y, MainCamera.transform.position.z), 0.1f);
    }
    void Update() => Instance = this;
    public void Pop()
    {
        Debug.Log("POP");
        //This is where I'll put die stuff
    }
}
