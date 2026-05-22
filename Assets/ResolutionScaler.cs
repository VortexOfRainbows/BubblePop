using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class ResolutionScaler : MonoBehaviour
{
    public Canvas ScalingCanvas;
    public void Update()
    {
        if (ScalingCanvas == null)
            return;
        RectTransform r = ScalingCanvas.GetComponent<RectTransform>();
        Debug.Log($"{r.rect.width}, {r.rect.height}");

        Vector2 assetResolution = new Vector2(7680, 2480);
        float pixelScaler = 100f; //arbitrary default value for Unity sprites
        pixelScaler *= Camera.main.orthographicSize;

        float ratioX = assetResolution.x / pixelScaler;
        float ratioY = assetResolution.y / pixelScaler;

        float scaleTo1920 = assetResolution.x / 1920;
        float scaleTo1080 = assetResolution.y / 1080;

        transform.localScale = new Vector3(1 / scaleTo1920, 1 / scaleTo1080, 1);
    }
    public void OldMenuScaling()
    {
        float scale = ScalingCanvas.scaleFactor * 1.8f;
        transform.localScale = new Vector3(Mathf.Max(1.25f, scale), 1, 1);
    }
}