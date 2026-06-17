using System;
using System.Collections.Generic;

public class Description
{
    public Description(object owner)
    {

    }
    public string NameText { get; protected set; }
    public string LoreText { get; protected set; }
    public string Brief { get; protected set; }
    public string Full { get; protected set; }
}
public class PowerDescription : Description
{
    private readonly string StartingText;
    public readonly Dictionary<Type, string> FullAlts = new();
    public readonly Dictionary<Type, string> ShortAlts = new();
    public string BlackMarketShort { get; protected set; }
    public string BlackMarketFull { get; protected set; }
    public bool HasBlackMarketVariants { get; protected set; }
    public PowerDescription(PowerUp owner, bool briefIsLong = false) : base(owner)
    {
        StartingText = "Power." + owner.GetType().FullName;
        NameText = Localization.Get($"{StartingText}.Title");
        Full = Localization.Get($"{StartingText}.Description");
        Brief = briefIsLong ? Full : Localization.Get($"{StartingText}.Brief");
        LoreText = Localization.Get($"{StartingText}.Lore");

        BlackMarketShort = Brief;
        BlackMarketFull = Full;
        HasBlackMarketVariants = false;
    }
    public PowerDescription WithAlt<T>(bool newLong = true, bool newBrief = false) where T : Equipment
    {
        Type t = typeof(T);
        ShortAlts[t] = newBrief ? Localization.Get($"{StartingText}.{t.FullName}Brief") : Brief;
        FullAlts[t] = newLong ? Localization.Get($"{StartingText}.{t.FullName}Description") : Full;
        return this;
    }
    public PowerDescription WithBlackMarketVariant(bool newLong = true, bool newBrief = false)
    {
        BlackMarketShort = newBrief ? Localization.Get($"{StartingText}.BlackMarketBrief") : Brief;
        BlackMarketFull = newLong ? Localization.Get($"{StartingText}.BlackMarketDescription") : Full;
        HasBlackMarketVariants = newLong || newBrief;
        return this;
    }
    public string GetDescription(bool brief)
    {
        if (Player.Instance != null) //Only need to check instanced player, as that's the only player that will see the description (future: multiplayer, other players will be Player.Instance on their clients)
        {
            Dictionary<Type, string> dict = brief ? ShortAlts : FullAlts;
            if(dict.Count > 0) //Look for alternate descriptions based on equipment
            {
                List<Type> equipTypes = new();
                for (int i = 0; i < 4; ++i)
                {
                    Type t = Player.Instance.Equips[i].GetType();
                    equipTypes.Add(t);

                    //In the case of inherited equipment, such as Slots/Dragonslots, they should share the descriptions from Slots
                    Type b = t.BaseType;
                    if (b != typeof(Equipment))
                        equipTypes.Add(b);
                }
                foreach (Type t in equipTypes)
                    if (dict.TryGetValue(t, out string lines))
                        return lines;
            }
        }
        return brief ? Brief : Full;
        //return CompleteShortDescription + (withDetails ? TabForMoreDetail : string.Empty);
    }
}