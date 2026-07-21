using UnityEngine;

public class FloorHazard : MonoBehaviour
{
    public Transform Visual;
    public SpriteRenderer Renderer;
    public float TimePassed { get; set; }
    public Vector2 TargetScale = Vector2.one;
    public float SizeMult {  get; set; }
    public int InitDuration { get; set; }
    public void Init(HazardSystem.HazardType type, int initialAppliedDuration, float sizeMultiplier = 1.0f)
    {
        Visual.transform.localPosition += Vector3.one;
        Visual.transform.localPosition += (Vector3)Utils.RandCircle(0.1f);
        Visual.transform.localScale = Vector3.zero;
        float x = Utils.RandFloat(0.9f, 1.0f);
        float y = Utils.RandFloat(0.9f, 1.0f);
        TargetScale = new Vector2(x, y);
        if (type == HazardSystem.HazardType.Oil)
            Renderer.color = ColorHelper.KingOilColor.WithAlpha(0);
        SizeMult = sizeMultiplier;
        InitDuration = initialAppliedDuration;
    }
    public void TickUpdate(float timeLeft)
    {
        float scaleMult = SizeMult;
        if (timeLeft < InitDuration && TimePassed > 15)
        {
            float timeRemaining = timeLeft / (float)InitDuration;
            scaleMult *= timeRemaining;
        }
        Visual.transform.LerpLocalScale(TargetScale * scaleMult, 0.1f);
        ++TimePassed;
        float fadeIn = Mathf.Clamp01(TimePassed / 15f);
        if (timeLeft < InitDuration / 2f)
        {
            float timeRemaining = timeLeft / InitDuration * 2f;
            fadeIn *= timeRemaining;
        }

        Renderer.color = Renderer.color.WithAlpha(fadeIn);
    }
}
