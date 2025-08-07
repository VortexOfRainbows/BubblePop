using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CompendiumPage : MonoBehaviour
{

}
public abstract class TierListCompendiumPage : CompendiumPage
{
    private const int ArbitrarySort = 0;
    private const int RaritySort = 1;
    private const int FavSort = 2;
    public Compendium Owner => Compendium.Instance;
    public Canvas MyCanvas => Owner.MyCanvas;
    public bool AutoNextTierList { get; set; }
    public bool MouseInCompendiumArea { get; set; }
    /// <summary>
    /// Unique behaviors:
    /// -1: Deleted on load (intended for helping with editor behavior)
    /// -2: None
    /// -3: Empty hand, with special delete logic for elements temporarily on the tier list
    /// -4: Empty hand, with special logic for after you have placed all available elements
    /// </summary>
    public int SelectedType { get; set; }
    public bool ShowOnlyUnlocked { get; set; }
    public bool ShowCounts { get; set; }
    private int SortMode { get; set; } //could be made into a field
    private bool TierListActive { get; set; } //could be made into a field
    private bool HasSnappedTierList { get; set; } //could be made into a field
    private bool HasInit { get; set; } //could be made into a field
    public CompendiumPowerUpElement HoverCPUE;
    public RectTransform SelectionArea;
    public GridLayoutGroup PowerUpLayoutGroup;
    public RectTransform ContentRectangle;
    public Button UnlockButton, CountButton;
    public TextMeshProUGUI SortText, TierListText;
    public ScrollRect ContentScrollRect;
    public Transform SortBar, TierListParent;
    public RectTransform ViewPort;
    public TierList TierList;
    public bool HoldingAPower => SelectedType >= 0; // set => SelectedType = value ? SelectedType : -3; }
    public bool HoldingALockedPower => HoldingAPower && PowerUp.Get(SelectedType).PickedUpCountAllRuns <= 0;
    public void UpdateSelectedType(int i)
    {
        SelectedType = i;
        if (Owner.DisplayCPUE.PowerID != SelectedType && SelectedType >= 0)
        {
            Owner.UpdateDisplay(SelectedType);
            Owner.UpdateDescription(true, SelectedType);
        }
        else
            Owner.UpdateDescription(false, SelectedType);
        if (HoverCPUE.PowerID != SelectedType && SelectedType >= 0)
        {
            TierList.RemovePower(HoverCPUE.PowerID);
            bool hasSelectedPower = PowerUp.Get(SelectedType).PickedUpCountAllRuns > 0;
            HoverCPUE.gameObject.SetActive(hasSelectedPower && TierListActive); //change this to color scaling or other continuous function for disappearance and reappearance animation
            HoverCPUE.Init(SelectedType, MyCanvas);
        }
    }
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
    public void Start()
    {
        //Instance = this;
        SelectedType = 0;
        ShowOnlyUnlocked = ShowCounts = TierListActive = MouseInCompendiumArea = AutoNextTierList = false;
    }
    public void Init()
    {
        for (int i = 0; i < PowerUp.Reverses.Count; ++i)
        {
            CompendiumPowerUpElement CPUE = Instantiate(CompendiumPowerUpElement.Prefab, PowerUpLayoutGroup.transform, false).GetComponent<CompendiumPowerUpElement>();
            CPUE.Init(i, MyCanvas);
            TierList.OnTierList[i] = false;
        }
        UpdateContentSize();
        ShowCounts = true;
        ToggleCount(); //OFF by default
        SortMode = ArbitrarySort;
        ToggleSort(); //default sort is rare

        Owner.UpdateDisplay(SelectedType);
        Owner.UpdateDescription(true, SelectedType);
    }
    public void UpdateContentSize()
    {
        int c = PowerUpLayoutGroup.transform.childCount;
        if (c <= 0)
            return;
        Vector3 lastElement = PowerUpLayoutGroup.transform.GetChild(c - 1).localPosition;
        RectTransform r = PowerUpLayoutGroup.GetComponent<RectTransform>();
        float defaultDist = TierList.TotalDistanceCovered - 200;
        float paddingBonus = PowerUpLayoutGroup.padding.bottom + PowerUpLayoutGroup.cellSize.y;
        float dist = -lastElement.y + paddingBonus;
        r.sizeDelta = new Vector2(r.sizeDelta.x, Mathf.Max(defaultDist, dist));
        ContentRectangle.sizeDelta = Vector2.Lerp(ContentRectangle.sizeDelta, new Vector2(0, paddingBonus / 2f + (TierListActive ? defaultDist : 0)), 0.1f);

        PowerUpLayoutGroup.transform.localPosition = new Vector3(0, Mathf.Min(0, 600 - defaultDist), 0);
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
        return GetCPUEChildren(PowerUpLayoutGroup.transform, out count);
    }
    public CompendiumPowerUpElement NextSlot()
    {
        int i = 0;
        CompendiumPowerUpElement cpue = PowerUpLayoutGroup.transform.GetChild(i).GetComponent<CompendiumPowerUpElement>();
        while (cpue != null && cpue.MyElem.AppearLocked)
        {
            cpue = PowerUpLayoutGroup.transform.GetChild(++i).GetComponent<CompendiumPowerUpElement>();
        }
        return cpue;
    }
    public void SetVisibility()
    {
        List<CompendiumPowerUpElement> childs = GetCPUEChildren(out int c);
        foreach (CompendiumPowerUpElement cpue in childs)
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
        PowerUpLayoutGroup.transform.DetachChildren();
        if (SortMode == ArbitrarySort)
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
            CPUE.transform.SetParent(PowerUpLayoutGroup.transform);
            CPUE.transform.localPosition = new Vector3(CPUE.transform.localPosition.x, CPUE.transform.localPosition.y, 0);
        }
    }
    public void OnUpdate()
    {
        MouseInCompendiumArea = Utils.IsMouseHoveringOverThis(true, SelectionArea, 0, MyCanvas);
        if (Control.RightMouseClick)
        {
            UpdateSelectedType(-3);
        }
        if (TierListActive && MouseInCompendiumArea)
        {
            if (HoverCPUE.PowerID == SelectedType)
            {
                TierList.PlacePower(HoverCPUE.PowerID, !Control.LeftMouseClick);
            }
            else if (SelectedType == -3)
            {
                TierList.RemovePower(HoverCPUE.PowerID);
            }
        }
    }
    public void OnFixedUpdate()
    {
        UpdateSizing();
        if (!HasInit)
        {
            Init();
            HasInit = true;
        }
        UpdateSelectedType(SelectedType);
        TierListUpdate();
    }
    public void UpdateSizing()
    {
        float currentSize = PowerUpLayoutGroup.GetComponent<RectTransform>().rect.width;
        currentSize -= 22; //Padding
        float spacing = 31;
        float spaceForPowers = 160;
        float bonusSize = currentSize - spaceForPowers; //Spacing is not utilized on the first powerup of the layout
        spaceForPowers += spacing;
        bonusSize %= spaceForPowers;
        int halfBonus = (int)(bonusSize / 2f);
        PowerUpLayoutGroup.padding = new RectOffset(11 + halfBonus, 11 + halfBonus, 10, 10);
    }
    public void TierListUpdate()
    {
        UpdateContentSize();
        Owner.OpenCompendiumButton.interactable = !TierListActive;
        Vector2 newTopBarPositon = !TierListActive ? new Vector2(0, 540f) : new Vector2(0, 740f);
        Vector2 newSideBarPosition = !TierListActive ? new Vector2(Compendium.HalfResolution, 340f) : new Vector2(Compendium.HalfResolution, 540f);
        Vector2 newOpenButtonPosition = !TierListActive ? new Vector2(-Compendium.HalfResolution + 25.5f, 515f) : new Vector2(-Compendium.HalfResolution + 25.5f, 715f);
        Vector2 targetViewport = !TierListActive ? new Vector2(-Compendium.HalfResolution + 200, 390f) : new Vector2(-Compendium.HalfResolution + 200, 590f);
        Vector2 targetTierList = !TierListActive ? new Vector2(0, TierList.TotalDistanceCovered - 800) : new Vector2(0, -800f);
        Vector2 sortBarTarget = !TierListActive ? new Vector2(0, 00) : new Vector2(0, 200f);

        Utils.LerpSnap(Owner.TopBar, newTopBarPositon);
        Utils.LerpSnap(Owner.SideBar, newSideBarPosition);
        Utils.LerpSnap(Owner.OpenCompendiumButton.gameObject.transform, newOpenButtonPosition);
        Utils.LerpSnap(ViewPort.transform, targetViewport);
        Utils.LerpSnap(TierListParent.transform, targetTierList);
        Utils.LerpSnap(SortBar.transform, sortBarTarget);
        ViewPort.sizeDelta = Vector2.Lerp(ViewPort.sizeDelta, new Vector2(0, TierListActive ? 200 : 0), 0.1f);

        if (HasSnappedTierList != TierListActive)
        {
            if (ContentRectangle.transform.localPosition.Distance(Vector2.zero) > 0.5f)
                ContentRectangle.transform.localPosition = ContentRectangle.transform.localPosition.Lerp(Vector2.zero, 0.1f);
            else
            {
                ContentRectangle.transform.localPosition = Vector2.zero;
                HasSnappedTierList = TierListActive;
            }
        }
        ContentScrollRect.vertical = HasSnappedTierList == TierListActive;

        bool hasSelectedPower = SelectedType >= 0 && PowerUp.Get(SelectedType).PickedUpCountAllRuns > 0;
        HoverCPUE.gameObject.SetActive(hasSelectedPower && TierListActive); //change this to color scaling or other continuous function for disappearance and reappearance animation
        if (TierListActive)
        {
            Vector3 targetPosition = Utils.MouseWorld + new Vector2(2, 1);
            Rect boundingRect = ViewPort.rect;
            HoverCPUE.gameObject.transform.position = HoverCPUE.gameObject.transform.position.Lerp(targetPosition, 0.1f);
            Vector3 pos = Utils.ClampToRect(HoverCPUE.gameObject.transform.localPosition, boundingRect, 66);
            HoverCPUE.gameObject.transform.localPosition = pos;

            TierList.OnUpdate();

            //Debug.Log(SelectedType);
        }
    }
}
