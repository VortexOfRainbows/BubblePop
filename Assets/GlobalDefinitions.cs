using UnityEngine;

public class GlobalDefinitions : MonoBehaviour
{
    void Start() => Instance = this;
    void Update() => Instance = this;
    public static GlobalDefinitions Instance;
    public static GameObject Projectile => Instance.DefaultProjectile;
    public GameObject DefaultProjectile;
    public static Sprite SpikyProjectileSprite => Instance.SpikyProj;
    public Sprite SpikyProj;
    public static GameObject Ducky => Instance.Duck;
    public GameObject Duck;
    public static GameObject Soap => Instance.SoapNPC;
    public GameObject SoapNPC;
    public static GameObject TinySoap => Instance.SoapTinyNPC;
    public GameObject SoapTinyNPC;
    public static AudioClip[] audioClips => Instance.clips;
    public AudioClip[] clips;
}
