using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CompendiumEquipmentElement : CompendiumElement
{
    public static GameObject Prefab => Resources.Load<GameObject>("UI/Compendium/CompendiumEquipmentElement");
    public EquipmentUIElement MyElem;
    public Canvas CountCanvas;
    public TextMeshProUGUI count;
    private Canvas MyCanvas { get; set; }
    public int Style = 0;
    protected bool Selected { get; set; }
    public override void Init(int i, Canvas canvas)
    {
        TypeID = i;
        if (MyElem.ActiveEquipment != null)
            Destroy(MyElem.ActiveEquipment.gameObject);
        MyElem.UpdateEquipment(Main.Instance.EquipData.AllEquipmentsList[i].GetComponent<Equipment>());
        MyElem.SetCompendiumLayering(canvas.sortingLayerID, 50, Style == 3 ? 0 : 1); //2 = UICamera, 20 = compendium canvas size
        CountCanvas.sortingLayerID = canvas.sortingLayerID;
        MyCanvas = canvas;
        MyElem.CompendiumElement = true;
        int forceInitUpdates = 1;
        //if (Style == 2)
        //{
        //    BG.enabled = false;
        //    MyElem.ForceHideCount = true;
        //    transform.localScale = Vector3.one * 0.8f;
        //    transform.GetComponent<RectTransform>().pivot = Vector2.one * 0.5f;
        //    forceInitUpdates += 2;
        //}
        for(int a = 0; a < forceInitUpdates; ++a)
            Update();
    }
    public void Update()
    {
        if (TypeID == -1 && gameObject.activeSelf)
            Destroy(gameObject);
        count.gameObject.SetActive(Compendium.Instance.EquipPage.ShowCounts && !MyElem.DisplayOnly && !IsLocked());
        if(MyElem.ActiveEquipment != null)
        {
            MyElem.UpdateActive(MyCanvas, out bool hovering, out bool clicked, rectTransform);
            if (clicked)
            {
                Compendium.Instance.EquipPage.UpdateSelectedType(TypeID);
            }
            if (Style != 3)
            {
                Color target = Selected ? new Color(1, 1, .4f, 0.431372549f) : new Color(0, 0, 0, 0.431372549f);
                BG.color = Color.Lerp(BG.color, target, 0.125f);
            }
            Selected = TypeID == Compendium.Instance.EquipPage.SelectedType;
        }
        //MyElem.GrayOut = GrayOut;
    }
    public override void SetHovering(bool canHover)
    {
        MyElem.DisplayOnly = !canHover;
    }
    public override void SetGrayOut(bool value)
    {
        base.SetGrayOut(value);
    }
    public override bool IsLocked()
    {
        return !MyElem.Unlocked;
    }
    public override int GetRare()
    {
        return MyElem.ActiveEquipment.GetRarity();
    }
}
