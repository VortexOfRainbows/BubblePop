using UnityEngine;

public class Helicopter : Projectile
{
    public Vector3 ShadowOffset => new(0, -3.75f + Mathf.Sin(Timer4 * Mathf.PI) * 0.125f);
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
        SpriteRenderer.material = Main.TextureAssets.SpriteLit;
        SpriteRenderer.sprite = heli = heli != null ? heli : Resources.Load<Sprite>("Projectiles/Summons/Helicopter");
        heli2 = heli2 != null ? heli2 : Resources.Load<Sprite>("Projectiles/Summons/Helicopter2");
    }
    public Entity target = null;
    public int Timer3 = 0;
    public float Timer4 = 0;
    public int BurstCount()
    {
        return PlayerOwner.HelicopterStacks + 6;
    }
    public float GetAttackSpeedMultiplier()
    {
        float rate = PlayerOwner.HelicopterStacks * 0.2f + 0.8f;
        float mult = 0.6f + PlayerOwner.PassiveAttackSpeedModifier * 0.4f;
        return rate * mult;
    }
    public float GetMovespeedMultiplier()
    {
        float rate = PlayerOwner.HelicopterStacks * 0.1f + 0.9f;
        return rate;
    }
    public override void AI()
    {
        float mult = GetMovespeedMultiplier();
        float speed = 0.3f * mult;
        float lerpSpeed = 0.08f * mult;
        transform.LerpLocalScale(new Vector2(1, 1), 0.2f);
        SpriteRenderer.color = SpriteRenderer.color.Lerp(Color.white, 0.2f);
        if (PlayerOwner == null || PlayerOwner.HelicopterStacks <= 0)
        {
            Kill();
            return;
        }
        FrameUpdate();
        Vector2 actPosition = (Vector2)(transform.position + ShadowOffset);
        Entity newTarget = Enemy.FindClosest(actPosition, 20, out Vector2 _, null, true, false, false);
        if(target == null || target.transform.position.Distance(actPosition) - 10 > newTarget.transform.position.Distance(actPosition))
        {
            timer2 = 0;
            Timer3 = 0;
            target = newTarget;
        }
        if (target != null && target.Distance(PlayerOwner.gameObject) > 28)
            target = null;
        Vector2 toTarget;
        float magnitude;
        if (target == null)
        {
            Vector2 toOwner = PlayerOwner.Position - actPosition;
            SpriteRenderer.flipX = toOwner.x > 0;
            toOwner.x -= PlayerOwner.Body.FlipDir * 3;
            magnitude = toOwner.magnitude;
            toTarget = toOwner;
            timer2 = 0;
            Timer3 = 0;
        }
        else
        {
            Vector2 toEnemy = (Vector2)target.transform.position - actPosition;
            if (toEnemy.x > 0) //enemy is to the right of the helicopter
            {
                SpriteRenderer.flipX = true;
                toEnemy.x -= 6;
            }
            else
            {
                SpriteRenderer.flipX = false;
                toEnemy.x += 6;
            }
            magnitude = toEnemy.magnitude;
            toTarget = toEnemy;
            float direction = SpriteRenderer.flipX ? 1 : -1;
            if (magnitude < 10)
                timer2 += GetAttackSpeedMultiplier();
            while (timer2 > 120)
            {
                AudioManager.PlaySound(SoundID.GolemMultiShoot, transform.position, 0.9f, 2.0f);
                Vector2 leftOffset = new(-0.19f * direction, -0.875f);
                Vector2 rightOffset = new(1.49f * direction, -0.075f);
                int i = Timer3 % 2;
                Vector2 offset = i == 0 ? leftOffset : rightOffset;
                Vector2 shotPosition = (Vector2)transform.position + offset.RotatedBy(transform.localEulerAngles.z * Mathf.Deg2Rad);
                Vector2 bodyToTarget = (Vector2)target.transform.position + (10 * Time.fixedDeltaTime * target.RB.velocity) - shotPosition;
                Projectile.NewProjectile<TranscendentOilBubble>(shotPosition, bodyToTarget.normalized * 24f, 1, PlayerOwner, 0, 1);
                if (Timer3 < BurstCount())
                    timer2 -= 12;
                else
                {
                    Timer3 = 0;
                    timer2 -= 120;
                }
                ++Timer3;
            }
        }
        float travelDistance = Mathf.Min(magnitude, speed + 0.01f * Mathf.Pow(magnitude, 1.25f)) * 100; //framespeed is 100
        toTarget = travelDistance * toTarget.normalized;
        if (RB.velocity.magnitude > travelDistance)
            RB.velocity *= 0.5f;
        RB.velocity = Vector2.Lerp(RB.velocity, toTarget * 0.5f, lerpSpeed);
        transform.LerpLocalEulerZ(Mathf.Clamp(RB.velocity.x * -2, -15, 15), lerpSpeed * 0.5f);
        Timer4 += Time.fixedDeltaTime * 1.25f;
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
        bool solidTile = World.SolidTile(drawPos) ||
            World.SolidTile(drawPos + new Vector3(-0.25f, 0)) || 
            World.SolidTile(drawPos + new Vector3(0.25f, 0));
        if (solidTile)
            drawPos.y += 0.25f;
        SpriteBatch.Draw(Main.TextureAssets.Shadow, drawPos, new Vector2(3.0f, 1.4f), 0, new Color(0, 0, 0, 0.3f), solidTile ? LayerHelper.SolidTileSortingOrder + 1 : -40, Main.TextureAssets.AlphaShader);
    }
}
