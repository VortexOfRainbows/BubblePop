using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class TierList : MonoBehaviour
{
    public Compendium Owner;
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
        }
    }
    public void PlacePower(int i, bool preview = true)
    {
        if (SelectedCat == null)
            return;
        CompendiumPowerUpElement cpue = Powers.Find(g => g.PowerID == i);
        if (!OnTierList[i])
        {
            if (cpue == null)
            {
                cpue = Instantiate(CompendiumPowerUpElement.Prefab, SelectedCat.Grid.transform, false).GetComponent<CompendiumPowerUpElement>();
                cpue.Style = 2;
                cpue.Init(i, MyCanvas);
                Powers.Add(cpue);
            }
            else
            {
                cpue.transform.SetParent(SelectedCat.Grid.transform);
            }
        }
        if(preview && !OnTierList[i] && cpue != null)
            cpue.GrayOut = true;
        else
        {
            cpue.GrayOut = false;
            ModifyOnTierList(i, true);
        }
    }
    public void RemovePower(int i)
    {
        CompendiumPowerUpElement existingCPUE = Powers.Find(g => g.PowerID == i);
        if (existingCPUE != null)
        {
            Powers.Remove(existingCPUE);
            Destroy(existingCPUE.gameObject);
            ModifyOnTierList(i, false);
        }
    }
    public void ModifyOnTierList(int i, bool val)
    {
        OnTierList[i] = val;
        Owner.SetVisibility();
        Owner.Sort();
    }
}
