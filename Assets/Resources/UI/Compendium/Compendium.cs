using TMPro;
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
            PageButtons[i].targetGraphic.color = isSelectedPage ? Color.yellow : Color.white;
        }
        if(ActiveElement.TypeID >= 0)
        {
            UpdateDisplay(ActiveElement.TypeID);
            UpdateDescription(true, ActiveElement.TypeID);
        }
        CurrentlySelectedPage.UpdateAllButtons(SortText, UnlockButton, CountButton, ReverseButton);
        AutoButton.targetGraphic.color = CurrentlySelectedPage.AutoNextTierList ? Color.yellow : Color.white;
    }
    private int m_PageNumber = 1;
    public static PowerUpPage CurrentlySelectedPage => Instance.Pages[Instance.PageNumber] as PowerUpPage;
    public Canvas MyCanvas;
    public RectTransform MyCanvasRectTransform => MyCanvas.GetComponent<RectTransform>();
    public CompendiumPage[] Pages;
    public Button[] PageButtons;
    public PowerUpPage PowerPage { get; private set; }
    public PowerUpPage EquipPage { get; private set; }
    private bool Active { get; set; }
    public Button OpenCompendiumButton;
    public Transform TopBar;
    public Transform SideBar;
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
        PowerPage = Pages[0] as PowerUpPage;
        EquipPage = Pages[1] as PowerUpPage;
        m_Instance = this;
        SetPage(0);
    }
    public void Update()
    {
        m_Instance = this;
        ScreenResolution = MyCanvasRectTransform.rect.width; //1920 in most cases
        HalfResolution = ScreenResolution / 2f;
        if(PowerPage != null)
            PowerPage.OnUpdate();
        if (EquipPage != null)
            EquipPage.OnUpdate();
    }
    public void UpdatePage(PowerUpPage page)
    {
        if (page != null && Active && page.isActiveAndEnabled)
        {
            if (!page.HasInit)
            {
                page.Init(CountButton, SortText);
                page.HasInit = true;
            }
            page.OnFixedUpdate();
        }
    }
    public void FixedUpdate()
    {
        Vector2 startingPosition = new Vector3(-ScreenResolution, 0);
        UpdatePage(PowerPage);
        UpdatePage(EquipPage);
        Utils.LerpSnap(transform, Active ? Vector3.zero : startingPosition, 0.1f, 0.1f);
    }

    #region Display and description on the right side of the compendium
    public CompendiumElement ActiveElement => CurrentlySelectedPage == Pages[1] ? DisplayCEE : DisplayCPUE;
    public CompendiumPowerUpElement DisplayCPUE;
    public CompendiumEquipmentElement DisplayCEE;
    public TextMeshProUGUI DisplayPortDescription;
    public RectTransform DescriptionContentRect;
    public GameObject[] Stars;
    public void UpdateDisplay(int SelectedType)
    {
        if (PageNumber == 0)
        {
            DisplayCPUE.Init(SelectedType, MyCanvas);
            DisplayCPUE.gameObject.SetActive(true);
            DisplayCEE.gameObject.SetActive(false);
        }
        else
        {
            DisplayCEE.Style = 3;
            DisplayCEE.Init(SelectedType, MyCanvas);
            DisplayCPUE.gameObject.SetActive(false);
            DisplayCEE.gameObject.SetActive(true);
        }
    }
    public void UpdateDescription(bool reset, int SelectedType)
    {
        if (reset && SelectedType >= 0)
        {
            string shortLineBreak = "<size=12>\n\n</size>";
            int rare;
            if (PageNumber == 0)
            {
                PowerUp p = PowerUp.Get(SelectedType);
                rare = p.GetRarity() - 1;
                string concat = $"<size=42>{p.UnlockedName}</size>" + shortLineBreak;
                if (p.HasBriefDescription)
                {
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Brief\n")}</size>";
                    concat += p.ShortDescription + shortLineBreak;
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Detailed\n")}</size>";
                }
                else
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Description\n")}</size>";
                concat += p.TrueFullDescription;
                if (!DisplayCPUE.IsLocked())
                {
                    concat += shortLineBreak;
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Times Obtained\n")}</size>";
                    concat += p.PickedUpCountAllRuns + shortLineBreak;
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Greatest Stack\n")}</size>";
                    concat += p.PickedUpBestAllRuns + shortLineBreak;
                }
                DisplayPortDescription.text = DisplayCPUE.IsLocked() ? DetailedDescription.BastardizeText(concat, '?') : concat;
            }
            else
            {
                Equipment e = DisplayCEE.MyElem.ActiveEquipment;
                rare = e.GetRarity() - 1;
                string concat = $"<size=42>{e.GetName()}</size>" + shortLineBreak;
                concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Description\n")}</size>";
                concat += e.GetDescription() + shortLineBreak;
                if (!DisplayCEE.IsLocked())
                {
                    concat += shortLineBreak;
                    concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Times Used\n")}</size>";
                    concat += e.TotalTimesUsed + shortLineBreak;
                }
                DisplayPortDescription.text = DisplayCEE.IsLocked() ? DetailedDescription.BastardizeText(concat, '?') : concat;
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
        CurrentlySelectedPage.ToggleTierList(TierListText);
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
