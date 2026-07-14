using System.Collections.Generic;
using System.Linq;

public static partial class LocalizationBuilder
{
    private const int NormalTextSize = 28;
    private const int GrayTextSize = 24;
    public static Dictionary<string, string> PreProcessLocalization(Dictionary<string, string> translation)
    {
        //Doesn't really matter how optimized this is, as it will only be run once to preprocess all localizations. Players will not even run these (only ran on developer end)
        Dictionary<string, string> completeDictionary = new();
        foreach (string key in translation.Keys)
        {
            string value = translation[key];
            if (key.EndsWith("Lore"))
            {
                if (value.EndsWith("Lore")) //This usually means the lore entry is unfinished (we haven't written an entry yet)
                    value = (ExpandedTranslation["Common"] as Dictionary<string, object>)["LoreMissing"] as string;
                else //Preprocess the lore if it is finished
                {
                    var segments = value.Split("\n", System.StringSplitOptions.RemoveEmptyEntries);
                    string finalValue = string.Empty;
                    for (int i = 0; i < segments.Length; ++i)
                    {
                        var segment = segments[i];
                        finalValue += LoreSegmentToRichText(segment);
                        if(i < segments.Length - 1)
                            finalValue += "\n";
                    }
                    value = finalValue;
                }
            }
            else if (key.EndsWith("Description") || key.EndsWith("Secret"))
            {
                bool augmentSize = key.StartsWith("Power");
                value = ToRichText(value, augmentSize);
            }
            else if(key.EndsWith("TabForMoreInformation"))
                value = value.WithSizeAndColor(24, "#CB8A8A");
            else if(key.StartsWith("Equip"))
            {
                var segments = key.Split('.');
                if(segments.Last().StartsWith("Abl"))
                {
                    value = ToRichText(value, false);
                }
            } 
            completeDictionary[key] = value;
        }
        return completeDictionary;
    }
    private static string ToRichText(string t, bool withSizeModifiers)
    {
        string[] segments = t.Split(' ');
        string concat = string.Empty;
        int secondLast = segments.Length - 1;
        bool openBracket = false;
        for (int i = 0; i < segments.Length; ++i)
        {
            string segment = segments[i];
            concat += SegmentToRichRext(segment, withSizeModifiers, ref openBracket);
            if (i != secondLast)
                concat += ' ';
        }
        return concat;
    }
    private static string SegmentToRichRext(string t, bool withSizeModifiers, ref bool waitingForEnding)
    {
        string start2 = "";
        if (t.Length > 0 && t[0] == '\n')
        {
            start2 += "\n";
            t = t[1..];
        }
        if (t.Length > 2)
        {
            string first2 = t[..2];
            char third = t[2];
            char last = t[^1];
            bool isOpening = third == '[' || third == '(';
            bool isEnding = last == ']' || last == ')';
            //bool commaEnding = last == ',' && (last2 == ']' || last2 == ')');
            //if (commaEnding)
            //    isEnding = true;
            string start = string.Empty;
            string end = string.Empty;
            string contents = t;
            if (first2 == "G:")
                start = withSizeModifiers ? $"<size={GrayTextSize}><color={ColorHelper.GrayHex}>" : $"<color={ColorHelper.GrayHex}>";
            else if (first2 == "Y:")
                start = withSizeModifiers ? $"<size={NormalTextSize}><color={ColorHelper.YellowHex}>" : $"<color={ColorHelper.YellowHex}>";
            else if (first2 == "R:")
                start = withSizeModifiers ? $"<size={NormalTextSize}><color={ColorHelper.RarityColorHex[5]}>" : $"<color={ColorHelper.RarityColorHex[5]}>";
            if (isEnding && waitingForEnding && !isOpening) //If this string ends with a ']' and we have previously seen a '['
            {
                waitingForEnding = false;
                if (last != ')')
                    contents = t[..^1];
                end = withSizeModifiers ? "</color></size>" : "</color>";
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
                end = withSizeModifiers ? "</color></size>" : "</color>";
            }
            return start2 + $"{start}{contents}{end}";
        }
        return start2 + t;
    }
    private static string LoreSegmentToRichText(string segment)
    {
        var segments = segment.Split(": ", 2);
        if(segments.Length < 2)
            return segment;
        string first = segments[0];
        string second = segments[1];
        string hex = GetCharacterLoreColor(first);
        return second.WithColor(hex);
    }
    private static string GetCharacterLoreColor(string character)
    {
        if (character.StartsWith("B")) //B for Bubblemancer
            return "#DBF7FA"; //Bluish Color
        else if (character.StartsWith("T")) //T for Thought Bubble
            return "#E3B3D8"; //Pinkish Color
        else if (character.StartsWith("G")) //G for Gachapon
            return "#F3ECB7"; //Yellowish Color
        else if (character.StartsWith("F")) //F for Fizzy
            return "#D8454D"; //Red
        else if (character.StartsWith("K")) //K for King Oil
            return "#8A84AA"; //Purple-ish Color
        else if (character.StartsWith("U")) //U for Unknown
            return ColorHelper.GrayHex;
        return "#FFFFFF";
    }
}