using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePowerMenu : MonoBehaviour
{
    public static ChoicePowerMenu Instance;
    public static PowerUpButton[] PowerButtons => Instance.Buttons;
    public PowerUpButton[] Buttons = new PowerUpButton[5];
    public Canvas MyCanvas;
    public Button RerollButton, HideButton;
    public GameObject GemParent;
    public TextMeshProUGUI GemCostUI, RemainingUI, HideButtonUI;
    public HorizontalLayoutGroup Layout;
    public int Cost { get; set; } = 3;
    public int RemainingRerolls { get; set; } = 1;
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
        Instance.transform.localPosition = new Vector2(Instance.MyCanvas.pixelRect.width / 2, - Instance.MyCanvas.pixelRect.height / 2);
        CoinManager.ModifyGems(-Cost);
        Cost++;
        RemainingRerolls--;
        for (int i = 0; i < 5; i++)
        {
            if (PowerButtons[i].Active)
                PowerButtons[i].TurnOn();
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && PowerUp.PickingPowerUps)
        {
            Hide = !Hide;
            if(!Hide && PlayerData.PauseDuringPowerSelect)
                Main.PauseGame();
            else
                Main.UnpauseGame();
        }
        float lerpT = Utils.DeltaTimeLerpFactor(0.125f);
        if (Hide)
        {
            transform.LerpLocalPosition(new Vector2(MyCanvas.pixelRect.width / 2, 790 - MyCanvas.pixelRect.height / 2), lerpT);
            HideButton.transform.LerpLocalPosition(new Vector2(0, -260), lerpT);
            RerollButton.transform.LerpLocalPosition(new Vector2(0, -140), lerpT);
            HideButtonUI.text = "Show Choices";
            return;
        }
        else
        {
            transform.LerpLocalPosition(new Vector2(MyCanvas.pixelRect.width / 2, 60 - MyCanvas.pixelRect.height / 2), lerpT);
            HideButton.transform.LerpLocalPosition(new Vector2(110, -140), lerpT);
            RerollButton.transform.LerpLocalPosition(new Vector2(-110, -140), lerpT);
            HideButtonUI.text = "Hide Choices";
        }
        if (CoinManager.CurrentGems >= Cost) //Can afford
        {
            RerollButton.interactable = RemainingRerolls > 0;
            GemCostUI.color = ColorHelper.UIDefaultColor;
            //RerollButton.targetGraphic.color = ColorHelper.UIDefaultColor.WithAlpha(0.5f);
        }
        else //Cannot Afford
        {
            RerollButton.interactable = false;
            GemCostUI.color = ColorHelper.UIRedColor;
            //RerollButton.targetGraphic.color = ColorHelper.UIDefaultColor.WithAlpha(0.2f);
        }
        GemCostUI.text = Cost.ToString();
        RemainingUI.text = $"Remaining: {RemainingRerolls}";
        if(RemainingRerolls > 0)
            RemainingUI.color = ColorHelper.UIDefaultColor;
        else
        {
            RemainingUI.color = ColorHelper.UIRedColor;
        }

        if (RerollButton.interactable)
        {
            if (Input.GetKeyDown(KeyCode.R) && PowerUp.PickingPowerUps)
                Reroll();
        }
        else
        {

        }
    }
    public static void TurnOn(bool ExtraChoices)
    {
        Hide = false;
        Instance.RemainingRerolls = 1;
        Instance.transform.localPosition = new Vector2(Instance.MyCanvas.pixelRect.width / 2, - Instance.MyCanvas.pixelRect.height / 2);
        Instance.gameObject.SetActive(true);
        int max = ExtraChoices ? 5 : 3;
        var r = Instance.Layout.GetComponent<RectTransform>();
        r.sizeDelta = new Vector2(ExtraChoices ? 1300 : 900, r.sizeDelta.y);
        for (int i = 0; i < max; i++)
        {
            if (!PowerButtons[i].Active)
            {
                PowerButtons[i].TurnOn();
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
