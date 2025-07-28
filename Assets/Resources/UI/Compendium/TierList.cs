using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class TierList : MonoBehaviour
{
    public static bool ReadingFromSave = false;
    public static int QueueRemoval = -1;
    public Compendium Owner;
    public static float TotalDistanceCovered = 800f;
    public static readonly Dictionary<int, bool> OnTierList = new();
    public static readonly List<CompendiumPowerUpElement> Powers = new();
    public static readonly string TierNames = "SABCDF";
    public TierCategory[] Categories;
    private TierCategory SelectedCat;
    public void SetSelectedCategory(int i)
    {
        SelectedCat = Categories[i];
    }
    public Canvas MyCanvas;
    public static bool PowerHasBeenPlaced(int i)
    {
        return OnTierList[i];
    }
    public void Start()
    {
        OnTierList.Clear();
        Powers.Clear();
        TotalDistanceCovered = 800f;
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
        if(QueueRemoval >= 0)
        {
            RemovePower(QueueRemoval, false);
            QueueRemoval = -1;
        }
        if(SelectedCat == null && Owner.HoverCPUE.PowerID >= 0)
        {
            RemovePower(Owner.HoverCPUE.PowerID);
        }
        bool preventHovering = Compendium.HoldingAPower && !Compendium.HoldingALockedPower;
        foreach (CompendiumPowerUpElement cpue in Powers)
        {
            cpue.MyElem.PreventHovering = preventHovering;
        }
    }
    public List<float> UniqueYValues(List<CompendiumPowerUpElement> childs, float bonus)
    {
        List<float> unique = new();
        foreach(CompendiumPowerUpElement cpue in childs)
        {
            if (!unique.Contains(cpue.rectTransform.position.y))
            {
                unique.Add(cpue.rectTransform.position.y);
            }
        }
        if(!unique.Contains(bonus))
            unique.Add(bonus);
        return unique;
    }
    public float ConvertToClosestYValue(float y, List<float> uniqueYVals)
    {
        float dist = float.MaxValue;
        int best = 0;
        for(int i = 0; i < uniqueYVals.Count; ++i)
        {
            float compete = MathF.Abs(y - uniqueYVals[i]);
            if(compete < dist)
            {
                dist = compete;
                best = i;
            }
        }
        return uniqueYVals[best];
    }
    public void InsertIntoTransform(Transform parentGrid, CompendiumPowerUpElement newestCPU, int position = -1)
    {
        float currentPosX = newestCPU.rectTransform.position.x;
        float currentPosY = newestCPU.rectTransform.position.y;
        int c = parentGrid.childCount;
        newestCPU.transform.SetParent(parentGrid);
        if (c <= 0 || position >= c - 1)
        {
            return;
        }
        List<CompendiumPowerUpElement> childs = Compendium.GetCPUEChildren(parentGrid, out c);
        childs.Remove(newestCPU);
        --c;
        bool autoSelectPosition = position == -1;
        Vector2 mousePos = autoSelectPosition ? Utils.MouseWorld : Vector2.zero;
        if(autoSelectPosition)
        {
            List<float> RoundedMousePos = UniqueYValues(childs, currentPosY);
            mousePos.y = ConvertToClosestYValue(mousePos.y, RoundedMousePos);
            float offset = Camera.main.transform.position.x;
            float scalerX = 1.6f;// offset / 1.9f; //Divide offset by almost two to get roughly half the size needed
            float scalerY = 1.75f; //Scaller is different for Y based on resolution
            int closest = int.MaxValue;
            float best = float.MaxValue;
            for (int i = 0; i < c; ++i)
            {
                CompendiumPowerUpElement CPUE = childs[i];
                Vector2 pos = new(CPUE.rectTransform.position.x + offset, CPUE.rectTransform.position.y + 0.025f);
                //Debug.Log($"{pos}, {mousePos}");
                float d = pos.Distance(mousePos, 1, 100f);
                float dY = Mathf.Abs(pos.y - mousePos.y);
                float dY2 = Mathf.Abs(currentPosY - pos.y);
                float dY3 = Mathf.Abs(currentPosY - mousePos.y);
                bool selectorIsNearMouse = dY < scalerY;
                bool sameLevelAsPreviewVisual = dY2 < 0.1f;
                bool mouseSameLevelAsPreviewVisual = dY3 < scalerY;
                bool AllowedToPlaceNear = (sameLevelAsPreviewVisual && mouseSameLevelAsPreviewVisual) || selectorIsNearMouse;
                if (!AllowedToPlaceNear) //closest element is not on the same y level
                    continue;
                if (d < best)
                {
                    best = d;
                    closest = i;
                    float positionToConsider = mouseSameLevelAsPreviewVisual ? currentPosX : mousePos.x;
                    bool selectorIsAbove = currentPosY > pos.y && !sameLevelAsPreviewVisual; //If the selector is above, that is the same as it being to the left
                    bool right = pos.x < positionToConsider && !selectorIsAbove;
                    if(right) //If i am to the right of you, I want to check for your right side
                    {
                        if (pos.x + scalerX < mousePos.x)
                            closest++;
                    }
                    else //If i am to the left of you, I want to check for your left side
                    {
                        if (pos.x - scalerX < mousePos.x && (mouseSameLevelAsPreviewVisual || selectorIsAbove))
                            closest++;
                    }
                }
            }
            //Debug.Log(trueClosest + ": " + closest);
            position = closest;
        }
        if (position < 0)
            newestCPU.transform.SetSiblingIndex(0);
        int counter = 0;
        for (int i = 0; i < c; ++i)
        {
            CompendiumPowerUpElement CPUE = childs[i];
            if (position == i)
                newestCPU.transform.SetSiblingIndex(counter++);
            CPUE.transform.SetSiblingIndex(counter++);
        }
        if(position >= c)
            newestCPU.transform.SetSiblingIndex(c);
    }
    public void PlacePower(int i, bool preview = true)
    {
        if (SelectedCat == null)
            return;
        int insertPos = ReadingFromSave ? 10000 : -1;
        CompendiumPowerUpElement cpue = Powers.Find(g => g.PowerID == i);
        if (PowerUp.Get(i).PickedUpCountAllRuns <= 0)
            return;
        if (!OnTierList[i])
        {
            if (cpue == null)
            {
                cpue = Instantiate(CompendiumPowerUpElement.Prefab).GetComponent<CompendiumPowerUpElement>();
                InsertIntoTransform(SelectedCat.Grid.transform, cpue, insertPos);
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
                InsertIntoTransform(SelectedCat.Grid.transform, cpue, insertPos);
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
        if(!ReadingFromSave)
        {
            Owner.SetVisibility();
            Owner.Sort();
            if (!prevVal && val) //Place power
            {
                CompendiumPowerUpElement nextElem = Owner.NextSlot();
                if (nextElem != null && !nextElem.GrayOut && Compendium.AutoNextTierList)
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
}
