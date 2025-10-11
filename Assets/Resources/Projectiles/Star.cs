using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
public class StarProj : Projectile
{
    public SpecialTrail MyTrail;
    public override void Init()
    {
        SpriteRenderer.sprite = Main.TextureAssets.Sparkle;
        SpriteRenderer.color = SpriteRendererGlow.color = new Color(1f, 1f, 0.2f, 0.6f);
        if(Damage <= 0)
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
        if(Player.Instance.OrbitalStars)
        {
            Vector2 rotatedTarget = ((Vector2)transform.position).RotatedBy(Mathf.Deg2Rad * 15 * Data[2], Player.Position);
            Data1 = rotatedTarget.x;
            Data2 = rotatedTarget.y;
            RB.position += Player.Instance.rb.velocity * Time.fixedDeltaTime * 0.9f;
            RB.velocity += Player.Instance.rb.velocity * 0.1f * Time.fixedDeltaTime;
        }
        Vector2 toTarget = (target - (Vector2)transform.position);
        float dist = toTarget.magnitude;
        toTarget = toTarget.normalized;
        Vector2 newVelo = RB.velocity.magnitude * toTarget;
        if (timer < 60)
            RB.velocity *= 1.002f;
        if ((timer < 100 && dist > 1) || Player.Instance.OrbitalStars)
        {
            RB.velocity = Vector2.Lerp(RB.velocity, newVelo, Player.Instance.OrbitalStars ? 0.0775f : 0.065f).normalized * RB.velocity.magnitude;
        }
        else if (timer < 100)
            timer = 100;
        RB.rotation += Mathf.Sqrt(RB.velocity.magnitude) * Mathf.Sign(RB.velocity.x);
        float timeOut = Player.Instance.OrbitalStars ? 220 : 150;
        float fadeOut = 20f;
        if (timer > timeOut + fadeOut)
        {
            Kill();
        }
        Vector2 norm = RB.velocity.normalized;
        if(timer % 3 == 0 && timer > 10)
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f, .175f, norm * -.75f, 0.1f, Utils.RandFloat(0.45f, 0.55f), 2, SpriteRenderer.color * 0.5f);
        if (timer > timeOut)
        {
            float alphaOut = 1 - (timer - timeOut) / fadeOut;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alphaOut);
            SpriteRendererGlow.color = new Color(SpriteRendererGlow.color.r, SpriteRendererGlow.color.g, SpriteRendererGlow.color.b) * alphaOut;
            MyTrail.Trail.startColor = MyTrail.Trail.startColor.WithAlpha(0.4f * alphaOut);
            MyTrail.originalAlpha = 0.4f * alphaOut;
        }
        timer++;
    }
    public override bool DoHomingBehavior(Enemy target, Vector2 norm, float scale)
    {
        if (timer > 150)
            return false; 
        if (Player.Instance.OrbitalStars)
        {
            homingCounter += 3; //Basically check homing again if it was successful previously
        }
        else
            homingCounter += 2;
        Vector2 targetPos = new Vector2(Data1, Data2);
        float modAmt = 0.025f + Mathf.Sqrt(scale) * 0.0075f;
        if (Player.Instance.OrbitalStars)
            modAmt *= 0.045f;
        targetPos = Vector2.Lerp(targetPos, target.transform.position, modAmt);
        Data1 = targetPos.x;
        Data2 = targetPos.y;
        return true;
    }
    public override void OnHitTarget(Entity target)
    {
        if(Player.Instance.Supernova > 0)
        {
            float chance = 0.2f;
            if(Utils.RandFloat() < chance)
            {
                NewProjectile<SupernovaProj>(transform.position, Vector2.zero, 5 + Player.Instance.Supernova * 5);
            }
        }
        OnHitByStar(target);
    }
}
