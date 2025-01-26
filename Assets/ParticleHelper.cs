using UnityEngine;

//Fast reload hates this script for some reason
public class ParticleManager : MonoBehaviour
{
    [SerializeField] private static ParticleManager Instance;
    public ParticleSystem thisSystem;
    public ParticleSystem SecondSystem;
    public static void NewParticle(Vector2 pos, float size, Vector2 velo = default, float randomizeFactor = 0, float lifeTime = 0.5f, int type = 0, Color color = default)
    {
        if (color == default)
            color = Color.white;
        ParticleSystem.EmitParams style = new ParticleSystem.EmitParams
        {
            position = pos,
            startSize = size * Utils.RandFloat(0.9f, 1.1f),
            startColor = color,
            velocity = new Vector2(Utils.RandFloat(-1f, 1f), Utils.RandFloat(-1f, 1f)) * randomizeFactor + velo,
            startLifetime = lifeTime
        };
        if(type == 0)
            Instance.thisSystem.Emit(style, 1);
        else
            Instance.SecondSystem.Emit(style, 1);
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
