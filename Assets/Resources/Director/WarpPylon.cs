using UnityEngine;
using UnityEngine.UIElements;

public class WarpPylon : MonoBehaviour
{
    public SpriteRenderer Crystal;
    public SpriteRenderer Glow;
    public SpriteRenderer Base;
    public GameObject Portal;
    public Sound sound;
    public bool SoundActive => sound != null;
    public bool WaveActive { get; private set; } = false;
    public byte ProgressionNumber { get; set; } = 0;
    public int WavesPassed { get; private set; } = 0;
    public bool EndlessPylon = false;
    public bool Complete { get; private set; } = false;
    public bool Purified { get; private set; } = false;
    public bool PlayersNearby { get; private set; } = false;
    public Player ClosestPlayer => Player.FindClosest(transform.position, out _, out _);
    public void FixedUpdate()
    {
        PlayersNearby = true;
        foreach(Player p in Player.AllPlayers)
            if(p.Distance(gameObject) > Main.PylonActivationDist)
                PlayersNearby = false;
        if (PlayersNearby)
            if (!SoundActive)
                sound = AudioManager.PlaySound(SoundID.PylonDrone, transform.position, 1f, 1, 0);
        if (!Complete)
        {
            DisableAnimation();
        }
    }
    public void Update()
    {
        bool awaitingPurification = Complete && !Purified;
        bool nextPylon = (this == Main.NextPylon && Main.CurrentPylon == null) || (Main.CurrentPylon == this && !WaveDirector.WaveActive && !Complete && WavesPassed == 0);
        if (awaitingPurification || nextPylon)
            CreatePointers();
    }
    public float animCounter = 0;
    public int CompleteAnimCounter = 0;
    public void DisableAnimation()
    {
        float lerp = 0.045f;
        Crystal.transform.localPosition = Crystal.transform.localPosition.Lerp(new Vector3(0, 1, 1), lerp);
        Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * 0.6f, lerp);
    }
    public void AddQuests()
    {
        if(WavesPassed == 0)
        {
            WaveMeter.Instance.AddQuest(new Quest.QuestData("Begin your adventure", $"Distance: 0", Quest.QuestType.StartGame));
        }
    }
    public void CompletePylon()
    {
        Complete = true;
    }
    public void Enable()
    {
        WaveActive = true;

    }
    public float PointerAlpha = -3.0f;
    public void CreatePointers()
    {
        Vector3 position = Crystal.transform.position;

        Vector2 clamped = Utils.ClampToScreenEdge(position, 40);
        position.x = clamped.x;
        position.y = clamped.y;


        Vector2 toPointer = position - Crystal.transform.position;
        float distanceFromPointer = toPointer.magnitude;
        float scaleFactor = Mathf.Clamp(distanceFromPointer - 1, 0, 1);

        if (distanceFromPointer > 1)
            PointerAlpha += Time.unscaledDeltaTime * 1.5f;
        else
            PointerAlpha -= Time.unscaledDeltaTime;
        PointerAlpha = Mathf.Clamp(PointerAlpha, -3f, scaleFactor);

        SpriteBatch.Draw(Crystal.sprite, position, Vector2.one * 0.4f, 0, Color.white.WithAlpha(Mathf.Max(PointerAlpha * 0.7f, 0)), 21, Main.TextureAssets.SpriteGlowmask);
    }
}
