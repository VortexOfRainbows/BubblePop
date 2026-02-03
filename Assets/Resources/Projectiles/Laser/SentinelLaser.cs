using UnityEngine;

public class SentinelLaser : BoxProjectile
{
    public override void Init()
    {
        SpriteRenderer.enabled = false;
        SpriteRendererGlow.enabled = false;
        Friendly = false;
        Hostile = true;
        transform.SetEulerZ(RB.velocity.ToRotation() * Mathf.Rad2Deg);
    }
    public override void AI()
    {
        transform.position -= (Vector3)(RB.velocity * Time.fixedDeltaTime);
        if(timer <= 0)
        {
            Vector2 position = transform.position;
            timer = 0;
            Vector2 norm = RB.velocity;
            RaycastHit2D hit = Physics2D.Raycast(position, norm, 24, LayerMask.GetMask("World"));
            float dist = hit.distance == 0 ? 24 : hit.distance;
            C2D.size = new Vector2(dist, 0.5f);
            C2D.offset = new Vector2(dist / 2, 0);
            float increment = 0.25f;
            Vector2 spawnPos = (Vector2)position + norm;
            for (int i = 0; i < 20; ++i)
                ParticleManager.NewParticle(spawnPos, Utils.RandFloat(2.5f, 4.5f), norm * Utils.RandFloat(-1, 5), 5f, Utils.RandFloat(0.5f, 1.5f), ParticleManager.ID.Pixel, ColorHelper.SentinelColorsLerp(Utils.RandFloat()).WithAlpha(0.65f));
            Vector2 pos;
            for (int j = -1; j <= 1; j += 2)
            {
                pos = position + norm * 0.5f;
                Vector2 prev = pos;
                for (float i = 0; i < dist; i += increment)
                {
                    float secondPercentMult = i < 2 ? i / 2f : i > 18 ? 1 - (i - 18) / 6 : 1;
                    float scaleMult = 0.4f + i * 0.02f;
                    float sin = Mathf.Sin(i * Mathf.PI * 0.25f) * j * scaleMult;
                    spawnPos = pos + norm.RotatedBy(Mathf.PI / 2f) * sin;
                    Vector2 toPrev = prev - spawnPos;
                    float magnitude2 = toPrev.magnitude;
                    float r = -toPrev.ToRotation() * Mathf.Rad2Deg;
                    Color c2 = ColorHelper.SentinelColorsLerp(0.5f + 0.5f * sin * j);
                    ParticleManager.NewParticle(spawnPos, new Vector2(magnitude2, 0.2f + 0.4f * scaleMult), norm * 0.1f, 0, Mathf.Lerp(0.75f, 1.5f, 0.5f + 0.5f * sin * j), ParticleManager.ID.Line, c2.WithAlpha(0.6f * secondPercentMult),
                        r);
                    ParticleManager.NewParticle(spawnPos, new Vector2(magnitude2, 0.1f + 0.2f * scaleMult), norm * 0.1f, 0, Mathf.Lerp(0.6f, 1.5f, 0.5f + 0.5f * sin * j), ParticleManager.ID.Line, Color.white * secondPercentMult,
                        r);
                    ParticleManager.NewParticle(spawnPos, Utils.RandFloat(2.0f, 3.5f), norm * 3, 3f, Utils.RandFloat(0.5f, 1.5f), ParticleManager.ID.Pixel, c2.WithAlpha(0.5f * secondPercentMult));
                    pos += norm * increment;
                    prev = spawnPos;
                }
            }
            RB.velocity *= 0;
        }
        if (timer > 12)
            Hostile = false;
        if (++timer > 50)
            Kill();
    }
}
