using UnityEngine;

public static class ColorHelper
{
    private static Color New255(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }
    public static readonly Color[] RarityColors = { New255(207, 207, 255), New255(194, 255, 170), New255(170, 211, 254), New255(212, 170, 254), New255(252, 185, 52), New255(255, 170, 170) };
    public static readonly Color UISelectColor = Color.yellow;
    public static readonly Color UIDefaultColor = Color.white;
    public static readonly Color TokenColor = New255(176, 147, 58);
}
