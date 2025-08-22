using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardScrollArea : MonoBehaviour
{
    public TextMeshProUGUI TextBox;
    public RectTransform ContentRect;
    public void UpdateSizing()
    {
        float defaultHeight = 142;
        Vector2 textBoxSize = TextBox.GetRenderedValues();
        ContentRect.sizeDelta = new Vector2(ContentRect.sizeDelta.x, Mathf.Max(defaultHeight, textBoxSize.y + 14));
    }
    public void SetText(string t)
    {
        TextBox.text = t;
    }
}
