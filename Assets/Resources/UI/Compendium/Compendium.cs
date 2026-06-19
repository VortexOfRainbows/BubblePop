using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class Compendium : MonoBehaviour
{
    public static Compendium Instance { get => m_Instance != null ? m_Instance : m_Instance = FindFirstObjectByType<Compendium>(); set => m_Instance = value; }
    private static Compendium m_Instance;
    public static Vector2 ScreenResolution { get; set; }
    public static Vector2 HalfResolution { get; set; }
    public int PageNumber
    { 
        get
        {
            return m_PageNumber == -1 ? 1 : m_PageNumber;
        }
        set
        {
            SetPage(value);
        }
    }
    public void SetPage(int value)
    {
        if(m_PageNumber != -1)
            Main.CanvasManager.StaticPlaySound(); 
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
    private int m_PageNumber = -1;
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
        Main.CanvasManager.StaticPlaySound(); 
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
        ScreenResolution = new Vector2(MyCanvasRectTransform.rect.width, MyCanvasRectTransform.rect.height); //1920 in most cases
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
        Vector2 startingPosition = new Vector3(-ScreenResolution.x, 0);
        UpdatePage(EquipPage);
        UpdatePage(EnemyPage);
        UpdatePage(AchievementPage);
        UpdatePage(PowerPage); //Init this one last!
        Utils.LerpSnap(transform, Active ? Vector3.zero : startingPosition, 0.1f, 0.1f);
    }

    #region Display and description on the right side of the compendium
    public CompendiumElement ActiveElement => Elements[PageNumber];
    public CompendiumElement[] Elements;
    public CompendiumPowerUpElement DisplayCPUE { get; set; }
    public CompendiumEquipmentElement DisplayCEE { get; set; }
    public CompendiumEnemyElement DisplayCPEnemy { get; set; }
    public CompendiumAchievementElement DisplayCPAchievement { get; set; }
    public TextMeshProUGUI DisplayPortDescription;
    public RectTransform DescriptionContentRect;
    public GameObject[] Stars;
    public void UpdateDisplay(int SelectedType)
    {
        if(PageNumber == 1)
            DisplayCEE.Style = 3;
        Elements[PageNumber].Init(SelectedType, MyCanvas);
        if (Elements[PageNumber] is CompendiumPowerUpElement c)
        {
            c.MyElem.MyPower.ForceBlackMarket = CurrentlySelectedPage.BlackMarketMode;
            c.Init(SelectedType, MyCanvas);
            if (CurrentlySelectedPage.BlackMarketMode)
                c.MyElem.TurnedOn();
            c.MyElem.MyPower.ForceBlackMarket = false;
        }
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
                bool isBlackMarket = p.IsBlackMarket();
                rare = p.Rarity - 1;
                Dictionary<Type, string> AltDescriptions = p.Description.FullAlts;
                Dictionary<Equipment, string> UnlockedAlts = new();
                foreach (Type t in AltDescriptions.Keys)
                {
                    Equipment e = Main.GlobalEquipData.EquipFromType(t);
                    if (e.IsUnlocked)
                        UnlockedAlts[e] = AltDescriptions[t];
                }
                float size = 42;
                if (p is Electroluminescence)
                    size = 39.9f;
                string concat = $"<size={size}>{p.UnlockedName}</size>" + shortLineBreak;
                if (!p.BriefDescIsSameAsLong)
                {
                    bool hasAlt = UnlockedAlts.Count > 0 || (p.Description.HasBlackMarketVariants && p.BlackMarketVariantUnlockCondition.Unlocked);
                    concat += $"<size=26>{"Brief\n".WithRarityColor(rare, isBlackMarket)}</size>";
                    concat += p.ShortDescription + shortLineBreak;
                    concat += $"<size=26>{(hasAlt ? "Detailed (Default)\n" : "Detailed\n").WithRarityColor(rare, isBlackMarket)}</size>";
                }
                else
                    concat += $"<size=26>{"Description\n".WithRarityColor(rare, isBlackMarket)}</size>";
                concat += p.TrueFullDescription;
                if(p.Description.HasBlackMarketVariants && p.BlackMarketVariantUnlockCondition.Unlocked)
                {
                    concat += shortLineBreak + $"<size=26>{$"Detailed (Black Market)\n".WithRarityColor(rare, true)}</size>";
                    concat += p.BlackMarketFullDescription;
                }
                foreach(Equipment e in UnlockedAlts.Keys)
                {
                    concat += shortLineBreak + $"<size=26>{$"Detailed ({e.GetName(true)})\n".WithRarityColor(rare, isBlackMarket)}</size>";
                    concat += UnlockedAlts[e];
                }
                if (!DisplayCPUE.IsLocked())
                {
                    concat += shortLineBreak;
                    concat += $"<size=26>{"Times Obtained\n".WithRarityColor(rare, isBlackMarket)}</size>";
                    concat += p.PickedUpCountAllRuns + shortLineBreak;
                    concat += $"<size=26>{"Greatest Stack\n".WithRarityColor(rare, isBlackMarket)}</size>";
                    concat += p.PickedUpBestAllRuns + shortLineBreak;
                }
                DisplayPortDescription.text = DisplayCPUE.IsLocked() ? concat.Bastardize('?') : concat;
            }
            else if (PageNumber == 1)
            {
                Equipment e = DisplayCEE.MyElem.ActiveEquipment;
                UnlockCondition u = e.GetUnlockCondition();
                rare = e.GetRarity() - 1;
                string concat;
                if (!u.PreReqComplete && !e.IsUnlocked)
                {
                    rare = -1;
                    concat = $"<size=42>{e.GetName(true).WithColor(ColorHelper.LesserGrayHex)}</size>" + shortLineBreak;
                }
                else
                    concat = $"<size=42>{e.GetName()}</size>" + shortLineBreak;
                if (!DisplayCEE.IsLocked())
                {
                    //power pool contributions
                    concat += $"<size=26>{"Power Pool\n".WithRarityColor(rare, false)}</size>";
                    var powers = e.GetPowerPoolForDisplay();
                    string powerStr = string.Empty;
                    foreach (PowerUp p in powers)
                    {
                        string name = (p.PickedUpCountAllRuns > 0 ? p.Description.Name : "???").WithColor(ColorHelper.RarityColorHex[p.Rarity - 1]);
                        powerStr += " " + name + "\n";
                    }
                    concat += $"<size=26>{powerStr}</size>";
                    concat += shortLineBreak;
                }
                concat += $"<size=26>{"Description\n".WithRarityColor(rare, false)}</size>";
                if (!u.PreReqComplete && !e.IsUnlocked)
                    concat += e.GetDescription().WithColor(ColorHelper.GrayHex) + shortLineBreak;
                else
                    concat += e.GetDescription() + shortLineBreak;
                if (!DisplayCEE.IsLocked())
                {
                    //times used
                    concat += $"<size=26>{"Times Used\n".WithRarityColor(rare, false)}</size>";
                    concat += e.TotalTimesUsed + shortLineBreak;
                }
                else
                {
                    concat = concat.Bastardize('?');
                }
                concat += "Associated Achievement: \n".WithSizeAndColor(30, ColorHelper.LesserGrayHex);
                concat += u.GetName();
                DisplayPortDescription.text = concat;
            }
            else if(PageNumber == 2)
            {
                EnemyID.StaticEnemyData e = DisplayCPEnemy.MyElem.StaticData;
                rare = e.Rarity - 1;
                string concat = $"<size=42>{DisplayCPEnemy.MyElem.MyEnemyPrefab.Name().WithRarityColor(rare, false)}</size>" + shortLineBreak;

                concat += e.EnemyDescription.Full.WithSize(26) + shortLineBreak;

                concat += $"<size=26>{"Stats\n".WithRarityColor(rare, false)}</size>";
                concat += $" {"Base Health: ".WithColor(ColorHelper.RarityColorHex[5])}{e.BaseMaxLife}\n";
                string coinRange = e.BaseMinCoin != e.BaseMaxCoin ? $"{e.BaseMinCoin}-{e.BaseMaxCoin}" : $"{e.BaseMinCoin}";
                concat += $" {"Coin Range: ".WithColor(ColorHelper.YellowHex)}{coinRange}\n";
                string gemRange = e.BaseMinGem != e.BaseMaxGem ? $"{e.BaseMinGem}-{e.BaseMaxGem}" : $"{e.BaseMaxGem}";
                concat += $" {"Gem Bounty: ".WithColor(ColorHelper.RarityColors[1].ToHexString())}{gemRange}\n";
                concat += $" {"Summon Cost: ".WithColor(ColorHelper.LesserGrayHex)}{e.Cost:#.0}\n";
                concat += $" {"Wave: ".WithColor(ColorHelper.RarityColorHex[2])}{e.WaveNumber:#.#}\n";

                if (!DisplayCPEnemy.IsLocked())
                {
                    concat += shortLineBreak;
                    concat += $"<size=26>{"Kills\n".WithRarityColor(rare, false)}</size>";
                    concat += e.TimesKilled + shortLineBreak;

                    concat += $"<size=26>{"Skull Kills\n".WithRarityColor(rare, false)}</size>";
                    concat += e.TimesKilledSkull + shortLineBreak;
                }
                concat = DisplayCPEnemy.IsLocked() ? concat.Bastardize('?') : concat;
                DisplayPortDescription.text = concat;
            }
            else if(PageNumber == 3)
            {
                rare = DisplayCPAchievement.GetRare() - 1;
                string concat = $"<size=42>{DisplayCPAchievement.MyUnlock.GetName()}</size>" + shortLineBreak;
                concat += DisplayCPAchievement.MyUnlock.GetDescription() + shortLineBreak;
                if(DisplayCPAchievement.MyUnlock.PreReqComplete)
                {
                    if (DisplayCPAchievement.MyUnlock.AssociatedUnlocks.Count > 0)
                    {
                        concat += "Associated Unlocks: \n".WithSizeAndColor(30, ColorHelper.LesserGrayHex);
                        foreach (Equipment e in DisplayCPAchievement.MyUnlock.AssociatedUnlocks)
                        {
                            string name = e.IsUnlocked ? e.GetName() : e.GetName().Bastardize('?');
                            concat += " " + name + '\n';
                        }
                        concat += shortLineBreak;
                    }
                    if (DisplayCPAchievement.MyUnlock.AssociatedBlackMarketUnlocks.Count > 0)
                    {
                        concat += "Black Market Unlocks: \n".WithSizeAndColor(30, ColorHelper.LesserGrayHex);
                        foreach (PowerUp p in DisplayCPAchievement.MyUnlock.AssociatedBlackMarketUnlocks)
                        {
                            string name = DisplayCPAchievement.MyUnlock.Unlocked ? p.Description.Name.WithRarityColor(p.Rarity - 1, false) : "???".WithColor(ColorHelper.RarityColorHex[rare]);
                            concat += " " + name + '\n';
                        }
                        concat += shortLineBreak;
                    }
                    concat += "Achievement Category: \n".WithSizeAndColor(30, ColorHelper.LesserGrayHex);
                    if (DisplayCPAchievement.MyUnlock.AchievementZone == UnlockCondition.Meadows)
                        concat += " Meadows\n".WithColor(ColorHelper.RarityColorHex[1]);
                    else if (DisplayCPAchievement.MyUnlock.AchievementZone == UnlockCondition.City)
                        concat += " City\n".WithColor(ColorHelper.RarityColorHex[2]);
                    else if (DisplayCPAchievement.MyUnlock.AchievementZone == UnlockCondition.Lab)
                        concat += " Lab\n".WithColor(ColorHelper.RarityColorHex[3]);
                    if (DisplayCPAchievement.MyUnlock.AchievementCategory == UnlockCondition.Completionist)
                        concat += " Completionist\n".WithColor(ColorHelper.RarityColorHex[4]);
                    else if (DisplayCPAchievement.MyUnlock.AchievementCategory == UnlockCondition.Challenge)
                        concat += " Challenge\n".WithColor(ColorHelper.RarityColorHex[5]);
                    else if (DisplayCPAchievement.MyUnlock.AchievementCategory == UnlockCondition.Secret)
                        concat += " Secret\n".WithColor(ColorHelper.RarityColorHex[0]);
                }
                else
                    rare = -1;
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
