using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.U2D;
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
        if (m_PageNumber != -1)
            Main.CanvasManager.StaticPlaySound();
        m_PageNumber = value;
        for (int i = 0; i < Pages.Length; ++i)
        {
            bool isSelectedPage = i == value;
            Pages[i].gameObject.SetActive(isSelectedPage);
            PageButtons[i].targetGraphic.color = (isSelectedPage ? Color.yellow : Color.white).WithAlpha(0.8f);
        }
        if (ActiveElement.TypeID >= 0)
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
    public bool Active { get; private set; }
    public bool PrevActive { get; private set; } = false;
    public Button OpenCompendiumButton;
    public Transform TopBar;
    public Transform SideBar;
    public RectTransform SortBar;
    public static void StaticToggleActive()
    {
        Instance.ToggleActive();
    }
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
        MyCanvas.worldCamera = CameraManager.UICamera;
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
        MoveCompendiumUpdate(1.0f);
    }
    public void Update()
    {
        MyCanvas.worldCamera = CameraManager.UICamera;
        m_Instance = this;
        ScreenResolution = new Vector2(MyCanvasRectTransform.rect.width, MyCanvasRectTransform.rect.height); //1920, 1080 in most cases
        HalfResolution = ScreenResolution / 2f;
        foreach (BasicTierListCompendiumPage page in Pages.Cast<BasicTierListCompendiumPage>())
        {
            if (page != null)
                page.OnUpdate();
        }
        MoveCompendiumUpdate(Utils.DeltaTimeLerpFactor(.1f));
        if (PrevActive != Active && !PrevActive) //On reopen behavior (update stuff that is needed here)
        {
            UpdateDescription(true, ActiveElement.TypeID);
            CurrentlySelectedPage.Sort();
        }
        PrevActive = Active;
    }
    public void LateUpdate()
    {
        JustPlacedCounter--;
    }
    public void UpdatePage(BasicTierListCompendiumPage page, float lerpFactor)
    {
        if (page == null)
            return;
        if ((Active || (page == CurrentlySelectedPage && page.HasInit)) && page.isActiveAndEnabled)
        {
            if (!page.HasInit)
            {
                page.Init(CountButton, SortText);
                page.HasInit = true;
            }
            page.SecondaryUpdate(lerpFactor);
        }
    }
    public void MoveCompendiumUpdate(float lerpFactor)
    {
        Vector2 startingPosition = new Vector3(-ScreenResolution.x, 0);
        UpdatePage(EquipPage, lerpFactor);
        UpdatePage(EnemyPage, lerpFactor);
        UpdatePage(AchievementPage, lerpFactor);
        UpdatePage(PowerPage, lerpFactor); //Init this one last!
        Utils.LerpSnap(transform, Active ? Vector3.zero : startingPosition, lerpFactor, 0.1f);
    }
    #region Display and description on the right side of the compendium
    public CompendiumElement ActiveElement => Elements[PageNumber];
    public CompendiumElement[] Elements;
    public CompendiumPowerUpElement DisplayCPUE { get; set; }
    public CompendiumEquipmentElement DisplayCEE { get; set; }
    public CompendiumEnemyElement DisplayCPEnemy { get; set; }
    public CompendiumAchievementElement DisplayCPAchievement { get; set; }
    public GameObject[] Stars;
    public static int JustPlacedCounter = 0;
    public static bool JustPlacedAnElemOntoTierList
    {
        get => JustPlacedCounter > 0;
        set
        {
            JustPlacedCounter = value ? 3 : 0;
        }
    }
    public void UpdateDisplay(int SelectedType)
    {
        if (PageNumber == 1)
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
    public string GetDescriptionForElement()
    {
        return string.Empty;
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

    #region Description
    public TextMeshProUGUI DisplayPortDescription;
    public RectTransform DescriptionContentRect;
    public TextMeshProUGUI LoreSection;
    public RectTransform LoreSectionRect => LoreSection.transform.parent.GetComponent<RectTransform>();
    public static readonly string shortLineBreak = "<size=12>\n\n</size>";
    public void UpdateDescription(bool reset, int SelectedType)
    {
        if (reset && SelectedType >= 0)
        {
            int rare = 0;
            string finalText = string.Empty;
            object loreObject = null;
            if (PageNumber == 0)
            {
                PowerUp p = PowerUp.Get(SelectedType);
                loreObject = p;
                string concat = GenerateTierListDescription(p, ref rare);
                finalText = DisplayCPUE.IsLocked() ? concat.Bastardize('?') : concat;
                if (p.IsBlackMarket())
                    rare = 5;
            }
            else if (PageNumber == 1)
            {
                loreObject = DisplayCEE.MyElem.ActiveEquipment;
                finalText = GenerateTierListDescription(loreObject as Equipment, ref rare);
            }
            else if (PageNumber == 2)
            {
                loreObject = DisplayCPEnemy.MyElem.StaticData;
                finalText = GenerateTierListDescription(loreObject as EnemyID.StaticEnemyData, ref rare);
            }
            else if (PageNumber == 3)
                finalText = GenerateTierListDescription(DisplayCPAchievement, ref rare);
            UpdateStars(rare);

            if(PageNumber == 3) //Achievement page has no NOTES section for now, so it should disable it
            {
                //Disable notes section
            }
            else if(loreObject != null)//Enable notes section for other descriptions
            {
                finalText += shortLineBreak + $"<size=26>{"Notes".WithRarityColor(rare, false)}</size>";
                string loreText = GetLoreSegment(loreObject);
                LoreSection.text = loreText;
            }
            DisplayPortDescription.text = finalText;
        }
        Vector2 target = DisplayPortDescription.GetRenderedValues();
        Vector2 loreTarget = LoreSection.GetRenderedValues();
        float minW = 361;
        float minH = 100; // 460;
        float paddingBonusMain = 15; //5 + 10
        float paddingBonusLore = 10;
        DescriptionContentRect.sizeDelta = new Vector2(minW, Mathf.Max(minH, target.y + loreTarget.y + paddingBonusLore + paddingBonusMain));
        LoreSectionRect.sizeDelta = new Vector2(LoreSectionRect.sizeDelta.x, loreTarget.y + paddingBonusLore); //10 since the padding is 5, 5 (5 + 5 = 10)
    }
    public string GenerateTierListDescription(PowerUp p, ref int rare)
    {
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
        //if (!p.BriefDescIsSameAsLong)
        //{
        //    concat += $"<size=26>{"Brief\n".WithRarityColor(rare, isBlackMarket)}</size>";
        //    concat += p.ShortDescription + shortLineBreak;
        //    concat += $"<size=26>{(hasAlt ? "Detailed (Default)\n" : "Detailed\n").WithRarityColor(rare, isBlackMarket)}</size>";
        //}
        bool hasAlt = UnlockedAlts.Count > 0 || (p.Description.HasBlackMarketVariants && p.BlackMarketVariantUnlockCondition.IsComplete);
        concat += $"<size=26>{(hasAlt ? "Description (Default)\n" : "Description\n").WithRarityColor(rare, isBlackMarket)}</size>";
        concat += p.TrueFullDescription;
        if (p.Description.HasBlackMarketVariants && p.BlackMarketVariantUnlockCondition.IsComplete)
        {
            concat += shortLineBreak + $"<size=26>{$"Description (Black Market)\n".WithRarityColor(rare, true)}</size>";
            concat += p.BlackMarketFullDescription;
        }
        foreach (Equipment e in UnlockedAlts.Keys)
        {
            concat += shortLineBreak + $"<size=26>{$"Description ({e.GetName(true)})\n".WithRarityColor(rare, isBlackMarket)}</size>";
            concat += UnlockedAlts[e];
        }
        if (!DisplayCPUE.IsLocked())
        {
            concat += shortLineBreak;
            concat += $"<size=26>{"Stats\n".WithRarityColor(rare, isBlackMarket)}</size>";
            concat += $" {"Times Obtained: ".WithColor(ColorHelper.YellowHex)}{p.PickedUpCountAllRuns}\n";
            concat += $" {"Greatest Stack: ".WithColor(ColorHelper.YellowHex)}{p.PickedUpBestAllRuns}";
        }
        return concat;
    }
    public string GenerateTierListDescription(Equipment e, ref int rare)
    {
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
            for (int i = 0; i < powers.Count; ++i)
            {
                PowerUp p = powers[i];
                string name = (p.PickedUpCountAllRuns > 0 ? p.Description.Name : "???").WithColor(ColorHelper.RarityColorHex[p.Rarity - 1]);
                powerStr += " " + name + (i == powers.Count - 1 ? "" : "\n");
            }
            concat += $"<size=26>{powerStr}</size>";
            concat += shortLineBreak;
        }
        concat += $"<size=26>{"Description\n".WithRarityColor(rare, false)}</size>";
        if (!u.PreReqComplete && !e.IsUnlocked)
            concat += e.GetDescription().WithSizeAndColor(26, ColorHelper.GrayHex) + shortLineBreak;
        else
            concat += e.GetDescription().WithSize(26) + shortLineBreak;
        if (!DisplayCEE.IsLocked())
        {
            //times used
            concat += $"<size=26>{"Stats\n".WithRarityColor(rare, false)}</size>";
            if (e.HighestDifficultyUnlocked > 0)
                concat += $" {"Ascension: ".WithColor(ColorHelper.AscColorHex)}{e.HighestDifficultyUnlocked}\n";
            concat += $" {"Times Used: ".WithColor(ColorHelper.LesserGrayHex)}{e.TotalTimesUsed}\n";
            concat += $" {"Victories: ".WithColor(ColorHelper.YellowHex)}{e.VictoryCount}";
        }
        else
        {
            concat = concat.Bastardize('?');
        }
        concat += shortLineBreak + "Associated Achievement: \n".WithSizeAndColor(26, ColorHelper.LesserGrayHex);
        concat += u.GetName();
        return concat;
    }
    public string GenerateTierListDescription(EnemyID.StaticEnemyData e, ref int rare)
    {
        bool locked = DisplayCPEnemy.IsLocked();
        rare = e.Rarity - 1;
        string concat = $"<size=42>{DisplayCPEnemy.MyElem.MyEnemyPrefab.Name().WithRarityColor(rare, false)}</size>" + shortLineBreak;

        concat += $"<size=26>{$"Description\n".WithRarityColor(rare, false)}</size>";
        concat += e.EnemyDescription.Full.WithSize(26) + shortLineBreak;

        concat += $"<size=26>{"Stats\n".WithRarityColor(rare, false)}</size>";
        concat += $" {"Base Health: ".WithColor(ColorHelper.RarityColorHex[5])}{e.BaseMaxLife}\n";
        string coinRange = e.BaseMinCoin != e.BaseMaxCoin ? $"{e.BaseMinCoin}-{e.BaseMaxCoin}" : $"{e.BaseMinCoin}";
        concat += $" {"Coin Range: ".WithColor(ColorHelper.YellowHex)}{coinRange}\n";
        string gemRange = e.BaseMinGem != e.BaseMaxGem ? $"{e.BaseMinGem}-{e.BaseMaxGem}" : $"{e.BaseMaxGem}";
        concat += $" {"Gem Bounty: ".WithColor(ColorHelper.RarityColors[1].ToHexString())}{gemRange}\n";
        concat += $" {"Summon Cost: ".WithColor(ColorHelper.LesserGrayHex)}{e.Cost:#.0}\n";
        concat += $" {"Wave: ".WithColor(ColorHelper.RarityColorHex[2])}{e.WaveNumber:#.#}";
        if (!locked)
        {
            concat += $"\n {"Kills: ".WithColor(ColorHelper.RarityColorHex[3])}{e.TimesKilled}\n";
            concat += $" {"Skull Kills: ".WithColor(ColorHelper.RarityColorHex[3])}{e.TimesKilledSkull}";
        }
        concat = locked ? concat.Bastardize('?') : concat;
        return concat;
    }
    public string GenerateTierListDescription(CompendiumAchievementElement DisplayCPAchievement, ref int rare)
    {
        rare = DisplayCPAchievement.GetRare() - 1;
        bool isSecret = DisplayCPAchievement.MyUnlock.AchievementCategory == UnlockCondition.Secret;
        string concat = $"<size=42>{DisplayCPAchievement.MyUnlock.GetName()}</size>" + shortLineBreak;
        if (isSecret && !DisplayCPAchievement.MyUnlock.IsComplete)
            concat += Localization.Get("Common.UnlockSecret") + shortLineBreak;
        else
            concat += DisplayCPAchievement.MyUnlock.GetDescription() + shortLineBreak;
        if (DisplayCPAchievement.MyUnlock.PreReqComplete)
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
                    string name = DisplayCPAchievement.MyUnlock.IsComplete ? p.Description.Name.WithRarityColor(p.Rarity - 1, false) : "???".WithColor(ColorHelper.RarityColorHex[rare]);
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
            else if (isSecret)
                concat += " Secret\n".WithColor(ColorHelper.RarityColorHex[0]);
        }
        else
            rare = -1;
        return concat;
    }
    public string GetLoreSegment(object loreObject)
    {
        string localizationKey;
        if (loreObject is PowerUp power)
            localizationKey = $"Power.{power.GetType().FullName}.Lore";
        else if (loreObject is Equipment equip)
            localizationKey = $"Equip.{equip.GetType().FullName}.Lore";
        else if (loreObject is EnemyID.StaticEnemyData enemy)
            localizationKey = $"Enemy.{enemy.OriginalPrefab.GetComponent<Enemy>().GetType().FullName}.Lore";
        else
            throw new Exception("THIS OBJECT HAS NO ASSOCIATED LORE");

        string loreSegment = string.Empty;

        int i = 0;
        while (Localization.TryGet(localizationKey + i.ToString(), out string segment))
        {
            bool textSegmentUnlocked = true;
            if (segment.StartsWith("["))
            { 
                int number = int.Parse(segment.Substring(1, 1));
                textSegmentUnlocked = LoreSegmentCharacterOwnerUnlocked(number);
                if (textSegmentUnlocked)
                    segment = segment[3..];
            }
            ++i;
            if (textSegmentUnlocked)
                loreSegment += segment;
            else
            {
                loreSegment += Localization.Get("Common.LoreUnfilled").WithSizeAndColor(21, ColorHelper.GrayHex);
                break;
            }
        }
        if(i == 0)
            return Localization.Get(localizationKey);
        return loreSegment;
    }
    public bool LoreSegmentCharacterOwnerUnlocked(int charID)
    {
        if(charID == 0) return true; //Bubblemancer always unlocked
        if(charID == 1) return UnlockCondition.Get<ThoughtBubbleUnlock>().IsComplete;
        if (charID == 2) return UnlockCondition.Get<GachaponUnlock>().IsComplete;
        if (charID == 3) return UnlockCondition.Get<FizzyUnlock>().IsComplete;
        if (charID == 4) return UnlockCondition.Get<KingOilUnlock>().IsComplete;
        return true;
    }
    #endregion
}
