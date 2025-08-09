using UnityEngine;
using UnityEngine.UI;

public class CompendiumEquipmentElement : CompendiumElement
{
    public static GameObject Prefab => Resources.Load<GameObject>("UI/Compendium/CompendiumEquipmentElement");
    public EquipmentUIElement MyElem;
    public int Style { get; set; }
    protected bool Selected { get; set; }
    private bool TempHasRun = false;
    public void LateUpdate()
    {
        if(!TempHasRun)
        {
            TempHasRun = true;
            Init(TypeID, Compendium.Instance.MyCanvas);
        }
    }
    public void Init(int i, Canvas canvas)
    {
        MyElem.UpdateEquipment(Main.Instance.EquipData.AllEquipmentsList[i].GetComponent<Equipment>());
        MyElem.UpdateLayering(canvas.sortingLayerID, 20); //2 = UICamera, 20 = compendium canvas size
        //MyElem.SetPowerType(TypeID = i);
        //MyElem.myCanvas = canvas;
        //MyElem.Count.text = PowerUp.Get(TypeID).PickedUpCountAllRuns.ToString();
        //MyElem.GrayOut = GrayOut;
        //int forceInitUpdates = 1;
        //if (Style == 2)
        //{
        //    BG.enabled = false;
        //    MyElem.ForceHideCount = true;
        //    transform.localScale = Vector3.one * 0.8f;
        //    transform.GetComponent<RectTransform>().pivot = Vector2.one * 0.5f;
        //    forceInitUpdates += 2;
        //}
        //for(int a = 0; a < forceInitUpdates; ++a)
        //    MyElem.OnUpdate();
    }
    public void Update()
    {
        if (TypeID == -1 && gameObject.activeSelf)
            Destroy(gameObject);
        //MyElem.GrayOut = GrayOut;
        //MyElem.OnUpdate();
        //if(!MyElem.PreventHovering)
        //{
        //    Selected = TypeID == Compendium.Instance.PowerPage.SelectedType;
        //    if (Style != 2)
        //    {
        //        Color target = Selected ? new Color(1, 1, .4f, 0.431372549f) : new Color(0, 0, 0, 0.431372549f);
        //        BG.color = Color.Lerp(BG.color, target, 0.125f);
        //    }
        //}
    }
}
