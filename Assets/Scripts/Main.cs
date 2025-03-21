using UnityEngine;

public class Main : MonoBehaviour
{
    public static bool GamePaused = false;
    public static bool GameStarted = false;
    public void OnGameOpen()
    {
        //PlayerPrefs.DeleteAll();
        PlayerData.LoadAll();
        CoinManager.InitCoinPrefabs();
    }
    public static void StartGame()
    {
        Time.timeScale = 1f;
        CoinManager.ModifySavings(-CoinManager.TotalEquipCost);
        GamePaused = false;
        GameStarted = true;
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
    public static GameObject Ducky => Instance.Duck;
    public GameObject Duck;
    public static GameObject Soap => Instance.SoapNPC;
    public GameObject SoapNPC;
    public static GameObject TinySoap => Instance.SoapTinyNPC;
    public GameObject SoapTinyNPC;
    public static GameObject flamingoFloatie => Instance.flamingo;
    public GameObject flamingo;
    public static GameObject FinalDuck => Instance.RadDuck;
    public GameObject RadDuck;
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