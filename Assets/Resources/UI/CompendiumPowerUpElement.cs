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
        MyElem.Count.text = PowerUp.Get(PowerID).PickedUpCountAllRuns.ToString();
    }
    public void Update()
    {
        if (PowerID == -1 && gameObject.activeSelf)
            Destroy(gameObject);
        MyElem.OnUpdate();
    }
}
