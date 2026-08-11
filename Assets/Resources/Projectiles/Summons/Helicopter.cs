using UnityEngine;

public class Helicopter : Projectile
{
    public static Vector3 ShadowOffset => new(0, -4);
    public static Sprite heli;
    public static Sprite heli2;
    public override void Init()
    {
        SpriteRendererGlow.enabled = false;
        Friendly = false;
        Hostile = false;
        transform.localScale = Vector3.one * 0.5f;
        SpriteRenderer.color = new Color(1, 1, 1, 0.5f);
        SpriteRenderer.sortingOrder = LayerHelper.TreeSortingOrder + 1;
        SpriteRenderer.material = PowerUp.OilOutline;
        SpriteRenderer.sprite = heli = heli != null ? heli : Resources.Load<Sprite>("Projectiles/Summons/Helicopter");
        heli2 = heli2 != null ? heli2 : Resources.Load<Sprite>("Projectiles/Summons/Helicopter2");
    }
    public override void AI()
    {
        float speed = 0.1f;
        float lerpSpeed = 0.08f;
        transform.LerpLocalScale(new Vector2(1, 1), 0.2f);
        SpriteRenderer.color = SpriteRenderer.color.Lerp(Color.white, 0.2f);
        if (PlayerOwner == null || PlayerOwner.HelicopterStacks <= 0)
        {
            Kill();
            return;
        }
        FrameUpdate();
        Vector2 actPosition = (Vector2)(transform.position + ShadowOffset);
        Entity target = Enemy.FindClosest(actPosition, 20, out Vector2 toEnemyNorm, null, true, false, true);
        Vector2 toTarget;
        float magnitude;
        if (target == null)
        {
            Vector2 toOwner = PlayerOwner.Position - actPosition;
            magnitude = toOwner.magnitude;
            toOwner = speed * Mathf.Pow(magnitude, 1.25f) * toOwner.normalized;
            toTarget = toOwner;
            timer2 = 0;
            SpriteRenderer.flipX = toTarget.x > 0;
        }
        else
        {
            Vector2 toEnemy = (Vector2)target.transform.position - actPosition;
            magnitude = toEnemy.magnitude;
            toEnemy = speed * Mathf.Pow(magnitude, 1.25f) * toEnemyNorm;
            toTarget = toEnemy;
            timer2 += 1.0f;
            float direction = Utils.SignNoZero(toTarget.x);
            SpriteRenderer.flipX = direction > 0;
            if (timer2 > 20)
            {
                Vector2 leftOffset = new(-0.19f * direction, -0.875f);
                Vector2 rightOffset = new(1.49f * direction, -0.075f);
                for(int i = 0; i < 2; ++i)
                {
                    Vector2 offset = i == 0 ? leftOffset : rightOffset;
                    Vector2 shotPosition = (Vector2)transform.position + offset.RotatedBy(transform.localEulerAngles.z * Mathf.Deg2Rad);
                    Vector2 bodyToTarget = (Vector2)target.transform.position - shotPosition;
                    Projectile.NewProjectile<TranscendentBubble>(shotPosition, bodyToTarget.normalized * 24f, 1, PlayerOwner, 0, 1);
                }
                timer2 = 0;
            }
        }
        if (magnitude < 4)
            RB.velocity = Vector2.Lerp(RB.velocity, -toTarget * 6f, lerpSpeed);
        else if (magnitude > 5)
            RB.velocity = Vector2.Lerp(RB.velocity, toTarget * 6, lerpSpeed);
        else
            RB.velocity *= 1 - lerpSpeed;
        transform.LerpLocalEulerZ(Mathf.Clamp(RB.velocity.x * -2, -15, 15), lerpSpeed);
    }
    public void FrameUpdate()
    {
        int spinRate = 8;
        ++timer;
        if (timer % spinRate == 0)
        {
            heli = heli != null ? heli : Resources.Load<Sprite>("Projectiles/Summons/Helicopter");
            heli2 = heli2 != null ? heli2 : Resources.Load<Sprite>("Projectiles/Summons/Helicopter2");
            SpriteRenderer.sprite = timer % (spinRate * 2) == 0 ? heli : heli2;
        }
    }
    public override bool? CanBeAffectedByHoming() => false;
    public override bool OnInsideTile() => false;
    public override bool OnTileCollide(Collider2D collision) => false;
    public void Update()
    {
        DoShadow();
    }
    public void DoShadow()
    {
        Vector3 drawPos = transform.position + ShadowOffset;
        drawPos.y -= 0.5f;
        bool solidTile = World.SolidTile(World.RealTileMap.Map.WorldToCell(drawPos)) ||
            World.SolidTile(World.RealTileMap.Map.WorldToCell(drawPos + new Vector3(-0.25f, 0))) || 
            World.SolidTile(World.RealTileMap.Map.WorldToCell(drawPos + new Vector3(0.25f, 0)));
        if (solidTile)
            drawPos.y += 0.25f;
        SpriteBatch.Draw(Main.TextureAssets.Shadow, drawPos, new Vector2(3.0f, 1.4f), 0, new Color(0, 0, 0, 0.3f), LayerHelper.SolidTileSortingOrder + 1, Main.TextureAssets.AlphaShader);
    }
}
