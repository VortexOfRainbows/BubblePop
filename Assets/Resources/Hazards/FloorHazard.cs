using Unity.VisualScripting;
using UnityEngine;

public class FloorHazard : MonoBehaviour
{
    public Transform Visual;
    public SpriteRenderer Renderer;
    public SpriteRenderer BorderRenderer;
    public float TimePassed { get; set; }
    public Vector2 TargetScale = Vector2.one;
    public float SizeMult {  get; set; }
    public int InitDuration { get; set; }
    public void Init(HazardSystem.HazardType type, int initialAppliedDuration, float sizeMultiplier = 1.0f)
    {
        if (type == HazardSystem.HazardType.Oil)
        {
            Renderer.color = ColorHelper.KingOilColor.WithAlpha(0);
            BorderRenderer.color = Renderer.color.Lerp(Color.black, 0.5f).WithAlpha(0);
        }
        Visual.transform.localPosition += (Vector3)Utils.RandCircle(0.1f);
        float x = Utils.RandFloat(0.9f, 1.0f);
        float y = Utils.RandFloat(0.9f, 1.0f);
        TargetScale = new Vector2(x, y);
        SizeMult = sizeMultiplier;
        InitDuration = initialAppliedDuration;
        Visual.transform.localScale = TargetScale * SizeMult;
    }
    public void TickUpdate(float timeLeft)
    {
        float scaleMult = SizeMult;
        if (timeLeft < InitDuration && TimePassed > 10)
        {
            float timeRemaining = timeLeft / (float)InitDuration;
            scaleMult *= timeRemaining;
        }
        Visual.transform.LerpLocalScale(TargetScale * scaleMult, 0.1f);
        ++TimePassed;
        float fadeIn = Mathf.Clamp01(TimePassed / 10f);
        if (timeLeft < 20)
        {
            float timeRemaining = timeLeft / 20f;
            fadeIn *= timeRemaining;
        }
        fadeIn *= 0.5f;
        Renderer.color = Renderer.color.WithAlpha(fadeIn);
        BorderRenderer.color = BorderRenderer.color.WithAlpha(fadeIn);
    }
}
