using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePowerMenu : MonoBehaviour
{
    public static ChoicePowerMenu Instance;
    public static PowerUpButton[] PowerButtons => Instance.Buttons;
    public static bool IsBlackMarket { get; private set; } = false;
    public PowerUpButton[] Buttons = new PowerUpButton[5];
    public Canvas MyCanvas;
    public Button RerollButton, HideButton;
    public GameObject GemParent;
    public TextMeshProUGUI GemCostUI, RemainingUI, HideButtonUI;
    public HorizontalLayoutGroup Layout;
    public CanvasGroup MyGroup;
    public int Cost => (IsBlackMarket ? 7 : 0) + CostScaling;
    public int CostScaling { get; set; } = 3;
    public static int GetBaseRerolls()
    {
        return 1 + Player.Instance.Eureka;
    }
    public int RemainingRerolls { get; set; } = 1;
    public int RerollsInARow { get; set; } = 0;
    public static bool Hide { get; set; } = false;
    public void Start()
    {
        if(Instance == null)
        {
            foreach(PowerUpButton pb in Buttons)
            {
                pb.Init();
            }
            RerollButton.onClick.AddListener(Reroll);
            gameObject.SetActive(false);
            Instance = this;
        }
    }
    public void Reroll()
    {
        Instance.transform.localPosition = new Vector2(Instance.MyCanvas.GetComponent<RectTransform>().rect.width / 2, - MyCanvas.GetComponent<RectTransform>().rect.height / 2);
        if(Cost > 0)
            CoinManager.ModifyGems(-Cost);
        CostScaling++;
        RemainingRerolls--;
        RerollsInARow++;
        if(RerollsInARow >= 10)
            UnlockCondition.Get<ThoughtBubbleDecisionsDecisions>().SetComplete();
        for (int i = 0; i < 5; i++)
        {
            if (PowerButtons[i].Active)
                PowerButtons[i].TurnOn();
        }
    }
    public void ToggleHide()
    {
        Hide = !Hide;
        if (!Hide && PlayerData.PauseDuringPowerSelect)
            Main.PauseGame();
        else
            Main.UnpauseGame();
        if (!Hide)
            PowerUpCheatUI.CloseAllMenus();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && HideButton.interactable)
            ToggleHide();
        //if(!PowerUpCheatUI.ShardInstance.Hide || !PowerUpCheatUI.CrucibleInstance.Hide)
        //{
        //    HideButton.interactable = false;
        //}
        //else
        //    HideButton.interactable = true;
        float lerpT = Utils.DeltaTimeLerpFactor(0.1f);
        Vector2 defaultPosition = new(MyCanvas.GetComponent<RectTransform>().rect.width / 2, 60 - MyCanvas.GetComponent<RectTransform>().rect.height / 2);
        if (Hide)
        {
            defaultPosition.y -= 20;
            transform.LerpLocalPosition(defaultPosition, lerpT);
            MyGroup.alpha -= 10 * Time.unscaledDeltaTime;
            foreach (PowerUpButton pb in Buttons)
                pb.PowerUI.PreventHovering = true;
        }
        else
        {
            transform.LerpLocalPosition(defaultPosition, lerpT);
            MyGroup.alpha += 10 * Time.unscaledDeltaTime;
            foreach (PowerUpButton pb in Buttons)
                pb.PowerUI.PreventHovering = false;
        }
        MyGroup.blocksRaycasts = !Hide;
        MyGroup.alpha = Mathf.Clamp01(MyGroup.alpha);
        if (CoinManager.CurrentGems >= Cost) //Can afford
        {
            RerollButton.interactable = RemainingRerolls > 0;
            GemCostUI.color = ColorHelper.UI.DefaultColor;
            //RerollButton.targetGraphic.color = ColorHelper.UIDefaultColor.WithAlpha(0.5f);
        }
        else //Cannot Afford
        {
            RerollButton.interactable = false;
            GemCostUI.color = ColorHelper.UI.RedColor;
            //RerollButton.targetGraphic.color = ColorHelper.UIDefaultColor.WithAlpha(0.2f);
        }
        GemCostUI.text = Cost <= 0 ? "Free" : Cost.ToString();
        RemainingUI.text = $"Remaining: {RemainingRerolls}";
        if (RemainingRerolls > 0)
            RemainingUI.color = ColorHelper.UI.DefaultColor;
        else
        {
            RemainingUI.color = ColorHelper.UI.RedColor;
        }

        if (RerollButton.interactable && Input.GetKeyDown(KeyCode.R) && !Hide)
            Reroll();
    }
    public static void TurnOn(bool ExtraChoices, bool BlackMarket)
    {
        IsBlackMarket = BlackMarket;
        Hide = true;
        Instance.ToggleHide();
        Instance.RerollsInARow = 0;
        Instance.RemainingRerolls = GetBaseRerolls();
        Instance.transform.localPosition = new Vector2(Instance.MyCanvas.GetComponent<RectTransform>().rect.width / 2, - Instance.MyCanvas.GetComponent<RectTransform>().rect.height / 2);
        Instance.gameObject.SetActive(true);
        int max = ExtraChoices ? 5 : 3;
        var r = Instance.Layout.GetComponent<RectTransform>();
        r.sizeDelta = new Vector2(ExtraChoices ? 1300 : 900, r.sizeDelta.y);
        for (int i = 0; i < max; i++)
        {
            var p = PowerButtons[i];
            if (!p.Active)
            {
                p.TurnOn();
            }
        }
    }
    public static void TurnOff()
    {
        int max = 5;
        for (int i = 0; i < max; i++)
        {
            if (PowerButtons[i].Active)
            {
                PowerButtons[i].TurnOff();
            }
        }
        Instance.gameObject.SetActive(false);
    }
}
