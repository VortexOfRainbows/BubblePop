using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class ModifierCard : MonoBehaviour
{
    public CardData cardData = null;
    public Image BG;
    public TextMeshProUGUI TitleText;
    public CardScrollArea Modifiers;
    public CardScrollArea Rewards;
    public float DifficultyMultiplier = 1f;
    public void Start() => cardData ??= new(this);
    public void UpdateText()
    {
        string shortLineBreak = "<size=12>\n</size>";
        string concat = string.Empty;
        if (cardData.ModifierClause.PermanentModifiers.Count > 0 || (!cardData.EnemyClause.AlreadyInPool && cardData.EnemyClause.Enemy.IsPermanent))
        {
            concat = "Permanently: \n".WithSizeAndColor(28, "#FFAAAA");
            if(!cardData.EnemyClause.AlreadyInPool && cardData.EnemyClause.Enemy.IsPermanent)
                concat += cardData.EnemyClause.Enemy.Description() + '\n';
            foreach (DirectorModifier mod in cardData.ModifierClause.PermanentModifiers)
                concat += mod.Description() + '\n';
            if (cardData.ModifierClause.TemporaryModifiers.Count > 0)
                concat += shortLineBreak;
        }
        if (cardData.ModifierClause.TemporaryModifiers.Count > 0 || (!cardData.EnemyClause.AlreadyInPool && !cardData.EnemyClause.Enemy.IsPermanent))
        {
            concat += "This Wave Only: \n".WithSizeAndColor(28, "#FFAAAA");
            if (!cardData.EnemyClause.AlreadyInPool && !cardData.EnemyClause.Enemy.IsPermanent)
                concat += cardData.EnemyClause.Enemy.Description() + '\n';
            foreach (DirectorModifier mod in cardData.ModifierClause.TemporaryModifiers)
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
        cardData ??= new(this);
        cardData.Generate();
        UpdateText();
    }
    public void UpdateSizing()
    {
        Modifiers.UpdateSizing();
        Rewards.UpdateSizing();
    }
    public void UpdateSelectVisuals(bool selected = false, bool hovering = false)
    {
        if (selected)
            hovering = selected;
        transform.LerpLocalScale(selected ? Vector2.one * 1.05f : Vector2.one, 0.2f);
        BG.color = Color.Lerp(BG.color, selected ? Color.yellow : (hovering ? Color.Lerp(Color.yellow, Color.white, 0.9f) : Color.white), 0.2f);
    }
}
