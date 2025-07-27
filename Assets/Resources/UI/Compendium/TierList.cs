using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TierList : MonoBehaviour
{
    private readonly string TierNames = "SABCDF";
    public TierCategory[] Categories;
    private TierCategory SelectedCat;
    public Canvas MyCanvas;
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
    public void PlacePower(int i)
    {
        if (SelectedCat == null)
            return;
        CompendiumPowerUpElement CPUE = Instantiate(CompendiumPowerUpElement.Prefab, SelectedCat.Grid.transform, false).GetComponent<CompendiumPowerUpElement>();
        CPUE.Style = 2;
        CPUE.Init(i, MyCanvas);
    }
}
