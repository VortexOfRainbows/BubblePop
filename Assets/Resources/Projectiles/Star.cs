using UnityEngine;
public class StarProj : Projectile
{
    public SpecialTrail MyTrail;
    public override void Init()
    {
        SpriteRenderer.sprite = Main.Sparkle;
        SpriteRenderer.color = SpriteRendererGlow.color = new Color(1f, 1f, 0.2f, 0.6f);
        Damage = 2;
        Friendly = true;
        MyTrail = SpecialTrail.NewTrail(transform, Color.Lerp(Color.blue * 0.85f, Player.Instance.Body.PrimaryColor, 0.75f).WithAlpha(0.4f), 3f, 0.25f);
    }
    public override void AI()
    {
        SparkleAI();
    }
    public override void OnKill()
    {
        if (timer >= 165)
            return;
        for (int i = 0; i < 8; i++)
        {
            Vector2 circular = new Vector2(2, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position, Utils.RandFloat(0.2f, 0.3f), circular * Utils.RandFloat(1, 2) + RB.velocity * 0.5f, 1f, 0.3f, 0, SpriteRenderer.color.WithAlpha(Utils.RandFloat(0.1f, 0.5f)));
        }
        AudioManager.PlaySound(SoundID.StarbarbImpact.GetVariation(2), transform.position, 0.45f, 0.66f);
    }
    public void SparkleAI()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.66f, 0.085f);

        Vector2 target = new Vector2(Data1, Data2);
        Vector2 toTarget = (target - (Vector2)transform.position);
        float dist = toTarget.magnitude;
        toTarget = toTarget.normalized;
        Vector2 newVelo = RB.velocity.magnitude * toTarget;
        if (timer < 60)
            RB.velocity *= 1.002f;
        if (timer < 100 && dist > 1)
            RB.velocity = Vector2.Lerp(RB.velocity, newVelo, 0.065f).normalized * RB.velocity.magnitude;
        else if (timer < 100)
            timer = 100;
        RB.rotation += Mathf.Sqrt(RB.velocity.magnitude) * Mathf.Sign(RB.velocity.x);
        if (timer > 170)
        {
            Kill();
        }
        Vector2 norm = RB.velocity.normalized;
        if(timer % 3 == 0 && timer > 10)
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f, .175f, norm * -.75f, 0.1f, Utils.RandFloat(0.45f, 0.55f), 2, SpriteRenderer.color * 0.5f);
        if (timer > 150)
        {
            float alphaOut = 1 - (timer - 150) / 20f;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alphaOut);
            SpriteRendererGlow.color = new Color(SpriteRendererGlow.color.r, SpriteRendererGlow.color.g, SpriteRendererGlow.color.b) * alphaOut;
            MyTrail.Trail.startColor = MyTrail.Trail.startColor.WithAlpha(0.4f * alphaOut);
            MyTrail.originalAlpha = 0.4f * alphaOut;
        }
        timer++;
    }
    public override bool DoHomingBehavior(Enemy target, Vector2 norm, float scale)
    {
        Vector2 targetPos = new Vector2(Data1, Data2);
        float modAmt = 0.03f + Mathf.Sqrt(scale) * 0.01f;
        targetPos = Vector2.Lerp(targetPos, target.transform.position, modAmt);
        Data1 = targetPos.x;
        Data2 = targetPos.y;
        return true;
    }
    public override void OnHitTarget(Entity target)
    {
        if (target.Life <= 0)
        {
            if (Player.Instance.Starbarbs > 0)
            {
                Vector2 norm = RB.velocity.normalized;
                float randRot = norm.ToRotation();
                for (int i = 0; i < 30; i++)
                {
                    Vector2 randPos = new Vector2(3.5f, 0).RotatedBy(i / 15f * Mathf.PI);
                    randPos.x *= Utils.RandFloat(0.5f, 0.7f);
                    randPos = randPos.RotatedBy(randRot);
                    ParticleManager.NewParticle(target.transform.position, Utils.RandFloat(0.95f, 1.05f), -norm * 4.5f + randPos * Utils.RandFloat(4, 5) + Utils.RandCircle(.3f), 0.1f, .6f, 0, SpriteRenderer.color);
                }
                int stars = 2 + Player.Instance.Starbarbs;
                for (; stars > 0; --stars)
                {
                    Vector2 targetPos = (Vector2)target.transform.position + norm * 9 + Utils.RandCircle(7);
                    NewProjectile<StarProj>(target.transform.position, norm.RotatedBy(Utils.RandFloat(360) * Mathf.Deg2Rad) * -Utils.RandFloat(16f, 24f), targetPos.x, targetPos.y);
                }
            }
            if (Player.Instance.LuckyStar > 0)
            {
                float chance = 0.02f + Player.Instance.LuckyStar * 0.02f;
                if (Utils.RandFloat(1) < chance)
                {
                    PowerUp.Spawn(PowerUp.RandomFromPool(), (Vector2)target.transform.position, 0);
                }
            }
        }
    }
}
