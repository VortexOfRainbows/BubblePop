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
    public void FixedUpdate()
    {
        PlayersNearby = true;
        foreach(Player p in Player.AllPlayers)
            if(p.Distance(gameObject) > Main.PylonActivationDist)
                PlayersNearby = false;
        if (PlayersNearby)
        {
            if (!SoundActive)
                sound = AudioManager.PlaySound(SoundID.PylonDrone, transform.position, 1f, 1, 0);
        }
        else
            IdleAnimation();
    }
    public void Update()
    {
        bool nextPylon = (this == Main.NextPylon && Main.CurrentPylon == null) || (Main.CurrentPylon == this && !WaveDirector.WaveActive);
        if (nextPylon)
            CreatePointers();
    }
    public float animCounter = 0;
    public int CompleteAnimCounter = 0;
    public void IdleAnimation()
    {
        float lerp = 0.045f;
        Crystal.transform.localPosition = Crystal.transform.localPosition.Lerp(new Vector3(0, 1, 1), lerp);
        Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * 0.6f, lerp);
    }
    public void AddQuests()
    {
        WaveMeter.Instance.AddQuest(new Quest.QuestData("Begin your adventure", $"Distance: 0", Quest.QuestType.StartGame));
    }
}
