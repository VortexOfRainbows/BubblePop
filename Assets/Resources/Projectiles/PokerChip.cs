using UnityEngine;

public class PokerChip : Projectile
{
    public float HomingRate = 10.0f;
    public int HomingNum = 0;
    public override void Init()
    {
        SpriteRenderer.sprite = Resources.Load<Sprite>("Projectiles/RedChip");
        timer += Utils.RandInt(41);
        HomingNum = Utils.RandInt(10);
        Damage = 3;
        Friendly = true;
        SpriteRendererGlow.gameObject.SetActive(true);
        SpriteRendererGlow.color = new Color(0.7137f, 0.2352f, 0.2588f);
        SpriteRendererGlow.transform.localScale *= 0.5f;
    }
    public override void AI()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 2, 0.1f);
        RB.rotation = RB.velocity.ToRotation() * Mathf.Rad2Deg;
        RB.velocity *= 1.007f;
        if(timer % 10 == HomingNum)
        {
            Enemy target =  Enemy.FindClosest(transform.position, 7, out Vector2 norm2, true);
            //if (target == null)
            //norm2 = (Utils.MouseWorld - (Vector2)transform.position).normalized;
            if (target != null)
            {
                float previousSpeed = RB.velocity.magnitude;
                RB.velocity += norm2 * HomingRate;
                RB.velocity = RB.velocity.normalized * previousSpeed;
            }
        }

        float deathTime = 200;
        if (Player.Instance.EternalBubbles > 0)
        {
            deathTime += 40 + 40 * Player.Instance.EternalBubbles;
        }
        float FadeOutTime = 20;
        if (timer > deathTime + FadeOutTime)
        {
            Kill();
        }
        if ((int)timer % 3 == 0)
        {
            Vector2 norm = RB.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f, .3f, norm * -.75f, 0.8f, 0.3f, 2, SpriteRendererGlow.color);
        }
        if (timer > deathTime)
        {
            float alphaOut = 1 - (timer - deathTime) / FadeOutTime;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alphaOut);
        }
        timer++;
    }
    public override void OnKill()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 circular = new Vector2(1, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.3f, 0.6f), circular * Utils.RandFloat(3, 6), 4f, 0.4f, 0, SpriteRendererGlow.color);
        }
        AudioManager.PlaySound(SoundID.BubblePop, transform.position, 0.7f, 0.6f);
    }
}
public class BlueChip : PokerChip
{
    public override void Init()
    {
        base.Init();
        SpriteRenderer.sprite = Resources.Load<Sprite>("Projectiles/BlueChip");
        SpriteRendererGlow.color = new Color(0.1764706f, .6f, 0.6941177f);
        Damage = 6;
        HomingRate = 20.0f;
    }
}