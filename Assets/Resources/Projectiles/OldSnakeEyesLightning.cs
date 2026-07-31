using UnityEngine;
public class SnakeLightning : Projectile
{
    public override void Init()
    {
        transform.localScale = Vector3.one * 3f;
        SpriteRenderer.enabled = false;
        SpriteRendererGlow.gameObject.SetActive(false);
        SpriteRenderer.sprite = null;
        Damage = 1;
        Penetrate = -1;
        Friendly = false;
        Hostile = false;
        Lightning();
    }
    public void Lightning()
    {
        for (int i = 0; i < 15; ++i)
        {
            Vector2 circular = new Vector2(3 + Utils.RandFloat(6), 0).RotatedBy(Mathf.PI * (i / 5f * 2) + Utils.RandFloat(Mathf.PI * 0.4f));
            ParticleManager.NewParticle(transform.position, Utils.RandFloat(2, 3), circular, 5, Utils.RandFloat(0.5f, 1.5f), 3, Color.green * 1.5f);
        }
        ParticleManager.SummonLightningPylon((Vector2)transform.position - RB.velocity * 0.4f, (Vector2)transform.position + RB.velocity * 2.4f, Color.green);
    }
    public override void AI()
    {
        timer++;
        if (timer > 15)
            Friendly = true;
        if (timer > 40)
            Kill();
    }
    public override bool? CanBeAffectedByHoming() => false;
}