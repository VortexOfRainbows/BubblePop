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
    public CanvasGroup MyGroup;
    //Serialized
    public PowerMenuType MyType = PowerMenuType.None;
    public bool Hide { get; set; } = true;
    public static PowerUpCheatUI CrucibleInstance { get; set; }
    public static PowerUpCheatUI ShardInstance { get; set; }
    public static int ProcessQuantity { get; set; } = 1;
    public static int UpdatedProcessQuantity { get; set; } = 0;
    public bool MouseInCompendiumArea { get; private set; }
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
    public PowerUpButton ChoiceTemplate, CrucibleTemplate;
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
    public KeyCode Keybind { get; set; }
    public void Start()
    {
        gameObject.SetActive(true);
        NOPOWERS.SetActive(false);
        Hide = true;
        transform.localPosition = new Vector3(Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.width / 2, 140, 0);
        InitializeType();
        QuantitySlider.Update();
        SetHideButtonText();
        ChoiceTemplate.PowerUI.myCanvas = CrucibleTemplate.PowerUI.myCanvas = MyCanvas;
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
    public void ToggleHide(bool pauseBehavior = true)
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
            if(!ChoicePowerMenu.Hide && ChoicePowerMenu.Instance.gameObject.activeSelf)
                ChoicePowerMenu.Instance.ToggleHide();
        }
        else
        {
            ResetPowers();
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
        QuantitySlider.LoadSetting();
        transform.localScale = 0.9f * Vector3.one;
    }
    public void Disable()
    {
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
            yield return new WaitForSecondsRealtime(0.02f);
        AwaitingPowerReset = false;
        for (int i = 0; i < PowerUp.TotalPowerUps; ++i)
        {
            PowerUpButton p = Instantiate(ChoiceTemplate, GridParent.transform);
            p.SetType(i);
            p.gameObject.SetActive(true);
            p.CheatButton = p.NonChoiceButton = p.PowerUI.CrucibleElement = p.PowerUI.IncludeRainbowShards = p.PowerUI.HasIdleAnimation = true;
            p.PowerUI.IdleAnimationOffset = i / 5f + Utils.RandFloat(-0.05f, 0.05f);
            p.PowerUI.Cost = p.PowerUI.MyPower.ShardReplicationCost();
            p.PowerUI.CostText.text = p.PowerUI.Cost.ToString();
            if (i % 3 == 2)
                yield return new WaitForSecondsRealtime(0.01f);
            if(AwaitingPowerReset)
                break;
        }
        yield break;
    }
    public IEnumerator InitCrucibleButtons()
    {
        if (AwaitingPowerReset)
            yield return new WaitForSecondsRealtime(0.02f);
        int RainbowFlowerId = PowerUp.Get<RainbowFlower>().MyID;
        AwaitingPowerReset = false;
        for (int i = 0; i < Player.GlobalPowers.Count; i++)
        {
            PowerUp power = PowerUp.Get(Player.GlobalPowers[i]);
            int powerCost = 0;
            bool useShards = MyType == PowerMenuType.Shard || (powerCost = power.CrucibleGems(true)) < 0;
            PowerUpButton p = Instantiate(useShards ? ChoiceTemplate : CrucibleTemplate, GridParent.transform);
            p.SetType(power.Type);
            p.gameObject.SetActive(true);
            p.PowerUI.Count.gameObject.SetActive(power.Stack > 1);
            p.PowerUI.Count.text = power.Stack.ToString();
            if (MyType == PowerMenuType.Crucible)
            {
                p.Crucible = Crucible.PlayerClosestCrucible;
                p.PowerUI.Cost = powerCost * (useShards ? -1 : 1);
                if (power.MyID == RainbowFlowerId)
                    p.PowerUI.CostText.text = "?";
                else
                    p.PowerUI.CostText.text = p.PowerUI.Cost.ToString();
            }
            else
            {
                p.PowerUI.IncludeRainbowShards = true;
                p.PowerUI.Cost = power.ShardReplicationCost(ProcessQuantity);
                p.PowerUI.CostText.text = p.PowerUI.Cost.ToString();
            }
            p.CheatButton = false;
            p.NonChoiceButton = p.PowerUI.CrucibleElement = p.PowerUI.HasIdleAnimation = true;
            p.PowerUI.IdleAnimationOffset = i / 5f + Utils.RandFloat(-0.05f, 0.05f);
            if (i % 3 == 2)
                yield return new WaitForSecondsRealtime(0.01f);
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
        //if (!ChoicePowerMenu.Hide && ChoicePowerMenu.Instance.gameObject.activeSelf)
        //{
        //    if (!Hide)
        //        ToggleHide(true);
        //    HideButton.interactable = false;
        //}
        //else
        //    HideButton.interactable = true;
        UpdateContentSize();
        MouseInCompendiumArea = !Hide && Utils.IsMouseHoveringOverThis(true, SelectionArea, 0, MyCanvas, false, !gameObject.activeSelf);
        float lerpT = Utils.DeltaTimeLerpFactor(0.1f);
        transform.LerpLocalScale(Vector2.one, lerpT);
        ShardCountTxt.text = Main.DebugSettings.PowerUpCheat ? "Inf" : CoinManager.CurrentShards.ToString();
        Vector2 defaultPos = new(Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.width / 2, -Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.height / 2);
        if (Hide)
        {
            defaultPos.y -= 20;
            transform.LerpLocalPosition(defaultPos, lerpT);
            MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, Mathf.Lerp(MyRect.sizeDelta.y, 0, Utils.DeltaTimeLerpFactor(0.15f)));
            MyGroup.alpha -= 10 * Time.unscaledDeltaTime;
            if(GridParent.transform.childCount > 2 & MyGroup.alpha < 0.1f)
                ResetPowers();
        }
        else
        {
            transform.LerpLocalPosition(defaultPos, lerpT);
            MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, Mathf.Lerp(MyRect.sizeDelta.y, TargetSize, Utils.DeltaTimeLerpFactor(0.15f)));
            MyGroup.alpha += 10 * Time.unscaledDeltaTime;
        }
        MyGroup.blocksRaycasts = !Hide;
        MyGroup.alpha = Mathf.Clamp01(MyGroup.alpha);
    }
    public void SetHideButtonText()
    {
        HideButtonTextUI.text = MyType == PowerMenuType.Crucible ? "Hide Crucible" : Main.DebugSettings.PowerUpCheat ? "Hide Cheats" : "Hide Shards";
    }
    public float TargetSize = 0;
    public void UpdateContentSize()
    {
        int c = GridParent.transform.childCount;
        bool noPowers = c <= 2;
        NOPOWERS.SetActive(!Hide && noPowers && MyGroup.alpha > 0.5f);
        Vector3 lastElement = GridParent.transform.GetChild(c - 1).localPosition;
        RectTransform r = GridParent.GetComponent<RectTransform>();
        float dist = -lastElement.y + GridParent.padding.bottom * 1 + GridParent.cellSize.y * 0.5f;
        float fit3inhere = GridParent.cellSize.y * 3 + GridParent.spacing.y * 2 + GridParent.padding.top + GridParent.padding.bottom;
        float fit1inhere = GridParent.cellSize.y + GridParent.padding.top + GridParent.padding.bottom;
        TargetSize = noPowers ? fit1inhere : Mathf.Min(fit3inhere, dist);
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
