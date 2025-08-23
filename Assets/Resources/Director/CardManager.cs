using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject Visual;
    public static CardManager Instance { get; private set; }
    public void Start()
    {
        Instance = this;
        Instance.Visual.SetActive(false);
    }
    public ModifierCard[] Cards;
    public static ModifierCard[] Card => Instance.Cards;
    public static ModifierCard ChosenCard => Card[ChosenCardIndex];
    public static int ChosenCardIndex { get; private set; } = -1;
    public void Update()
    {
        Instance = this;
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateCards();
        }
    }
    public static void GenerateNewCards()
    {
        Instance.GenerateCards();
    }
    public static void DrawCards()
    {
        Instance.Visual.SetActive(true);
    }
    public static void ChooseCard(int i)
    {
        //select new wave cards
        ChosenCardIndex = i;
    }
    public static void ResolveChosenCard()
    {
        if (ChosenCardIndex < 0)
            return;
        //Run current wave card rewards (on completion rewards)
        ChosenCard.cardData.GrantCompletionRewards();
        //Resolve previous wave cards (on resolutions)
        ChosenCard.cardData.ResolveModifiers();

        //Update enemy pool and reset temporary modifiers
        if (WaveDirector.PermanentModifiers.WaveSpecialBonusEnemy != null) //If it was permanent, add the previous wave's special enemy to the enemy pool
        {
            WaveDirector.EnemyPool.Add(WaveDirector.PermanentModifiers.WaveSpecialBonusEnemy);
            WaveDirector.PermanentModifiers.WaveSpecialBonusEnemy = null;
        }
        WaveDirector.TemporaryModifiers.CloneValues(WaveDirector.PermanentModifiers); //Apply permanent modifiers to temporary modifiers

        ChosenCardIndex = -1;
    }
    public static void ApplyChosenCard()
    {
        if (ChosenCardIndex < 0)
            return;
        ChosenCard.cardData.ApplyPermanentModifiers(); //Permanent modifiers
        WaveDirector.TemporaryModifiers.CloneValues(WaveDirector.PermanentModifiers); //Apply permanent modifiers to temporary modifiers
        ChosenCard.cardData.ApplyTemporaryModifiers(); //Apply temporary modifiers

        //Run current wave card rewards (immediate rewards)
        ChosenCard.cardData.GrantImmediateRewards();
    }
    public void GenerateCards()
    {
        foreach(ModifierCard card in Cards)
        {
            card.GenerateCardData();
        }
    }
}
