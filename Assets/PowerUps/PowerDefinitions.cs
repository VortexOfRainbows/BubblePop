using UnityEngine;

public class PowerDefinitions : MonoBehaviour
{
    void Start() => Instance = this;
    void Update() => Instance = this;
    public static PowerDefinitions Instance;
    public static PowerUpObject PowerUpObj => Instance.PowerObj;
    public PowerUpObject PowerObj;
    public static PowerUpUIElement PowerUpUIElem => Instance.UIElement;
    public PowerUpUIElement UIElement;
}
