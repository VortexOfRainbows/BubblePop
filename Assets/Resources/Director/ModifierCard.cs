using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModifierCard : MonoBehaviour
{
    public CardData cardData;
    public TextMeshProUGUI TitleText;
    public CardScrollArea Modifiers;
    public CardScrollArea Rewards;
    public float DifficultyMultiplier = 1f;
    public void Start()
    {
        cardData = new(this);
    }
    public void Update()
    {
        Modifiers.UpdateSizing();
        Rewards.UpdateSizing();
    }
    public void UpdateText()
    {
        string shortLineBreak = "<size=12>\n</size>";
        string concat = string.Empty;
        if (cardData.ModifierClause.PermanentModifiers.Count > 0 || cardData.EnemyClause.Enemy.IsPermanent)
        {
            concat = "Permanently: \n".WithSizeAndColor(28, "#FFAAAA");
            if(cardData.EnemyClause.Enemy.IsPermanent)
                concat += cardData.EnemyClause.Enemy.Description() + '\n';
            foreach (DirectorModifier mod in cardData.ModifierClause.PermanentModifiers)
                concat += mod.Description() + '\n';
            if (cardData.ModifierClause.Modifiers.Count > 0)
                concat += shortLineBreak;
        }
        if (cardData.ModifierClause.Modifiers.Count > 0 || !cardData.EnemyClause.Enemy.IsPermanent)
        {
            concat += "This Wave Only: \n".WithSizeAndColor(28, "#FFAAAA");
            if (!cardData.EnemyClause.Enemy.IsPermanent)
                concat += cardData.EnemyClause.Enemy.Description() + '\n';
            foreach (DirectorModifier mod in cardData.ModifierClause.Modifiers)
                concat += mod.Description() + '\n';
        }
        Modifiers.SetText(concat);

        concat = string.Empty;
        if (cardData.RewardClause.PreRewards.Count > 0)
        {
            concat = "Immediately: \n".WithSizeAndColor(28, DetailedDescription.LesserGray);
            foreach (Reward r in cardData.RewardClause.PreRewards)
                concat += r.Description() + '\n';
            if (cardData.RewardClause.PostRewards.Count > 0)
                concat += shortLineBreak;
        }
        if(cardData.RewardClause.PostRewards.Count > 0)
        {
            concat += "On Completion: \n".WithSizeAndColor(28, DetailedDescription.LesserGray);
            foreach (Reward r in cardData.RewardClause.PostRewards)
                concat += r.Description() + '\n';
        }
        Rewards.SetText(concat);

        TitleText.text = cardData.CardName();
    }
    public void GenerateCardData()
    {
        cardData.Generate();
        UpdateText();
    }
}
