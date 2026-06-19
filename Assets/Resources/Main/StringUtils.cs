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
    public static string WithRarityColor(this string text, int rare, bool blackMarket)
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
    public static string WithSize(this string s, int size)
    {
        return $"<size={size}>{s}</size>";
    }
    public static string WithColor(this string s, string color)
    {
        return $"<color={color}>{s}</color>";
    }
    public static string Bastardize(this string s, char bChar)
    {
        string ret = string.Empty;
        bool open = false;
        for (int i = 0; i < s.Length; ++i)
        {
            char c = s[i];
            if (!open)
            {
                if (c == '<')
                {
                    open = true;
                }
                else if (char.IsLetterOrDigit(c))
                {
                    c = bChar;
                }
            }
            else if (c == '>')
                open = false;
            ret += c;
        }
        return ret;
    }
}
