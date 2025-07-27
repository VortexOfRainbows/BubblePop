using UnityEngine;
using UnityEngine.UI;

public class CompendiumPowerUpElement : MonoBehaviour
{
    public static GameObject Prefab => Resources.Load<GameObject>("UI/Compendium/CompendiumPowerUpElement");
    public Image BG;
    public PowerUpUIElement MyElem;
    public int PowerID = 0;
    private bool Selected;
    public int Style = 0;
    public void Init(int i, Canvas canvas)
    {
        MyElem.SetPowerType(PowerID = i);
        MyElem.myCanvas = canvas;
        MyElem.Count.text = PowerUp.Get(PowerID).PickedUpCountAllRuns.ToString();
    }
    public void Update()
    {
        if (PowerID == -1 && gameObject.activeSelf)
            Destroy(gameObject);
        MyElem.OnUpdate();
        if(!MyElem.PreventHovering)
        {
            Selected = PowerID == Compendium.SelectedType;
            Color target = Selected ? new Color(1, 1, .4f, 110 / 255f) : new Color(0, 0, 0, 110 / 255f);
            BG.color = Color.Lerp(BG.color, target, 0.125f);
        }
    }
}
