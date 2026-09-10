using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BubblePlant : MonoBehaviour
{
    private void OnBecameVisible()
    {
        MyLight.enabled = enabled = true;
    }
    private void OnBecameInvisible()
    {
        MyLight.enabled = enabled = false;
    }
    public Transform Mushroom;
    public Light2D MyLight;
    public void Update()
    {
        if(Mushroom != null)
        {
            float sin = 0.5f + 0.5f * Mathf.Sin(World.GlobalTimeElapsedCounter * Mathf.PI * 0.5f + transform.position.x + transform.position.y);
            Mushroom.transform.localScale = new Vector3(1 + sin * 0.05f, 1 + sin * 0.1f, 1);
        }
        MyLight.pointLightOuterRadius = (2.4f + Lighting.NightSin) * transform.localScale.x * 5.5f;
    }
    public void FixedUpdate()
    {
        if(Mushroom != null)
        {
            if (Lighting.NightSin > 0.2f)
            {
                if (Utils.RandFloat(50) < Lighting.NightSin)
                {
                    Vector2 pos = (Vector2)Mushroom.position;
                    pos.y += 0.5f;
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
