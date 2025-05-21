using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Wormhole : MonoBehaviour
{
    public static Wormhole Spawn(Vector2 location, GameObject[] EnemyPrefabs, float spawnDelay = 20)
    {
        Wormhole w = Instantiate(EnemyID.PortalPrefab, location, Quaternion.identity).GetComponent<Wormhole>();
        w.QueuedEnemies = EnemyPrefabs;
        w.SpawnDelay = spawnDelay;
        return w;
    }

    public GameObject[] QueuedEnemies;
    public int enemyNum = 0;
    public float SpawnDelay = 0;

    public Light2D Light;
    public List<GameObject> Rings;
    public GameObject Visual;
    public float ParticleEffectMult => ScaleMultiplier / 9f;
    private float ScaleMultiplier = 5.25f;
    public float Timer;
    public float Timer2;
    public float ScaleSpeed = 0;
    public float Scale = 0;
    public bool Closing = false;
    public void Start()
    {
        Visual.transform.localScale = new Vector3(ScaleMultiplier, ScaleMultiplier, 1);
        FixedUpdate();   
    }
    public void FixedUpdate()
    {
        //if (Input.GetMouseButton(1))
        //{
        //    Timer = 0;
        //    ScaleSpeed = 0;
        //    Scale = 0;
        //    Timer2 = 0;
        //    Closing = false;
        //}
        float p = Mathf.Min(2, Timer / 2f);
        if(p < 1 && !Closing)
        {
            for(int i = 0; i < 5; ++i)
            {
                Vector2 circular = new Vector2(15 + p * 4 + Utils.RandFloat(2), 0).RotatedBy(Mathf.PI * (i / 5f * 2) + Utils.RandFloat(Mathf.PI * 0.4f)) * ParticleEffectMult;
                ParticleManager.NewParticle(transform.position, Utils.RandFloat(1, 3), circular, 1 + p, Utils.RandFloat(0.5f, 1.0f), 3, Color.red * 1.5f);
            }
        }
        else
        {
            for (int i = 0; i < 2; ++i)
            {
                Vector2 circular = new Vector2(1 * Scale, 0).RotatedBy(Mathf.PI * Utils.RandFloat(2));
                float r = Utils.RandFloat(0.2f, 1);
                r = Mathf.Max(r, Utils.RandFloat(1));
                ParticleManager.NewParticle((Vector2)transform.position + circular * r * 5 * ParticleEffectMult, Utils.RandFloat(1, 3), circular * Utils.RandFloat(1, 5), 1, Utils.RandFloat(0.4f, 0.7f), 3, Color.red * (r + 0.1f) * 1.5f);
            }
        }

        if (p >= 2)
        {
            Timer2++;
            if(Timer2 >= 20 && !Closing)
            {
                for (int i = 0; i < 30; ++i)
                {
                    Vector2 circular = new Vector2(10 + Utils.RandFloat(10), 0).RotatedBy(Mathf.PI * (i / 5f * 2) + Utils.RandFloat(Mathf.PI * 0.4f)) * ParticleEffectMult;
                    ParticleManager.NewParticle(transform.position, Utils.RandFloat(1, 3), circular, 1 + p, Utils.RandFloat(0.5f, 1.0f), 3, Color.red * 1.5f);
                }
                if(enemyNum < QueuedEnemies.Length)
                {
                    Instantiate(QueuedEnemies[enemyNum++], transform.position, Quaternion.identity);
                }
                if (enemyNum >= QueuedEnemies.Length)
                {
                    Timer2 = 0;
                    Closing = true;
                }
                else
                    Timer2 -= SpawnDelay;
            }
        }

        if(Closing)
        {
            if(Timer2 > 50)
            {
                float percent = (Timer2 - 50) / 60f;
                float sin = Mathf.Sin(percent * Mathf.PI) * 0.6f + 1 - percent;
                Scale = Mathf.Lerp(Scale, Mathf.Max(0, sin), 0.1f);
            }
            if (Scale <= 0.2f)
                Kill();
        }
        else
        {
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
        }
        Timer += Time.fixedDeltaTime * 10;
        transform.localScale = Vector3.one * Scale;
        Light.intensity = p * p * 5f * Scale;
        Light.pointLightOuterRadius = 0.7f * ScaleMultiplier * Scale;
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
    public void Kill()
    {
        for (int i = 0; i < 60; ++i)
        {
            Vector2 circular = new Vector2(4 + Utils.RandFloat(20), 0).RotatedBy(Mathf.PI * (i / 5f * 2) + Utils.RandFloat(Mathf.PI * 0.4f)) * ParticleEffectMult;
            ParticleManager.NewParticle(transform.position, Utils.RandFloat(2, 3), circular, 5, Utils.RandFloat(0.5f, 1.5f), 3, Color.red * 1.5f);
        }
        Destroy(gameObject);
    }
}
