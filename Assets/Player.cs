using System.Reflection;
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
    private Camera MainCamera;
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
                squash = 0.5f;
            }
        dashTimer -= Time.fixedDeltaTime;

        //Final stuff
        velocity += movespeed * speed;
        float currentSpeed = velocity.magnitude;
        if (currentSpeed > MaxSpeed)
        {
            velocity = velocity.normalized * (MaxSpeed + (currentSpeed - MaxSpeed) * 0.8f);
        }
        if (currentSpeed > MaxSpeed + 1)
        {
        }

        rb.velocity = velocity;
        Control.LastDash = Control.Dash;

        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, velocity.ToRotation() * Mathf.Rad2Deg, 0.15f));
        transform.localScale = new Vector3(1 + (1 - squash) * 2, squash, 1);
        squash = Mathf.Lerp(squash, 1, 0.05f);
    }
    private void FixedUpdate()
    {
        MovementUpdate();
        //MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, new Vector3(transform.position.x, transform.position.y, MainCamera.transform.position.z), 0.1f);
    }
    void Update()
    {
    }
}
