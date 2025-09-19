using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
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
    public int SelectedType { get; private set; }
    public bool ShowOnlyUnlocked { get; set; }
    public bool ShowCounts { get; set; }
    public bool ReverseSort { get; set; }
    private int SortMode { get; set; } //could be made into a field
    private bool TierListActive { get; set; } //could be made into a field
    private bool HasSnappedTierList { get; set; } //could be made into a field
    public bool WideDisplayStyle { get; set; } = false;
    public bool HasInit { get; set; } = false;
    public CompendiumElement HoverCPUE;
    public RectTransform SelectionArea;
    public GridLayoutGroup PowerUpLayoutGroup;
    public RectTransform ContentRectangle;
    public RectTransform SpriteMaskRectangle;
    public ScrollRect ContentScrollRect;
    public Transform TierListParent;
    public RectTransform ViewPort;
    public TierList TierList;
    public bool HoldingAPower => SelectedType >= 0; // set => SelectedType = value ? SelectedType : -3; }
    public bool HoldingALockedPower => HoldingAPower && HoverCPUE.IsLocked();
    public void UpdateSelectedType(int i)
    {
        SelectedType = i;
        if (Owner.ActiveElement.TypeID != SelectedType && SelectedType >= 0)
        {
            Owner.UpdateDisplay(SelectedType);
            Owner.UpdateDescription(true, SelectedType);
        }
        else
            Owner.UpdateDescription(false, SelectedType);
        if (HoverCPUE.TypeID != SelectedType && SelectedType >= 0)
        {
            if(TierList != null)
                TierList.RemovePower(HoverCPUE.TypeID);
            HoverCPUE.Init(SelectedType, MyCanvas);
            HoverCPUE.gameObject.SetActive(!HoverCPUE.IsLocked() && TierListActive); //change this to color scaling or other continuous function for disappearance and reappearance animation
        }
    }
    public void ToggleSort(TextMeshProUGUI sortText)
    {
        SortMode = (SortMode + 1) % 3;
        UpdateSort(sortText);
    }
    public void ToggleTierList(TextMeshProUGUI tierListText)
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
        tierListText.text = TierListActive ? "Save Tier List" : "Make Tier List";
        SetVisibility();
        Sort();
    }
    public void ToggleDisplayMode(TextMeshProUGUI tierListText)
    {
        WideDisplayStyle = !WideDisplayStyle;
        tierListText.text = WideDisplayStyle ? "Display: Line" : "Display: Grid";
        SetVisibility();
        Sort();
    }
    public void UpdateSort(TextMeshProUGUI sortText)
    {
        if (SortMode == FavSort && HoverCPUE is CompendiumAchievementElement)
            SortMode = ArbitrarySort;
        if (SortMode == ArbitrarySort) //Arbitrary / ID
            sortText.text = "Sort: ID";
        else if (SortMode == RaritySort) //By rarity
            sortText.text = "Sort: Rarity";
        else if (SortMode == FavSort) //By count
        {
            if(Compendium.CurrentlySelectedPage == Compendium.Instance.EnemyPage)
                sortText.text = "Sort: Kill Count";
            else
                sortText.text = "Sort: Favorite";
        }
        Sort();
    }
    public void ToggleUnlock(Button unlockButton)
    {
        ShowOnlyUnlocked = !ShowOnlyUnlocked;
        unlockButton.targetGraphic.color = ShowOnlyUnlocked ? Color.yellow : Color.white;
        SetVisibility();
    }
    public void ToggleCount(Button countButton)
    {
        ShowCounts = !ShowCounts;
        countButton.targetGraphic.color = ShowCounts ? Color.yellow : Color.white;
    }
    public void ToggleReverse(Button reverseButton)
    {
        ReverseSort = !ReverseSort;
        reverseButton.targetGraphic.color = ReverseSort ? Color.yellow : Color.white;
        Sort();
    }
    public void UpdateAllButtons(TextMeshProUGUI sortText, TextMeshProUGUI tierListText, Button unlockButton, Button countButton, Button reverseButton)
    {
        if (SortMode == FavSort && HoverCPUE is CompendiumAchievementElement)
            SortMode = ArbitrarySort;
        if (SortMode == ArbitrarySort) //Arbitrary / ID
            sortText.text = "Sort: ID";
        else if (SortMode == RaritySort) //By rarity
            sortText.text = "Sort: Rarity";
        else if (SortMode == FavSort) //By count
        {
            if (Compendium.CurrentlySelectedPage == Compendium.Instance.EnemyPage)
                sortText.text = "Sort: Kill Count";
            else
                sortText.text = "Sort: Favorite";
        }
        tierListText.text = TierList != null ? "Make Tier List" : WideDisplayStyle ? "Display: Line" : "Display: Grid";
        unlockButton.targetGraphic.color = ShowOnlyUnlocked ? Color.yellow : Color.white;
        countButton.targetGraphic.color = ShowCounts ? Color.yellow : Color.white;
        reverseButton.targetGraphic.color = ReverseSort ? Color.yellow : Color.white;
    }
    public void Start()
    {
        //Instance = this;
        SelectedType = 0;
        if(this == Compendium.Instance.AchievementPage)
        {
            SelectedType = GetCPUEChildren(out int i)[0].TypeID;
        }
        ShowOnlyUnlocked = ShowCounts = TierListActive = MouseInCompendiumArea = AutoNextTierList = false;
    }
    public void Init(Button countButton, TextMeshProUGUI sortText)
    {
        if (HoverCPUE is CompendiumPowerUpElement)
        {
            for (int i = 0; i < PowerUp.Reverses.Count; ++i)
            {
                CompendiumPowerUpElement CPUE = Instantiate(CompendiumPowerUpElement.Prefab, PowerUpLayoutGroup.transform, false).GetComponent<CompendiumPowerUpElement>();
                CPUE.Init(i, MyCanvas);
                TierList.OnTierList[i] = false;
            }
        }
        else if(HoverCPUE is CompendiumEquipmentElement)
        {
            if (HoverCPUE is CompendiumAchievementElement)
            {
                for (int i = 0; i < UnlockCondition.Unlocks.Count; ++i)
                {
                    if (UnlockCondition.Get(i).AssociatedUnlocks.Count > 0)
                    {
                        CompendiumAchievementElement CAE = Instantiate(CompendiumAchievementElement.Prefab, PowerUpLayoutGroup.transform, false).GetComponent<CompendiumAchievementElement>();
                        CAE.Init(i, MyCanvas);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Main.Instance.EquipData.AllEquipmentsList.Count; ++i)
                {
                    CompendiumEquipmentElement CPUE = Instantiate(CompendiumEquipmentElement.Prefab, PowerUpLayoutGroup.transform, false).GetComponent<CompendiumEquipmentElement>();
                    CPUE.Init(i, MyCanvas);
                    TierList.OnTierList[i] = false;
                }
            }
        }
        else if (HoverCPUE is CompendiumEnemyElement)
        {
            for (int i = 0; i < EnemyID.AllEnemiesList.Count; ++i)
            {
                CompendiumEnemyElement CPUE = Instantiate(CompendiumEnemyElement.Prefab, PowerUpLayoutGroup.transform, false).GetComponent<CompendiumEnemyElement>();
                CPUE.Init(i, MyCanvas);
                TierList.OnTierList[i] = false;
            }
        }
        UpdateContentSize();
        ShowCounts = true;
        ToggleCount(countButton); //OFF by default

        if(HoverCPUE is CompendiumAchievementElement)
        {
            SortMode = FavSort;
            ToggleSort(sortText); //default sort is ID
        }
        else
        {
            SortMode = ArbitrarySort;
            ToggleSort(sortText); //default sort is rare
        }

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
        float defaultDist = (TierList != null ? TierList.TotalDistanceCovered : 800f) - 200;
        float paddingBonus = PowerUpLayoutGroup.padding.bottom + PowerUpLayoutGroup.cellSize.y;
        float dist = -lastElement.y;
        r.sizeDelta = new Vector2(r.sizeDelta.x, Mathf.Max(defaultDist, dist));
        ContentRectangle.sizeDelta = Vector2.Lerp(ContentRectangle.sizeDelta, new Vector2(0, dist - defaultDist + (TierListActive ? defaultDist : 0)), 0.1f);
        if(SpriteMaskRectangle != null)
        {
            SpriteMaskRectangle.localScale = new Vector3(ViewPort.rect.width, ViewPort.rect.height, 1);
        }
        PowerUpLayoutGroup.transform.localPosition = new Vector3(0, Mathf.Min(0, 600 - defaultDist), 0);
    }
    public static List<CompendiumElement> GetCPUEChildren(Transform parent, out int count)
    {
        count = parent.childCount;
        List<CompendiumElement> childs = new();
        for (int i = 0; i < count; ++i)
            childs.Add(parent.GetChild(i).GetComponent<CompendiumElement>());
        return childs;
    }
    public List<CompendiumElement> GetCPUEChildren(out int count)
    {
        return GetCPUEChildren(PowerUpLayoutGroup.transform, out count);
    }
    public CompendiumElement NextSlot()
    {
        int i = 0;
        CompendiumElement cpue = PowerUpLayoutGroup.transform.GetChild(i).GetComponent<CompendiumElement>();
        while (cpue != null && cpue.IsLocked())
        {
            cpue = PowerUpLayoutGroup.transform.GetChild(++i).GetComponent<CompendiumElement>();
        }
        return cpue;
    }
    public void SetVisibility()
    {
        List<CompendiumElement> childs = GetCPUEChildren(out int c);
        foreach (CompendiumElement cpue in childs)
        {
            bool locked = cpue.IsLocked();
            cpue.gameObject.SetActive(!locked || !ShowOnlyUnlocked);
            if (!TierListActive)
                cpue.SetGrayOut(false);
            else
                cpue.SetGrayOut(TierList.HasBeenPlaced(cpue.TypeID));
            cpue.transform.localPosition = new Vector3(0, 0, 0); //Failsafe for repositioning elements as disabling them sometimes has weird behavior with layout group
        }
    }
    public int SortMultiplier => ReverseSort ? -1 : 1;
    public int CompareID(CompendiumElement e1, CompendiumElement e2)
    {
        int id1 = e1.TypeID;
        int id2 = e2.TypeID;
        if (e1.IsLocked())
            id1 += SortMultiplier * 20000;
        if (e1.GrayOut)
            id1 += SortMultiplier * 10000;
        if (e2.IsLocked())
            id2 += SortMultiplier * 20000;
        if (e2.GrayOut)
            id2 += SortMultiplier * 10000;
        int num = id1 - id2;
        return num;
    }
    public int CompareRare(CompendiumElement e1, CompendiumElement e2)
    {
        int rare1 = e1.GetRare(ReverseSort);
        int rare2 = e2.GetRare(ReverseSort);
        if (e1.IsLocked())
            rare1 += SortMultiplier * 20;
        else if (e1.GrayOut)
            rare1 += SortMultiplier * 10;
        if (e2.IsLocked())
            rare2 += SortMultiplier * 20;
        else if (e2.GrayOut)
            rare2 += SortMultiplier * 10;
        int num = rare1 - rare2;
        return num == 0 ? CompareID(e1, e2) : num;
    }
    public int CompareFav(CompendiumElement e1, CompendiumElement e2)
    {
        int count1 = e1.GetCount();
        int count2 = e2.GetCount();
        if (e1.IsLocked())
            count1 += SortMultiplier * (int.MinValue >> 1);
        else if (e1.GrayOut)
            count1 += SortMultiplier * (int.MinValue >> 2);
        if (e2.IsLocked())
            count2 += SortMultiplier * (int.MinValue >> 1);
        else if (e2.GrayOut)
            count2 += SortMultiplier * (int.MinValue >> 2);
        int num = count2 - count1;
        return num == 0 ? CompareRare(e1, e2) : num;
    }
    public void Sort()
    {
        List<CompendiumElement> childs = GetCPUEChildren(out int c);
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
        if(!ReverseSort)
        {
            for (int i = 0; i < c; ++i)
            {
                CompendiumElement CPUE = childs[i];
                CPUE.transform.SetParent(PowerUpLayoutGroup.transform);
                CPUE.transform.localPosition = new Vector3(CPUE.transform.localPosition.x, CPUE.transform.localPosition.y, 0);
            }
        }
        else
        {
            for (int i = c - 1; i >= 0; --i)
            {
                CompendiumElement CPUE = childs[i];
                CPUE.transform.SetParent(PowerUpLayoutGroup.transform);
                CPUE.transform.localPosition = new Vector3(CPUE.transform.localPosition.x, CPUE.transform.localPosition.y, 0);
            }
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
            if (HoverCPUE.TypeID == SelectedType)
            {
                TierList.PlacePower(HoverCPUE.TypeID, !Control.LeftMouseClick);
            }
            else if (SelectedType == -3)
            {
                TierList.RemovePower(HoverCPUE.TypeID);
            }
        }
    }
    public void OnFixedUpdate()
    {
        UpdateSizing();
        UpdateSelectedType(SelectedType);
        TierListUpdate();
    }
    public void UpdateSizing()
    {
        RectTransform r = PowerUpLayoutGroup.GetComponent<RectTransform>();
        float currentSize = r.rect.width;
        currentSize -= 22; //Padding
        float spacing = 31;
        float spaceForPowers = 160;

        float bonusSize = currentSize - spaceForPowers; //Spacing is not utilized on the first powerup of the layout
        spaceForPowers += spacing;
        if (!WideDisplayStyle)
        {
            bonusSize %= spaceForPowers;
            int halfBonus = (int)(bonusSize / 2f);
            PowerUpLayoutGroup.padding = new RectOffset(11 + halfBonus, 11 + halfBonus, 10, 10);
        }
        else
        {
            PowerUpLayoutGroup.padding = new RectOffset(11, 11, 10, 10);
        }

        if (WideDisplayStyle)
        {
            spacing += (r.rect.width - spaceForPowers * 2 - PowerUpLayoutGroup.padding.left) / 2;
        }
        else if (this == Compendium.Instance.AchievementPage)
        {
            spacing = 31;
        }

        PowerUpLayoutGroup.spacing = new Vector2(spacing, 10);
    }
    public void TierListUpdate()
    {
        UpdateContentSize();
        Owner.OpenCompendiumButton.interactable = !TierListActive;
        foreach(Button b in Owner.PageButtons)
            b.interactable = !TierListActive;
        Vector2 newTopBarPositon = !TierListActive ? new Vector2(0, 540f) : new Vector2(0, 740f);
        Vector2 newSideBarPosition = !TierListActive ? new Vector2(Compendium.HalfResolution, 340f) : new Vector2(Compendium.HalfResolution, 540f);
        Vector2 newOpenButtonPosition = !TierListActive ? new Vector2(-Compendium.HalfResolution + 37f, 515f) : new Vector2(-Compendium.HalfResolution + 25.5f, 715f);
        Vector2 targetViewport = !TierListActive ? new Vector2(-Compendium.HalfResolution + 200, 390f) : new Vector2(-Compendium.HalfResolution + 200, 590f);
        if(TierList != null)
        {
            Vector2 targetTierList = !TierListActive ? new Vector2(0, TierList.TotalDistanceCovered - 800) : new Vector2(0, -800f);
            Utils.LerpSnap(TierListParent.transform, targetTierList);
        }
        Utils.LerpSnap(Owner.TopBar, newTopBarPositon);
        Utils.LerpSnap(Owner.SideBar, newSideBarPosition);
        Utils.LerpSnap(Owner.OpenCompendiumButton.gameObject.transform, newOpenButtonPosition);
        Utils.LerpSnap(ViewPort.transform, targetViewport);
        //Utils.LerpSnap(SortBar.transform, sortBarTarget);
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

        bool hasSelectedPower = SelectedType >= 0 && !HoverCPUE.IsLocked();
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
