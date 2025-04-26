using System.Collections.Generic;
using UnityEngine;

//Fast reload hates this script for some reason
public class ParticleManager : MonoBehaviour
{
    public static readonly Color DefaultColor = new Color(0.89f, 206 / 255f, 240 / 255f, 0.5f);
    public static readonly Color BathColor = new Color(189 / 255f, 227 / 255f, 246 / 255f, 0.6f);
    public static ParticleManager Instance;
    public List<ParticleSystem> thisSystem;
    public static void NewParticle(Vector2 pos, float size, Vector2 velo = default, float randomizeFactor = 0, float lifeTime = 0.5f, int type = 0, Color color = default)
    {
        if (ParticleManager.Instance == null)
            return;
        if (color == default)
            color = DefaultColor;
        ParticleSystem.EmitParams style = new ParticleSystem.EmitParams
        {
            position = pos,
            rotation = Utils.RandFloat(360),
            startColor = color,
            velocity = new Vector2(Utils.RandFloat(-1f, 1f), Utils.RandFloat(-1f, 1f)) * randomizeFactor + velo,
            startLifetime = lifeTime,
        };
        style.startSize = Instance.thisSystem[type].main.startSizeMultiplier * size * Utils.RandFloat(0.9f, 1.1f);
        Instance.thisSystem[type].Emit(style, 1);
    }
    void Start()
    {
        Instance = this;
    }
    void Update()
    {
        Instance = this;
    }
}
