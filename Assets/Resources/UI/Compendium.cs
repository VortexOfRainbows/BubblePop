using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Compendium : MonoBehaviour
{
    //public static Compendium Instance;
    public static bool MouseInCompendiumArea = false;
    public static int SelectedType = 0;
    public CompendiumPowerUpElement PrimaryCPUE;
    public TextMeshProUGUI PrimaryCPUEDescription;
    public RectTransform SelectionArea;
    public RectTransform DescriptionScrollArea;
    private const int ArbitrarySort = 0;
    private const int RaritySort = 1;
    private const int FavSort = 2;
    public Canvas MyCanvas;
    public GridLayoutGroup ContentLayout;
    public RectTransform ContentLayoutRect;
    private bool Active = false;
    private bool HasInit = false;
    private Vector3 StartingPosition;
    public int SortMode = 0;
    public static bool ShowOnlyUnlocked, ShowCounts;
    public Button SortButton, UnlockButton, CountButton, TierListButton, OpenButton;
    public TextMeshProUGUI SortText;
    public TextMeshProUGUI TierListText;
    public GameObject[] Stars;
    public void ToggleSort()
    {
        SortMode = (SortMode + 1) % 3;
        UpdateSort();
    }
    public void ToggleTierList()
    {
        TierListActive = !TierListActive;
        TierListText.text = TierListActive ? "Save Tier List" : "Make Tier List";
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
        UpdateContentSize();
        ShowCounts = false;
        ToggleCount(); //On by default
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
        float defaultDist = 600;
        float paddingBonus = ContentLayout.padding.bottom + ContentLayout.cellSize.y;
        float dist = -lastElement.y + paddingBonus;
        r.sizeDelta = new Vector2(r.sizeDelta.x, Mathf.Max(defaultDist, dist));
        ContentLayoutRect.sizeDelta = Vector2.Lerp(ContentLayoutRect.sizeDelta, new Vector2(0, paddingBonus / 2f + (TierListActive ? defaultDist : 0)), 0.1f);
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
    public void Update()
    {
        MouseInCompendiumArea = Utils.IsMouseHoveringOverThis(true, SelectionArea, 0, MyCanvas);
    }
    public void FixedUpdate()
    {
        //Instance = this;
        if (Active)
        {
            if (!HasInit)
            {
                Init();
                HasInit = true;
            }
            if (transform.position.x > -0.5f)
                transform.position = Vector3.zero;
            else transform.position = transform.position.Lerp(new Vector3(0, 0, 0), 0.1f);
            if(PrimaryCPUE.PowerID != SelectedType)
            {
                PrimaryCPUE.Init(SelectedType, MyCanvas);
                UpdateDescription(true);
            }
            else
                UpdateDescription(false);
        }
        else
        {
            if (transform.position.x < -1919.5f)
                transform.position = StartingPosition;
            else transform.position = transform.position.Lerp(StartingPosition, 0.1f);
        }
        TierListUpdate();
    }
    public void UpdateDescription(bool reset)
    {
        if(reset)
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
    public GameObject ViewPort, TierList;
    public bool TierListActive = false;
    public VerticalLayoutGroup TierListLayoutGroup;
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
        Vector2 targetTierList = !TierListActive ? new Vector2(0, 0) : new Vector2(0, -800f);

        LerpSnap(TopBar.transform, newTopBarPositon);
        LerpSnap(SideBar.transform, newSideBarPosition);
        LerpSnap(OpenButton.gameObject.transform, newOpenButtonPosition);
        LerpSnap(ViewPort.transform, targetViewport);
        LerpSnap(TierList.transform, targetTierList);
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
    }
}
