using UnityEngine;
using UnityEngine.UI;

public class CompendiumPowerUpElement : MonoBehaviour
{
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
