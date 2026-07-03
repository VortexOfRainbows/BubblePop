using UnityEngine;

public class PylonBase : MonoBehaviour
{
    public SpriteRenderer Crystal;
    public SpriteRenderer Glow;
    public SpriteRenderer Base;
    public float PointerAlpha = -3.0f;
    public void CreatePointers()
    {
        Vector3 position = Crystal.transform.position;

        Vector2 clamped = Utils.ClampToScreenEdge(position, 40);
        position.x = clamped.x;
        position.y = clamped.y;


        Vector2 toPointer = position - Crystal.transform.position;
        float distanceFromPointer = toPointer.magnitude;
        float scaleFactor = Mathf.Clamp(distanceFromPointer - 1, 0, 1);

        if (distanceFromPointer > 1)
            PointerAlpha += Time.unscaledDeltaTime * 1.5f;
        else
            PointerAlpha -= Time.unscaledDeltaTime;
        PointerAlpha = Mathf.Clamp(PointerAlpha, -3f, scaleFactor);

        SpriteBatch.Draw(Crystal.sprite, position, Vector2.one * 0.4f, 0, Color.white.WithAlpha(Mathf.Max(PointerAlpha * 0.7f, 0)), 21, Main.TextureAssets.SpriteGlowmask);
    }
}
