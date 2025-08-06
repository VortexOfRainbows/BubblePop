using UnityEngine;

public class Main : MonoBehaviour
{
    public static bool MouseHoveringOverButton { get; set; }
    public static int GameUpdateCount = 0;
    public const float SnakeEyeChance = 0.0278f;
    public static bool DebugCheats = false;
    public static bool PlayerNearPylon => PrevPylon != null;
    public static Pylon CurrentPylon = null;
    private static Pylon PrevPylon = null;
    public static bool GamePaused => Time.timeScale == 0;
    public static bool WavesUnleashed = false;
    public GameObject DirectorCanvas;
    public GameObject PowerupCheatCanvas;
    public void FixedUpdate()
    {
        GameUpdateCount++;
        Projectile.StaticUpdate(); 
        PrevPylon = CurrentPylon;
        CurrentPylon = null;
        if (DebugCheats && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.RightShift))
        {
            PlayerData.ResetAll();
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
        WavesUnleashed = true;
    }
    public static void PauseGame()
    {
        Time.timeScale = 0f;
    }
    public static void UnpauseGame()
    {
        Time.timeScale = 1f;
    }
    public void OnGameClose()
    {
        PlayerData.SaveAll();
    }
    public void Awake()
    {
        Shadow = Resources.Load<Sprite>("Shadow");
        BuffIcon.Load();
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
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && DebugCheats)
            DirectorCanvas.SetActive(!DirectorCanvas.activeSelf);
        if (Input.GetKeyDown(KeyCode.P) && DebugCheats)
            PowerupCheatCanvas.SetActive(!PowerupCheatCanvas.activeSelf);
        if (Input.GetKeyDown(KeyCode.U) && DebugCheats)
            UnlockCondition.ForceUnlockAll = true;
        Instance = this;
    }
    public static Main Instance;
    public static GameObject ProjPrefab => Instance.DefaultProjectile;
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
    public static Sprite Shadow;
}