using System;
using System.Collections.Generic;

public class Description
{
    public Description(object owner)
    {
        if(owner is PowerUp p)
        {
            string Name = p.GetType().FullName;
            NameText = Localization.Get(Name);
            FlavorText = "N/A";
            DescriptionText = "N/A";
            ExtendedText = "N/A";
        }
    }
    public string NameText { get; set; }
    public string FlavorText { get; set; }
    public string DescriptionText { get; set; }
    public string ExtendedText { get; set; }
}
public class PowerDescription : Description
{
    private readonly Dictionary<Type, string> AltText = new();
    private readonly Dictionary<Type, string> ShortAltText = new();

    public PowerDescription(object owner) : base(owner)
    {
    }
}