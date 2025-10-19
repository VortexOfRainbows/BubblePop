using System.Collections.Generic;
using UnityEngine;
public static class SoundID
{
    public static SoundClip BubblePop = new("BubblePop/", 
        "BubblePop1", "BubblePop2", "BubblePop3", "BubblePop4", "BubblePop5", "BubblePop6", "BubblePop7", "BubblePop8");

    public static SoundClip Dash = new("Player/",
        "Dash1", "Dash2", "Dash3", "Dash4");
    public static SoundClip Death = new("Player/",
        "BubbleDeath1", "BubbleDeath2");

    public static SoundClip DuckDeath = new("NPCs/Duck/",
        "DuckDeath");
    public static SoundClip DuckNoise = new("NPCs/Duck/",
        "DuckNoise");
    public static SoundClip LenardNoise = new("NPCs/Duck/",
        "BigDuckSound1", "BigDuckSound2");
    public static SoundClip LenardLaser = new("NPCs/Duck/",
        "LaserShot", "LongLaserBlast");

    public static SoundClip FlamingoNoise = new("NPCs/Flamingo/",
        "Honk1", "Honk2", "Honk3");
    public static SoundClip FlamingoShot = new("NPCs/Flamingo/",
        "Shot1", "Shot2");

    public static SoundClip SoapDie = new("NPCs/Soap/",
        "Break1", "Break2", "Break3");
    public static SoundClip SoapSlide = new("NPCs/Soap/",
        "Slide1", "Slide2", "Slide3", "Slide4");

    public static SoundClip BathBombSizzle = new("BathBomb/",
        "Sizzle");
    public static SoundClip BathBombBurst = new("BathBomb/",
        "Break");
    public static SoundClip BathBombSplash = new("BathBomb/",
        "WaterSplash");

    public static SoundClip ChargePoint = new("Player/",
        "ChargePoint1", "ChargePoint2", "ChargePoint3");
    public static SoundClip ChargeWindup = new("Player/",
        "ChargeWindup");
    public static SoundClip ShootBubbles = new("Player/",
        "Shoot");

    public static SoundClip PickupPower = new("Powerup/",
        "Pickup");
    public static SoundClip StarbarbImpact = new("Projectile/",
        "StarbarbImpact1", "StarbarbImpact2", "StarbarbImpact3");

    public static SoundClip CoinPickup = new("Coin/",
        "CoinTier1", "CoinTier2", "CoinTier3");

    public static SoundClip Teleport = new("ThoughtBubble/",
        "Teleport");
    public static SoundClip TeleportCharge = new("ThoughtBubble/",
        "TeleportCharge");
    public static SoundClip TeleportSustain = new("ThoughtBubble/",
        "TeleportSustain");

    public static SoundClip PylonDrone = new("Pylon/",
        "DroningPylon");

    public static SoundClip PylonStart = new("Pylon/",
        "PylonThunder");

    public static SoundClip ElectricZap = new("ThoughtBubble/",
        "ElectricReturn");
    public static SoundClip Starbarbs = new("Projectile/",
        "Starbarb");
    public static SoundClip ElectricCast = new("ThoughtBubble/",
        "ElectricAttack1", "ElectricAttack2");
}
public class SoundClip
{
    private const string audioPath = "Audio/";
    private string myPath;
    private List<AudioClip> variations = new();
    private AudioClip Fetch(string audioFileName)
    {
        return Resources.Load<AudioClip>($"{myPath}{audioFileName}");
    }
    /// <summary>
    /// Creates a new sound clip
    /// </summary>
    /// <param name="path">The path to the sound clip inside the Resources/Audio/ directory</param>
    /// <param name="args">A list of the sound clips that are considered variants of each other</param>
    public SoundClip(string path, params string[] args)
    {
        myPath = audioPath + path;
        foreach(string s in args)
        {
            variations.Add(Fetch(s));
        }
    }
    public AudioClip GetRandom()
    {
        return GetVariation(Utils.RandInt(variations.Count));
    }
    public AudioClip GetVariation(int variation = -1)
    {
        if (variation <= -1)
            return GetRandom();
        return variations[variation];
    }
}
