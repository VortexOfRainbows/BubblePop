using UnityEngine;

//#if UNITY_EDITOR
//[ExecuteAlways]
//#endif
public class TitleScreen : MonoBehaviour
{
    public Canvas ScalingCanvas;
    public RectTransform TargetRect;
    public RectTransform Bubblemancer, BubbleAnchor, ThoughtBubble, TBAnchor, Fizzy, FizzyAnchor, Golem, Infector;
    public RectTransform Title;
    public RectTransform Portal;
    public float AnimateCounter = 0;
    public void Update()
    {
        float horizontalRelativeSize = Screen.width / ScalingCanvas.scaleFactor;
        //3840 is the size of the banner itself, where -1600 is the correct number
        //3840 - x = -1600
        //x = 5440
        float pinchAmt = horizontalRelativeSize - 5440;
        pinchAmt = Mathf.Clamp(pinchAmt, -3130, -1600);
        float percent = (pinchAmt + 1600) / (-3130 + 1600);
        //Debug.Log(Fizzy.anchoredPosition);
        TargetRect.sizeDelta = new Vector2(pinchAmt, TargetRect.sizeDelta.y);
        BubbleAnchor.localScale = Vector2.Lerp(Vector2.one, new Vector2(0.8f, 0.8f), percent);
        //TBAnchor.localScale = Vector2.Lerp(Vector2.one, new Vector2(0.9f, 0.9f), percent);
        TBAnchor.transform.localPosition = new Vector3(BubbleAnchor.transform.localPosition.x + Mathf.Lerp(2850, 2100, percent), TBAnchor.transform.localPosition.y, TBAnchor.transform.localPosition.z);
        Fizzy.localPosition = new Vector3(Mathf.Lerp(100, 427, percent) - 2378 / 2, 0, 0);
        FizzyAnchor.localScale = Vector2.Lerp(Vector2.one, new Vector2(0.8f, 0.8f), percent);

        Animate();
    }
    public void Animate()
    {
        AnimateCounter += Time.unscaledDeltaTime * 0.5f;
        float sin2 = Mathf.Sin(AnimateCounter * Mathf.PI * 0.5f);
        //Bubblemancer.SetLocalEulerZ(1f * sin);
        Bubblemancer.localScale = new Vector3(1 - sin2 * 0.02f, 1 + sin2 * 0.02f, 1);

        float sin3 = Mathf.Cos(AnimateCounter * Mathf.PI * 0.25f);
        float sin4 = Mathf.Cos(AnimateCounter * Mathf.PI * 0.5f);
        ThoughtBubble.SetLocalEulerZ(0.5f * sin3);
        ThoughtBubble.localScale = new Vector3(1 - sin4 * 0.02f, 1 + sin4 * 0.04f, 1);

        float sin5 = Mathf.Sin(AnimateCounter * Mathf.PI * 0.4f + Mathf.PI * 0.25f);
        Fizzy.localScale = new Vector3(1 - sin5 * 0.01f, 1 + sin5 * 0.015f, 1);

        float sin6 = 3 * Mathf.Sin(AnimateCounter * Mathf.PI / 9f);
        Title.transform.localScale = Vector3.one * (0.88f + 0.03f * sin2);
        Title.transform.localEulerAngles = new Vector3(0, 0, sin6);


        Golem.transform.localScale = new Vector3(1 - sin3 * 0.02f, 1 + sin3 * 0.04f, 1) * 0.9f;
        float sin = Mathf.Sin(AnimateCounter * Mathf.PI * 0.3f);
        Infector.transform.SetLocalXY(Mathf.Sin(AnimateCounter * Mathf.PI * 0.375f) * 5 - 334/2f, sin * 20 + 391/2f); //334 and 391 are the dimensions of the infector rect
        sin = Mathf.Sin(AnimateCounter * Mathf.PI * 0.55f);
        Portal.transform.localScale = new Vector3(1 - sin * 0.01f, 1 + sin * 0.03f, 1);
    }
    public void OldMenuScaling()
    {
        float scale = ScalingCanvas.scaleFactor * 1.8f;
        transform.localScale = new Vector3(Mathf.Max(1.25f, scale), 1, 1);
    }
}