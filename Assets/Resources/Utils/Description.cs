using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class DetailedDescription
{
    public static string BastardizeText(string s, char bChar)
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
    public int Rarity;
    public static readonly string[] Rares = new string[] { "#CFCFFF", "#C2FFAA", "#AAD3FE", "#D4AAFE", "#FCB934", "#FFAAAA" };
    public static readonly string Yellow = "#FFED75";
    public static readonly string Gray = "#999999";
    public static readonly string LesserGray = "#DDDDDD";
    private static readonly string TabForMoreDetail = " (TAB for more detail)".WithSizeAndColor(24, "#CB8A8A");
    private static readonly int NormalTextSize = 28;
    private static readonly int GrayTextSize = 24;
    public void WithoutSizeAugments()
    {
        WithSizeAugments = false;
    }
    private bool WithSizeAugments = true;
    public string ToRichText(string t)
    {
        string[] segments = t.Split(' ');
        string concat = string.Empty;
        int secondLast = segments.Length - 1;
        bool openBracket = false;
        for (int i = 0; i < segments.Length; ++i)
        {
            string segment = segments[i];
            concat += SegmentToRichRext(segment, ref openBracket);
            if (i != secondLast)
                concat += ' ';
        }
        return concat;
    }
    public string SegmentToRichRext(string t, ref bool waitingForEnding)
    {
        if (t.Length > 2)
        {
            string first2 = t[..2];
            char third = t[2];
            char last = t[^1];
            char last2 = t[^2];
            bool isOpening = third == '[' || third == '(';
            bool isEnding = last == ']' || last == ')';
            //bool commaEnding = last == ',' && (last2 == ']' || last2 == ')');
            //if (commaEnding)
            //    isEnding = true;
            string start = string.Empty;
            string end = string.Empty;
            string contents = t;
            if (first2 == "G:")
                start = WithSizeAugments ? $"<size={GrayTextSize}><color={Gray}>" : $"<color={Gray}>";
            else if (first2 == "Y:")
                start = WithSizeAugments ? $"<size={NormalTextSize}><color={Yellow}>" : $"<color={Yellow}>";
            if (isEnding && waitingForEnding && !isOpening) //If this string ends with a ']' and we have previously seen a '['
            {
                waitingForEnding = false;
                if (last != ')')
                    contents = t[..^1];
                end = WithSizeAugments ? "</color></size>" : "</color>";
            }
            else if (isOpening && !waitingForEnding && !isEnding) //If this string starts with a '[' and we have not seen a ']'
            {
                waitingForEnding = true;
                if (third != '(')
                    contents = t[3..];
                else
                    contents = t[2..];
            }
            else if (start != string.Empty) //there are no brackets, so we start and end at once
            {
                if (isOpening && isEnding && third != '(')
                {
                    contents = t[3..^1];
                }
                else
                    contents = t[2..];
                end = WithSizeAugments ? "</color></size>" : "</color>";
            }
            return $"{start}{contents}{end}";
        }
        return t;
    }
    public DetailedDescription(PowerUp p)
    {
        Name = p.GetType().FullName.ToSpacedString();
        Description = "N/A";
    }
    public DetailedDescription(int rare, string name)
    {
        Rarity = rare;
        Name = name;
        Description = "N/A";
    }
    public DetailedDescription WithDescription(string lines)
    {
        Description = lines;
        return this;
    }
    public DetailedDescription WithShortDescription(string lines)
    {
        ShortDescription = lines;
        return this;
    }
    public DetailedDescription WithDescriptionVariant<T>(string lines) where T : Equipment
    {
        AltDescriptions.Add(typeof(T), lines);
        return this;
    }
    public DetailedDescription WithName(string name)
    {
        Name = name;
        return this;
    }
    private readonly Dictionary<Type, string> AltDescriptions = new();
    private readonly Dictionary<Type, string> CompleteAltDescriptions = new();
    private string Name;
    private string Description;
    private string ShortDescription = null;
    private string CompleteDescription = string.Empty;
    private string CompleteShortDescription = string.Empty;
    public string FullDescription()
    {
        if (CompleteDescription == string.Empty)
            CompleteDescription = ToRichText(Description);
        if (Player.Instance != null)
        {
            Type[] equipTypes = new Type[] { 
                Player.Instance.Body.GetType(), 
                Player.Instance.Hat.GetType(),
                Player.Instance.Weapon.GetType(), 
                Player.Instance.Accessory.GetType() 
            };
            foreach(Type t in equipTypes)
            {
                if (AltDescriptions.TryGetValue(t, out string lines))
                {
                    if (!CompleteAltDescriptions.TryGetValue(t, out string lines2))
                    {
                        CompleteAltDescriptions[t] = ToRichText(lines);
                    }
                    else
                        return lines2;
                }
            }
        }
        return CompleteDescription;
    }
    public bool HasBriefDescription => ShortDescription != null;
    public string BriefDescription(bool withDetails = false)
    {
        if (!HasBriefDescription)
            return FullDescription();
        if (CompleteShortDescription == string.Empty)
            CompleteShortDescription = ToRichText(ShortDescription);
        return CompleteShortDescription + (withDetails ? TabForMoreDetail : string.Empty);
    }
    public static string TextBoundedByColor(string color, string text)
    {
        return $"<color={color}>{text}</color>";
    }
    public static string TextBoundedByRarityColor(int rare, string text, bool blackMarket)
    {
        return TextBoundedByColor(Rares[blackMarket ? 5 : rare], text);
    }
    public string TextBoundedByRarityColor(string text)
    {
        return TextBoundedByRarityColor(Rarity, text, false);
    }
    public string GetName(bool noColor = false, bool blackMarket = false)
    {
        if (noColor)
            return Name;
        return TextBoundedByRarityColor(Rarity, Name, blackMarket);
    }
    private bool hasLoaded = false;
    public Dictionary<Type, string> LoadAllAlternativeDescriptions()
    {
        if(hasLoaded)
            return CompleteAltDescriptions;
        foreach (Type t in AltDescriptions.Keys)
        {
            if (AltDescriptions.TryGetValue(t, out string lines) && !CompleteAltDescriptions.TryGetValue(t, out string lines2))
                CompleteAltDescriptions[t] = ToRichText(lines);
        }
        hasLoaded = true;
        return CompleteAltDescriptions;
    }
}