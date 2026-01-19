using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Pylon : MonoBehaviour
{
    public SpriteRenderer Crystal;
    public SpriteRenderer Glow;
    public SpriteRenderer Base;
    public GameObject Portal;
    public Sound sound;
    public bool SoundActive => sound != null;
    public bool WaveActive { get; private set; } = false;
    public int WavesRequired = 2;
    public int WavesPassed { get; private set; } = 0;
    public bool EndlessPylon = false;
    public bool Complete { get; private set; } = false;
    public void FixedUpdate()
    {
        bool nearby = Player.Position.Distance(transform.position) < Main.PylonActivationDist;
        if (nearby)
            if (!SoundActive)
                sound = AudioManager.PlaySound(SoundID.PylonDrone, transform.position, 1f, 1, 0);
        if (!Complete)
        {
            if (nearby || Main.CurrentPylon == this)
                ActiveAnim();
            else
                DisableAnimation();
        }
        else
        {
            if (WaveDirector.SkullEnemiesActive <= 0 || CompleteAnimCounter > 0)
                CompletionAnimation();
            else
                ActiveAnim();
        }
        if(SoundActive)
            sound.PylonSoundUpdate(this);
    }
    public float animCounter = 0;
    public int CompleteAnimCounter = 0;
    public void ActiveAnim()
    {
        Main.SetClosestPylon(this);
        animCounter++;

        float sin = Mathf.Sin(animCounter * Mathf.Deg2Rad * 1.4f) * 0.3f;
        float lerp = 0.035f;
        Crystal.transform.localPosition = Crystal.transform.localPosition.Lerp(new Vector3(0, 3 + sin, 1), lerp);
        Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * 0.8f, lerp);

        if (WaveActive && Main.CurrentPylon == this)
        {
            if (!Portal.activeSelf)
            {
                for (int i = -1; i <= 1; ++i)
                {
                    SummonLightning((Vector2)Portal.transform.position + new Vector2(0, 24).RotatedBy(Mathf.Deg2Rad * 33 * i), Portal.transform.position, Color.red);
                }
                Portal.SetActive(true);
                AudioManager.PlaySound(SoundID.PylonStart, transform.position, 2f, 1, 0);
            }
            else if(Utils.RandFloat(1) < 0.01f)
            {
                SummonLightning((Vector2)Portal.transform.position + new Vector2(0, Utils.RandFloat(3, 6.5f)).RotatedBy(Utils.RandFloat(-Mathf.PI, Mathf.PI)), Portal.transform.position, Color.red);
            }
            //float percent = WaveDirector.WaveProgressPercent >= 1 ? 0 : WaveDirector.WaveProgressPercent;
            //percent = (WavesPassed + percent) / Mathf.Max(1, WavesRequired);
            Color c = Color.red;// Color.Lerp(Color.red, CompletionColor(), percent * 0.75f);
            Crystal.color = Color.Lerp(Crystal.color, c, 0.1f).WithAlpha(Crystal.color.a);
            Glow.color = Color.Lerp(Glow.color, c * 0.55f, 0.1f);
            Glow.transform.localScale = Glow.transform.localScale.Lerp(Vector3.one * 20f, lerp);
        }
    }
    public void DisableAnimation()
    {
        float lerp = 0.045f;
        Crystal.transform.localPosition = Crystal.transform.localPosition.Lerp(new Vector3(0, 1, 1), lerp);
        Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * 0.6f, lerp);
    }
    public Color CompletionColor()
    {
        float sin = Mathf.Sin(animCounter * Mathf.Deg2Rad * 1.4f);
        return Color.Lerp(new Color(0.3f, 1f, 0.4f), new Color(0.4f, 1.0f, 0.3f), sin * 0.5f + 0.5f);//.WithAlpha(0.95f);
    }
    public void CompletionAnimation()
    {
        animCounter++;
        float sin = Mathf.Sin(animCounter * Mathf.Deg2Rad * 1.4f);
        float lerp = 0.02f;
        if(CompleteAnimCounter >= 1 || Player.Position.Distance(transform.position) < Main.PylonActivationDist * 0.9f)
        {
            if(CompleteAnimCounter == 0)
                AudioManager.PlaySound(SoundID.ChestSpawn, transform.position, 0.5f, 0.2f, 0);
            if(CompleteAnimCounter >= 186 && Portal != null)
                Portal.GetComponent<Wormhole>().Closing = true;
            CompleteAnimCounter++;
        }
        if (Input.GetKey(KeyCode.G))
            CompleteAnimCounter = 0;

        Color c = CompletionColor();
        float percent = Mathf.Min(1, CompleteAnimCounter / 200f);
        float perSin = Mathf.Sin(percent * percent * Mathf.PI);
        if (perSin < 0)
            perSin = 0;
        sin *= 1 - perSin;
        if(percent > 0 && CompleteAnimCounter % 4 == 0 && percent < 1)
            CompletionCircle(percent);
        if (CompleteAnimCounter >= 200)
        {
            if (CompleteAnimCounter == 200)
            {
                AudioManager.PlaySound(SoundID.PylonStart, transform.position, 0.5f, 0.5f, 0);
                float amt = 100;
                for(int i = 0; i < amt; ++i)
                {
                    float p = i / amt * Mathf.PI * 2;
                    Vector2 circular = new Vector2(0, 1.5f + 0.5f * Mathf.Sin(p * 5)).RotatedBy(p - Mathf.PI / 10f);
                    ParticleManager.NewParticle((Vector2)Crystal.transform.position + circular, Utils.RandFloat(0.35f, 0.4f), circular * 6f + Vector2.down * 2.4f, 0.7f, Utils.RandFloat(1.2f, 1.3f), ParticleManager.ID.Trail, c);
                }
                Crystal.transform.localScale = Vector2.one * 0.9f;
            }
            else if(Utils.RandFloat() < 0.1f)
            {
                float p = Utils.RandFloat() * Mathf.PI * 2;
                Vector2 circular = new Vector2(0, 2f).RotatedBy(p - Mathf.PI / 10f);
                ParticleManager.NewParticle((Vector2)Crystal.transform.position + circular, Utils.RandFloat(2.0f, 3.0f), circular * Utils.RandFloat(), 0.7f, Utils.RandFloat(1.4f, 1.6f), ParticleManager.ID.Pixel, c);
            }
            Crystal.color = c.WithAlpha(Crystal.color.a);
            Glow.color = c * 0.5f;
            Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * 0.8f, lerp);
        }
        else
        {
            percent *= 0.75f;
            Glow.color = Color.Lerp(Color.red * 0.55f, c * 0.5f, percent);
            Crystal.color = Color.Lerp(Color.red, c, percent).WithAlpha(Crystal.color.a);
            Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * (0.8f - percent * 0.1f), lerp);
        }
        Crystal.transform.localPosition = Crystal.transform.localPosition.Lerp(new Vector3(0, 3 + sin * 0.3f + Mathf.Sqrt(perSin) * 1.5f * percent, 1), lerp);

    }
    public void CompletionCircle(float percent)
    {
        Color c = Color.Lerp(Color.red, CompletionColor(), percent * percent) * (0.25f + Mathf.Sqrt(percent));
        float iPer = 1 - percent;
        float prevPercent = percent - 0.02f;
        float prevIPer = 1 - prevPercent;
        float total = 6f;
        for(int i = 0; i < total; ++i)
        {
            float bonus = i * Mathf.PI * 2 / total;
            float r = percent * Mathf.PI * 2 + bonus;
            float r2 = prevPercent * Mathf.PI * 2 + bonus;
            Vector2 circular = new Vector2(6 * iPer + 1.5f, 0).RotatedBy(r);
            Vector2 prev = new Vector2(6 * prevIPer + 1.5f, 0).RotatedBy(r2);
            Vector2 toPrev = prev - circular;
            Vector2 pos = (Vector2)Crystal.transform.position + circular;
            float mag = toPrev.magnitude;
            r = -toPrev.ToRotation() * Mathf.Rad2Deg;
            ParticleManager.NewParticle(pos, new Vector2(mag, 0.2f + 0.2f * iPer), Vector2.zero, 0, 0.5f, ParticleManager.ID.Line, c, r);
            if(Utils.RandFloat() < 0.5f)
            {
                ParticleManager.NewParticle(pos, 2.0f + 2.5f * iPer, Vector2.zero, 6f, 0.5f, ParticleManager.ID.Pixel, c);
            }
        }
    }
    public void IncrementWave()
    {
        WavesPassed++;
        if(!EndlessPylon)
        {
            if (WavesPassed >= WavesRequired)
            {
                CompletePylon();
                if(Main.CurrentPylon == this)
                    Main.FinishPylon();
            }
        }
    }
    public void CompletePylon()
    {
        Complete = true;
    }
    public void Enable()
    {
        WaveActive = true;

    }
    public static void SummonLightning(Vector2 start, Vector2 end, Color c)
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
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 0.1f) * 1.0f, .5f * scaleMult), Vector2.zero, 0, 1.2f, 4, c, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 0.1f) * 1.0f, .3f * scaleMult), Vector2.zero, 0, 1.2f, 4, Color.white * 1f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 1.2f) * 1.0f, .4f * scaleMult), Vector2.zero, 0, 1.2f, 4, c * 0.8f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 1.2f) * 1.0f, .2f * scaleMult), Vector2.zero, 0, 1.2f, 4, Color.white * 0.8f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 2.3f) * 1.0f, .3f * scaleMult), Vector2.zero, 0, 1.2f, 4, c * 0.6f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 2.3f) * 1.0f, .1f * scaleMult), Vector2.zero, 0, 1.2f, 4, Color.white * 0.6f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(Vector2.Lerp(pos, prev, Utils.RandFloat(1)), Utils.RandFloat(2, 3), Vector2.zero, 5, Utils.RandFloat(0.7f, 1.5f), 3, c * 1.5f);
            prev = pos;
        }
    }
    public static void SummonLightning2(Vector2 start, Vector2 end, Color c, float lifeMult = 0.6f)
    {
        float dist = Vector2.Distance(start, end);
        float distRounded = (int)(dist * 2.25f);
        Vector2 prev = start;
        if (distRounded <= 0)
            return;
        for (int i = 0; i <= distRounded; i++)
        {
            float perc = i / distRounded;
            float scaleMult = 1 - 0.5f * perc;
            Vector2 pos = Vector2.Lerp(start, end, perc) + 0.8f * Utils.RandCircle(Mathf.Sqrt(Mathf.Abs(Mathf.Sin(perc * Mathf.PI))));
            Vector2 toPrev = prev - pos;
            float mag = toPrev.magnitude + 0.1f;
            float r = -toPrev.ToRotation() * Mathf.Rad2Deg;
            ParticleManager.NewParticle(pos, new Vector2(mag * 1.0f, .5f * scaleMult), Vector2.zero, 0, lifeMult, 4, c, r);
            ParticleManager.NewParticle(pos, new Vector2(mag * 1.0f, .3f * scaleMult), Vector2.zero, 0, lifeMult, 4, Color.white * 1f, r);
            ParticleManager.NewParticle(Vector2.Lerp(pos, prev, Utils.RandFloat(1)), Utils.RandFloat(2, 3), Vector2.zero, 5, lifeMult, 3, Color.white);
            prev = pos;
        }
    }
}
