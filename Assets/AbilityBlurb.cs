using System;
using TMPro;
using UnityEngine;
public class AbilityBlurb : MonoBehaviour
{
    public Canvas MyCanvas { get; set; }
    public RectTransform HoverArea;
    public TextMeshProUGUI IconText;
    public int Type { get; set; }
    public void Update()
    {
        if (Utils.IsMouseHoveringOverThis(true, HoverArea, 0, MyCanvas))
        {
            HoverArea.LerpLocalScale(Vector2.one * 1.1f, Utils.DeltaTimeLerpFactor(0.1f));
            string text;
            if (Type == Ability.ID.Primary)
                text = "Primary";
            else if (Type == Ability.ID.Secondary)
                text = "Secondary";
            else if (Type == Ability.ID.Ability)
                text = "Ability";
            else
                text = "Passive";
            PopUpTextUI.Enable(text, "");
        }
        else
        { 
            HoverArea.LerpLocalScale(Vector2.one, Utils.DeltaTimeLerpFactor(0.1f));
        }
    }
}

public class Ability
{
    public static class ID
    { 
        public const int Primary = 0;
        public const int Secondary = 1;
        public const int Ability = 2;
        public const int Passive = 3;
    }
    public string Blurb;
    public int Type { get; set; }
    public Ability(int type, string blurb)
    {
        Blurb = blurb;
        Type = type;
    }
    public GameObject CreateAbilityBlurb(Transform parent, Canvas canvas)
    {
        var g = GameObject.Instantiate(Main.PrefabAssets.AbilityBlurbPrefab, parent);
        g.GetComponent<TextMeshProUGUI>().text = Blurb;
        var b = g.GetComponent<AbilityBlurb>();
        b.MyCanvas = canvas;
        b.Type = Type;
        if(Type == ID.Primary)
            b.IconText.text = "LMB";
        else if (Type == ID.Secondary)
            b.IconText.text = "RMB";
        else if (Type == ID.Ability)
            b.IconText.text = "ABL";
        else
            b.IconText.text = "PSV";
        return g;
    }



    //This is for the new stuff

    private static float NoProgress() => 0;
    private static int NoNumber() => -1;
    public float ProgressDisplay => ProgressDisplayFunc();
    public int NumberDisplay => NumberDisplayFunc();
    public Func<float> ProgressDisplayFunc { get; private set; } = NoProgress;
    public Func<int> NumberDisplayFunc { get; private set; } = NoNumber;
    public void SetProgressDisplayFunc(Func<float> func) => ProgressDisplayFunc = func;
    public void SetNumberDisplayFunc(Func<int> func) => NumberDisplayFunc = func;
}