using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompendiumDescriptionSegment : MonoBehaviour
{
    public static CompendiumDescriptionSegment NewTitle(Transform parent, string text, float textSize = -1) => New(TitleObject, parent, text, textSize);
    public static CompendiumDescriptionSegment NewDescription(Transform parent, string text, float textSize = 28) => New(DefaultObject, parent, text, textSize);
    private static CompendiumDescriptionSegment New(CompendiumDescriptionSegment prefab, Transform parent, string text, float textSize)
    {
        var obj = Instantiate(prefab, parent);
        if(textSize > 0)
        {
            obj.Description.fontSize = textSize;
            obj.Description.enableAutoSizing = false;
        }
        obj.Description.text = text;
        obj.Description.ForceMeshUpdate();
        obj.TrueHeight = obj.GetHeight();
        return obj;
    }
    public float Padding = 5;
    public static CompendiumDescriptionSegment DefaultObject => Resources.Load<GameObject>("UI/Compendium/Description/CompendiumContentSegment").GetComponent<CompendiumDescriptionSegment>();
    public static CompendiumDescriptionSegment TitleObject => Resources.Load<GameObject>("UI/Compendium/Description/TitleSegment").GetComponent<CompendiumDescriptionSegment>();
    public RectTransform Casing;
    public Image BackgroundImage;
    public TextMeshProUGUI Description;
    public float TrueHeight { get; private set; }
    private float GetHeight()
    {
        Vector2 size = Description.GetRenderedValues();
        float trueHeight = size.y + Padding * 2;
        Casing.sizeDelta = new Vector2(Casing.sizeDelta.x, trueHeight);
        return Mathf.Max(0, trueHeight);
    }
}
