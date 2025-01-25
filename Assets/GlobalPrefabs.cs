using UnityEngine;

public class GlobalPrefabs : MonoBehaviour
{
    public static GlobalPrefabs Instance;
    public static GameObject Projectile => Instance.DefaultProjectile;
    public GameObject DefaultProjectile;
    void Start()
    {
        Instance = this;
    }
    void Update()
    {
        Instance = this;
    }
}
