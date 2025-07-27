using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class TierList : MonoBehaviour
{
    public Compendium Owner;
    public static float TotalDistanceCovered = 800f;
    public static readonly Dictionary<int, bool> OnTierList = new();
    private readonly List<CompendiumPowerUpElement> Powers = new();
    private readonly string TierNames = "SABCDF";
    public TierCategory[] Categories;
    private TierCategory SelectedCat;
    public Canvas MyCanvas;
    public static bool PowerHasBeenPlaced(int i)
    {
        return OnTierList[i];
    }
    public void Start()
    {
        InitializeCategories();
    }
    public Color CalculateTierColor(int i)
    {
        return Utils.PastelRainbow(-i * Mathf.PI / 3.6f, 0.7f);
    }
    public void InitializeCategories()
    {
        for(int i = 0; i < Categories.Length; ++i)
        {
            TierCategory cat = Categories[i];
            cat.TierSlot.color = CalculateTierColor(i);
            cat.Text.text = TierNames[i].ToString();
        }
    }
    public void OnUpdate()
    {
        Color unselectColor = new(0.24706f, 0.24706f, 0.24706f);
        Color selectColor = new(.6f, .6f, .25f); 
        SelectedCat = null;
        TotalDistanceCovered = 0;
        for (int i = 0; i < Categories.Length; ++i)
        {
            TierCategory cat = Categories[i];
            if(Utils.IsMouseHoveringOverThis(true, cat.RectTransform, 0, MyCanvas))
            {
                SelectedCat = cat;
                cat.TierRect.color = Color.Lerp(cat.TierRect.color, selectColor, 0.25f);
            }
            else
            {
                cat.TierRect.color = Color.Lerp(cat.TierRect.color, unselectColor, 0.25f);
            }
            cat.CalculateSizeNeededToHousePowerups();
        }
        if(SelectedCat == null && Owner.HoverCPUE.PowerID >= 0)
        {
            RemovePower(Owner.HoverCPUE.PowerID);
        }
        bool canHover = Owner.HoldingAPower;
        foreach (CompendiumPowerUpElement cpue in Powers)
        {
            cpue.MyElem.PreventHovering = canHover;
        }
    }
    public void InsertIntoTransform(Transform parentGrid, CompendiumPowerUpElement newestCPU, int position = -1)
    {
        float currentPosX = newestCPU.rectTransform.position.x;
        newestCPU.transform.SetParent(null);
        int c = parentGrid.childCount;
        if (c <= 0 || position >= c - 1)
        {
            newestCPU.transform.SetParent(parentGrid);
            return;
        }
        List<CompendiumPowerUpElement> childs = Compendium.GetCPUEChildren(parentGrid, out c);
        bool autoSelectPosition = position == -1;
        Vector2 mousePos = autoSelectPosition ? Utils.MouseWorld : Vector2.zero;
        if(autoSelectPosition)
        {
            position = 0;
            float firstElementRelativePosition = MathF.Min(currentPosX - parentGrid.position.x, childs[0].rectTransform.position.x - parentGrid.position.x) * 1.2f;
            for (int i = 0; i < c; ++i)
            {
                CompendiumPowerUpElement CPUE = childs[i];
                Vector2 pos = (Vector2)CPUE.rectTransform.position;
                Debug.Log($"{pos.y}, {mousePos.y}, {firstElementRelativePosition}");
                bool right = (pos.x + firstElementRelativePosition) < mousePos.x; // || (pos.y + rect.height / 2f) > mousePos.y;
                bool above = (pos.y + firstElementRelativePosition * 0.6f) < mousePos.y; // || (pos.y + rect.height / 2f) > mousePos.y;
                bool down = (pos.y - firstElementRelativePosition * 0.6f) > mousePos.y; // || (pos.y + rect.height / 2f) > mousePos.y;
                if ((right && !above) || down)
                    ++position;
            }
            Debug.Log(position);
        }
        parentGrid.DetachChildren();
        for (int i = 0; i < c; ++i)
        {
            CompendiumPowerUpElement CPUE = childs[i];
            if (position == i)
                newestCPU.transform.SetParent(parentGrid);
            CPUE.transform.SetParent(parentGrid);
        }
        if(position >= c)
        {
            newestCPU.transform.SetParent(parentGrid);
        }
    }
    public void PlacePower(int i, bool preview = true)
    {
        if (SelectedCat == null)
            return;
        CompendiumPowerUpElement cpue = Powers.Find(g => g.PowerID == i);
        if (PowerUp.Get(i).PickedUpCountAllRuns <= 0)
            return;
        if (!OnTierList[i])
        {
            if (cpue == null)
            {
                cpue = Instantiate(CompendiumPowerUpElement.Prefab).GetComponent<CompendiumPowerUpElement>();
                InsertIntoTransform(SelectedCat.Grid.transform, cpue);
                cpue.Style = 2;
                cpue.MyElem.PreventHovering = true;
                cpue.GrayOut = true;
                cpue.MyElem.HoverRadius = 0;
                cpue.Init(i, MyCanvas);
                Powers.Add(cpue);
            }
            else
            {
                cpue.transform.SetParent(SelectedCat.Grid.transform);
                InsertIntoTransform(SelectedCat.Grid.transform, cpue);
            }
        }
        else
        {
            ModifyOnTierList(i, false);
            cpue.transform.SetParent(SelectedCat.Grid.transform);
        }
        if (preview && !OnTierList[i] && cpue != null)
            cpue.GrayOut = true;
        else
        {
            cpue.GrayOut = false;
            ModifyOnTierList(i, true);
        }
    }
    public void RemovePower(int i, bool OnlyIfGray = true)
    {
        CompendiumPowerUpElement existingCPUE = Powers.Find(g => g.PowerID == i);
        if (existingCPUE != null && existingCPUE.GrayOut == OnlyIfGray)
        {
            Powers.Remove(existingCPUE);
            Destroy(existingCPUE.gameObject);
            ModifyOnTierList(i, false);
        }
    }
    public void ModifyOnTierList(int i, bool val)
    {
        if (i < 0)
            return;
        bool prevVal = OnTierList[i];
        OnTierList[i] = val;
        Owner.SetVisibility();
        Owner.Sort();
        if (!prevVal && val) //Place power
        {
            CompendiumPowerUpElement nextElem = Owner.NextSlot();
            if (nextElem != null && !nextElem.GrayOut)
            {
                Owner.UpdateSelectedType(nextElem.PowerID);
            }
            else
            {
                Owner.UpdateSelectedType(-4);
            }
        }
    }
}
