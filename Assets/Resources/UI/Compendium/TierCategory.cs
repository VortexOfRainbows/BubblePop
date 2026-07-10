using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TierCategory : MonoBehaviour
{
    public static readonly float DefaultDist = 133.333333f;
    public GridLayoutGroup Grid;
    public RectTransform RectTransform;
    public Image TierSlot;
    public Image TierRect;
    public TextMeshProUGUI Text;
    public void CalculateSizeNeededToHousePowerups(TierList list)
    {
        int c = Grid.transform.childCount;
        if (c <= 0)
        {
            list.TotalDistanceCovered += DefaultDist;
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, DefaultDist);
            return;
        }
        Transform lastElement = Grid.transform.GetChild(c - 1);
        float paddingBonus = lastElement.GetComponent<RectTransform>().rect.height / 2f;
        float dist = -lastElement.localPosition.y + paddingBonus + (DefaultDist - Grid.cellSize.y) / 2f;
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, Mathf.Max(DefaultDist, dist));
        list.TotalDistanceCovered += RectTransform.sizeDelta.y;
    }
}
