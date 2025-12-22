using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CompendiumAchievementElement : CompendiumEquipmentElement
{
    public static new GameObject Prefab => Resources.Load<GameObject>("UI/Compendium/CompendiumAchievementElement");
    public UnlockCondition MyUnlock { get; private set; }
    public RectTransform DescriptionArea, CombinedRect;
    public Image DescriptionImage;
    public TextMeshProUGUI NameText, DescriptionText;
    public override void Init(int i, Canvas canvas)
    {
        MyUnlock = UnlockCondition.Get(i);
        base.Init(MyUnlock.AssociatedUnlocks.Count > 0 ? MyUnlock.FrontPageUnlock().IndexInAllEquipPool : Main.GlobalEquipData.Bubblemancer.GetComponent<Equipment>().IndexInAllEquipPool, canvas);
        if(MyUnlock.AssociatedUnlocks.Count <= 0)
        {
            MyElem.ActiveEquipment.spriteRender.sprite = Resources.Load<Sprite>("UI/StarAch");
        }
        MyElem.AchievementElement = true;
        TypeID = i;
        if (MyUnlock.Unlocked && !Selected && Style != 3)
        {
            Color c = new Color(.1f, .7f, .1f, 0.431372549f);
            DescriptionImage.color = c;
            BG.color =c;
        }
        if (Style == 3)
            MyElem.DisplayOnly = true;
    }
    ///// <summary>
    ///// Currently unused, as this element does not have a 
    ///// </summary>
    //public override CompendiumElement Instantiate(TierList parent, TierCategory cat, Canvas canvas, int i, int position)
    //{
        //CompendiumAchievementElement cpue = Instantiate(Prefab).GetComponent<CompendiumAchievementElement>();
        //parent.InsertIntoTransform(cat.Grid.transform, cpue, position);
        //cpue.Style = 2;
        //cpue.MyElem.DisplayOnly = true;
        //cpue.SetGrayOut(true);
        //cpue.Init(i, canvas);
        //cpue.MyElem.HoverRadius = 0;
        //return cpue;
    //}
    public override int GetRare(bool reverse = false)
    {
        return MyUnlock.Rarity;
    }
    public override int GetCount()
    {
        return TypeID + 1;
    }
    public override bool IsLocked()
    {
        return MyUnlock.Unlocked; //In this case, more like it is completed
    }
    public new void Update()
    {
        base.Update();
        if(Style != 3)
            UpdateText();
    }
    public void UpdateText()
    {
        float x = Compendium.Instance.AchievementPage.PowerUpLayoutGroup.spacing.x - Compendium.Instance.AchievementPage.PowerUpLayoutGroup.padding.right;
        DescriptionArea.sizeDelta = new Vector2(x, DescriptionArea.sizeDelta.y);
        DescriptionArea.gameObject.SetActive(Compendium.Instance.AchievementPage.WideDisplayStyle);
        NameText.text = MyUnlock.GetName();
        DescriptionText.text = /*MyUnlock.Completed ? "Completed!" :*/ MyUnlock.GetDescription();
        CombinedRect.sizeDelta = new Vector2(DescriptionArea.sizeDelta.x, 0);
    }
}
