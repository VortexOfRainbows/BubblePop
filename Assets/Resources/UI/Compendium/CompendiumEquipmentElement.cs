using TMPro;
using UnityEngine;
public class CompendiumEquipmentElement : CompendiumElement
{
    public static GameObject Prefab => Resources.Load<GameObject>("UI/Compendium/CompendiumEquipmentElement");
    public EquipmentUIElement MyElem;
    public Canvas CountCanvas;
    public TextMeshProUGUI count;
    protected Canvas MyCanvas { get; set; }
    public int Style = 0;
    protected bool Selected { get; set; }
    public override void Init(int i, Canvas canvas)
    {
        TypeID = i;
        if (MyElem.ActiveEquipment != null)
            Destroy(MyElem.ActiveEquipment.gameObject);
        MyElem.UpdateEquipment(Main.GlobalEquipData.AllEquipmentsList[i].GetComponent<Equipment>());
        MyElem.SetCompendiumLayering(canvas.sortingLayerID, Style == 4 ? 65 : 45, Style == 3 ? 0 : 1); //2 = UICamera, 20 = compendium canvas size
        CountCanvas.sortingLayerID = canvas.sortingLayerID;
        MyCanvas = canvas;
        MyElem.CompendiumElement = true;
        int forceInitUpdates = 1;
        if (Style == 2)
        {
            BG.enabled = false;
            //MyElem.ForceHideCount = true;
            transform.localScale = Vector3.one * 0.8f;
            transform.GetComponent<RectTransform>().pivot = Vector2.one * 0.5f;
            forceInitUpdates += 2;
        }
        for(int a = 0; a < forceInitUpdates; ++a)
            Update();
    }
    public void Update()
    {
        if (Style == 5)
            return;
        if (TypeID == -1 && gameObject.activeSelf)
            Destroy(gameObject);
        RectTransform hoverTransform = rectTransform;
        bool isAchieve = false;
        if (this is CompendiumAchievementElement achieve)
        {
            isAchieve = true;
            hoverTransform = achieve.CombinedRect;
        }
        bool isWithinMaskRange = (count.transform.position.y > Compendium.Instance.SortBar.position.y + Compendium.Instance.SortBar.sizeDelta.y * 0.5f * Compendium.Instance.SortBar.lossyScale.y);
        count.gameObject.SetActive((isAchieve ? Compendium.Instance.AchievementPage.ShowCounts : Compendium.Instance.EquipPage.ShowCounts) && !MyElem.DisplayOnly && (!IsLocked() || isAchieve) && Style <= 1 && !isWithinMaskRange);
        if (MyElem.ActiveEquipment != null)
        {
            if(isAchieve)
                count.text = "#" + GetCount().ToString();
            else
                count.text = GetCount().ToString();
            MyElem.UpdateActive(MyCanvas, out bool hovering, out bool clicked, hoverTransform);
            if (clicked)
            {
                if (!isAchieve)
                    Compendium.Instance.EquipPage.UpdateSelectedType(TypeID);
                else
                    Compendium.Instance.AchievementPage.UpdateSelectedType(TypeID);
            }
            if (hovering && Control.RightMouseClick)
            {
                if (!isAchieve)
                    Compendium.Instance.EquipPage.TierList.QueueRemoval = TypeID;
            }    
            if (Style <= 1)
            {
                Color target = Selected ? new Color(1, 1, .4f, 0.431372549f) : new Color(0, 0, 0, 0.431372549f);
                if (this is CompendiumAchievementElement achieve2 && achieve2.DescriptionImage != null)
                {
                    if(achieve2.MyUnlock.Unlocked && !Selected)
                    {
                        target = new Color(.1f, .7f, .1f, 0.431372549f);
                    }
                    achieve2.DescriptionImage.color = Color.Lerp(achieve2.DescriptionImage.color, target, 0.125f);
                    BG.color = Color.Lerp(BG.color, target, 0.125f);
                }
                else
                    BG.color = Color.Lerp(BG.color, target, 0.125f);

            }
            Selected = isAchieve ? TypeID == Compendium.Instance.AchievementPage.SelectedType : TypeID == Compendium.Instance.EquipPage.SelectedType;
            bool locked = isAchieve ? !IsLocked() : IsLocked();
            if (locked)
            {
                MyElem.UpdateColor(Color.black);
            }
            else
            {
                if (GrayOut)
                    MyElem.LerpColor(PowerUpUIElement.GrayColor, 0.7f);
                else
                    MyElem.SetColorToOriginal();
            }
        }
    }
    public override void SetHovering(bool canHover)
    {
        MyElem.DisplayOnly = !canHover;
    }
    public override bool IsLocked()
    {
        return !MyElem.Unlocked;
    }
    public override int GetRare(bool reverse = false)
    {
        return MyElem.ActiveEquipment.GetRarity();
    }
    public override int GetCount()
    {
        return MyElem.ActiveEquipment.TotalTimesUsed;
    }
    public override CompendiumElement Instantiate(TierList parent, TierCategory cat, Canvas canvas, int i, int position)
    {
        CompendiumEquipmentElement cpue = Instantiate(Prefab).GetComponent<CompendiumEquipmentElement>();
        parent.InsertIntoTransform(cat.Grid.transform, cpue, position);
        cpue.Style = 2;
        cpue.MyElem.DisplayOnly = true;
        cpue.SetGrayOut(true);
        cpue.Init(i, canvas);
        cpue.MyElem.HoverRadius = 0;
        return cpue;
    }
}
