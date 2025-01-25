using UnityEngine;

//Fast reload hates this script for some reason
public class ParticleManager : MonoBehaviour
{
    [SerializeField] private static ParticleManager Instance;
    [SerializeField] private ParticleSystem thisSystem;
    public static void NewParticle(Vector2 pos, float size, Vector2 velo = default, float randomizeFactor = 0, float lifeTime = 0.5f)
    {
        ParticleSystem.EmitParams style = new ParticleSystem.EmitParams
        {
            position = pos,
            startSize = size * Utils.RandFloat(0.9f, 1.1f),
            startColor = Color.white,
            velocity = new Vector2(Utils.RandFloat(-1f, 1f), Utils.RandFloat(-1f, 1f)) * randomizeFactor + velo,
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
