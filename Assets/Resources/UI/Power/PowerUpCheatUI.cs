using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpCheatUI : MonoBehaviour
{
    public enum PowerMenuType
    {
        None = -1,
        Crucible = 0,
        Shard = 1, //Same as CHEAT
    }
    //Serialized
    public PowerMenuType MyType = PowerMenuType.None;
    public bool Hide { get; set; } = true;
    public static PowerUpCheatUI CrucibleInstance { get; set; }
    public static PowerUpCheatUI ShardInstance { get; set; }
    public static int ProcessQuantity { get; set; } = 1;
    public static int UpdatedProcessQuantity { get; set; } = 0;
    public static bool MouseInCompendiumArea { get; private set; }
    public static bool HasCrucible => Crucible.PlayerClosestCrucible != null;
    public static bool HasShards => (CoinManager.CurrentShards >= 1) || (Main.DebugCheats && Main.DebugSettings.PowerUpCheat);
    public bool CanOpenMenu()
    {
        if (MyType == PowerMenuType.Crucible)
            return HasCrucible;
        else if(MyType == PowerMenuType.Shard)
            return HasShards;
        return false;
    }
    public static void StaticUpdate()
    {
        if(CrucibleInstance != null)
            CrucibleInstance.InstanceUpdate();
        if (ShardInstance != null)
            ShardInstance.InstanceUpdate();
        if (UpdatedProcessQuantity > 0)
            --UpdatedProcessQuantity;
    }
    public static void CloseAllMenus()
    {
        if (!ShardInstance.Hide)
            ShardInstance.ToggleHide();
        if (!CrucibleInstance.Hide)
            CrucibleInstance.ToggleHide();
    }
    public PowerUpButton ChoiceTemplate;
    public GridLayoutGroup GridParent;
    public CheatMenuQuantityInputField QuantitySlider;
    public Button HideButton;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description, HideButtonTextUI, ShardCountTxt;
    public RectTransform MyRect, SelectionArea;
    public Canvas MyCanvas;
    public GameObject NOPOWERS;
    public GameObject CrucibleDisplay;
    public GameObject ShardDisplay;
    public KeyCode Keybind;
    public void Start()
    {
        gameObject.SetActive(true);
        NOPOWERS.SetActive(false);
        HideButton.onClick.AddListener(ToggleHide);
        Hide = true;
        transform.localPosition = new Vector3(Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.width / 2, 140, 0);
        InitializeType();
        QuantitySlider.Update();
    }
    public void InitializeType()
    {
        switch (MyType)
        {
            case PowerMenuType.Crucible:
                Title.text = "Crucible";
                Description.text = "Convert Powers to Gems";
                CrucibleDisplay.SetActive(true);
                ShardDisplay.SetActive(false);
                CrucibleInstance = this;
                Keybind = KeyCode.E;
                break;
            case PowerMenuType.Shard:
                Title.text = "Rainbow Shards";
                Description.text = "Use Shards to Clone Any Power";
                CrucibleDisplay.SetActive(false);
                ShardDisplay.SetActive(true);
                ShardInstance = this;
                Keybind = KeyCode.C;
                break;
        }
    }
    private void ToggleHide() => ToggleHide(true);
    public void ToggleHide(bool pauseBehavior)
    {
        Hide = !Hide;
        if (pauseBehavior && PlayerData.PauseDuringPowerSelect)
        {
            if (!Hide && !Main.GamePaused)
                Main.PauseGame();
            else if(Hide && Main.GamePaused)
                Main.UnpauseGame();
        }
        if (!Hide)
        {
            LaunchMenu();
        }
    }
    public static void UpQuantity()
    {
        int amt = 1;
        if (Input.GetKey(KeyCode.LeftShift))
            amt *= 5;
        if (Input.GetKey(KeyCode.LeftControl))
            amt *= 20;
        string makeThisNotUseStringsLater = (ProcessQuantity + amt).ToString();
        CrucibleInstance.QuantitySlider.TryParseInput(makeThisNotUseStringsLater, true);
        ShardInstance.QuantitySlider.TryParseInput(makeThisNotUseStringsLater, true);
    }
    public static void DownQuantity()
    {
        int amt = 1;
        if (Input.GetKey(KeyCode.LeftShift))
            amt *= 5;
        if (Input.GetKey(KeyCode.LeftControl))
            amt *= 20;
        string makeThisNotUseStringsLater = (ProcessQuantity - amt).ToString();
        CrucibleInstance.QuantitySlider.TryParseInput(makeThisNotUseStringsLater, true);
        ShardInstance.QuantitySlider.TryParseInput(makeThisNotUseStringsLater, true);
    }
    public bool AwaitingPowerReset = false;
    public void ResetPowers()
    {
        AwaitingPowerReset = true;
        foreach (PowerUpButton t in GridParent.GetComponentsInChildren<PowerUpButton>(false))
            Destroy(t.gameObject);
    }
    public void LaunchMenu()
    {
        //Close the other cheat menu when opening this one
        if (MyType == PowerMenuType.Crucible && !ShardInstance.Hide)
            ShardInstance.ToggleHide(false);
        else if (MyType == PowerMenuType.Shard && !CrucibleInstance.Hide)
            CrucibleInstance.ToggleHide(false);
        LoadPowers();
        transform.localScale = 0.9f * Vector3.one;
    }
    public void Disable()
    {
        ResetPowers();
        if (!Hide)
            ToggleHide(false);
        if (PlayerData.PauseDuringPowerSelect)
            Main.UnpauseGame();
    }
    public void LoadPowers()
    {
        NOPOWERS.SetActive(false);
        ResetPowers();
        if (MyType == PowerMenuType.Shard)
            StartCoroutine(Main.DebugSettings.PowerUpCheat ? InitCheatButtons() :  InitCrucibleButtons());
        else if(MyType == PowerMenuType.Crucible)
            StartCoroutine(InitCrucibleButtons());
    }
    public IEnumerator InitCheatButtons()
    {
        if(AwaitingPowerReset)
            yield return new WaitForSecondsRealtime(0.03f);
        AwaitingPowerReset = false;
        for (int i = 0; i < PowerUp.TotalPowerUps; ++i)
        {
            PowerUpButton p = Instantiate(ChoiceTemplate, GridParent.transform);
            p.SetType(i);
            p.gameObject.SetActive(true);
            p.CheatButton = p.NonChoiceButton = p.PowerUI.CrucibleElement = p.PowerUI.IncludeRainbowShards = true;
            p.PowerUI.Cost = p.PowerUI.MyPower.ShardReplicationCost();
            p.PowerUI.CostText.text = p.PowerUI.Cost.ToString();
            if (i % 3 == 2)
                yield return new WaitForSecondsRealtime(0.02f);
            if(AwaitingPowerReset)
                break;
        }
        yield break;
    }
    public IEnumerator InitCrucibleButtons()
    {
        if (AwaitingPowerReset)
            yield return new WaitForSecondsRealtime(0.03f);
        AwaitingPowerReset = false;
        for (int i = 0; i < Player.GlobalPowers.Count; i++)
        {
            PowerUp power = PowerUp.Get(Player.GlobalPowers[i]);
            PowerUpButton p = Instantiate(ChoiceTemplate, GridParent.transform);
            p.SetType(power.Type);
            p.gameObject.SetActive(true);
            p.PowerUI.Count.gameObject.SetActive(power.Stack > 1);
            p.PowerUI.Count.text = power.Stack.ToString();
            if (MyType == PowerMenuType.Crucible)
                p.Crucible = Crucible.PlayerClosestCrucible;
            else
                p.PowerUI.IncludeRainbowShards = true;
            p.CheatButton = false;
            p.NonChoiceButton = true;
            p.PowerUI.CrucibleElement = true;
            p.PowerUI.Cost = power.ShardReplicationCost(ProcessQuantity);
            p.PowerUI.CostText.text = p.PowerUI.Cost.ToString();
            if (i % 3 == 2)
                yield return new WaitForSecondsRealtime(0.02f);
            if (AwaitingPowerReset)
                break;
        }
        yield break;
    }
    public void InstanceUpdate()
    {
        if (!Hide && !CanOpenMenu())
        {
            Disable();
        }
        if (Input.GetKeyDown(Keybind) && HideButton.interactable && gameObject.activeSelf && CanOpenMenu())
        {
            ToggleHide(true);
        }
        if (!ChoicePowerMenu.Hide && ChoicePowerMenu.Instance.gameObject.activeSelf)
        {
            if (!Hide)
                ToggleHide(true);
            HideButton.interactable = false;
        }
        else
            HideButton.interactable = true;
        UpdateContentSize();
        MouseInCompendiumArea = Utils.IsMouseHoveringOverThis(true, SelectionArea, 0, MyCanvas, false, Hide || !gameObject.activeSelf);
        float lerpT = Utils.DeltaTimeLerpFactor(0.125f);
        transform.LerpLocalScale(Vector2.one, Utils.DeltaTimeLerpFactor(0.1f));
        ShardCountTxt.text = Main.DebugSettings.PowerUpCheat ? "Inf" : CoinManager.CurrentShards.ToString();
        float buttonPosition = -65;
        if (Hide)
        {
            transform.LerpLocalPosition(new Vector2(Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.width / 2, 140), lerpT);
            if (MyType != PowerMenuType.Crucible && CanOpenMenu())
                buttonPosition = -205;
            HideButton.transform.LerpLocalPosition(new Vector2(ChoicePowerMenu.Hide && ChoicePowerMenu.Instance.gameObject.activeSelf ? 710 : 600, buttonPosition), lerpT);
            MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, Mathf.Lerp(MyRect.sizeDelta.y, 0, Utils.DeltaTimeLerpFactor(0.07f)));
        }
        else
        {
            transform.LerpLocalPosition(new Vector2(Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.width / 2, -Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.height / 2), lerpT);
            HideButton.transform.LerpLocalPosition(new Vector2(600, buttonPosition), lerpT);
            MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, Mathf.Lerp(MyRect.sizeDelta.y, TargetSize, Utils.DeltaTimeLerpFactor(0.07f)));
        }
        SetHideButtonText();
    }
    public void SetHideButtonText()
    {
        if(Hide)
            HideButtonTextUI.text = MyType == PowerMenuType.Crucible ? "Show Crucible" : Main.DebugSettings.PowerUpCheat ? "Show Cheats" : "Show Shards";
        else
            HideButtonTextUI.text = MyType == PowerMenuType.Crucible ? "Hide Crucible" : Main.DebugSettings.PowerUpCheat ? "Hide Cheats" : "Hide Shards";
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
        r.sizeDelta = new Vector2(r.sizeDelta.x, (Hide ? 0 : dist - MyRect.rect.height));
    }
    public void Update() //This is for reload testing, remove when ready to ship
    {
        if (MyType == PowerMenuType.Crucible)
            CrucibleInstance = this;
        if (MyType == PowerMenuType.Shard)
            ShardInstance = this;
    }
}
