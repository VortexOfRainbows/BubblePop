using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static bool MouseHoveringOverButton { get; set; }
    public static int GameUpdateCount = 0;
    public const float SnakeEyeChance = 0.0278f;
    public static bool DebugCheats = false;
    public static bool PlayerNearPylon => PrevPylon != null && Player.Position.Distance(PrevPylon.transform.position) < 11;
    public static Vector2 PylonPositon => CurrentPylon != null ? CurrentPylon.transform.position : PrevPylon != null ? PrevPylon.transform.position : Player.Position;
    public static Pylon CurrentPylon = null;
    private static Pylon PrevPylon = null;
    public static bool GamePaused => Time.timeScale == 0;
    public static bool WavesUnleashed { get; set; } = false;
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
        UnpauseGame();
        CoinManager.ModifySavings(-CoinManager.TotalEquipCost);
        WavesUnleashed = true;
        CardManager.DrawCards();
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
        Instance = this;
        Shadow = Resources.Load<Sprite>("Shadow");
        BuffIcon.Load();
        OnGameOpen();
        EquipData.LoadAllEquipList();
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

    public GlobalEquipData EquipData;
    [Serializable]
    public class GlobalEquipData
    {
        public static Dictionary<Type, int> EquipTypeToIndex = new();
        public static List<DetailedDescription> DescriptionData = new();
        public static List<GameObject> AllEquipList => Instance.EquipData.AllEquipmentsList;
        public static List<int> TimesUsedList = new();
        public static Dictionary<Type, Equipment> TypeToEquipPrefab = new();
        public List<GameObject>[] PrimaryEquipments = new List<GameObject>[4];
        public List<GameObject> Hats;
        public List<GameObject> Accessories;
        public List<GameObject> Weapons;
        public List<GameObject> Characters;
        public List<GameObject> AllEquipmentsList = new();
        public void LoadAllEquipList()
        {
            TypeToEquipPrefab.Clear();
            EquipTypeToIndex.Clear();
            TimesUsedList.Clear();
            DescriptionData.Clear();
            AllEquipmentsList.Clear();
            PrimaryEquipments[0] = Hats;
            PrimaryEquipments[1] = Accessories;
            PrimaryEquipments[2] = Weapons;
            PrimaryEquipments[3] = Characters;
            for (int j = 0; j < PrimaryEquipments.Length; j++)
            {
                for (int i = 0; i < PrimaryEquipments[j].Count; ++i)
                {
                    Equipment equip = PrimaryEquipments[j][i].GetComponent<Equipment>();
                    equip.SetUpData(AllEquipmentsList.Count);
                    Debug.Log($"Equipment: <color=#FFFF00>{equip.GetName()}</color> has been added into the pool at index {equip.IndexInAllEquipPool}");
                    AllEquipmentsList.Add(equip.gameObject);
                    TypeToEquipPrefab.Add(equip.GetType(), equip);
                    if (equip.SubEquipment != null)
                    {
                        for (int k = 0; k < equip.SubEquipment.Count; k++)
                        {
                            Equipment subEquip = equip.SubEquipment[k].GetComponent<Equipment>();
                            subEquip.SetUpData(AllEquipmentsList.Count);
                            Debug.Log($"Equipment: <color=#FF0000>{subEquip.GetName()}</color> has been added into the pool at index {AllEquipmentsList.Count}");
                            AllEquipmentsList.Add(subEquip.gameObject);
                            TypeToEquipPrefab.Add(subEquip.GetType(), subEquip);
                        }
                    }
                }
            }
        }
        public static Equipment FindBaseEquipFromType<T>() where T: Equipment
        {
            return EquipFromType(typeof(T));
        }
        public static Equipment EquipFromType(Type t)
        {
            return TypeToEquipPrefab[t];
        }
    }
}