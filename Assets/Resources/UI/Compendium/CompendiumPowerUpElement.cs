using UnityEngine;
using UnityEngine.UI;

public class CompendiumPowerUpElement : MonoBehaviour
{
    public static int CompareID(CompendiumPowerUpElement e1, CompendiumPowerUpElement e2)
    {
        int id1 = e1.PowerID;
        int id2 = e2.PowerID;
        if (e1.GrayOut)
            id1 += 10000;
        if (e2.GrayOut)
            id2 += 10000;
        int num = id1 - id2;
        return num;
    }
    public static int CompareRare(CompendiumPowerUpElement e1, CompendiumPowerUpElement e2)
    {
        int rare1 = PowerUp.Get(e1.PowerID).GetRarity();
        int rare2 = PowerUp.Get(e2.PowerID).GetRarity();
        if (e1.GrayOut)
            rare1 += 10;
        if (e2.GrayOut)
            rare2 += 10;
        int num = rare1 - rare2;
        return num == 0 ? CompareID(e1, e2) : num;
    }
    public static int CompareFav(CompendiumPowerUpElement e1, CompendiumPowerUpElement e2)
    {
        int count1 = PowerUp.Get(e1.PowerID).PickedUpCountAllRuns;
        int count2 = PowerUp.Get(e2.PowerID).PickedUpCountAllRuns;
        if (e1.GrayOut)
            count1 = (int.MinValue >> 1) + count1;
        if (e2.GrayOut)
            count2 = (int.MinValue >> 1) + count2;
        int num = count2 - count1;
        return num == 0 ? CompareRare(e1, e2) : num;
    }
    public static GameObject Prefab => Resources.Load<GameObject>("UI/Compendium/CompendiumPowerUpElement");
    public RectTransform rectTransform;
    public Image BG;
    public PowerUpUIElement MyElem;
    public int PowerID = 0;
    private bool Selected;
    public int Style = 0;
    public bool GrayOut { get; set; } = false;
    public void Init(int i, Canvas canvas)
    {
        MyElem.SetPowerType(PowerID = i);
        MyElem.myCanvas = canvas;
        MyElem.Count.text = PowerUp.Get(PowerID).PickedUpCountAllRuns.ToString();
        MyElem.GrayOut = GrayOut;
        int forceInitUpdates = 1;
        if (Style == 2)
        {
            BG.enabled = false;
            MyElem.ForceHideCount = true;
            transform.localScale = Vector3.one * 0.8f;
            transform.GetComponent<RectTransform>().pivot = Vector2.one * 0.5f;
            forceInitUpdates += 2;
        }
        for(int a = 0; a < forceInitUpdates; ++a)
            MyElem.OnUpdate();
    }
    public void Update()
    {
        if (PowerID == -1 && gameObject.activeSelf)
            Destroy(gameObject);
        MyElem.GrayOut = GrayOut;
        MyElem.OnUpdate();
        if(!MyElem.PreventHovering)
        {
            Selected = PowerID == Compendium.SelectedType;
            if (Style != 2)
            {
                Color target = Selected ? new Color(1, 1, .4f, 0.431372549f) : new Color(0, 0, 0, 0.431372549f);
                BG.color = Color.Lerp(BG.color, target, 0.125f);
            }
        }
    }
}
