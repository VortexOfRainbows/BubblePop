using System.Collections.Generic;
using UnityEngine;

//Fast reload hates this script for some reason
public class ParticleManager : MonoBehaviour
{
    public static class ID
    {
        public const int Bubble = 0;
        public const int Square = 1;
        public const int Circle = 2;
        public const int Pixel = 3;
        public const int Line = 4;
        public const int Trail = 5;
        public const int LineForeground = 6;
        public const int Snow = 7;
        public const int Fire = 8;
    }
    public static readonly Color DefaultColor = new(0.89f, 0.8078f, 0.9412f, 0.5f);
    public static ParticleManager Instance;
    public Transform ParticleSuperParent;
    public List<ParticleSystem> ParticleSystems { get; private set; }
    public List<ParticleSystem> FetchMySystems()
    {
        List<ParticleSystem> list = new List<ParticleSystem>();
        int childCount = ParticleSuperParent.childCount;
        for (int i = 0; i < childCount; i++)
            list.Add(ParticleSuperParent.GetChild(i).GetComponent<ParticleSystem>());
        return list;
    }
    public static void NewParticle(Vector2 pos, float size, Vector2 velo = default, float randomizeFactor = 0, float lifeTime = 0.5f, int type = 0, Color color = default)
    {
        if (Instance == null)
            return;
        float rotation = type == ID.Fire ? 0 : Utils.RandFloat(360);
        if (color == default)
            color = type == ID.Bubble ? DefaultColor : Color.white;
        ParticleSystem.EmitParams style = new()
        {
            position = pos,
            rotation = rotation,
            startColor = color,
            velocity = new Vector2(Utils.RandFloat(-1f, 1f), Utils.RandFloat(-1f, 1f)) * randomizeFactor + velo,
            startLifetime = lifeTime,
            startSize = Instance.ParticleSystems[type].main.startSizeMultiplier * size * Utils.RandFloat(0.9f, 1.1f),
        };
        Instance.ParticleSystems[type].Emit(style, 1);
    }
    public static void NewParticle(Vector2 pos, Vector2 size, Vector2 velo = default, float randomizeFactor = 0, float lifeTime = 0.5f, int type = 0, Color color = default, float rotation = 0)
    {
        if (Instance == null)
            return;
        if (color == default)
            color = type == ID.Bubble ? DefaultColor : Color.white;
        ParticleSystem.EmitParams style = new()
        {
            position = pos,
            rotation = rotation,
            startColor = color,
            velocity = new Vector2(Utils.RandFloat(-1f, 1f), Utils.RandFloat(-1f, 1f)) * randomizeFactor + velo,
            startLifetime = lifeTime,
            startSize3D = Instance.ParticleSystems[type].main.startSizeMultiplier * new Vector3(size.x, size.y, 1)
        };
        Instance.ParticleSystems[type].Emit(style, 1);
    }
    public static void ForegroundLightning(Vector2 start, Vector2 end, Color c, float lifeMult = 0.6f, float whiteMult = 1.0f, float scaleX = 1.0f, float scaleY = 0.5f)
    {
        float dist = Vector2.Distance(start, end);
        float distRounded = (int)(dist * 2.25f);
        Vector2 prev = start;
        if (distRounded <= 0)
            return;
        for (int i = 0; i <= distRounded; i++)
        {
            float perc = i / distRounded;
            float scaleMult = Mathf.Lerp(scaleX, scaleY, perc);
            Vector2 pos = Vector2.Lerp(start, end, perc) + 0.8f * Utils.RandCircle(Mathf.Sqrt(Mathf.Abs(Mathf.Sin(perc * Mathf.PI))));
            Vector2 toPrev = prev - pos;
            float mag = toPrev.magnitude + 0.1f;
            float r = -toPrev.ToRotation() * Mathf.Rad2Deg;
            NewParticle(pos, new Vector2(mag * 1.0f, .5f * scaleMult), Vector2.zero, 0, lifeMult, ID.LineForeground, c, r);
            NewParticle(pos, new Vector2(mag * 1.0f, .3f * scaleMult), Vector2.zero, 0, lifeMult, ID.LineForeground, Color.white * whiteMult, r);
            NewParticle(Vector2.Lerp(pos, prev, Utils.RandFloat(1)), Utils.RandFloat(2, 3), Vector2.zero, 5, lifeMult, 3, Color.white * whiteMult);
            prev = pos;
        }
    }
    public static void LightningKillEffect(Vector2 pos)
    {
        Color lColor = new(0.4f, 0.6f, 1.0f);
        AudioManager.PlaySound(SoundID.Infect, pos, 0.75f, 0.75f, 0);
        ForegroundLightning(pos, pos + new Vector2(0, 10), lColor, 1.25f, scaleX: 2.5f, scaleY: 0.25f);
        int particleType = ID.Trail;
        int aura = 20;
        for (int i = 0; i < aura; ++i)
        {
            float r2 = Utils.RandFloat(1.0f, 1.2f);
            Vector2 circular = new Vector2(r2, 0).RotatedBy(i / (float)(0.5f * aura) * Mathf.PI);
            float size = 0.7f - r2 * 0.1f;
            NewParticle(pos, Mathf.Clamp(size, 0.1f, 1), circular * 12f, 2, Utils.RandFloat(0.5f, 1.0f), particleType, lColor);
        }
        for (int i = 0; i < 6; ++i)
        {
            Vector2 pos2 = pos + new Vector2(2.5f, 0).RotatedBy(i * Utils.TwoPI / 6f) + Utils.RandCircle(1);
            ForegroundLightning(pos, pos2, lColor, Utils.RandFloat(1.0f, 1.25f), scaleX: 2f, scaleY: 0.25f);
        }
        for (int i = 0; i < 8; ++i)
        {
            Vector2 pos2 = pos + new Vector2(1.0f, 0).RotatedBy(i * Utils.TwoPI / 8f) + Utils.RandCircle(0.5f);
            PopupText.NewPopupText(pos2, Utils.RandCircle(4), lColor.Lerp(new Color(0.8f, 0.9f, 1.0f), Utils.RandFloat()), "314159", true, Utils.RandFloat(0.5f, 0.6f), 70);
        }
    }
    private void Start()
    {
        ParticleSystems = FetchMySystems();
        Instance = this;
    }
    private void Update() => Instance = this;

