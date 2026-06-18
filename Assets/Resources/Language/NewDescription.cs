using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Description
{
    protected string StartingText { get; set; }
    public Description(object owner)
    {

    }
    public string Name { get; protected set; }
    public string Lore { get; protected set; }
    public string Full { get; protected set; }
}
public class PowerDescription : Description
{
    public readonly Dictionary<Type, string> FullAlts = new();
    public readonly Dictionary<Type, string> ShortAlts = new();
    public string Brief { get; protected set; }
    public string BlackMarketShort { get; protected set; }
    public string BlackMarketFull { get; protected set; }
    public bool HasBlackMarketVariants { get; protected set; }
    public PowerDescription(PowerUp owner, bool briefIsLong = false) : base(owner)
    {
        StartingText = "Power." + owner.GetType().FullName;
        Name = Localization.Get($"{StartingText}.Title");
        Full = Localization.Get($"{StartingText}.Description");
        Brief = briefIsLong ? Full : Localization.Get($"{StartingText}.Brief");
        Lore = Localization.Get($"{StartingText}.Lore");

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
public class EquipDescription : Description
{
    public List<Ability> Abilities { get; private set; } = new();
    public EquipDescription(Equipment owner) : base(owner)
    {
        StartingText = "Equip." + owner.GetType().FullName;
        Name = Localization.Get($"{StartingText}.Title");
        Full = Localization.Get($"{StartingText}.Description");
        Lore = Localization.Get($"{StartingText}.Lore");
    }
    public EquipDescription RequestAbilitySlots(params int[] types)
    {
        for(int i = 0; i < types.Length; ++i)
            Abilities.Add(new Ability(types[i], Localization.Get($"{StartingText}.Abl{i + 1}")));
        return this;
    }
}
public class UnlockDescription : Description
{
    public UnlockDescription(UnlockCondition owner) : base(owner)
    {
        StartingText = "Unlock." + owner.GetType().FullName;
        Name = Localization.Get($"{StartingText}.Title");
        Full = Localization.Get($"{StartingText}.Description");
        Lore = string.Empty; //Unlocks don't have lore
    }
}
public class EnemyDescription : Description
{
    public EnemyDescription(Enemy owner) : base(owner)
    {
        StartingText = "Enemy." + owner.GetType().FullName;
        Name = Localization.Get($"{StartingText}.Title");
        Full = Localization.Get($"{StartingText}.Description");
        Lore = Localization.Get($"{StartingText}.Lore");
    }
}