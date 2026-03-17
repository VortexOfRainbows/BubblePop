using UnityEngine;
public class BonusFizz : Shotgun
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        base.InitializeDescription(ref description);
        description.WithName("Bonus Fizz");
    }
    public override Sprite GetTexture()
    {
        var s = Resources.Load<Sprite>($"PowerUps/Shotgun");
        return s != null ? s : Main.TextureAssets.PowerUpPlaceholder;
    }
}
public class BottleBlast : BubbleBlast
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        base.InitializeDescription(ref description);
        description.WithName("Bottle Blast");
        description.WithDescription("Your Y:[secondary attack] releases Y:4 G:(+4 per stack) additional bubbles on impact");
        description.WithShortDescription("Your secondary attack releases extra bubbles");
}
    public override Sprite GetTexture()
    {
        var s = Resources.Load<Sprite>($"PowerUps/BubbleBlast");
        return s != null ? s : Main.TextureAssets.PowerUpPlaceholder;
    }
}
public class BottleFlip : PowerUp
{
    public override void Init()
    {
        Weighting = Rare;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bottle Flip");
        description.WithDescription("Your Y:[secondary attack] bounces and explodes Y:1 G:(+1 per stack) additional times");
        description.WithShortDescription("Your secondary attack bounces and explodes an additional time");
    }
    public override void HeldEffect(Player p)
    {
        p.BottleFlip += Stack;
    }
    public override Sprite GetTexture()
    {
        return Main.TextureAssets.PowerUpPlaceholder;
    }
}
