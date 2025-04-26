using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Wormhole : MonoBehaviour
{
    public Light2D Light;
    public List<GameObject> Rings;
    public float Timer;
    public float ScaleSpeed = 0;
    public float Scale = 0;
    void Start()
    {
        FixedUpdate();   
    }
    void FixedUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            Timer = 0;
            ScaleSpeed = 0;
            Scale = 0;
        }
        float p = Mathf.Min(2, Timer / 2f);
        if(p < 1)
        {
            for(int i = 0; i < 5; ++i)
            {
                Vector2 circular = new Vector2(15 + p * 4 + Utils.RandFloat(2), 0).RotatedBy(Mathf.PI * (i / 5f * 2) + Utils.RandFloat(Mathf.PI * 0.4f));
                ParticleManager.NewParticle(transform.position, Utils.RandFloat(1, 3), circular, 1 + p, Utils.RandFloat(0.5f, 1.0f), 3, Color.red * 1.5f);
            }
        }
        else
        {
            for (int i = 0; i < 2; ++i)
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * Utils.RandFloat(2));
                float r = Utils.RandFloat(0.2f, 1);
                r = Mathf.Max(r, Utils.RandFloat(1));
                ParticleManager.NewParticle((Vector2)transform.position + circular * r * 5, Utils.RandFloat(1, 3), circular * Utils.RandFloat(1, 5), 1, Utils.RandFloat(0.4f, 0.7f), 3, Color.red * (r + 0.1f) * 1.5f);
            }
        }
        Light.intensity = p * p * 5f;
        if (Scale < 1)
        {
            ScaleSpeed += 0.01f * (2 - p);
        }
        else
        {
            ScaleSpeed -= 0.01f * (2 - p);
        }
        ScaleSpeed *= 0.9f;
        Scale = Mathf.Lerp(Scale, 1, 0.05f);
        Scale = Mathf.Lerp(Scale, Scale + ScaleSpeed, 0.5f);
        Timer += Time.fixedDeltaTime * 10;
        transform.localScale = Vector3.one * Scale;
        for (int i = 0; i < Rings.Count; ++i)
        {
            float aPercent = (float)(i + 1) / Rings.Count;
            float percent = (float)i / Rings.Count;
            float iPercent = 1 - percent;
            float myPercent = p * 0.4f + 0.6f * Mathf.Min(1, p - iPercent * 2);
            GameObject ring = Rings[i];
            ring.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, myPercent * aPercent);
            Vector2 circular = new Vector2(iPercent * 0.012f, 0).RotatedBy(Mathf.PI * percent * 4 + Timer * iPercent);
            ring.transform.localPosition = new Vector3(circular.x, circular.y, ring.transform.localPosition.z);

            float r = percent * 420 + Timer * (i * (0.5f + iPercent));
            float sin = Mathf.Sin(r * Mathf.Deg2Rad * 1.5f);
            ring.transform.localEulerAngles = new Vector3(0, 0, r * 3 * (i % 2 * 2 - 1));
            ring.transform.localScale = Vector3.one * (sin * 0.1f + 1f);
        }
    }
}
