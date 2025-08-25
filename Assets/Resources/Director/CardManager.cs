using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static bool DespawningCards { get; set; } = false;
    public static float DespawnTimer = 0;
    public Canvas MyCanvas;
    public GameObject Visual;
    public Image ConfirmButton;
    public TextMeshProUGUI ConfirmButtonText;
    public static CardManager Instance { get; private set; }
    public void Start()
    {
        ChosenCardIndex = -1;
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
        if (DespawningCards)
            DespawnCards();
        if (!Visual.activeSelf || DespawningCards)
            return;
        if (Input.GetKeyDown(KeyCode.R) && Main.DebugCheats)
            GenerateCards();
        bool isClickingOnACard = false;
        for (int i = 0; i < Cards.Length; ++i)
        {
            bool Hovered = Utils.IsMouseHoveringOverThis(true, Cards[i].BG.rectTransform, 0, MyCanvas);
            if(Hovered && Control.LeftMouseClick)
            {
                ChosenCardIndex = ChosenCardIndex != i ? i : -1;
                isClickingOnACard = true;
                break;
            }
        }
        if(!isClickingOnACard)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                ChosenCardIndex = ChosenCardIndex != 0 ? 0 : -1;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                ChosenCardIndex = ChosenCardIndex != 1 ? 1 : -1;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                ChosenCardIndex = ChosenCardIndex != 2 ? 2 : -1;
        }
        foreach (ModifierCard card in Cards)
            card.UpdateSizing();
        bool cardSelected = ChosenCardIndex != -1;
        bool confirmButtonHovered = Utils.IsMouseHoveringOverThis(true, ConfirmButton.rectTransform, 0, MyCanvas) && cardSelected && Control.LeftMouseClick;
        if(confirmButtonHovered || (Control.Interact && cardSelected))
            ConfirmCard();
        VisualUpdate();
    }
    public void VisualUpdate()
    {
        for(int i = 0; i < Cards.Length; ++i)
        {
            bool Hovered = Utils.IsMouseHoveringOverThis(true, Cards[i].BG.rectTransform, 0, MyCanvas);
            Cards[i].UpdateSelectVisuals(i == ChosenCardIndex, Hovered);
            Cards[i].SpawnAnimation();
        }
        bool cardSelected = ChosenCardIndex != -1;
        bool confirmButtonHovered = Utils.IsMouseHoveringOverThis(true, ConfirmButton.rectTransform, 0, MyCanvas) && cardSelected;
        Color c = confirmButtonHovered ? Color.yellow : Color.white;
        ConfirmButton.color = Color.Lerp(ConfirmButton.color, c, Utils.DeltaTimeLerpFactor(0.2f));
        ConfirmButtonText.color = Color.white;
        ConfirmButton.gameObject.SetActive(cardSelected);
        if(cardSelected)
            ConfirmButton.transform.localPosition = new Vector3(ChosenCard.transform.localPosition.x, ConfirmButton.transform.localPosition.y, ConfirmButton.transform.localPosition.z);
    }
    public void DespawnCards()
    {
        for (int i = 0; i < Cards.Length; ++i)
            Cards[i].DespawnAnimation();
        ConfirmButton.gameObject.SetActive(false);
        DespawnTimer += Time.unscaledDeltaTime;
        if (DespawnTimer > 1.0f)
        {
            DespawnTimer = 0;
            DespawningCards = false;
            Instance.Visual.SetActive(false);
            if(PlayerData.PauseDuringCardSelect)
                Main.UnpauseGame();
        }
    }
    public static void ConfirmCard()
    {
        DespawnTimer = 0;
        DespawningCards = true;
        WaveDirector.StartWave();
    }
    public static void GenerateNewCards()
    {
        Instance.GenerateCards();
    }
    public static void DrawCards()
    {
        if (PlayerData.PauseDuringCardSelect)
            Main.PauseGame();
        Instance.Visual.SetActive(true);
        GenerateNewCards();
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
            card.GenerateCardData();
    }
}
