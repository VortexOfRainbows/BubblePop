using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Main : MonoBehaviour
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
        CanvasManager.StaticPlaySound();
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
    public void OnDestroy()
    {
        OnGameClose();
    }
    public void Awake()
    {
        Instance = this;
        TextureAssets.Load();
        PrefabAssets.Load();
        BuffIcon.Load();
        OnGameOpen();
        GlobalEquipData.LoadAllEquipList();
        UnlockCondition.PrepareStatics();
    }
    public void OnApplicationQuit()
    {
        OnGameClose();
    }
    public void Start()
    {
        Main.WavesUnleashed = false; //Basically this needs to run at the start of each scene. If/once main is made persistent, the way this is handled may have to be changed
        Instance = this;
        UIManager.AddListeners();
    }
    public void Update()
    {
        Instance = this;
        if (Input.GetKeyDown(KeyCode.F) && DebugCheats)
            DirectorCanvas.SetActive(!DirectorCanvas.activeSelf);
        if (Input.GetKeyDown(KeyCode.P) && DebugCheats)
            PowerupCheatCanvas.SetActive(!PowerupCheatCanvas.activeSelf);
        if (Input.GetKeyDown(KeyCode.U) && DebugCheats)
            UnlockCondition.ForceUnlockAll = true;
        if (DebugCheats && Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.RightShift))
            WaveDirector.WaveNum += 1;

        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (GamePaused)
            {
                if (UIManager.SettingsMenu.activeSelf)
                {
                    UIManager.ToggleSettings();
                }
                else
                {
                    UIManager.Resume();
                }
            }
            else
                UIManager.Pause();
        }
        UIManager.DeadHighscoreText.text = $"Wave: {WaveDirector.WaveNum}";
    }
    public static Main Instance;
    public static class GlobalEquipData
    {
        public static Dictionary<Type, int> EquipTypeToIndex = new();
        public static List<DetailedDescription> DescriptionData = new();
        public static List<GameObject> AllEquipList => AllEquipmentsList;
        public static List<int> TimesUsedList = new();
        public static Dictionary<Type, Equipment> TypeToEquipPrefab = new();
        public static List<GameObject>[] PrimaryEquipments = new List<GameObject>[4];
        public static List<GameObject> Hats = new();
        public static List<GameObject> Accessories = new();
        public static List<GameObject> Weapons = new();
        public static List<GameObject> Characters = new();
        public static List<GameObject> AllEquipmentsList = new();
        public static void LoadAllEquipList()
        {
            foreach(UnlockCondition unlock in UnlockCondition.Unlocks.Values)
                unlock.AssociatedUnlocks.Clear();
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
            //foreach(GameObject g in AllEquipmentsList)
            //{
            //    Equipment equip = g.GetComponent<Equipment>();
            //}
        }
        public static Equipment FindBaseEquipFromType<T>() where T: Equipment
        {
            return EquipFromType(typeof(T));
        }
        public static Equipment EquipFromType(Type t)
        {
            return TypeToEquipPrefab[t];
        }

        public static Queue<GameObject> EquipmentLoadQueue;
        public static readonly GameObject Bubblemancer = LoadEquipment("Bubblemancer/Bubblemancer");
        public static readonly GameObject BubblemancerHat = LoadEquipment("Bubblemancer/BubblemancerHat");
        public static readonly GameObject BubblemancerCape = LoadEquipment("Bubblemancer/BubblemancerCape");
        public static readonly GameObject BubblemancerWeapon = LoadEquipment("Bubblemancer/BubblemancerWand");
        public static readonly GameObject ThoughtBubble = LoadEquipment("ThoughtBubble/ThoughtBubble");
        public static readonly GameObject ThoughtBubbleHat = LoadEquipment("ThoughtBubble/LightBulb");
        public static readonly GameObject ThoughtBubbleCape = LoadEquipment("ThoughtBubble/LabCoat");
        public static readonly GameObject ThoughtBubbleWeapon = LoadEquipment("ThoughtBubble/Book");
        public static readonly GameObject Gachapon = LoadEquipment("Gachapon/Gachapon");
        public static readonly GameObject GachaponHat = LoadEquipment("Gachapon/Dice");
        public static readonly GameObject GachaponCape = LoadEquipment("Gachapon/Emerald");
        public static GameObject LoadEquipment(string path)
        {
            return LoadEquipment(Resources.Load<GameObject>($"Player/{path}"));
        }
        public static GameObject LoadEquipment(GameObject Prefab)
        {
            Equipment EquipType = Prefab.GetComponent<Equipment>();
            if(EquipType is Hat)
                Hats.Add(Prefab);
            else if(EquipType is Accessory)
                Accessories.Add(Prefab);
            else if(EquipType is Weapon)
                Weapons.Add(Prefab);
            else if(EquipType is Body)
                Characters.Add(Prefab);
            return Prefab;
        }
        public static void DefineSubequipRelationships()
        {

        }
    }
}