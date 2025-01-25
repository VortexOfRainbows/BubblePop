using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private static ParticleManager Instance;
    [SerializeField] private ParticleSystem system;
    public static void NewParticle(Vector2 pos)
    {
        ParticleSystem.EmitParams style = new ParticleSystem.EmitParams();
        style.position = pos;
        style.startSize = Utils.RandFloat(0.3f, 0.5f);
        style.startColor = Color.white;
        style.velocity = new Vector2(Utils.RandFloat(-0.5f, 0.5f), Utils.RandFloat(-0.5f, 0.5f)) * 1.5f + new Vector2(0, -0.1f);
        Instance.system.Emit(style, 1);
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
