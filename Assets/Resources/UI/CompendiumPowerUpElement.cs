using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CompendiumPowerUpElement : MonoBehaviour
{
    public static GameObject Prefab => Resources.Load<GameObject>("UI/CompendiumPowerUpElement");
    public PowerUpUIElement MyElem;
    public int PowerID = 0;
    public void Init(int i, Canvas canvas)
    {
        MyElem.SetPowerType(PowerID = i);
        MyElem.myCanvas = canvas;
    }
    public void Update()
    {
        if (PowerID < 0 && gameObject.activeSelf)
            Destroy(gameObject);
        MyElem.OnUpdate();
    }
}
