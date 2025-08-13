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
    private static readonly string[] Rares = new string[] { "#CFCFFF", "#C2FFAA", "#AAD3FE", "#D4AAFE", "#FCB934" };
    private static readonly string Yellow = "#FFED75";
    private static readonly string Gray = "#999999";
    private static readonly string TabForMoreDetail = " <size=24><color=#CB8A8A>(TAB for more detail)</color></size>";
    private static readonly int NormalTextSize = 28;
    private static readonly int GrayTextSize = 24;
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
            bool isOpening = third == '[' || third == '(';
            bool isEnding = last == ']' || last == ')';
            string start = string.Empty;
            string end = string.Empty;
            string contents = t;
            if (first2 == "G:")
                start = $"<size={GrayTextSize}><color={Gray}>";
            else if (first2 == "Y:")
                start = $"<size={NormalTextSize}><color={Yellow}>";
            if (isEnding && waitingForEnding && !isOpening) //If this string ends with a ']' and we have previously seen a '['
            {
                waitingForEnding = false;
                if (last != ')')
                    contents = t[..^1];
                end = "</color></size>";
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
                end = "</color></size>";
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
    public static string TextBoundedByRarityColor(int rare, string text)
    {
        return $"<color={Rares[rare]}>{text}</color>";
    }
    public string TextBoundedByRarityColor(string text)
    {
        return TextBoundedByRarityColor(Rarity, text);
    }
    public string GetName(bool noColor = false)
    {
        if (noColor)
            return Name;
        return TextBoundedByRarityColor(Name);
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