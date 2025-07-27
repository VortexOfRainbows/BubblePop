using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierList : MonoBehaviour
{
    private readonly string TierNames = "SABCDF";
    public TierCategory[] Categories;
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
}
