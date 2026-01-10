using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Compendium : MonoBehaviour
{
    public static Compendium Instance { get => m_Instance != null ? m_Instance : m_Instance = FindFirstObjectByType<Compendium>(); set => m_Instance = value; }
    private static Compendium m_Instance;
    public static float ScreenResolution { get; set; }
    public static float HalfResolution { get; set; }
    public int PageNumber
    { 
        get
        {
            return m_PageNumber;
        }
        set
        {
            SetPage(value);
        }
    }
    public void SetPage(int value)
    {
        m_PageNumber = value;
        for (int i = 0; i < Pages.Length; ++i)
        {
            bool isSelectedPage = i == value;
            Pages[i].gameObject.SetActive(isSelectedPage);
            PageButtons[i].targetGraphic.color = (isSelectedPage ? Color.yellow : Color.white).WithAlpha(0.8f);
        }
        if(ActiveElement.TypeID >= 0)
        {
            UpdateDisplay(ActiveElement.TypeID);
            UpdateDescription(true, ActiveElement.TypeID);
        }
        CurrentlySelectedPage.UpdateAllButtons(SortText, TierListText, UnlockButton, CountButton, ReverseButton);
        AutoButton.targetGraphic.color = CurrentlySelectedPage.AutoNextTierList ? Color.yellow : Color.white;
    }
    private int m_PageNumber = 1;
    public static BasicTierListCompendiumPage CurrentlySelectedPage => Instance.Pages[Instance.PageNumber] as BasicTierListCompendiumPage;
    public Canvas MyCanvas;
    public RectTransform MyCanvasRectTransform => MyCanvas.GetComponent<RectTransform>();
    public CompendiumPage[] Pages;
    public Button[] PageButtons;
    public BasicTierListCompendiumPage PowerPage { get; private set; }
    public BasicTierListCompendiumPage EquipPage { get; private set; }
    public BasicTierListCompendiumPage EnemyPage { get; private set; }
    public BasicTierListCompendiumPage AchievementPage { get; private set; }
    private bool Active { get; set; }
    public Button OpenCompendiumButton;
    public Transform TopBar;
    public Transform SideBar;
    public RectTransform SortBar;
    public void ToggleActive()
    {
        ToggleActive(!Active);
    }
    public void ToggleActive(bool on)
    {
        Active = on;
    }
    public void Start()
    {
        PowerPage = Pages[0] as BasicTierListCompendiumPage;
        EquipPage = Pages[1] as BasicTierListCompendiumPage;
        EnemyPage = Pages[2] as BasicTierListCompendiumPage;
        AchievementPage = Pages[3] as BasicTierListCompendiumPage;
        DisplayCPUE = Elements[0] as CompendiumPowerUpElement;
        DisplayCEE = Elements[1] as CompendiumEquipmentElement;
        DisplayCPEnemy = Elements[2] as CompendiumEnemyElement;
        DisplayCPAchievement = Elements[3] as CompendiumAchievementElement;
        AchievementPage.ToggleDisplayMode(TierListText);
        m_Instance = this;
        SetPage(0);
    }
    public void Update()
    {
        m_Instance = this;
        ScreenResolution = MyCanvasRectTransform.rect.width; //1920 in most cases
        HalfResolution = ScreenResolution / 2f;
        foreach (BasicTierListCompendiumPage page in Pages.Cast<BasicTierListCompendiumPage>())
        {
            if (page != null)
                page.OnUpdate();
        }
    }
    public void UpdatePage(BasicTierListCompendiumPage page)
    {
        if (page != null && Active && page.isActiveAndEnabled)
        {
            page.OnFixedUpdate();
        }
        else
        {
            if (!page.HasInit)
            {
                page.Init(CountButton, SortText);
                page.HasInit = true;
            }
        }
    }
    public void FixedUpdate()
    {
        Vector2 startingPosition = new Vector3(-ScreenResolution, 0);
        UpdatePage(EquipPage);
        UpdatePage(EnemyPage);
        UpdatePage(AchievementPage);
        UpdatePage(PowerPage); //Init this one last!
        Utils.LerpSnap(transform, Active ? Vector3.zero : startingPosition, 0.1f, 0.1f);
    }

    #region Display and description on the right side of the compendium
    public CompendiumElement ActiveElement => Elements[PageNumber];
    public CompendiumElement[] Elements;
    public CompendiumPowerUpElement DisplayCPUE;
    public CompendiumEquipmentElement DisplayCEE;
    public CompendiumEnemyElement DisplayCPEnemy;
    public CompendiumAchievementElement DisplayCPAchievement; 
    public TextMeshProUGUI DisplayPortDescription;
    public RectTransform DescriptionContentRect;
    public GameObject[] Stars;
    public void UpdateDisplay(int SelectedType)
    {
        if(PageNumber == 1)
            DisplayCEE.Style = 3;
        Elements[PageNumber].Init(SelectedType, MyCanvas);
        for (int i = 0; i < 4; ++i)
            Elements[i].gameObject.SetActive(i == PageNumber);
    }
    public void UpdateDescription(bool reset, int SelectedType)
    {
        if (reset && SelectedType >= 0)
        {
            string shortLineBreak = "<size=12>\n\n</size>";
            int rare = 0;
            if (PageNumber == 0)
            {
                PowerUp p = PowerUp.Get(SelectedType);
                rare = p.GetRarity() - 1;
                var AltDescriptions = p.DetailedDescription.LoadAllAlternativeDescriptions();
                bool hasAlt = AltDescriptions.Count > 0;
                float size = 42;
                if (p is Electroluminescence)
                    size = 39.9f;
                string concat = $"<size={size}>{p.UnlockedName}</size>" + shortLineBreak;
                if (p.HasBriefDescription)
                {
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Brief\n", p.IsBlackMarket())}</size>";
                    concat += p.ShortDescription + shortLineBreak;
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, hasAlt ? "Detailed (Default)\n" : "Detailed\n", p.IsBlackMarket())}</size>";
                }
                else
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Description\n", p.IsBlackMarket())}</size>";
                concat += p.TrueFullDescription;
                foreach(Type t in AltDescriptions.Keys)
                {
                    concat += shortLineBreak + $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, $"Detailed ({Main.GlobalEquipData.EquipFromType(t).GetName(true)})\n", p.IsBlackMarket())}</size>";
                    concat += AltDescriptions[t];
                }
                if (!DisplayCPUE.IsLocked())
                {
                    concat += shortLineBreak;
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Times Obtained\n", p.IsBlackMarket())}</size>";
                    concat += p.PickedUpCountAllRuns + shortLineBreak;
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Greatest Stack\n", p.IsBlackMarket())}</size>";
                    concat += p.PickedUpBestAllRuns + shortLineBreak;
                }
                DisplayPortDescription.text = DisplayCPUE.IsLocked() ? DetailedDescription.BastardizeText(concat, '?') : concat;
            }
            else if (PageNumber == 1)
            {
                Equipment e = DisplayCEE.MyElem.ActiveEquipment;
                rare = e.GetRarity() - 1;
                string concat = $"<size=42>{e.GetName()}</size>" + shortLineBreak;
                concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Description\n", false)}</size>";
                concat += e.GetDescription() + shortLineBreak;
                if (!DisplayCEE.IsLocked())
                {
                    concat += shortLineBreak;
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Times Used\n", false)}</size>";
                    concat += e.TotalTimesUsed + shortLineBreak;
                }
                else
                {
                    concat = DetailedDescription.BastardizeText(concat, '?');
                }
                concat += shortLineBreak;
                concat += "Associated Achievement: \n".WithSizeAndColor(30, DetailedDescription.LesserGray);
                concat += e.GetUnlockCondition().GetName();
                DisplayPortDescription.text = concat;
            }
            else if(PageNumber == 2)
            {
                EnemyID.StaticEnemyData e = DisplayCPEnemy.MyElem.StaticData;
                rare = e.Rarity - 1;
                string concat = $"<size=42>{DetailedDescription.TextBoundedByRarityColor(rare, DisplayCPEnemy.MyElem.MyEnemyPrefab.Name(), false)}</size>" + shortLineBreak;
                concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Stats\n", false)}</size>";
                concat += $"{DetailedDescription.TextBoundedByColor(DetailedDescription.Rares[5], "Base Health: ")}{e.BaseMaxLife}\n";
                string coinRange = e.BaseMinCoin != e.BaseMaxCoin ? $"{e.BaseMinCoin}-{e.BaseMaxCoin}" : $"{e.BaseMinCoin}";
                concat += $"{DetailedDescription.TextBoundedByColor(DetailedDescription.Yellow, "Coin Range: ")}{coinRange}\n";
                string gemRange = e.BaseMinGem != e.BaseMaxGem ? $"{e.BaseMinGem}-{e.BaseMaxGem}" : $"{e.BaseMaxGem}";
                concat += $"{DetailedDescription.TextBoundedByColor(ColorHelper.RarityColors[1].ToHexString(), "Gem Bounty: ")}{gemRange}\n";
                concat += $"{DetailedDescription.TextBoundedByColor(DetailedDescription.LesserGray, "Summon Cost: ")}{e.Cost:#.0}\n";

                if (!DisplayCPEnemy.IsLocked())
                {
                    concat += shortLineBreak;
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Kills\n", false)}</size>";
                    concat += e.TimesKilled + shortLineBreak;

                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Skull Kills\n", false)}</size>";
                    concat += e.TimesKilledSkull + shortLineBreak;
                }
                concat = DisplayCPEnemy.IsLocked() ? DetailedDescription.BastardizeText(concat, '?') : concat;
                DisplayPortDescription.text = concat;
            }
            else if(PageNumber == 3)
            {
                rare = DisplayCPAchievement.GetRare() - 1;
                string concat = $"<size=42>{DisplayCPAchievement.MyUnlock.GetName()}</size>" + shortLineBreak;
                concat += DisplayCPAchievement.MyUnlock.GetDescription() + shortLineBreak;
                if(DisplayCPAchievement.MyUnlock.AssociatedUnlocks.Count > 0)
                {
                    concat += "Associated Unlocks: \n".WithSizeAndColor(30, DetailedDescription.LesserGray);
                    foreach (Equipment e in DisplayCPAchievement.MyUnlock.AssociatedUnlocks)
                    {
                        string name = e.IsUnlocked ? e.GetName() : DetailedDescription.BastardizeText(e.GetName(), '?');
                        concat += "  " + name + '\n';
                    }
                    concat += shortLineBreak;
                }
                if (DisplayCPAchievement.MyUnlock.AssociatedBlackMarketUnlocks.Count > 0)
                {
                    concat += "Black Market Unlocks: \n".WithSizeAndColor(30, DetailedDescription.LesserGray);
                    foreach (PowerUp p in DisplayCPAchievement.MyUnlock.AssociatedBlackMarketUnlocks)
                    {
                        string name = DisplayCPAchievement.MyUnlock.Unlocked ? p.DetailedDescription.GetName() : DetailedDescription.BastardizeText(p.UnlockedName, '?');
                        concat += "  " + name + '\n';
                    }
                    concat += shortLineBreak;
                }
                concat += "Achievement Category: \n".WithSizeAndColor(30, DetailedDescription.LesserGray);
                if(DisplayCPAchievement.MyUnlock.AchievementZone == UnlockCondition.Meadows)
                    concat += "  Meadows\n".WithColor(DetailedDescription.Rares[1]);
                else if (DisplayCPAchievement.MyUnlock.AchievementZone == UnlockCondition.City)
                    concat += "  City\n".WithColor(DetailedDescription.Rares[2]);
                else if(DisplayCPAchievement.MyUnlock.AchievementZone == UnlockCondition.Lab)
                    concat += "  Lab\n".WithColor(DetailedDescription.Rares[3]);
                if (DisplayCPAchievement.MyUnlock.AchievementCategory == UnlockCondition.Completionist)
                    concat += "  Completionist\n".WithColor(DetailedDescription.Rares[4]);
                else if (DisplayCPAchievement.MyUnlock.AchievementCategory == UnlockCondition.Challenge)
                    concat += "  Challenge\n".WithColor(DetailedDescription.Rares[5]);
                else if (DisplayCPAchievement.MyUnlock.AchievementCategory == UnlockCondition.Secret)
                    concat += "  Secret\n".WithColor(DetailedDescription.Rares[0]);
                DisplayPortDescription.text = concat;
            }
            UpdateStars(rare);
        }
        Vector2 target = DisplayPortDescription.GetRenderedValues();
        float minW = 361;
        float minH = 460;
        DescriptionContentRect.sizeDelta = new Vector2(minW, Mathf.Max(minH, target.y + 40));
    }
    public void UpdateStars(int rare)
    {
        for (int j = 0; j < Stars.Length; ++j)
            Stars[j].SetActive(rare == j);
    }
    #endregion

    #region Tier List Universal Buttons
    public Button AutoButton, UnlockButton, CountButton, ReverseButton;
    public TextMeshProUGUI SortText, TierListText;
    public void ToggleAuto()
    {
        CurrentlySelectedPage.AutoNextTierList = !CurrentlySelectedPage.AutoNextTierList;
        AutoButton.targetGraphic.color = CurrentlySelectedPage.AutoNextTierList ? Color.yellow : Color.white;
    }
    public void CancelTierListChanges()
    {
        ClearTierList();
        PlayerData.LoadTierList(CurrentlySelectedPage.TierList);
        CurrentlySelectedPage.UpdateSelectedType(-3);
        CurrentlySelectedPage.SetVisibility();
        CurrentlySelectedPage.Sort();
    }
    public void ClearTierList()
    {
        TierList.ReadingFromSave = true;
        for (int i = CurrentlySelectedPage.TierList.Elems.Count - 1; i >= 0; --i)
        {
            CurrentlySelectedPage.TierList.RemovePower(CurrentlySelectedPage.TierList.Elems[i].TypeID, false);
        }
        TierList.ReadingFromSave = false;
        CurrentlySelectedPage.UpdateSelectedType(-3);
        CurrentlySelectedPage.SetVisibility();
        CurrentlySelectedPage.Sort();
    }
    public void OnSortButtonPressed()
    {
        CurrentlySelectedPage.ToggleSort(SortText);
    }
    public void OnLockButtonPressed()
    { 
        CurrentlySelectedPage.ToggleUnlock(UnlockButton);
    }
    public void OnTierListButtonPressed()
    {
        if (CurrentlySelectedPage.TierList != null)
            CurrentlySelectedPage.ToggleTierList(TierListText);
        else
            CurrentlySelectedPage.ToggleDisplayMode(TierListText);
    }
    public void OnCountButtonPressed()
    {
        CurrentlySelectedPage.ToggleCount(CountButton);
    }
    public void OnReverseButtonPressed()
    {
        CurrentlySelectedPage.ToggleReverse(ReverseButton);
    }
    #endregion
}
