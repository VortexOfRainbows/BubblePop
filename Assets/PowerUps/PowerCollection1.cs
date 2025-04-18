using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Choice : PowerUp
{
    protected override string Name() => "Choice";
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
        return Player.Instance != null && Player.Instance.Weapon != null ? Player.Instance.Weapon.spriteRender.sprite : null;
    }
    public override Sprite GetAdornment()
    {
        return Resources.Load<Sprite>("PowerUps/WeaponUpgrade");
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
        Player.Instance.Weapon.ModifyUIOffsets(true, ref offset, ref rot, ref scale);
        inner.transform.localPosition = offset * secondaryScalar * finalScaler * scale;
        inner.transform.eulerAngles = new Vector3(0, 0, rot);
        inner.transform.localScale = Player.Instance.Weapon.transform.localScale * scale * finalScaler;
    }
    public override void HeldEffect(Player p)
    {
        p.AttackSpeedModifier += Stack * 0.1f;
    }
}
public class Overclock : PowerUp
{
    public override void Init()
    {
        Weighting = 0.75f;
    }
    protected override string Name() => "Overclock";
    protected override string Description() => "Reduces ability cooldown";
    public override void HeldEffect(Player p)
    {
        p.AbilityRecoverySpeed += 0.2f * Stack;
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
    protected override string Description() => "Adds damaging stars to your ability";
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
        return Resources.Load<Sprite>("PowerUps/DurationUpgrade");
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
        Weighting = 0.08f;
    }
    protected override string Name() => "Bubble Birb";
    protected override string Description() => "Resurrect in a dance of flames after death";
    public override void HeldEffect(Player p)
    {
        p.BonusPhoenixLives += Stack; 
    }
    public override void OnPickup(int count)
    {
        Player.Instance.PickedUpPhoenixLivesThisRound += count;
    }
}
public class BubbleTrail : PowerUp
{
    public override void Init()
    {
        Weighting = 0.7f;
    }
    protected override string Name() => "Bubble Trail";
    protected override string Description() => "Periodically release bubbles behind you";
    public override void HeldEffect(Player p)
    {
        p.BubbleTrail += Stack;
    }
}
public class Coalescence : PowerUp
{
    public override void Init()
    {
        Weighting = 0.3f;
    }
    protected override string Name() => "Coalescence";
    protected override string Description() => "Secondary attack may be charged an additional time\nSlightly increases secondary attack speed";
    public override void HeldEffect(Player p)
    {
        p.Coalescence += Stack;
        p.SecondaryAttackSpeedModifier += 0.05f * Stack;
    }
}
public class LuckyStar : PowerUp
{
    public override void Init()
    {
        Weighting = 0.3f;
    }
    protected override string Name() => "Lucky Star";
    protected override string Description() => "Enemies killed by stars have an additional chance to drop power ups";
    public override void HeldEffect(Player p)
    {
        p.LuckyStar += Stack;
    }
}
public class TrailOfThoughts : PowerUp
{
    public override void Init()
    {
        Weighting = 0.7f;
    }
    protected override string Name() => "Trail of Thoughts";
    protected override string Description() => "Increases the maximum length of your thought trail";
    public override void HeldEffect(Player p)
    {
        p.TrailOfThoughts += Stack;
    }
}
public class Magnet : PowerUp
{
    public override void Init()
    {
        Weighting = 1f;
    }
    protected override string Name() => "Magnet";
    protected override string Description() => "Extends the distance coins are collected from";
    public override void HeldEffect(Player p)
    {
        p.Magnet += Stack;
    }
}
public class SpearOfLight : PowerUp
{
    public override void Init()
    {
        Weighting = 0.27f;
    }
    protected override string Name() => "Spear of Light";
    protected override string Description() => "Periodically fire beams of light at nearby enemies";
    public override void HeldEffect(Player p)
    {
        p.LightSpear += Stack;
    }
}
public class NeuronActivation : PowerUp
{
    public override void Init()
    {
        Weighting = 0.08f;
    }
    protected override string Name() => "Chain Reaction";
    protected override string Description() => "Enemies fire beams of light at other enemies when struck by light";
    public override void HeldEffect(Player p)
    {
        p.LightChainReact += Stack;
    }
}