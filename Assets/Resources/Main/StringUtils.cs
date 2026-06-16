using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringUtils
{
    public static string ToSpacedString(this string str)
    {
        for (int i = str.Length - 1; i > 0; --i)
        {
            if (char.IsUpper(str[i]))
                str = str.Insert(i, " ");
        }
        return str;
    }
    public static string TextBoundedByRarityColor(int rare, string text, bool blackMarket)
    {
        if (rare == -1)
            return text.WithColor(ColorHelper.LesserGrayHex);
        else if (rare == -2)
            return text.WithColor(ColorHelper.GrayHex);
        return text.WithColor(ColorHelper.RarityColorHex[blackMarket ? 5 : rare]);
    }
    public static string WithSizeAndColor(this string s, int size, string color)
    {
        return $"<size={size}><color={color}>{s}</color></size>";
    }
    public static string WithColor(this string s, string color)
    {
        return $"<color={color}>{s}</color>";
    }
}
