using System;
using System.Linq;
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
    public PowerUpUIElement AlternativeDisplayElement;
    public bool IsPowerUnlock;
    public override void Init(int i, Canvas canvas)
    {
        MyUnlock = UnlockCondition.Get(i);
        IsPowerUnlock = MyUnlock.AssociatedUnlocks.Count <= 0;
        if (IsPowerUnlock)
        {
            //Initialize a fake equipment in the background just for certain behavor stuff
            base.Init(Main.GlobalEquipData.Bubblemancer.GetComponent<Equipment>().IndexInAllEquipPool, canvas);
            InitPowerUpVersion(MyUnlock.AssociatedBlackMarketUnlocks.Count > 0 ? MyUnlock.AssociatedBlackMarketUnlocks.FirstOrDefault() : PowerUp.Get<Choice>());
            AlternativeDisplayElement.MyPower.ForceBlackMarket = true;
            AlternativeDisplayElement.OnUpdate();
            AlternativeDisplayElement.OnUpdate();
            AlternativeDisplayElement.MyPower.ForceBlackMarket = false;
            AlternativeDisplayElement.ForceHideCount = true;
        }
        else
        {
            base.Init(MyUnlock.FrontPageUnlock().IndexInAllEquipPool, canvas);
        }
        AlternativeDisplayElement.gameObject.SetActive(IsPowerUnlock);
        MyElem.Visual.SetActive(!IsPowerUnlock);
        MyElem.AchievementElement = true;
        TypeID = i;
        if (MyUnlock.Unlocked && !Selected && Style != 3 && Style != 5)
        {
            Color c = new(.1f, .7f, .1f, 0.431372549f);
            DescriptionImage.color = c;
            BG.color = c;
        }
        if (Style == 3)
            MyElem.DisplayOnly = true;
    }
    public void InitPowerUpVersion(PowerUp p)
    {
        AlternativeDisplayElement.SetPowerType(p.Type);
        AlternativeDisplayElement.SpecialLockedSprite = !MyUnlock.Unlocked;
        AlternativeDisplayElement.ForceUnhideElement = MyUnlock.Unlocked;
    }
    /// <summary>
    /// Currently unused, as this element does not have a tier list
    /// </summary>
    public override CompendiumElement Instantiate(TierList parent, TierCategory cat, Canvas canvas, int i, int position)
    {
        throw new NotImplementedException("Currently unused, as this element does not have a tier list");
    }
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
        if(Style != 3 && Style != 5)
            UpdateText();
        if(IsPowerUnlock)
        {
            if (AlternativeDisplayElement != null)
            {
                AlternativeDisplayElement.MyPower.ForceBlackMarket = true;
                AlternativeDisplayElement.transform.localScale = MyElem.transform.localScale;
                AlternativeDisplayElement.OnUpdate();
                AlternativeDisplayElement.MyPower.ForceBlackMarket = false;
            }
        }
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
