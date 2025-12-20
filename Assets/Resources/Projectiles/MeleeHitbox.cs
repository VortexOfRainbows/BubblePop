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
    public override void AI()
    {

    }
    public override void OnKill()
    {

    }
}
