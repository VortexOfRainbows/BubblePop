using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompendiumAchievementElement : CompendiumEquipmentElement
{
    public static new GameObject Prefab => Resources.Load<GameObject>("UI/Compendium/CompendiumAchievementElement");
    public override CompendiumElement Instantiate(TierList parent, TierCategory cat, Canvas canvas, int i, int position)
    {
        CompendiumAchievementElement cpue = Instantiate(Prefab).GetComponent<CompendiumAchievementElement>();
        parent.InsertIntoTransform(cat.Grid.transform, cpue, position);
        cpue.Style = 2;
        cpue.MyElem.DisplayOnly = true;
        cpue.SetGrayOut(true);
        cpue.Init(i, canvas);
        cpue.MyElem.HoverRadius = 0;
        return cpue;
    }
}
