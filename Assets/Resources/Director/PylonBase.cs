using UnityEngine;

public class PylonBase : InteractableWorldObject
{
    public SpriteRenderer Crystal;
    public SpriteRenderer Glow;
    public SpriteRenderer Base;
    public float PointerAlpha = -3.0f;
    public void Start()
    {
        Player.ObjectsConsideredForUIInteraction.Add(gameObject);
    }
    public void CreatePointers()
    {
        Vector3 position = Crystal.transform.position;

        float clampDistance = 190;
        Vector2 clamped = Utils.ClampToScreenEdge(position, clampDistance * Main.ActivePrimaryCanvas.scaleFactor);
        Vector2 realPosition = Utils.ClampToScreenEdge(position, 10 * Main.ActivePrimaryCanvas.scaleFactor); //So the pointer fades away when the player gets close
        position.x = clamped.x;
        position.y = clamped.y;


        Vector2 toPointer = realPosition - (Vector2)Crystal.transform.position;
        float distanceFromPointer = toPointer.magnitude;
        Vector2 toPointer2 = position - Crystal.transform.position;
        float scaleFactor = Mathf.Clamp(toPointer2.magnitude - 1f, 0, 1);

        if (distanceFromPointer > 1)
            PointerAlpha += Time.unscaledDeltaTime * 1.5f;
        else
            PointerAlpha -= Time.unscaledDeltaTime;
        PointerAlpha = Mathf.Clamp(PointerAlpha, -3f, scaleFactor);

        SpriteBatch.Draw(Crystal.sprite, position, Vector2.one * 0.45f, 0, Color.white.WithAlpha(Mathf.Max(PointerAlpha * 0.75f, 0)), 21, Main.TextureAssets.SpriteGlowmask);
    }
}
