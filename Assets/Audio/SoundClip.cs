using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class SoundID
{
    public static SoundClip BubblePop = new SoundClip("BubblePop/", 
        "BubblePop1", "BubblePop2", "BubblePop3", "BubblePop4", "BubblePop5", "BubblePop6", "BubblePop7", "BubblePop8");

    public static SoundClip Dash = new SoundClip("Player/",
        "Dash1", "Dash2", "Dash3", "Dash4");
    public static SoundClip Death = new SoundClip("Player/",
        "BubbleDeath1", "BubbleDeath2");

    public static SoundClip DuckDeath = new SoundClip("NPCs/Duck/",
        "DuckDeath");
    public static SoundClip DuckNoise = new SoundClip("NPCs/Duck/",
        "DuckNoise");
    public static SoundClip LenardNoise = new SoundClip("NPCs/Duck/",
        "BigDuckSound1", "BigDuckSound2");
    public static SoundClip LenardLaser = new SoundClip("NPCs/Duck/",
        "LaserShot", "LongLaserBlast");

    public static SoundClip FlamingoNoise = new SoundClip("NPCs/Flamingo/",
        "Honk1", "Honk2", "Honk3");
    public static SoundClip FlamingoShot = new SoundClip("NPCs/Flamingo/",
        "Shot1", "Shot2");

    public static SoundClip SoapDie = new SoundClip("NPCs/Soap/",
        "Break1", "Break2", "Break3");
    public static SoundClip SoapSlide = new SoundClip("NPCs/Soap/",
        "Slide1", "Slide2", "Slide3", "Slide4");

    public static SoundClip BathBombSizzle = new SoundClip("BathBomb/",
        "Sizzle");
    public static SoundClip BathBombBurst = new SoundClip("BathBomb/",
        "Break");
    public static SoundClip BathBombSplash = new SoundClip("BathBomb/",
        "WaterSplash");

    public static SoundClip ChargePoint = new SoundClip("Player/",
        "ChargePoint1", "ChargePoint2", "ChargePoint3");
    public static SoundClip ChargeWindup = new SoundClip("Player/",
        "ChargeWindup");
    public static SoundClip ShootBubbles = new SoundClip("Player/",
        "Shoot");

    public static SoundClip PickupPower = new SoundClip("Powerup/",
        "Pickup");
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
