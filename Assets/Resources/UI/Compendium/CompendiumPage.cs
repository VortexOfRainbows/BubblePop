using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CompendiumPage : MonoBehaviour
{

}
public abstract class TierListableCompendiumPage : MonoBehaviour
{
    public void Sort()
    {
        List<CompendiumPowerUpElement> childs = GetCPUEChildren(out int c);
        ContentLayout.transform.DetachChildren();
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
            CPUE.transform.SetParent(ContentLayout.transform);
            CPUE.transform.localPosition = new Vector3(CPUE.transform.localPosition.x, CPUE.transform.localPosition.y, 0);
        }
    }
    public void Init()
    {
       
    }
    public Transform SortBar;
    public Button SortButton, UnlockButton, CountButton, TierListButton;
    public bool ShowOnlyUnlocked, ShowCounts;
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
    public void ToggleAuto()
    {
        AutoNextTierList = !AutoNextTierList;
        AutoButton.targetGraphic.color = AutoNextTierList ? Color.yellow : Color.white;
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
    public void CancelTierListChanges()
    {
        ClearTierList();
        PlayerData.LoadTierList(TierList);
        UpdateSelectedType(-3);
        SetVisibility();
        Sort();
    }
    public void ClearTierList()
    {
        TierList.ReadingFromSave = true;
        for (int i = TierList.Powers.Count - 1; i >= 0; --i)
        {
            TierList.RemovePower(TierList.Powers[i].PowerID, false);
        }
        TierList.ReadingFromSave = false;
        UpdateSelectedType(-3);
        SetVisibility();
        Sort();
    }
}

