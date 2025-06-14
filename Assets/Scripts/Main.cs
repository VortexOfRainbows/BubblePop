using UnityEngine;

public class Main : MonoBehaviour
{
    public const float SnakeEyeChance = 0.0278f;
    public static bool DebugCheats = false;
    public static bool PlayerNearPylon => PrevPylon != null;
    public static Pylon CurrentPylon = null;
    private static Pylon PrevPylon = null;
    public static bool GamePaused = false;
    public static bool WavesUnleashed = false;
    public GameObject DirectorCanvas;
    public void FixedUpdate()
    {
        PrevPylon = CurrentPylon;
        CurrentPylon = null;

        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.F) && Main.DebugCheats)
            DirectorCanvas.SetActive(true);
        else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.G) && Main.DebugCheats)
            DirectorCanvas.SetActive(false);
        if (DebugCheats && Input.GetKey(KeyCode.O))
        {
            PlayerPrefs.DeleteAll(); //Reset all save data persistently for the purposes of the playtest
            PlayerData.LoadAll();
            PlayerData.SaveAll();
        }
    }
    public void OnGameOpen()
    {
        PlayerData.LoadAll();
        CoinManager.InitCoinPrefabs();
    }
    public static void StartGame()
    {
        Time.timeScale = 1f;
        CoinManager.ModifySavings(-CoinManager.TotalEquipCost);
        GamePaused = false;
        WavesUnleashed = true;
    }
    public static void PauseGame()
    {
        Time.timeScale = 0f;
        GamePaused = true;
    }
    public static void UnpauseGame()
    {
        Time.timeScale = 1f;
        GamePaused = false;
    }
    public void OnGameClose()
    {
        PlayerData.SaveAll();
    }
    public void Awake()
    {
        OnGameOpen();
    }
    public void OnDestroy()
    {
        OnGameClose();
    }
    public void Start()
    {
        Instance = this;
    }
    public void Update() => Instance = this;
    public static Main Instance;
    public static GameObject Projectile => Instance.DefaultProjectile;
    public GameObject DefaultProjectile;
    public static Sprite SpikyProjectileSprite => Instance.SpikyProj;
    public Sprite SpikyProj;
    public static Sprite BathBombSprite => Instance.BathBomb;
    public Sprite BathBomb;
    public static Sprite BubbleSprite => Instance.BigBubble;
    public Sprite BigBubble;
    public static Sprite BubbleSmall => Instance.BubbleParticle;
    public Sprite BubbleParticle;
    public static Sprite SquareBubble => Instance.SqrParticle;
    public Sprite SqrParticle;
    public static Sprite[] bathBombShards => Instance.bathbombShards;
    public Sprite[] bathbombShards;
    public static Sprite BubblePower1 => Instance.PowerBubble1;
    public Sprite PowerBubble1;
    public static Sprite BubblePower2 => Instance.PowerBubble2;
    public Sprite PowerBubble2;
    public static Sprite Feather => Instance.FlamigoFeater;
    public Sprite FlamigoFeater;
    public static Sprite Laser => Instance.LaserProj;
    public Sprite LaserProj;
    public static Sprite Sparkle => Instance.sparkle;
    public Sprite sparkle;
}