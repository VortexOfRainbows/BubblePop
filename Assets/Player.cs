using System.Reflection;
using System.Transactions;
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
public class Player : MonoBehaviour
{
    [SerializeField]
    private Sprite WandUp;
    [SerializeField]
    private Sprite WandDown;
    [SerializeField]
    private Camera MainCamera;
    [SerializeField]
    private GameObject Wand;
    [SerializeField]
    private GameObject Body;
    private Rigidbody2D rb;
    private float speed = 2.5f;
    private float MovementDeacceleration = 0.9f;
    private float MaxSpeed = 6f;
    private float squash = 1f;
    void Start()
    {
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
                velocity = velocity * MaxSpeed + movespeed * speed * 20f;
                squash = 0.45f;
            }
        dashTimer -= Time.fixedDeltaTime;

        //Final stuff
        velocity += movespeed * speed;
        float currentSpeed = velocity.magnitude;
        if (currentSpeed > MaxSpeed)
        {
            velocity = velocity.normalized * (MaxSpeed + (currentSpeed - MaxSpeed) * 0.8f);
        }

        rb.velocity = velocity;
        Control.LastDash = Control.Dash;

        Body.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(Body.transform.eulerAngles.z, velocity.ToRotation() * Mathf.Rad2Deg, 0.12f));
        Body.transform.localScale = new Vector3(1 + (1 - squash) * 2.5f, squash, 1);
        if (squash < 1)
            squash += 0.005f;
        squash = Mathf.Lerp(squash, 1, 0.025f);
    }
    private Vector3 WandEulerAngles = new Vector3(0, 0, 0);
    private float PointDirOffset;
    private float MoveOffset;
    private float DashOffset;
    private void WandUpdate()
    {
        Vector2 toMouse = Utils.MouseWorld - (Vector2)transform.position;
        float dir = Mathf.Sign(toMouse.x);
        float bodyDir = Mathf.Sign(rb.velocity.x);
        Vector2 attemptedPosition = new Vector2(0.8f, 0.3f * dir).RotatedBy(toMouse.ToRotation()) + rb.velocity.normalized * 0.1f;

        //Debug.Log(attemptedPosition.ToRotation() * Mathf.Rad2Deg);

        PointDirOffset = -40 * dir * squash;
        MoveOffset = -5 * bodyDir * squash;
        DashOffset = 100 * dir * (1 - squash);

        float r = attemptedPosition.ToRotation() * Mathf.Rad2Deg - PointDirOffset - MoveOffset + DashOffset;
        Wand.transform.localPosition = Vector2.Lerp(Wand.transform.localPosition, attemptedPosition, 0.08f);
        Wand.GetComponent<SpriteRenderer>().flipY = PointDirOffset < 0;
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.15f);
        Wand.transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
    }
    private void FixedUpdate()
    {
        MovementUpdate();
        WandUpdate();
        //MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, new Vector3(transform.position.x, transform.position.y, MainCamera.transform.position.z), 0.1f);
    }
    void Update()
    {

    }
}
