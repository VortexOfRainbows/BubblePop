using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class Main : MonoBehaviour
{
    public static bool SceneMainMenu => SceneManager.GetActiveScene().buildIndex == 0;
    public static float FramesPerSeconds { get; private set; }
    public static float TimeElapsedDuringLowFPS { get; private set; } = 0;
    public static float FramesElapsedDuringLowFPS { get; private set; } = 0;
    public static bool GameFinishedLoading { get; private set; }
    public static int GameUpdateCount = 0;
    public static bool DebugCheats { get; set; } = false;
    public static bool PlayerNearPylon => CurrentPylon != null && CurrentPylon.PlayersNearby && !CurrentPylon.Complete;
    public static Vector2 PylonPositon => CurrentPylon == null ? Player.Instance1Pos : CurrentPylon.transform.position;
    public static WavePylon CurrentPylon { get; private set; } = null;
    public static WavePylon NextPylon => World.Pylons.Count > PylonProgressionNumber ? World.Pylons[PylonProgressionNumber] : CurrentPylon;
    private static WavePylon PrevPylon { get; set; } = null;
    public static bool PylonActive => CurrentPylon != null && !CurrentPylon.Purified && CurrentPylon.WaveActive;
    public static bool JustSwitchedPylons => PrevPylon != CurrentPylon && PrevPylon != null;
    public static byte PylonProgressionNumber { get; set; } = 0;
    public static void FinishPylon()
    {
        PylonProgressionNumber = CurrentPylon.ProgressionNumber;
        CurrentPylon = null;
        ++PylonProgressionNumber;
    }
    public static bool GamePaused => Time.timeScale == 0;
    public static bool WavesUnleashed { get; set; } = false;
    public GameObject DirectorCanvas;
    public GameObject PowerupCheatCanvas;
    public Transform SpritebatchSuperParent;
    public RawImage TileLightRenderTarget;
    public RawImage BorderRenderTarget;
    public RawImage LightShapeVisualizer;
    public Light2D GlobalLight;
    public static Transform SpritebatchParent => Instance.SpritebatchSuperParent;
    public static int UICameraLayerID { get; private set; } = -1;
    public static readonly int PylonActivationDist = 11;
    public static void SetClosestPylon(WavePylon pylon)
    {
        if ((WavesUnleashed && CurrentPylon != null) || pylon.Complete) //Might need to replace WavesUnleashed with something else
            return;
        PrevPylon = CurrentPylon;
        if (CurrentPylon == null)
            CurrentPylon = pylon;
        else if(pylon.transform.position.Distance(CurrentPylon.ClosestPlayer.Position)
            < CurrentPylon.transform.position.Distance(CurrentPylon.ClosestPlayer.Position))
        {
            CurrentPylon = pylon;
        }
        PylonProgressionNumber = CurrentPylon.ProgressionNumber;
    }
    public void FixedUpdate()
    {
        transform.position = Vector3.zero;
        GameUpdateCount++;
        Projectile.StaticUpdate();
        if (!WavesUnleashed && Player.AllPlayers.Count > 0 && Player.FindClosest(PylonPositon, out _, out float dist) != null && dist > PylonActivationDist)
            CurrentPylon = null;
        //if (DebugCheats && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.RightShift))
        //    PlayerData.ResetAll();
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
        Instance = this;
        if(!GameFinishedLoading)
        {
            Debug.Log("<color=#00FF00>Game Awoke!</color>");
            UICameraLayerID = SortingLayer.NameToID("UICamera");
            TextureAssets.Load();
            PrefabAssets.Load();
            BuffIcon.Load();
            PlayerData.LoadAll(); //This must come before LoadAllEquipList()
            GlobalEquipData.LoadAllEquipList();
            UnlockCondition.PrepareStatics();
            CoinManager.InitCoinPrefabs();
            PlayerData.TryVersionResetProcedure(); //This should come last. Right now it resets equip values to fix lingering bugs from old equip system
            SpriteBatch.Setup();
            LightBatch.Setup();
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
        ResetContainers();
        Main.WavesUnleashed = false; //Basically this needs to run at the start of each scene. If/once main is made persistent, the way this is handled may have to be changed
        Instance = this;
    }
    public void Update()
    {
        Instance = this;
        DebugSettings.Update(this);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isMainMenu = SceneMainMenu;
            if(!isMainMenu)
            {
                if (UIManager.MultiplayerMenu == null || !UIManager.MultiplayerMenu.activeSelf)
                {
                    if (WarpUI.IsCurrentlyOpen)
                        WarpUI.Close();
                    else if(GamePaused)
                    {
                        if (Compendium.Instance != null && Compendium.Instance.Active)
                        {
                            if (Compendium.CurrentlySelectedPage.TierListActive)
                                Compendium.CurrentlySelectedPage.ToggleTierList(Compendium.Instance.TierListText);
                            else
                                Compendium.Instance.ToggleActive();
                        }
                        else if (UIManager.SettingsMenu.activeSelf)
                            CanvasManager.ToggleSettings();
                        else if (UIManager.DebugMenu.activeSelf)
                            CanvasManager.ToggleDebugMenu();
                        else
                            CanvasManager.Resume();
                    }
                    else
                        CanvasManager.Pause();
                }
                else if (UIManager.MultiplayerMenu != null)
                {
                    CanvasManager.CloseMultiplayerMenu();
                }
            }
            else
            {
                if (Compendium.Instance != null && Compendium.Instance.Active)
                {
                    if (Compendium.CurrentlySelectedPage.TierListActive)
                        Compendium.CurrentlySelectedPage.ToggleTierList(Compendium.Instance.TierListText);
                    else
                        Compendium.Instance.ToggleActive();
                }
                else if (UIManager.SettingsMenu.activeSelf)
                    CanvasManager.ToggleSettings();
                else if (UIManager.DebugMenu.activeSelf)
                    CanvasManager.ToggleDebugMenu();
            }
        }
        //if(Main.DebugCheats && Input.GetKey(KeyCode.B))
        //    PowerUp.Spawn(PowerUp.RandomFromPool(0, 1, -1), Utils.MouseWorld);
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.U) && Main.DebugCheats)
        {
            foreach(UnlockCondition u in UnlockCondition.Unlocks.Values)
                PopUpTextUI.UnlockQueue.Enqueue(u);
        }
        if (DebugCheats && Input.GetKeyDown(KeyCode.K) && Input.GetKey(KeyCode.RightShift))
        {
            Main.WavesUnleashed = true;
            PylonProgressionNumber++;
        }
        if (DebugCheats && Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftControl))
        {
            PopUpTextUI.ForceHide = !PopUpTextUI.ForceHide;
        }
        UIManager.DeadHighscoreText.text = $"Wave: {WaveDirector.WaveNum}";
        if (CharacterSelect.Instance != null)
        {
            if (DebugCheats && Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl) && !Main.WavesUnleashed)
            {
                CharacterSelect.Instance.SwapPlayerEquipment(Player.Instance, Main.GlobalEquipData.Characters[Utils.RandInt(Main.GlobalEquipData.Characters.Count)].GetComponent<Equipment>());
                CharacterSelect.Instance.SwapPlayerEquipment(Player.Instance, Main.GlobalEquipData.Hats[Utils.RandInt(Main.GlobalEquipData.Hats.Count)].GetComponent<Equipment>());
                CharacterSelect.Instance.SwapPlayerEquipment(Player.Instance, Main.GlobalEquipData.Accessories[Utils.RandInt(Main.GlobalEquipData.Accessories.Count)].GetComponent<Equipment>());
                CharacterSelect.Instance.SwapPlayerEquipment(Player.Instance, Main.GlobalEquipData.Weapons[Utils.RandInt(Main.GlobalEquipData.Weapons.Count)].GetComponent<Equipment>());
            }
            CharacterSelect.Instance.OnUpdate();
        }
    }
    public void LateUpdate()
    {
        SpriteBatch.OnUpdate();
        LightBatch.OnUpdate();

        FramesPerSeconds = 1.0f / Time.unscaledDeltaTime;
        //Debug.Log(FramesPerSeconds);
        if(FramesPerSeconds <= 12) //The achievement says 10, but we'll do 12
        {
            FramesElapsedDuringLowFPS += 1;
            TimeElapsedDuringLowFPS += Time.unscaledDeltaTime;
            if(TimeElapsedDuringLowFPS >= 3 && FramesElapsedDuringLowFPS >= 10)
                UnlockCondition.Get<SlowThingsDownALittle>().SetComplete();
        }
        else
        {
            TimeElapsedDuringLowFPS = FramesElapsedDuringLowFPS = 0;
        }
    }
    public static Main Instance;
    public static class GlobalEquipData
    {
        public class EquipData
        {
            public EquipData(int timesUsed, int victoryCount, int highestDifficultyCleared)
            {
                TimesUsed = timesUsed;
                VictoryCount = victoryCount;
                HighestDifficultyCleared = highestDifficultyCleared;
            }
            public int TimesUsed;
            public int VictoryCount;
            public int HighestDifficultyCleared;
        }
        public static readonly Dictionary<Type, int> EquipTypeToIndex = new();
        public static readonly List<EquipDescription> DescriptionData = new();
        public static List<EquipData> EquipDataList { get; private set; } = new();
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
            EquipDataList.Clear();
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
        public static readonly GameObject UtilityBelt = LoadSubEquipment(ThoughtBubbleCape, "ThoughtBubble/Belt");
        public static readonly GameObject Fizzy = LoadEquipment("Fizzy/Fizzy");
        public static readonly GameObject Cap = LoadEquipment("Fizzy/Cap");
        public static readonly GameObject Kicks = LoadEquipment("Fizzy/Kicks");
        public static readonly GameObject Cola = LoadEquipment("Fizzy/Cola");
        public static readonly GameObject ThoughtBubblePhysicsBook = LoadSubEquipment(ThoughtBubbleWeapon, "ThoughtBubble/PhysicsBook");
        public static readonly GameObject CatEars = LoadSubEquipment(GachaponHat, "Gachapon/CatEars");
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
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.N))
                DebugCheats = true;
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
            if (DebugCheats && Input.GetKeyDown(KeyCode.Backspace))
                DebugCheats = false;
            instance.DirectorCanvas.SetActive(DirectorView);
            if(PowerUpCheatUI.Instance != null)
                PowerUpCheatUI.StaticUpdate();
        }
        public static bool DirectorView = false;
        public static bool PowerUpCheat = false;
        public static bool SkipWaves = false;
        public static ref bool ForceUnlockAll => ref UnlockCondition.ForceUnlockAll;
    }
}