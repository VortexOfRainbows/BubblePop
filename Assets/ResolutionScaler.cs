using UnityEngine;

public class ResolutionScaler : MonoBehaviour
{
    public Canvas ScalingCanvas;
    private void Update()
    {
        float scale = ScalingCanvas.scaleFactor * 1.8f;
        //Debug.Log(scale);
        transform.localScale = new Vector3(Mathf.Max(1.25f, scale), 1, 1);
    }
}
