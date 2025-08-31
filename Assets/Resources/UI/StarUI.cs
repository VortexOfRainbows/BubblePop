using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarUI : MonoBehaviour
{
    private List<Color> defaultColors = null;
    private List<Image> childImages = null;
    private void Init()
    {
        defaultColors = new List<Color>();
        childImages = new List<Image>();
        GetComponentsInChildren(true, childImages);
        foreach (Image i in childImages)
        {
            defaultColors.Add(i.color);
        }
    }
    public void SetAlpha(float a)
    {
        if (defaultColors == null)
            Init();
        for (int i = 0; i < childImages.Count; ++i)
        {
            childImages[i].color = defaultColors[i].WithAlphaMultiplied(a);
        }
    }
}
