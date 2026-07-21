using Unity.VisualScripting;
using UnityEngine;

public class FloorHazard : MonoBehaviour
{
    public Transform Visual;
    public SpriteRenderer Renderer;
    public float TimePassed { get; set; }
    public Vector2 TargetScale = Vector2.one;
    public float SizeMult {  get; set; }
    public int InitDuration { get; set; }
    public int Counter { get; set; } = 0;
    public void Init(HazardSystem.HazardType type, int initialAppliedDuration, float sizeMultiplier = 1.0f)
    {
        if (type == HazardSystem.HazardType.Oil)
            Renderer.color = ColorHelper.KingOilColor.Lerp(Color.black, 0.2f).WithAlpha(0);
        Visual.transform.localPosition += (Vector3)Utils.RandCircle(0.1f);
        float x = Utils.RandFloat(0.9f, 1.0f);
        float y = Utils.RandFloat(0.9f, 1.0f);
        TargetScale = new Vector2(x, y);
        SizeMult = sizeMultiplier;
        InitDuration = initialAppliedDuration;
        Visual.transform.localScale = TargetScale * SizeMult;
    }
    public bool TickUpdate()
    {
        float timeLeft = InitDuration - Counter;
        if (timeLeft <= 0)
            return false;
        float scaleMult = SizeMult;
        if (timeLeft < InitDuration && TimePassed > 15)
        {
            float timeRemaining = Mathf.Max(0.2f, timeLeft / InitDuration);
            scaleMult *= timeRemaining;
        }
        Visual.transform.LerpLocalScale(TargetScale * scaleMult, 0.1f);
        ++TimePassed;
        float fadeIn = Mathf.Clamp01(TimePassed / 15f);
        if (timeLeft < 50)
        {
            float timeRemaining = timeLeft / 50f;
            fadeIn *= timeRemaining;
        }
        Renderer.color = Renderer.color.WithAlpha(fadeIn);
        Counter++;
        return true;
    }
}
