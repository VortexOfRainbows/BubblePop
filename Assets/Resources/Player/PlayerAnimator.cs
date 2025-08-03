using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Vector2 LookPosition => IsMainPlayerAnimator ? Utils.MouseWorld : Player.Position;
    public float PointDirOffset;
    public float MoveOffset;
    public float DashOffset;
    public bool IsMainPlayerAnimator = false;
    public float squash = 1f;
    public float Bobbing = 1f;
    public Rigidbody2D rb;
    [SerializeField] public Body Body;
    [SerializeField] public Weapon Weapon;
    [SerializeField] public Hat Hat;
    [SerializeField] public Accessory Accessory;
    public SpriteRenderer BodyR => Body.spriteRender;
    public float Direction => Mathf.Sign(lastVelo.x);
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
    }
    public float BobbingUpdate()
    {
        float abs = Mathf.Sqrt(Mathf.Abs(rb.velocity.magnitude)) * 0.5f;
        walkTimer += abs + (IsMainPlayerAnimator ? 1 : 0.5f);
        walkTimer %= 100f;
        float sin = Mathf.Sin(walkTimer / 50f * Mathf.PI);
        float bobbing = 0.9f + 0.1f * sin;
        return bobbing;
    }
    public float MoveDashRotation()
    {
        float bonusR = Body is Bubblemancer && IsMainPlayerAnimator ? 1f * Mathf.Max(0, Player.Instance.abilityTimer / Player.Instance.AbilityCD) : 0;
        float r = new Vector2(Mathf.Abs(lastVelo.x), lastVelo.y * Direction).ToRotation() * Mathf.Rad2Deg * (0.3f + bonusR);
        return r;
    }
}
