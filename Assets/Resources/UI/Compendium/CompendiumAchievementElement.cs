using TMPro;
using UnityEngine;

public class CompendiumAchievementElement : CompendiumEquipmentElement
{
    public static new GameObject Prefab => Resources.Load<GameObject>("UI/Compendium/CompendiumAchievementElement");
    public UnlockCondition MyUnlock { get; private set; }
    public RectTransform DescriptionArea;
    public TextMeshProUGUI NameText, DescriptionText;
    public override void Init(int i, Canvas canvas)
    {
        MyUnlock = UnlockCondition.Get(i);
        base.Init(MyUnlock.AssociatedUnlocks.Count > 0 ? MyUnlock.FrontPageUnlock().IndexInAllEquipPool : 0, canvas);
        MyElem.AchievementElement = true;
        TypeID = i;
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
        return MyUnlock.Completed; //In this case, more like it is completed
    }
    public new void Update()
    {
        base.Update();
        float x = Compendium.Instance.AchievementPage.PowerUpLayoutGroup.spacing.x - Compendium.Instance.AchievementPage.PowerUpLayoutGroup.padding.right;
        DescriptionArea.sizeDelta = new Vector2(x, DescriptionArea.sizeDelta.y);
        DescriptionArea.gameObject.SetActive(Compendium.Instance.AchievementPage.WideDisplayStyle);
        UpdateText();
    }
    public void UpdateText()
    {
        NameText.text = MyUnlock.GetName();
        DescriptionText.text = /*MyUnlock.Completed ? "Completed!" :*/ MyUnlock.GetDescription();
    }
}
