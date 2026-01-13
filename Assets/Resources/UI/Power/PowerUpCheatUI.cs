using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCheatUI : MonoBehaviour
{
    public PowerUpButton ChoiceTemplate;
    public void Start()
    {
        for(int i = 1; i < PowerUp.Reverses.Count; ++i)
        {
            PowerUpButton p = Instantiate(ChoiceTemplate, transform);
            p.SetType(i);
        }
    }
}
