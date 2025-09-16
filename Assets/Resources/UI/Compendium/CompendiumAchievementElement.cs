using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CompendiumAchievementElement : CompendiumEquipmentElement
{
    public static new GameObject Prefab => Resources.Load<GameObject>("UI/Compendium/CompendiumAchievementElement");
    public UnlockCondition MyUnlock { get; private set; }
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
        return 1;
    }
    public override bool IsLocked()
    {
        return !MyUnlock.Completed;
    }
}
