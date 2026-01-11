using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Main : MonoBehaviour
{
    public static bool GameFinishedLoading { get; private set; }
    public static bool MouseHoveringOverButton { get; set; }
    public static int GameUpdateCount = 0;
    public const float SnakeEyeChance = 0.0278f;
    public static bool DebugCheats { get; set; } = false;
    public static bool PlayerNearPylon => PrevPylon != null && Player.Position.Distance(PrevPylon.transform.position) < 11;
    public static Vector2 PylonPositon => CurrentPylon != null ? CurrentPylon.transform.position : PrevPylon != null ? PrevPylon.transform.position : Player.Position;
    public static Pylon CurrentPylon = null;
    private static Pylon PrevPylon = null;
    public static bool GamePaused => Time.timeScale == 0;
    public static bool WavesUnleashed { get; set; } = false;
    public GameObject DirectorCanvas;
    public GameObject PowerupCheatCanvas;
    public static int UICameraLayerID { get; private set; } = -1;
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
    }
    public static void StartGame()
    {
        UnpauseGame();
        //CoinManager.ModifySavings(-CoinManager.TotalEquipCost);
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
        if(!GameFinishedLoading)
        {
            Debug.Log("<color=#00FF00>Game Awoke!</color>");
            Instance = this;
            UICameraLayerID = SortingLayer.NameToID("UICamera");
            TextureAssets.Load();
            PrefabAssets.Load();
            BuffIcon.Load();
            PlayerData.LoadAll(); //This must come before LoadAllEquipList()
            GlobalEquipData.LoadAllEquipList();
            UnlockCondition.PrepareStatics();
            CoinManager.InitCoinPrefabs();
            PlayerData.TryVersionResetProcedure(); //This should come last. Right now it resets equip values to fix lingering bugs from old equip system
            GameFinishedLoading = true;

            OnGameOpen();
        }
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
        DebugSettings.Update(this);
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (GamePaused)
            {
                if (UIManager.SettingsMenu.activeSelf)
                    UIManager.ToggleSettings();
                else if (UIManager.DebugMenu.activeSelf)
                    UIManager.OpenDebugMenu();
                else
                    UIManager.Resume();
            }
            else
                UIManager.Pause();
        }
        if(Main.DebugCheats && Input.GetKey(KeyCode.B))
        {
            PowerUp.Spawn(PowerUp.RandomFromPool(0, 1, -1), Utils.MouseWorld);
        }
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.U) && Main.DebugCheats)
        {
            foreach(UnlockCondition u in UnlockCondition.Unlocks.Values)
                PopUpTextUI.UnlockQueue.Enqueue(u);
        }
        UIManager.DeadHighscoreText.text = $"Wave: {WaveDirector.WaveNum}";
    }
    public static Main Instance;
    public static class GlobalEquipData
    {
        public static readonly Dictionary<Type, int> EquipTypeToIndex = new();
        public static readonly List<DetailedDescription> DescriptionData = new();
        public static readonly List<int> TimesUsedList = new();
        public static readonly Dictionary<Type, Equipment> TypeToEquipPrefab = new();
        public static readonly List<GameObject>[] PrimaryEquipments = new List<GameObject>[4];
        public static readonly List<GameObject> Hats = new();
        public static readonly List<GameObject> Accessories = new();
        public static readonly List<GameObject> Weapons = new();
        public static readonly List<GameObject> Characters = new();
        public static readonly List<GameObject> AllEquipmentsList = new();
        public static void LoadAllEquipList()
        {
            foreach(UnlockCondition unlock in UnlockCondition.Unlocks.Values)
            {
                unlock.AssociatedUnlocks.Clear();
                unlock.AssociatedBlackMarketUnlocks.Clear();
            }
            TypeToEquipPrefab.Clear();
            EquipTypeToIndex.Clear();
            TimesUsedList.Clear();
            DescriptionData.Clear();
            AllEquipmentsList.Clear();
            PrimaryEquipments[0] = Characters;
            PrimaryEquipments[1] = Hats;
            PrimaryEquipments[2] = Accessories;
            PrimaryEquipments[3] = Weapons;
            while(EquipmentAddQueue.TryDequeue(out GameObject g))
            {
                AddEquip(g);
            }
        }
        private static void AddEquip(GameObject g)
        {
            Equipment e = g.GetComponent<Equipment>();
            if (!e.IsSubEquip)
            {
                if (e is Hat)
                    Hats.Add(g);
                else if (e is Accessory)
                    Accessories.Add(g);
                else if (e is Weapon)
                    Weapons.Add(g);
                else if (e is Body)
                    Characters.Add(g);
            }
            e.SetUpData(AllEquipmentsList.Count);
            AllEquipmentsList.Add(g);
            TypeToEquipPrefab.Add(e.GetType(), e);
            Debug.Log($"Equipment: <color=#FFFF00>{e.GetName()}</color> has been added into the pool at index {e.IndexInAllEquipPool}: [{AllEquipmentsList.Count}]");
        }
        public static Equipment FindBaseEquipFromType<T>() where T: Equipment
        {
            return EquipFromType(typeof(T));
        }
        public static Equipment EquipFromType(Type t)
        {
            return TypeToEquipPrefab[t];
        }
        public static readonly Queue<GameObject> EquipmentAddQueue = new();
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
        public static readonly GameObject BlueHat = LoadSubEquipment(BubblemancerHat, "Bubblemancer/BlueHat");
        public static readonly GameObject BlueCape = LoadSubEquipment(BubblemancerCape, "Bubblemancer/BlueCape");
        public static readonly GameObject BubbleStaff = LoadSubEquipment(BubblemancerWeapon, "Bubblemancer/BigWand");
        public static readonly GameObject BubbleGun = LoadSubEquipment(BubblemancerWeapon, "Bubblemancer/BubbleGun");
        public static readonly GameObject BunceHat = LoadSubEquipment(BubblemancerHat, "Bubblemancer/BunceHat");
        public static readonly GameObject RedHat = LoadSubEquipment(BubblemancerHat, "Bubblemancer/RedHat");
        public static readonly GameObject RedCape = LoadSubEquipment(BubblemancerCape, "Bubblemancer/RedCape");
        public static readonly GameObject CrownOfCommand = LoadSubEquipment(ThoughtBubbleHat, "ThoughtBubble/Crown/Crown");
        public static readonly GameObject CrystalSkull = LoadSubEquipment(GachaponCape, "Gachapon/Cryskull");
        public static readonly GameObject SusCape = LoadSubEquipment(GachaponCape, "Gachapon/ShadyCoat");
        public static readonly GameObject GachaponWeapon = LoadEquipment("Gachapon/SlotMachine/SlotMachine");
        public static readonly GameObject DragonSlots = LoadSubEquipment(GachaponWeapon, "Gachapon/SlotMachine/DragonSlots");
        public static GameObject LoadEquipment(string path) => LoadEquipment(Resources.Load<GameObject>($"Player/{path}"));
        public static GameObject LoadEquipment(GameObject Prefab)
        {
            Prefab.GetComponent<Equipment>().SubEquipment.Clear();
            EquipmentAddQueue.Enqueue(Prefab);
            return Prefab;
        }
        public static GameObject LoadSubEquipment(GameObject ParentReference, string path) => LoadSubEquipment(ParentReference, Resources.Load<GameObject>($"Player/{path}"));
        public static GameObject LoadSubEquipment(GameObject ParentReference, GameObject Prefab)
        {
            Equipment parent = ParentReference.GetComponent<Equipment>();
            Equipment child = Prefab.GetComponent<Equipment>();
            parent.SubEquipment.Add(Prefab);
            child.IsSubEquip = true;
            child.SubEquipParent = parent;
            return LoadEquipment(Prefab);
        }
    }
    public static class DebugSettings
    {
        public static void Update(Main instance)
        {
            if (/*Input.GetKey(KeyCode.R) &&*/Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.N))
            {
                UIManager.EnableDebugButtons();
                DebugCheats = true;
            }
            if(DebugCheats)
                UIManager.EnableDebugButtons();
            if (Input.GetKeyDown(KeyCode.F) && DebugCheats)
                DirectorView = !DirectorView;
            if (Input.GetKeyDown(KeyCode.P) && DebugCheats)
                PowerUpCheat = !PowerUpCheat;
            if (Input.GetKeyDown(KeyCode.U) && DebugCheats)
            {
                foreach (UnlockCondition condition in UnlockCondition.Unlocks.Values) {
                    condition.ForceUnlock();
                }
                UnlockCondition.ForceUnlockAll = true;
            }
            if (Input.GetKeyDown(KeyCode.L) && DebugCheats)
                SkipWaves = !SkipWaves;

            instance.PowerupCheatCanvas.SetActive(PowerUpCheat);
            instance.DirectorCanvas.SetActive(DirectorView);
        }
        public static bool DirectorView = false;
        public static bool PowerUpCheat = false;
        public static bool SkipWaves = false;
        public static ref bool ForceUnlockAll => ref UnlockCondition.ForceUnlockAll;
    }
}