using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Compendium : MonoBehaviour
{
    public static bool AutoNextTierList { get; set; } = false;
    private const int ArbitrarySort = 0;
    private const int RaritySort = 1;
    private const int FavSort = 2;
    //public static Compendium Instance;
    public static bool MouseInCompendiumArea = false;
    /// <summary>
    /// Unique behaviors:
    /// -1: Deleted on load (intended for helping with editor behavior)
    /// -2: None
    /// -3: Empty hand, with special delete logic for elements temporarily on the tier list
    /// -4: Empty hand, with special logic for after you have placed all available elements
    /// </summary>
    public static int SelectedType { get; set; }
    public void UpdateSelectedType(int i)
    {
        SelectedType = i;
        if (PrimaryCPUE.PowerID != SelectedType && SelectedType >= 0)
        {
            PrimaryCPUE.Init(SelectedType, MyCanvas);
            UpdateDescription(true);
        }
        else
            UpdateDescription(false);
        if (HoverCPUE.PowerID != SelectedType && SelectedType >= 0)
        {
            TierList.RemovePower(HoverCPUE.PowerID);
            bool hasSelectedPower = PowerUp.Get(SelectedType).PickedUpCountAllRuns > 0;
            HoverCPUE.gameObject.SetActive(hasSelectedPower && TierListActive); //change this to color scaling or other continuous function for disappearance and reappearance animation
            HoverCPUE.Init(SelectedType, MyCanvas);
        }
    }
    public CompendiumPowerUpElement PrimaryCPUE;
    public CompendiumPowerUpElement HoverCPUE;
    public TextMeshProUGUI PrimaryCPUEDescription;
    public RectTransform SelectionArea;
    public RectTransform DescriptionScrollArea;
    public Canvas MyCanvas;
    public GridLayoutGroup ContentLayout;
    public RectTransform ContentLayoutRect;
    private bool Active = false;
    private bool HasInit = false;
    private Vector3 StartingPosition;
    public int SortMode = 0;
    public static bool ShowOnlyUnlocked, ShowCounts;
    public Button SortButton, UnlockButton, CountButton, TierListButton, OpenButton, AutoButton;
    public TextMeshProUGUI SortText;
    public TextMeshProUGUI TierListText;
    public GameObject[] Stars;
    public static bool HoldingAPower { get => SelectedType >= 0; } // set => SelectedType = value ? SelectedType : -3; }
    public static bool HoldingALockedPower { get => HoldingAPower && PowerUp.Get(SelectedType).PickedUpCountAllRuns <= 0; }
    public void ToggleSort()
    {
        SortMode = (SortMode + 1) % 3;
        UpdateSort();
    }
    public void ToggleTierList()
    {
        if (TierListActive) //Save
        {
            PlayerData.SaveTierList(TierList);
        }
        else //Load
        {
            PlayerData.LoadTierList(TierList);
        }
        UpdateSelectedType(-4);
        TierListActive = !TierListActive;
        TierListText.text = TierListActive ? "Save Tier List" : "Make Tier List";
        SetVisibility();
        Sort();
    }
    public void UpdateSort()
    {
        if (SortMode == ArbitrarySort) //Arbitrary / ID
        {
            SortText.text = "Sort: ID";
        }
        if (SortMode == RaritySort) //By rarity
        {
            SortText.text = "Sort: Rarity";
        }
        if (SortMode == FavSort) //By count
        {
            SortText.text = "Sort: Favorite";
        }
        Sort();
    }
    public void ToggleUnlock()
    {
        ShowOnlyUnlocked = !ShowOnlyUnlocked;
        UnlockButton.targetGraphic.color = ShowOnlyUnlocked ? Color.yellow : Color.white;
        SetVisibility();
    }
    public void ToggleCount()
    {
        ShowCounts = !ShowCounts;
        CountButton.targetGraphic.color = ShowCounts ? Color.yellow : Color.white;
    }
    public void ToggleAuto()
    {
        AutoNextTierList = !AutoNextTierList;
        AutoButton.targetGraphic.color = AutoNextTierList ? Color.yellow : Color.white;
    }
    public void CancelTierListChanges()
    {
        PlayerData.LoadTierList(TierList);
        UpdateSelectedType(-3);
        SetVisibility();
        Sort();
    }
    public void ClearTierList()
    {
        TierList.ReadingFromSave = true;
        for(int i = TierList.Powers.Count - 1; i >= 0; --i)
        {
            TierList.RemovePower(TierList.Powers[i].PowerID, false);
        }
        TierList.ReadingFromSave = false;
        UpdateSelectedType(-3);
        SetVisibility();
        Sort();
    }
    public void Start()
    {
        //Instance = this;
        SelectedType = 0;
        ShowOnlyUnlocked = ShowCounts = TierListActive = MouseInCompendiumArea = AutoNextTierList = false;
        StartingPosition = transform.position;
    }
    public void ToggleActive()
    {
        ToggleActive(!Active);
    }
    public void ToggleActive(bool on)
    {
        Active = on;
    }
    public void Init()
    {
        for (int i = 0; i < PowerUp.Reverses.Count; ++i)
        {
            CompendiumPowerUpElement CPUE = Instantiate(CompendiumPowerUpElement.Prefab, ContentLayout.transform, false).GetComponent<CompendiumPowerUpElement>();
            CPUE.Init(i, MyCanvas);
            TierList.OnTierList[i] = false;
        }
        UpdateContentSize();
        ShowCounts = true;
        ToggleCount(); //OFF by default
        SortMode = ArbitrarySort;
        ToggleSort(); //default sort is rare

        PrimaryCPUE.Init(SelectedType, MyCanvas);
        UpdateDescription(true);
    }
    public void UpdateContentSize()
    {
        int c = ContentLayout.transform.childCount;
        if (c <= 0)
            return;
        Vector3 lastElement = ContentLayout.transform.GetChild(c - 1).localPosition;
        RectTransform r = ContentLayout.GetComponent<RectTransform>();
        float defaultDist = TierList.TotalDistanceCovered - 200;
        float paddingBonus = ContentLayout.padding.bottom + ContentLayout.cellSize.y;
        float dist = -lastElement.y + paddingBonus;
        r.sizeDelta = new Vector2(r.sizeDelta.x, Mathf.Max(defaultDist, dist));
        ContentLayoutRect.sizeDelta = Vector2.Lerp(ContentLayoutRect.sizeDelta, new Vector2(0, paddingBonus / 2f + (TierListActive ? defaultDist : 0)), 0.1f);

        ContentLayout.transform.localPosition = new Vector3(0, Mathf.Min(0, 600 - defaultDist), 0);
    }
    public static List<CompendiumPowerUpElement> GetCPUEChildren(Transform parent, out int count)
    {
        count = parent.childCount;
        List<CompendiumPowerUpElement> childs = new();
        for (int i = 0; i < count; ++i)
            childs.Add(parent.GetChild(i).GetComponent<CompendiumPowerUpElement>());
        return childs;
    }
    public List<CompendiumPowerUpElement> GetCPUEChildren(out int count)
    {
        return GetCPUEChildren(ContentLayout.transform, out count);
    }
    public CompendiumPowerUpElement NextSlot()
    {
        int i = 0;
        CompendiumPowerUpElement cpue = ContentLayout.transform.GetChild(i).GetComponent<CompendiumPowerUpElement>();
        while (cpue != null && cpue.MyElem.AppearLocked)
        {
            cpue = ContentLayout.transform.GetChild(++i).GetComponent<CompendiumPowerUpElement>();
        }
        return cpue;
    }
    public void SetVisibility()
    {
        List<CompendiumPowerUpElement> childs = GetCPUEChildren(out int c);
        foreach(CompendiumPowerUpElement cpue in childs)
        {
            bool locked = cpue.MyElem.AppearLocked;
            cpue.gameObject.SetActive(!locked || !ShowOnlyUnlocked);
            if (!TierListActive)
                cpue.GrayOut = false;
            else
                cpue.GrayOut = TierList.PowerHasBeenPlaced(cpue.PowerID);
            cpue.transform.localPosition = new Vector3(0, 0, 0); //Failsafe for repositioning elements as disabling them sometimes has weird behavior with layout group
        }
    }
    public int CompareID(CompendiumPowerUpElement e1, CompendiumPowerUpElement e2)
    {
        int id1 = e1.PowerID;
        int id2 = e2.PowerID;
        if (e1.GrayOut)
            id1 += 10000;
        if (e2.GrayOut)
            id2 += 10000;
        int num = id1 - id2;
        return num;
    }
    public int CompareRare(CompendiumPowerUpElement e1, CompendiumPowerUpElement e2)
    {
        int rare1 = PowerUp.Get(e1.PowerID).GetRarity();
        int rare2 = PowerUp.Get(e2.PowerID).GetRarity();
        if (e1.GrayOut)
            rare1 += 10;
        if (e2.GrayOut)
            rare2 += 10;
        int num = rare1 - rare2;
        return num == 0 ? CompareID(e1, e2) : num;
    }
    public int CompareFav(CompendiumPowerUpElement e1, CompendiumPowerUpElement e2)
    {
        int count1 = PowerUp.Get(e1.PowerID).PickedUpCountAllRuns;
        int count2 = PowerUp.Get(e2.PowerID).PickedUpCountAllRuns;
        if (e1.GrayOut)
            count1 = (int.MinValue >> 1) + count1;
        if (e2.GrayOut)
            count2 = (int.MinValue >> 1) + count2;
        int num = count2 - count1;
        return num == 0 ? CompareRare(e1, e2) : num;
    }
    public void Sort()
    {
        List<CompendiumPowerUpElement> childs = GetCPUEChildren(out int c);
        if(SortMode == ArbitrarySort)
        {
            childs.Sort(CompareID);
        }
        if (SortMode == RaritySort)
        {
            childs.Sort(CompareRare);
        }
        if (SortMode == FavSort)
        {
            childs.Sort(CompareFav);
        }
        for (int i = 0; i < c; ++i)
        {
            CompendiumPowerUpElement CPUE = childs[i];
            CPUE.transform.SetSiblingIndex(i);
        }
    }
    public void Update()
    {
        MouseInCompendiumArea = Utils.IsMouseHoveringOverThis(true, SelectionArea, 0, MyCanvas);
        if (Control.RightMouseClick)
        {
            UpdateSelectedType(-3);
        }
        if (TierListActive && MouseInCompendiumArea)
        {
            if(HoverCPUE.PowerID == SelectedType)
            {
                TierList.PlacePower(HoverCPUE.PowerID, !Control.LeftMouseClick);
            }
            else if(SelectedType == -3)
            {
                TierList.RemovePower(HoverCPUE.PowerID);
            }
        }
    }
    public void FixedUpdate()
    {
        if (Active)
        {
            if (!HasInit)
            {
                Init();
                HasInit = true;
            }
            if (transform.position.x > -0.1f)
                transform.position = Vector3.zero;
            else transform.position = transform.position.Lerp(new Vector3(0, 0, 0), 0.1f);
            UpdateSelectedType(SelectedType);
        }
        else
        {
            if (transform.position.x < -1919.9f)
                transform.position = StartingPosition;
            else transform.position = transform.position.Lerp(StartingPosition, 0.1f);
        }
        TierListUpdate();
    }
    public void UpdateDescription(bool reset)
    {
        if(reset && SelectedType >= 0)
        {
            PowerUp p = PowerUp.Get(SelectedType);
            string shortLineBreak = "<size=12>\n\n</size>";
            int rare = p.GetRarity() - 1;
            string concat = $"<size=42>{p.UnlockedName}</size>" + shortLineBreak;
            concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Brief\n")}</size>";
            concat += p.ShortDescription + shortLineBreak;
            concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Detailed\n")}</size>";
            concat += p.TrueFullDescription;
            if(!PrimaryCPUE.MyElem.AppearLocked)
            {
                concat += shortLineBreak;
                concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Times Obtained\n")}</size>";
                concat += p.PickedUpCountAllRuns + shortLineBreak;
                concat += $"<size=26>{DetailedDescription.TextBoundedByRarityColor(rare, "Greatest Stack\n")}</size>";
                concat += p.PickedUpBestAllRuns + shortLineBreak;
            }
            PrimaryCPUEDescription.text = PrimaryCPUE.MyElem.AppearLocked ? DetailedDescription.BastardizeText(concat, '?') : concat;
            UpdateStars(rare);
        }
        Vector2 target = PrimaryCPUEDescription.GetRenderedValues();
        float minW = 361;
        float minH = 480;
        DescriptionScrollArea.sizeDelta = new Vector2(minW, Mathf.Max(minH, target.y + 40));
    }
    public void UpdateStars(int rare)
    {
        for (int j = 0; j < Stars.Length; ++j)
            Stars[j].SetActive(rare == j);
    }
    public ScrollRect ContentScrollRect;
    public GameObject TopBar;
    public GameObject SideBar;
    public GameObject ViewPort, ContentTierList;
    public bool TierListActive = false;
    public TierList TierList;
    private Vector2 LerpSnap(Transform target, Vector2 position, float amt = 0.1f, float tolerance = 0.5f)
    {
        if (target.localPosition.Distance(position) < tolerance)
            target.localPosition = position;
        else
            target.localPosition = target.localPosition.Lerp(position, amt);
        return target.localPosition;
    }
    private bool hasSnappedTierList = false;
    public void TierListUpdate()
    {
        UpdateContentSize();
        OpenButton.interactable = !TierListActive;
        Vector2 newTopBarPositon = !TierListActive ? new Vector2(0, 540f) : new Vector2(0, 740f);
        Vector2 newSideBarPosition = !TierListActive ? new Vector2(960f, 340f) : new Vector2(960f, 540f);
        Vector2 newOpenButtonPosition = !TierListActive ? new Vector2(-934.5f, 515f) : new Vector2(-934.5f, 715f);
        Vector2 targetViewport = !TierListActive ? new Vector2(-760f, 390f) : new Vector2(-760f, 590f);
        Vector2 targetTierList = !TierListActive ? new Vector2(0, TierList.TotalDistanceCovered - 800) : new Vector2(0, -800f);

        LerpSnap(TopBar.transform, newTopBarPositon);
        LerpSnap(SideBar.transform, newSideBarPosition);
        LerpSnap(OpenButton.gameObject.transform, newOpenButtonPosition);
        LerpSnap(ViewPort.transform, targetViewport);
        LerpSnap(ContentTierList.transform, targetTierList);
        RectTransform r = ViewPort.GetComponent<RectTransform>();
        r.sizeDelta = Vector2.Lerp(r.sizeDelta, new Vector2(0, TierListActive ? 200 : 0), 0.1f);

        if (hasSnappedTierList != TierListActive)
        {
            if(ContentLayoutRect.transform.localPosition.Distance(Vector2.zero) > 0.5f)
                ContentLayoutRect.transform.localPosition = ContentLayoutRect.transform.localPosition.Lerp(Vector2.zero, 0.1f);
            else
            {
                ContentLayoutRect.transform.localPosition = Vector2.zero;
                hasSnappedTierList = TierListActive;
            }
        }
        ContentScrollRect.vertical = hasSnappedTierList == TierListActive;

        bool hasSelectedPower = SelectedType >= 0 && PowerUp.Get(SelectedType).PickedUpCountAllRuns > 0;
        HoverCPUE.gameObject.SetActive(hasSelectedPower && TierListActive); //change this to color scaling or other continuous function for disappearance and reappearance animation
        if(TierListActive)
        {
            Vector3 targetPosition = Utils.MouseWorld + new Vector2(2, 1);
            Rect boundingRect = r.rect;
            HoverCPUE.gameObject.transform.position = HoverCPUE.gameObject.transform.position.Lerp(targetPosition, 0.1f);
            Vector3 pos = Utils.ClampToRect(HoverCPUE.gameObject.transform.localPosition, boundingRect, 66);
            HoverCPUE.gameObject.transform.localPosition = pos;

            TierList.OnUpdate();

            //Debug.Log(SelectedType);
        }
    }
}
