using System;
using System.Collections.Generic;

public class Description
{
    public Description(object owner)
    {

    }
    public string NameText { get; set; }
    public string LoreText { get; set; }
    public string BriefDesc { get; set; }
    public string LongDesc { get; set; }
}
public class PowerDescription : Description
{
    private readonly string StartingText;
    private readonly Dictionary<Type, string> AltText = new();
    private readonly Dictionary<Type, string> ShortAltText = new();
    public string BlackMarketShort { get; set; }
    public string BlackMarketDesc { get; set; }
    public PowerDescription(PowerUp owner) : base(owner)
    {
        StartingText = "Power." + owner.GetType().FullName;
        NameText = Localization.Get($"{StartingText}.Title");
        BriefDesc = Localization.Get($"{StartingText}.Brief");
        LongDesc = Localization.Get($"{StartingText}.Description");
        LoreText = Localization.Get($"{StartingText}.Lore");
    }
    public PowerDescription WithAlt<T>(bool newLong = true, bool newBrief = false) where T : Equipment
    {
        Type t = typeof(T);
        ShortAltText[t] = newBrief ? Localization.Get($"{StartingText}.Brief{t.FullName}") : BriefDesc;
        AltText[t] = newLong ? Localization.Get($"{StartingText}.Description{t.FullName}") : LongDesc;
        return this;
    }
    public PowerDescription WithBlackMarketVariant(bool newLong = true, bool newBrief = false)
    {
        BlackMarketShort = newBrief ? Localization.Get($"{StartingText}.BriefBlackMarket") : BriefDesc;
        BlackMarketDesc = newLong ? Localization.Get($"{StartingText}.DescriptionBlackMarket") : LongDesc;
        return this;
    }
}