    public static void SummonLightningPylon(Vector2 start, Vector2 end, Color c, int Type = 4, float scaleX = 1.4f, float scaleY = 1.0f)
    {
        float dist = Vector2.Distance(start, end);
        float distRounded = (int)dist;
        Vector2 prev = start;
        for (int i = 0; i < distRounded; i++)
        {
            float perc = i / distRounded;
            float scaleMult = Mathf.Lerp(scaleX, scaleY, perc);
            Vector2 pos = Vector2.Lerp(start, end, perc) + 0.8f * Utils.RandCircle(Mathf.Sqrt(Mathf.Abs(Mathf.Sin(perc * Mathf.PI))));
            Vector2 toPrev = prev - pos;
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 0.1f) * 1.0f, .5f * scaleMult), Vector2.zero, 0, 1.2f, Type, c, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 0.1f) * 1.0f, .3f * scaleMult), Vector2.zero, 0, 1.2f, Type, Color.white * 1f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 1.2f) * 1.0f, .4f * scaleMult), Vector2.zero, 0, 1.2f, Type, c * 0.8f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 1.2f) * 1.0f, .2f * scaleMult), Vector2.zero, 0, 1.2f, Type, Color.white * 0.8f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 2.3f) * 1.0f, .3f * scaleMult), Vector2.zero, 0, 1.2f, Type, c * 0.6f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 2.3f) * 1.0f, .1f * scaleMult), Vector2.zero, 0, 1.2f, Type, Color.white * 0.6f, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(Vector2.Lerp(pos, prev, Utils.RandFloat(1)), Utils.RandFloat(2, 3), Vector2.zero, 5, Utils.RandFloat(0.7f, 1.5f), 3, c * 1.5f);
            prev = pos;
        }
    }
    public static void SummonLightningPylon(Vector2 start, Vector2 end, Color c1, Color c2)
    {
        float dist = Vector2.Distance(start, end);
        float distRounded = (int)dist;
        Vector2 prev = start;
        for (int i = 0; i < distRounded; i++)
        {
            float perc = i / distRounded;
            float iPer = 1 - perc;
            float iPer2 = 1 - perc * perc;
            Color c = Color.Lerp(c1, c2, perc);
            float scaleMult = 1.25f + 0.75f * iPer;
            Vector2 pos = Vector2.Lerp(start, end, perc) + 0.8f * Utils.RandCircle(Mathf.Sqrt(Mathf.Abs(Mathf.Sin(perc * Mathf.PI))));
            Vector2 toPrev = prev - pos;
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 0.1f) * 1.0f, .6f * scaleMult), Vector2.zero, 0, 1.5f, 4, c * iPer2, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(pos, new Vector2((toPrev.magnitude + 0.1f) * 1.0f, .3f * scaleMult), Vector2.zero, 0, 1.5f, 4, Color.white * iPer2, -toPrev.ToRotation() * Mathf.Rad2Deg);
            ParticleManager.NewParticle(Vector2.Lerp(pos, prev, Utils.RandFloat(1)), Utils.RandFloat(2, 3), Vector2.zero, 5, Utils.RandFloat(0.7f, 1.5f), 3, c * 1.5f);
            prev = pos;
        }
    }
    public static void SummonLightningPylon2(Vector2 start, Vector2 end, Color c, float lifeMult = 0.6f, float whiteMult = 1.0f, float scaleX = 1.0f, float scaleY = 0.5f)
    {
        float dist = Vector2.Distance(start, end);
        float distRounded = (int)(dist * 2.25f);
        Vector2 prev = start;
        if (distRounded <= 0)
            return;
        for (int i = 0; i <= distRounded; i++)
        {
            float perc = i / distRounded;
            float scaleMult = Mathf.Lerp(scaleX, scaleY, perc);
            Vector2 pos = Vector2.Lerp(start, end, perc) + 0.8f * Utils.RandCircle(Mathf.Sqrt(Mathf.Abs(Mathf.Sin(perc * Mathf.PI))));
            Vector2 toPrev = prev - pos;
            float mag = toPrev.magnitude + 0.1f;
            float r = -toPrev.ToRotation() * Mathf.Rad2Deg;
            ParticleManager.NewParticle(pos, new Vector2(mag * 1.0f, .5f * scaleMult), Vector2.zero, 0, lifeMult, 4, c, r);
            ParticleManager.NewParticle(pos, new Vector2(mag * 1.0f, .3f * scaleMult), Vector2.zero, 0, lifeMult, 4, Color.white * whiteMult, r);
            ParticleManager.NewParticle(Vector2.Lerp(pos, prev, Utils.RandFloat(1)), Utils.RandFloat(2, 3), Vector2.zero, 5, lifeMult, 3, Color.white * whiteMult);
            prev = pos;
        }
    }
}
