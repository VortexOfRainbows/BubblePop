using Unity.Properties;
using UnityEngine;

public class MeleeHitbox : Projectile
{
    public override bool OnInsideTile()
    {
        return false;
    }
    public override bool OnTileCollide(Collider2D collision)
    {
        return false;
    }
    public float Type => Data.Length > 0 ? Data1 : -1;
    public override void Init()
    {
        SpriteRendererGlow.color = new Color(245 / 255f, 191 / 255f, 7 / 255f);
        SpriteRenderer.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.transform.localScale *= 1.1f;
        SpriteRendererGlow.color = new Color(1, 0.1f, 0.1f, 1f);
        SpriteRenderer.color = new Color(1, 1f, 1f, 5f);
        SpriteRenderer.material = Resources.Load<Material>("Materials/Additive");
        SpriteRendererGlow.material = Resources.Load<Material>("Particles/Bubble2");
        SpriteRendererGlow.enabled = false;
        SpriteRenderer.enabled = false;
        Friendly = false;
        Hostile = false;
        Penetrate = -1;
    }
    public override void OnHitTarget(Entity target)
    {
        if (Type == 0)
        {
            //float coinValue = Damage / 12;
            //coinValue = Mathf.Clamp(coinValue, 0, 1 + Player.Instance.ConsolationPrize * 0.1f);
            //float bonus = coinValue - (int)coinValue;
            //if (Utils.RandFloat() < bonus)
            //    coinValue++;
            if(target.Life <= 0)
                CoinManager.SpawnToken(target.transform.position, 0.1f);

            AudioManager.PlaySound(SoundID.StarbarbImpact, transform.position, 0.6f, 0.475f, 0);
            AudioManager.PlaySound(SoundID.SoapDie, transform.position, 2, 1.7f, 0);
            for (int i = 0; i < 35; ++i)
                ParticleManager.NewParticle(target.transform.position + new Vector3(Utils.RandFloat(-1f, 1f), Utils.RandFloat(-1f, 1f)), 3, RB.velocity * Utils.RandFloat(1.0f) + Utils.RandCircle(5), 5, Utils.RandFloat(0.7f, 0.8f), 3,
                    Color.Lerp(ColorHelper.RarityColors[0], ColorHelper.RarityColors[4], Utils.RandFloat()) * 0.95f);
        }
    }
    public override void AI()
    {
        if (Type == 0)
        {
            transform.position -= (Vector3)(RB.velocity * Time.fixedDeltaTime);
        }
    }
    public override void OnKill()
    {

    }
    public override bool CanBeAffectedByHoming()
    {
        return false;
    }
}
