using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class Pylon : MonoBehaviour
{
    public SpriteRenderer Crystal;
    public SpriteRenderer Glow;
    public SpriteRenderer Base;
    public GameObject Portal;
    public bool Active = false;
    public void Start()
    {
        
    }
    public void FixedUpdate()
    {
        if ((Player.Position - (Vector2)transform.position).magnitude < 11 || Main.WavesUnleashed)
        {
            if(!Active)
                AudioManager.PlaySound(SoundID.PylonDrone, transform.position, 1f, 1, 0);
            Active = true;
        }
        else
            Active = false;
        if (Active)
            ActiveAnim();
        else
            DisableAnimation();
    }
    public float animCounter = 0;
    public void ActiveAnim()
    {
        Main.CurrentPylon = this;
        animCounter++;

        float sin = Mathf.Sin(animCounter * Mathf.Deg2Rad * 1.4f) * 0.3f;
        float lerp = 0.035f;
        Crystal.transform.localPosition = Crystal.transform.localPosition.Lerp(new Vector3(0, 3 + sin, 1), lerp);
        Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * 0.8f, lerp);

        if (Main.WavesUnleashed)
        {
            if (!Portal.activeSelf)
            {
                for (int i = -1; i <= 1; ++i)
                {
                    SummonLightning((Vector2)Portal.transform.position + new Vector2(0, 24).RotatedBy(Mathf.Deg2Rad * 33 * i), Portal.transform.position);
                }
                Portal.SetActive(true);
                AudioManager.PlaySound(SoundID.PylonStart, transform.position, 2f, 1, 0);
            }
            else if(Utils.RandFloat(1) < 0.01f)
            {
                SummonLightning((Vector2)Portal.transform.position + new Vector2(0, Utils.RandFloat(3, 6.5f)).RotatedBy(Utils.RandFloat(-Mathf.PI, Mathf.PI)), Portal.transform.position);
            }
            Crystal.color = Color.Lerp(Crystal.color, Color.red, 0.1f);
            Glow.color = Color.Lerp(Glow.color, Color.red * 0.55f, 0.1f);
            Glow.transform.localScale = Glow.transform.localScale.Lerp(Vector3.one * 20f, lerp);
        }
    }
    public void DisableAnimation()
    {
        float lerp = 0.045f;
        Crystal.transform.localPosition = Crystal.transform.localPosition.Lerp(new Vector3(0, 1, 1), lerp);
        Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * 0.6f, lerp);
    }
    public void SummonLightning(Vector2 start, Vector2 end)
    {
        float dist = Vector2.Distance(start, end);
        float distRounded = (int)dist;
        Vector2 prev = start;
        for(int i = 0; i < distRounded; i++)
        {
            float perc = i / distRounded;
            float scaleMult = 1f + 0.5f * (1 - perc);
            Vector2 pos = Vector2.Lerp(start, end, perc) + 0.8f * Utils.RandCircle(Mathf.Sqrt(Mathf.Abs(Mathf.Sin(perc * Mathf.PI))));
            Vector2 toPrev = prev - pos;
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 0.1f) * 1.0f, .5f * scaleMult), Vector2.zero, 0, 1.2f, 4, Color.red, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 0.1f) * 1.0f, .3f * scaleMult), Vector2.zero, 0, 1.2f, 4, Color.white * 1f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 1.2f) * 1.0f, .4f * scaleMult), Vector2.zero, 0, 1.2f, 4, Color.red * 0.8f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 1.2f) * 1.0f, .2f * scaleMult), Vector2.zero, 0, 1.2f, 4, Color.white * 0.8f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 2.3f) * 1.0f, .3f * scaleMult), Vector2.zero, 0, 1.2f, 4, Color.red * 0.6f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 2.3f) * 1.0f, .1f * scaleMult), Vector2.zero, 0, 1.2f, 4, Color.white * 0.6f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            prev = pos;
        }
    }
}
