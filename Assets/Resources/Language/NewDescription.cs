using System;
using System.Collections.Generic;

public class Description
{
    public Description(object owner)
    {
        if(owner is PowerUp p)
        {
            string Name = p.GetType().FullName;
            NameText = Localization.Get($"{Name}.Title");
            Short = Localization.Get($"{Name}.Short");
            Desc = Localization.Get($"{Name}.Description");
            FlavorText = Localization.Get($"{Name}.Flavor");
        }
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

    public PowerDescription(object owner) : base(owner)
    {
    }
}