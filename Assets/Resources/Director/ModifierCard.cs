using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModifierCard : MonoBehaviour
{
    public CardData cardData;
    public CardScrollArea Modifiers;
    public CardScrollArea Rewards;
    public void Start()
    {
        cardData = new();
    }
    public void Update()
    {
        Modifiers.UpdateSizing();
        Rewards.UpdateSizing();

        if(Input.GetKeyDown(KeyCode.R))
        {
            GenerateCardData();
            Modifiers.SetText(cardData.Modifiers.Modifiers.Count.ToString());
            Rewards.SetText(cardData.Rewards.Rewards.Count.ToString());
        }
    }
    public void GenerateCardData()
    {
        cardData.Generate();
    }
}
