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
    public DetailedDescription Blurb;
    public int Type { get; set; }
    public Ability(int type, string blurb)
    {
        Blurb = new DetailedDescription(1, "Ability Blurb");
        Blurb.WithoutSizeAugments();
        Blurb.WithDescription(blurb);
        Type = type;
    }
    public GameObject CreateAbilityBlurb(Transform parent, Canvas canvas)
    {
        var g = GameObject.Instantiate(Main.PrefabAssets.AbilityBlurbPrefab, parent);
        g.GetComponent<TextMeshProUGUI>().text = Blurb.FullDescription();
        var b = g.GetComponent<AbilityBlurb>();
        b.MyCanvas = canvas;
        b.Type = Type;
        if(Type == ID.Primary)
            b.IconText.text = "P";
        else if (Type == ID.Secondary)
            b.IconText.text = "S";
        else if (Type == ID.Ability)
            b.IconText.text = "A";
        else
            b.IconText.text = "*";
        return g;
    }
}