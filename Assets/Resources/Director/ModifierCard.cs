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
        GenerateCardData();
    }
    public void Update()
    {
        Modifiers.UpdateSizing();
        Rewards.UpdateSizing();
    }
    public void GenerateCardData()
    {
        cardData.Generate();
    }
}
