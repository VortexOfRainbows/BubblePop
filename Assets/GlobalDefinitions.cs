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
}
