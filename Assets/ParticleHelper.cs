using UnityEngine;

//Fast reload hates this script for some reason
public class ParticleManager : MonoBehaviour
{
    [SerializeField] private static ParticleManager Instance;
    [SerializeField] private ParticleSystem thisSystem;
    public static void NewParticle(Vector2 pos, float size, Vector2 velo = default, bool randomizeVelo = false, float lifeTime = 0.5f)
    {
        ParticleSystem.EmitParams style = new ParticleSystem.EmitParams
        {
            position = pos,
            startSize = size * Utils.RandFloat(0.9f, 1.1f),
            startColor = Color.white,
            velocity = (randomizeVelo ? new Vector2(Utils.RandFloat(-0.6f, 0.6f), Utils.RandFloat(-0.6f, 0.6f)) * 1.5f : Vector2.zero) + velo,
            startLifetime = lifeTime
        };
        Instance.thisSystem.Emit(style, 1);
    }
    void Start()
    {
        thisSystem = GetComponent<ParticleSystem>();
        Instance = this;
    }
    void Update()
    {
        Instance = this;
    }
}
