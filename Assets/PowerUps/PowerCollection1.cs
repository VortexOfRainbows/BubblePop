using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Choice : PowerUp
{
    protected override string Name() => "???";
    protected override string Description() => "Pick which power you want from a given selection";
    public override void HeldEffect(Player p)
    {
        if (Stack > 0 && !PowerUp.PickingPowerUps)
        {
            p.RemovePower(Type);
            PowerUp.TurnOnPowerUpSelectors();
        }
    }
}
public class WeaponUpgrade : PowerUp
{
    public override void Init()
    {
        Weighting = 0.75f;
    }
    protected override string Name() => "Haste";
    protected override string Description() => "Increases weapon attack speed";
    public override Sprite GetTexture()
    {
        return Player.Instance != null && Player.Instance.Wand != null ? Player.Instance.Wand.spriteRender.sprite : null;
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("WeaponUpgrade");
    }
    public override void AliveUpdate(GameObject inner, GameObject outer, bool UI = false)
    {
        sprite = GetTexture();
        //if (outer.TryGetComponent(out SpriteRenderer rend))
        //{
        //    rend.color = new Color(.49f, .82f, .95f, 0.8f);
        //}
        //if (outer.TryGetComponent(out Image image))
        //{
        //    image.color = new Color(.49f, .82f, .95f, 0.8f);
        //}
        Vector2 offset = Vector2.zero;
        float rot = 0f;
        float scale = 1f;
        float secondaryScalar = 1;
        float finalScaler = 0.315f;
        if (UI)
        {
            secondaryScalar = 100;
            finalScaler = 0.315f;
        }
        Player.Instance.Wand.ModifyUIOffsets(ref offset, ref rot, ref scale);
        inner.transform.localPosition = offset * secondaryScalar * finalScaler;
        inner.transform.eulerAngles = new Vector3(0, 0, rot);
        inner.transform.localScale = Player.Instance.Wand.transform.localScale * scale * finalScaler;
    }
    public override void HeldEffect(Player p)
    {
        p.AttackSpeedModifier += Stack * 0.1f;
    }
}
public class ChargeShot : PowerUp
{
    protected override string Name() => "Charge Shot";
    protected override string Description() => "Increases the size and damage of your charge attacks";
    public override void HeldEffect(Player p)
    {
        p.ChargeShotDamage += Stack;
    }
}
public class Shotgun : PowerUp
{
    protected override string Name() => "Shotgun";
    protected override string Description() => "Increases the amount of bubbles shot by your primary weapon";
    public override void HeldEffect(Player p)
    {
        p.ShotgunPower += Stack;
    }
}
public class Dash : PowerUp
{
    public override void Init()
    {
        Weighting = 0.8f;
    }
    protected override string Name() => "Sparkle Sparkle";
    protected override string Description() => "Scatter stars around you while dashing";
    public override void HeldEffect(Player p)
    {
        p.DashSparkle += Stack;
    }
}
public class ShotSpeed : PowerUp
{
    protected override string Name() => "Bubble Propulsion";
    protected override string Description() => "Blown bubbles travel further and faster";
    public override void HeldEffect(Player p)
    {
        p.FasterBulletSpeed += Stack;
    }
}
public class Starbarbs : PowerUp
{
    public override void Init()
    {
        Weighting = 0.3f;
    }
    protected override string Name() => "Starbarbs";
    protected override string Description() => "Enemies killed by stars explode into stars";
    public override void HeldEffect(Player p)
    {
        p.Starbarbs += Stack;
    }
}
public class SoapySoap : PowerUp
{
    public override void Init()
    {
        Weighting = 0.3f;
    }
    protected override string Name() => "Soapy Soap";
    protected override string Description() => "Charge attacks leave behind a trail of bubbles";
    public override void HeldEffect(Player p)
    {
        p.SoapySoap += Stack;
    }
}
public class BubbleBlast : PowerUp
{
    public override void Init()
    {
        Weighting = 0.75f;
    }
    protected override string Name() => "Bubble Blast";
    protected override string Description() => "Charge attacks release bubbles upon expiring";
    public override void HeldEffect(Player p)
    {
        p.BubbleBlast += Stack;
    }
}
public class Starshot : PowerUp
{
    public override void Init()
    {
        Weighting = 0.08f;
    }
    protected override string Name() => "Starshot";
    protected override string Description() => "Chance for stars to be fired alongside shotgun bubbles" +
        "\nIncreases the amount of bubbles shot by your primary weapon";
    public override void HeldEffect(Player p)
    {
        p.Starshot += Stack;
        p.ShotgunPower += Stack;
    }
}
public class BinaryStars : PowerUp
{
    public override void Init()
    {
        Weighting = 0.27f;
    }
    protected override string Name() => "Binary Stars";
    protected override string Description() => "Periodically release 2 stars";
    public override void HeldEffect(Player p)
    {
        p.BinaryStars += Stack;
    }
}
public class EternalBubbles : PowerUp
{
    public override void Init()
    {
        Weighting = 0.9f;
    }
    protected override string Name() => "Eternal Bubbles";
    protected override string Description() => "Increases lifespan of all small bubbles";
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("DurationUpgrade");
    }
    public override void HeldEffect(Player p)
    {
        p.EternalBubbles += Stack;
    }
}
public class BubbleBirb : PowerUp
{
    public override void Init()
    {
        Weighting = 1f;
    }
    protected override string Name() => "Bubble Birb";
    protected override string Description() => "Resurrect from a bath of flames after death";
    public override void HeldEffect(Player p)
    {

    }
}

