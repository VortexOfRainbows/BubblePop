using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class PowerUpCheatUI : MonoBehaviour
{
    public static bool Hide { get; set; } = false;
    public static bool AutoHide { get; set; } = true;
    public static PowerUpCheatUI Instance { get; set; }
    public static int ProcessQuantity { get; set; } = 1;
    public static int CurrentType = -1;
    public static bool MouseInCompendiumArea { get; private set; }
    public static Crucible CurrentCrucible { get; set; }
    public static bool HasCrucible => CurrentCrucible != null;
    public static bool HasShards => (CoinManager.CurrentShards > 0) || (Main.DebugCheats && Main.DebugSettings.PowerUpCheat);
    public static bool CanOpenMenu => HasCrucible || HasShards;
    public static bool PrevHadCrucible { get; set; } = false;
    public static bool PrevHadShards { get; set; } = false;
    public static void StaticUpdate()
    {
        if(CanOpenMenu)
        {
            int type = CurrentType; //if you have a current type, don't switch even if the other type is satsified
            if(type >= 0) //Only switch off the current type if you don't satisfy the condition
            {
                if (!HasCrucible && type == 0)
                    type = -1;
                else if (!HasShards && type == 1)
                    type = -1;
                if (HasCrucible && !PrevHadCrucible)
                    type = 0;
                else if (HasShards && !PrevHadShards)
                    type = 1;
            }
            if(type < 0)
            {
                if (HasCrucible)
                    type = 0;
                else if (HasShards)
                    type = 1;
            }
            TurnOn(type);
        }
        else
        {
            TurnOff();
        }
        if(!Instance.gameObject.activeSelf)
            Instance.Update();
        PrevHadCrucible = HasCrucible;
        PrevHadShards = HasShards;
    }
    public static void TurnOn(int type = 0)
    {
        if(!Instance.gameObject.activeSelf || CurrentType != type)
            Instance.Init(type);
    }
    public static void TurnOff()
    {
        if (Instance.gameObject.activeSelf)
            Instance.Disable();
    }
    public PowerUpButton ChoiceTemplate;
    public GridLayoutGroup GridParent;
    public SliderInputField QuantitySlider;
    public Button QuantityUp, QuantityDown, HideButton;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description, HideButtonTextUI, ShardCountTxt;
    public RectTransform MyRect, SelectionArea;
    public Canvas MyCanvas;
    public GameObject NOPOWERS;
    public GameObject CrucibleDisplay;
    public GameObject ShardDisplay;
    public void Start()
    {
        Instance = this;
        gameObject.SetActive(false);
        NOPOWERS.SetActive(false);
        QuantityUp.onClick.AddListener(UpQuantity);
        QuantityDown.onClick.AddListener(DownQuantity);
        HideButton.onClick.AddListener(ToggleHide);
        CrucibleDisplay.SetActive(false);
        ShardDisplay.SetActive(false);
    }
    public void ToggleHide()
    {
        Hide = !Hide;
        if (Hide && PlayerData.PauseDuringPowerSelect && Main.GamePaused)
            Main.UnpauseGame();
        if (!Hide)
            InitializePowers(CurrentType);
    }
    public void UpQuantity()
    {
        int amt = 1;
        if (Input.GetKey(KeyCode.LeftShift))
            amt *= 5;
        if (Input.GetKey(KeyCode.LeftControl))
            amt *= 20;
        QuantitySlider.TryParseInput((ProcessQuantity + amt).ToString(), true);
    }
    public void DownQuantity()
    {
        int amt = 1;
        if (Input.GetKey(KeyCode.LeftShift))
            amt *= 5;
        if (Input.GetKey(KeyCode.LeftControl))
            amt *= 20;
        QuantitySlider.TryParseInput((ProcessQuantity - amt).ToString(), true);
    }
    public void InitializePowers(int type)
    {
        gameObject.SetActive(true);
        CurrentType = type;
        ResetPowers();
        NOPOWERS.SetActive(false);
        UpdateType();
    }
    public void ResetPowers()
    {
        foreach (PowerUpButton t in GridParent.GetComponentsInChildren<PowerUpButton>(false))
            Destroy(t.gameObject);
    }
    public void Init(int type = 0)
    {
        InitializePowers(type);
        transform.localScale = 0.9f * Vector3.one;
        //if (Hide)
        //    ToggleHide();
        if (PlayerData.PauseDuringPowerSelect && !Hide)
            Main.PauseGame();
        AutoHide = true;
    }
    public void Disable()
    {
        ResetPowers();
        gameObject.SetActive(false);
        CurrentType = -1;
        if (Hide)
            ToggleHide();
        if (PlayerData.PauseDuringPowerSelect)
            Main.UnpauseGame();
    }
    public void UpdateType()
    {
        if (CurrentType == 1)
        {
            Title.text = "Rainbow Shards";
            Description.text = "Use Shards to Clone Any Power";
            CrucibleDisplay.SetActive(false);
            ShardDisplay.SetActive(true);
            //Using a couroutine here to make it less immediately laggy by spreading out the initalization of the stuff over frames.
            //This could maybe be done without a couroutine too, but would be more arduous
            StartCoroutine(Main.DebugSettings.PowerUpCheat ? InitCheatButtons() :  InitCrucibleButtons());
        }
        else
        {
            Title.text = "Crucible";
            Description.text = "Convert Powers to Gems";
            CrucibleDisplay.SetActive(true);
            ShardDisplay.SetActive(false);
            //Using a couroutine here to make it less immediately laggy by spreading out the initalization of the stuff over frames.
            //This could maybe be done without a couroutine too, but would be more arduous
            StartCoroutine(InitCrucibleButtons());
        }
    }
    public IEnumerator InitCheatButtons()
    {
        for (int i = 0; i < PowerUp.Reverses.Count; ++i)
        {
            PowerUpButton p = Instantiate(ChoiceTemplate, GridParent.transform);
            p.SetType(i);
            p.gameObject.SetActive(true);
            p.CheatButton = true;
            p.NonChoiceButton = true;
            p.PowerUI.CrucibleElement = true;
            if(i % 3 == 2)
                yield return new WaitForSecondsRealtime(0.025f);
        }
        yield return null;
    }
    public IEnumerator InitCrucibleButtons()
    {
        Player player = Player.Instance;
        for (int i = 0; i < player.Powers.Count; i++)
        {
            PowerUp power = PowerUp.Get(player.Powers[i]);
            PowerUpButton p = Instantiate(ChoiceTemplate, GridParent.transform);
            p.SetType(power.Type);
            p.gameObject.SetActive(true);
            p.PowerUI.Count.gameObject.SetActive(power.Stack > 1);
            p.PowerUI.Count.text = power.Stack.ToString();
            p.Crucible = CurrentType == 0 ? CurrentCrucible : null;

            p.CheatButton = false;
            p.NonChoiceButton = true;
            p.PowerUI.CrucibleElement = true;
            if(i % 3 == 2)
                yield return new WaitForSecondsRealtime(0.025f);
        }
        yield return null;
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
            ToggleHide();
        if (!ChoicePowerMenu.Hide && ChoicePowerMenu.Instance.gameObject.activeSelf)
        {
            if (!Hide)
                ToggleHide();
            HideButton.interactable = false;
        }
        else
            HideButton.interactable = true;
        Instance = this;
        UpdateContentSize();
        MouseInCompendiumArea = Utils.IsMouseHoveringOverThis(true, SelectionArea, 0, MyCanvas);
        float lerpT = Utils.DeltaTimeLerpFactor(0.125f);
        transform.LerpLocalScale(Vector2.one, Utils.DeltaTimeLerpFactor(0.1f));
        ShardCountTxt.text = Main.DebugSettings.PowerUpCheat ? "Inf" : CoinManager.CurrentShards.ToString();
        if (Hide)
        {
            transform.LerpLocalPosition(new Vector2(Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.width / 2, 140), lerpT);
            HideButton.transform.LerpLocalPosition(new Vector2(ChoicePowerMenu.Hide && ChoicePowerMenu.Instance.gameObject.activeSelf ? 710 : 600, -205), lerpT);
            HideButtonTextUI.text = CurrentType == 0 ? "Show Crucible" : "Show Shards";
            MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, Mathf.Lerp(MyRect.sizeDelta.y, 0, Utils.DeltaTimeLerpFactor(0.07f)));
            return;
        }
        else
        {
            transform.LerpLocalPosition(new Vector2(Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.width / 2, -Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.height / 2), lerpT);
            HideButton.transform.LerpLocalPosition(new Vector2(600, -65), lerpT);
            HideButtonTextUI.text = CurrentType == 0 ? "Hide Crucible" : "Hide Shards";
            MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, Mathf.Lerp(MyRect.sizeDelta.y, TargetSize, Utils.DeltaTimeLerpFactor(0.07f)));
        }
    }
    public float TargetSize = 0;
    public void UpdateContentSize()
    {
        int c = GridParent.transform.childCount;
        NOPOWERS.SetActive(c <= 1);
        Vector3 lastElement = GridParent.transform.GetChild(c - 1).localPosition;
        RectTransform r = GridParent.GetComponent<RectTransform>();
        float dist = -lastElement.y + GridParent.padding.bottom * 3;
        TargetSize = Mathf.Min(600, dist);
        r.sizeDelta = new Vector2(r.sizeDelta.x, dist - MyRect.rect.height);
    }
}
