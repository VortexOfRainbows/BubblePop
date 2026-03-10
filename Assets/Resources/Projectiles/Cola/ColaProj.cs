using UnityEngine;

public class ColaProj : Projectile
{
    public Vector2 Destination => new(Data[0], Data[1]);
    public Vector2 playerStartPos = Vector2.zero;
    public override void Init()
    {
        SpriteRenderer.color = SpriteRenderer.color.WithAlpha(0.85f);
        SpriteRenderer.sprite = Resources.Load<Sprite>("Player/Fizzy/bottle");
        SpriteRenderer.flipX = Data[2] < 0;
        timer2 = 0;
        transform.localScale *= 1f;
        Penetrate = -1;
        Friendly = false;
        SpriteRendererGlow.gameObject.SetActive(false);
        startPos = transform.position;
        playerStartPos = PlayerOwner.lastPos;
    }
    public override void AI()
    {
        float speedMult = PlayerOwner.SecondaryAttackSpeedModifier * 0.5f + 0.5f;
        float speed = RB.velocity.magnitude;
        if (speed < 1)
            speed = 1;
        float rtSpeed = Mathf.Sqrt(speed) * speedMult;
        RB.rotation -= rtSpeed * Mathf.Sign(RB.velocity.x);
        float targetScale = 1.1f;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, 0.075f + 0.02f * rtSpeed);

        if ((int)timer % 4 == 0)
        {
            Vector2 norm = RB.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f + Utils.RandCircle(transform.lossyScale.x * 0.4f), .225f, norm * -.75f, 0.6f, Utils.RandFloat(0.225f, 0.35f), 0, Player.ProjectileColor.WithAlphaMultiplied(0.8f));
        }

        float dist = startPos.Distance(Destination);
        float timeNeeded = 1 + dist / speed;
        float percent = timer / 100f;
        timer += speedMult / timeNeeded;
        if (percent > 1)
        {
            Kill();
            return;
        }
        Vector2 pos = Vector2.Lerp(startPos, Destination, percent);
        float arcY = Mathf.Sin(percent * Mathf.PI) * dist * 0.4f;
        pos.y += arcY;
        RB.MovePosition(pos);
    }
    public void Update()
    {
        float percent = timer / 100f;
        float sin = Mathf.Sin(percent * Mathf.PI);
        Vector3 drawPos = Vector2.Lerp(playerStartPos, Destination, percent);
        drawPos.y -= 0.5f;
        SpriteBatch.Draw(Main.TextureAssets.Shadow, drawPos, new Vector2(2.0f, 1.3f), 0, 
            new Color(0, 0, 0, 0.5f * sin), -50, Main.TextureAssets.AlphaShader);
    }
    public override void OnKill()
    {
        int c = 25;
        for (int i = 0; i < c; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(0, 0.5f), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(0.5f) * transform.localScale.x, Utils.RandFloat(0.3f, 0.5f), circular * Utils.RandFloat(4, 6), 4f, 0.36f, 0, Player.ProjectileColor.WithAlphaMultiplied(0.8f));
        }
        AudioManager.PlaySound(SoundID.BubblePop, transform.position, 0.5f, 1.1f);
    }
}