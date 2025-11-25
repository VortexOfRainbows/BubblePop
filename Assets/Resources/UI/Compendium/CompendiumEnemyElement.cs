using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CompendiumEnemyElement : CompendiumElement
{
    public static GameObject Prefab => Resources.Load<GameObject>("UI/Compendium/CompendiumEnemyElement");
    public EnemyUIElement MyElem;
    public Canvas CountCanvas;
    public TextMeshProUGUI count;
    private Canvas MyCanvas { get; set; }
    public int Style = 0;
    protected bool Selected { get; set; }
    public override void Init(int i, Canvas canvas)
    {
        MyElem.SpriteMask.localScale *= 1.05f;
        TypeID = i;
        if (Style == 4)
        {
            MyElem.MaskActive(true);
            MyElem.Mask.GetComponent<Canvas>().sortingOrder = 46;
            MyElem.Mask.GetComponent<Canvas>().sortingLayerID = Main.UICameraLayerID;
            MyElem.AddedDrawOrder = 50;
        }
        MyElem.Init(i);
        //MyElem.UpdateEquipment(Main.Instance.EquipData.AllEquipmentsList[i].GetComponent<Equipment>());
        //MyElem.SetCompendiumLayering(canvas.sortingLayerID, Style == 4 ? 65 : 45, Style == 3 ? 0 : 1); //2 = UICamera, 20 = compendium canvas size
        MyCanvas = canvas;
        MyElem.CompendiumElement = true;
        CountCanvas.sortingLayerID = canvas.sortingLayerID;
        int forceInitUpdates = 1;
        if (Style == 2 || Style == 4) //Hover element
        {
            BG.enabled = false;
            MyElem.HasHoverVisual = false;
            transform.localScale = Vector3.one * 0.72f;
            transform.GetComponent<RectTransform>().pivot = Vector2.one * 0.5f;
            forceInitUpdates += 2;
        }
        else if (Style == 3)
        {
            MyElem.HasHoverVisual = false;
            MyElem.MaskActive(true);
            MyElem.IsMainSelectedInCompendium = true;
        }
        for (int a = 0; a < forceInitUpdates; ++a)
            Update();
    }
    public void Update()
    {
        if (TypeID == -1 && gameObject.activeSelf)
            Destroy(gameObject);
        bool isWithinMaskRange = count.transform.position.y > Compendium.Instance.SortBar.position.y + Compendium.Instance.SortBar.sizeDelta.y * 0.5f * Compendium.Instance.SortBar.lossyScale.y;
        bool showActive = Compendium.Instance.EnemyPage.ShowCounts && MyElem.HasHoverVisual && !IsLocked() && Style <= 1 && !isWithinMaskRange;
        count.gameObject.SetActive(showActive);
        count.text = GetCount().ToString();
        MyElem.UpdateActive(MyCanvas, out bool hovering, out bool clicked, rectTransform);
        if (clicked)
        {
            Compendium.Instance.EnemyPage.UpdateSelectedType(TypeID);
        }
        if (hovering && Control.RightMouseClick)
        {
            Compendium.Instance.EnemyPage.TierList.QueueRemoval = TypeID;
        }
        if (Style <= 1)
        {
            Color target = Selected ? new Color(1, 1, .4f, 0.431372549f) : new Color(0, 0, 0, 0.431372549f);
            BG.color = Color.Lerp(BG.color, target, 0.125f);
        }
        Selected = TypeID == Compendium.Instance.EnemyPage.SelectedType;
        if (IsLocked())
        {
            MyElem.UpdateColor(true, false);
        }
        else
        {
            if (GrayOut)
                MyElem.UpdateColor(false, true);
            else
                MyElem.UpdateColor(false, false);
        }
    }
    public override void SetHovering(bool canHover)
    {
        MyElem.HasHoverVisual = canHover;
    }
    public override bool IsLocked()
    {
        return !MyElem.Unlocked;
    }
    public override int GetRare(bool reverse = false)
    {
        return MyElem.StaticData.Rarity;
    }
    public override int GetCount()
    {
        if(MyElem.MyEnemyPrefab == null)
            return 0;
        return MyElem.StaticData.TimesKilled;
    }
    public override CompendiumElement Instantiate(TierList parent, TierCategory cat, Canvas canvas, int i, int position)
    {
        CompendiumEnemyElement cpue = Instantiate(Prefab).GetComponent<CompendiumEnemyElement>();
        parent.InsertIntoTransform(cat.Grid.transform, cpue, position);
        cpue.Style = 2;
        cpue.MyElem.HasHoverVisual = false;
        cpue.SetGrayOut(true);
        cpue.Init(i, canvas);
        cpue.MyElem.HoverRadius = 0;
        return cpue;
    }
}
