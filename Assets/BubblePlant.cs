using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BubblePlant : MonoBehaviour
{
    public Transform Mushroom;
    public Light2D MyLight;
    public bool IsLilypad = false;
    private Vector3 originalScale = Vector3.one;
    private bool runOnce = true;
    private bool isVisible = true;
    public void AdjustLightVisiblity()
    {
        if (MyLight == null || CameraManager.MainCamera == null)
            return;

        Camera main = CameraManager.MainCamera;
        float deltaX = Mathf.Abs(transform.position.x - main.transform.position.x);
        float deltaY = Mathf.Abs(transform.position.y - main.transform.position.y);

        // Calculate the camera's exact half-dimensions in world units
        float camHalfHeight = main.orthographicSize;
        float camHalfWidth = camHalfHeight * main.aspect;

        // Check if the light position exceeds the camera width/height plus your custom buffer
        float screenBuffer = MyLight.pointLightOuterRadius; // Adjust this factor as needed
        isVisible = (deltaX <= camHalfWidth + screenBuffer) &&
                         (deltaY <= camHalfHeight + screenBuffer);
        MyLight.enabled = isVisible;
    }
    public void Update()
    {
        if (IsLilypad && runOnce)
        {
            originalScale = transform.localScale;
            runOnce = false;
        }
        AdjustLightVisiblity();
        if (!isVisible)
            return;
        if(Mushroom != null)
        {
            float sin = 0.5f + 0.5f * Mathf.Sin(World.GlobalTimeElapsedCounter * Mathf.PI * 0.5f + transform.position.x + transform.position.y);
            Mushroom.transform.localScale = new Vector3(originalScale.x  * (1 + sin * 0.05f), originalScale.y * ( 1 + sin * 0.1f), 1);
        }
        MyLight.pointLightOuterRadius = Mathf.Abs((2.4f + Lighting.NightSin) * transform.localScale.x * 5.5f);
    }
    public void FixedUpdate()
    {
        if (!isVisible)
            return;
        if (Mushroom != null)
        {
            if (Lighting.NightSin > 0.2f)
            {
                if (Utils.RandFloat(50) < Lighting.NightSin)
                {
                    float lilyMult = IsLilypad ? 0.5f : 1f;
                    Vector2 pos = (Vector2)Mushroom.position;
                    pos.y += 0.5f * lilyMult;
                    Vector2 velo = Utils.RandCircleEdge(0.7f);
                    ParticleManager.NewParticle(pos + velo, 2f, velo, 0.5f, 5.0f, ParticleManager.ID.Pixel, MyLight.color);
                }
            }
        }
        else
        {
            if (Lighting.NightSin > 0.2f)
            {
                if (Utils.RandFloat(50) < Lighting.NightSin)
                {
                    Vector2 pos = (Vector2)MyLight.transform.position;
                    Vector2 velo = Utils.RandCircleEdge(0.7f);
                    ParticleManager.NewParticle(pos + velo, 2f, velo, 0.5f, 5.0f, ParticleManager.ID.Pixel, MyLight.color);
                }
            }
        }
    }
}
