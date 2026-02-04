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
    public static readonly Color UIRedColor = New255(255, 50, 50);
    public static readonly Color UIGreyColor = new(1, 1, 1, 0.4f);
    public static readonly Color TokenColor = New255(176, 147, 58);
    public static readonly Color SentinelGreen = new(0.18f, 1.0f, .55f);
    public static readonly Color SentinelBlue = new(0.26f, 0.95f, 1.0f);
    public static readonly Color CommandInfector = new(1, .1f, .1f);
    public static Color SentinelColorsLerp(float t)
    {
        return Color.Lerp(SentinelGreen, SentinelBlue, t);
    }
}
