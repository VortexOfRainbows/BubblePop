using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WarpPylon : PylonBase
{
    public GameObject Portal;
    public Sound sound;
    public bool SoundActive => sound != null;
    public byte ProgressionNumber { get; set; } = 0;
    public bool PlayersNearby { get; private set; } = false;
    public Player ClosestPlayer => Player.FindClosest(transform.position, out _, out _);
    public float animCounter = 0;
    public void FixedUpdate()
    {
        PlayersNearby = true;
        foreach(Player p in Player.AllPlayers)
            if(p.Distance(gameObject) > Main.PylonActivationDist)
                PlayersNearby = false;
        if (PlayersNearby)
        {
            ActiveAnim();
            if (!SoundActive)
                sound = AudioManager.PlaySound(SoundID.PylonDrone, transform.position, 1f, 1, 0);
        }
        else
            IdleAnimation();
        if (SoundActive)
            sound.PylonSoundUpdate(this);
    }
    public void ActiveAnim()
    {
        animCounter++;

        float sin = Mathf.Sin(animCounter * Mathf.Deg2Rad * 1.4f) * 0.3f;
        float lerp = 0.035f;
        Crystal.transform.localPosition = Crystal.transform.localPosition.Lerp(new Vector3(0, 3 + sin, 1), lerp);
        Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * 0.8f, lerp);
    }
    public void Update()
    {
        bool nextPylon = Main.WavesUnleashed && World.Pylons.Count <= Main.PylonProgressionNumber;
        if (nextPylon)
            CreatePointers();
        if (!WarpUI.IsCurrentlyOpen && PlayersNearby)
        {
            EnableUI();
            if (Control.Interact)
            {
                DisableUI();
                WarpUI.Open();
            }
        }
    }
    public void IdleAnimation()
    {
        DisableUI();
        float lerp = 0.045f;
        Crystal.transform.localPosition = Crystal.transform.localPosition.Lerp(new Vector3(0, 1, 1), lerp);
        Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * 0.6f, lerp);
    }
}
