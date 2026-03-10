using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Player MyPlayer;
    public Transform DropShadow;
    public Vector2 LookPosition
    {
        get
        {
            if (RealPlayer && MyPlayer != null && MyPlayer.Control != null)
                return MyPlayer.Control.MousePosition;
            Player p = Player.FindClosest(transform.position, out _, out _);
            return p == null ? Utils.MouseWorld : p.Position;
        }
    }
    public float PointDirOffset;
    public float MoveOffset;
    public float DashOffset;
    public bool RealPlayer = false;
    public float squash = 1f;
    public float Bobbing = 1f;
    public Rigidbody2D rb;
    public Body Body;
    public Weapon Weapon;
    public Hat Hat;
    public Accessory Accessory;
    public float Direction => Utils.SignNoZero(lastVelo.x);
    public Vector2 lastVelo;
    private float walkTimer = 0;
    public float DeathKillTimer = 0;
    public void PostUpdate()
    {
        if (squash < 1)
            squash += 0.005f;
        squash = Mathf.Lerp(squash, 1, 0.025f);
        Bobbing = BobbingUpdate();
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
            lastVelo.x = rb.velocity.x;
        else if (lastVelo.x == 0)
            lastVelo.x = 0.01f;
        lastVelo.y = rb.velocity.y;
        if(DropShadow != null)
            if(Accessory is Kicks)
                DropShadow.transform.localPosition = new Vector3(0, -1.2f, 0);
            else
                DropShadow.transform.localPosition = new Vector3(0, -0.85f, 0);
    }
    public float BobbingUpdate()
    {
        float abs = Mathf.Sqrt(Mathf.Abs(rb.velocity.magnitude)) * 0.5f;
        walkTimer += abs + (RealPlayer ? 1 : 0.5f);
        walkTimer %= 100f;
        float sin = Mathf.Sin(walkTimer / 50f * Mathf.PI);
        float bobbing = 0.925f + 0.075f * sin;
        return bobbing;
    }
    public float MoveDashRotation()
    {
        float bonusR = Body is Bubblemancer && RealPlayer ? 1f * Mathf.Max(0, MyPlayer.abilityTimer / MyPlayer.AbilityCD) : 0;
        float r = new Vector2(Mathf.Abs(lastVelo.x), lastVelo.y * Direction).ToRotation() * Mathf.Rad2Deg * (0.3f + bonusR);
        return r;
    }
}
