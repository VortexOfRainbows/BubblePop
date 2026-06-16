using System;
using System.Collections.Generic;

public class Description
{
    public Description(object owner)
    {

    }
    public string NameText { get; set; }
    public string FlavorText { get; set; }
    public string Short { get; set; }
    public string Desc { get; set; }
}
public class PowerDescription : Description
{
    private readonly Dictionary<Type, string> AltText = new();
    private readonly Dictionary<Type, string> ShortAltText = new();
    public PowerDescription(PowerUp owner) : base(owner)
    {
        string Name = owner.GetType().FullName;
        NameText = Localization.Get($"Power.{Name}.Title");
        Short = Localization.Get($"Power.{Name}.Short");
        Desc = Localization.Get($"Power.{Name}.Description");
        FlavorText = Localization.Get($"Power.{Name}.Flavor");
    }
}