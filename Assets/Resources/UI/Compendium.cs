using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Compendium : MonoBehaviour
{
    private const int ArbitrarySort = 0;
    private const int RaritySort = 1;
    private const int FavSort = 2;
    public Canvas MyCanvas;
    public GridLayoutGroup ContentLayout;
    private bool Active = false;
    private bool HasInit = false;
    private Vector3 StartingPosition;
    public int SortMode = 0;
    public static bool ShowOnlyUnlocked, ShowCounts;
    public Button SortButton, UnlockButton, CountButton;
    public TextMeshProUGUI SortText;
    public void ToggleSort()
    {
        SortMode = (SortMode + 1) % 3;
        UpdateSort();
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
        }
        Vector3 lastElement = ContentLayout.transform.GetChild(ContentLayout.transform.childCount - 1).localPosition;
        RectTransform r = ContentLayout.GetComponent<RectTransform>();
        float dist = -lastElement.y + (ContentLayout.padding.bottom + ContentLayout.cellSize.y) / 2f;
        Debug.Log(dist);
        r.sizeDelta = new Vector2(r.sizeDelta.x, Mathf.Max(r.sizeDelta.y, dist));

        ToggleCount(); //On by default
    }
    public List<CompendiumPowerUpElement> GetCPUEChildren(out int count)
    {
        count = ContentLayout.transform.childCount;
        List<CompendiumPowerUpElement> childs = new();
        for (int i = 0; i < count; ++i)
            childs.Add(ContentLayout.transform.GetChild(i).GetComponent<CompendiumPowerUpElement>());
        return childs;
    }
    public void SetVisibility()
    {
        List<CompendiumPowerUpElement> childs = GetCPUEChildren(out int c);
        foreach(CompendiumPowerUpElement cpue in childs)
        {
            bool locked = cpue.MyElem.AppearLocked;
            cpue.gameObject.SetActive(!locked || !ShowOnlyUnlocked);
        }
    }
    public void Sort()
    {
        List<CompendiumPowerUpElement> childs = GetCPUEChildren(out int c);
        if(SortMode == ArbitrarySort)
        {
            childs.Sort(delegate (CompendiumPowerUpElement e1, CompendiumPowerUpElement e2)
            {
                return e1.PowerID - e2.PowerID;
            });
        }
        if (SortMode == RaritySort)
        {
            childs.Sort(delegate (CompendiumPowerUpElement e1, CompendiumPowerUpElement e2)
            {
                int rare1 = PowerUp.Get(e1.PowerID).GetRarity();
                int rare2 = PowerUp.Get(e2.PowerID).GetRarity();
                return rare1 - rare2;
            });
        }
        if (SortMode == FavSort)
        {
            childs.Sort(delegate (CompendiumPowerUpElement e1, CompendiumPowerUpElement e2)
            {
                int count1 = PowerUp.Get(e1.PowerID).PickedUpCountAllRuns;
                int count2 = PowerUp.Get(e2.PowerID).PickedUpCountAllRuns;
                return count2 - count1;
            });
        }
        ContentLayout.transform.DetachChildren();
        for (int i = 0; i < c; ++i)
        {
            CompendiumPowerUpElement CPUE = childs[i];
            CPUE.transform.SetParent(ContentLayout.transform);
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
            transform.position = transform.position.Lerp(new Vector3(0, 0, 0), 0.1f);
            if (transform.position.x > -0.5f)
                transform.position = Vector3.zero;
        }
        else
        {
            transform.position = transform.position.Lerp(StartingPosition, 0.1f);
            if (transform.position.x < -1919.5f)
                transform.position = StartingPosition;
        }
    }
}
