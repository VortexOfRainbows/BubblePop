using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierCardPopup : MonoBehaviour
{
    public ModifierCard[] Cards;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateCards();
        }
    }
    public void GenerateCards()
    {
        foreach(ModifierCard card in Cards)
        {
            card.GenerateCardData();
        }
    }
}
