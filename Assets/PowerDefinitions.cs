using UnityEngine;

public class PowerDefinitions : MonoBehaviour
{
    void Start() => Instance = this;
    void Update() => Instance = this;
    public static PowerDefinitions Instance;
}
