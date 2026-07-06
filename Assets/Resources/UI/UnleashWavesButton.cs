using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnleashWavesButton : MonoBehaviour
{
    public TextMeshProUGUI AscensionText;
    public TextMeshProUGUI AscensionNum;
    public Button StartButton;
    public Image StartButtonImage;
    public GameObject StartButtonCoinVisual;
    public TextMeshProUGUI Text;
    public Image PylonVisual;
    public Image InteractVisual;
    public Canvas MyCanvas;
    public int LoadedAscLevel { get; private set; } = -1;
    public bool CanAfford => true; // (CoinManager.TotalEquipCost <= CoinManager.Savings || CoinManager.TotalEquipCost <= 0);
    public bool CanUse => (PylonVisual == null || Main.PlayerNearPylon);
    public void SimulatePress()
    {
        Main.StartGame();
        Main.CanvasManager.StaticPlaySound();
    }
    public void Update()
    {
        StartButton.interactable = CanAfford && CanUse;
        if (StartButton.interactable)
        {
            StartButtonImage.color = new Color(1, 1, 1, 0.8f);
            InteractVisual.color = ColorHelper.UI.DefaultColor;
            Text.color = Color.white;
            if (Control.Interact)
                SimulatePress();
        }
        else
        {
            InteractVisual.color = Color.Lerp(new Color(1, 1, 1, 0.4f), new Color(0.9f, 0.0f, 0.0f, 0.25f), 0.5f);
            StartButtonImage.color = Color.Lerp(new Color(1, 1, 1, 0.8f), new Color(0.9f, 0.0f, 0.0f, 0.8f), 0.5f);
            Text.color = CanAfford ? Color.white : Color.red;
        }
        if (Utils.IsMouseHoveringOverThis(true, StartButton.GetComponent<RectTransform>(), 0, MyCanvas))
        {
            if (!StartButton.interactable)
            {
                if(Player.AllPlayers.Count > 1)
                    PopUpTextUI.Enable("All players must be near a pylon to begin!".WithColor(ColorHelper.RarityColorHex[5]), " ");
                else
                    PopUpTextUI.Enable("Must be near a pylon to begin!".WithColor(ColorHelper.RarityColorHex[5]), "");
            }
            Player.Instance.Control.BlockAttack = true;
        }
        UpdateAscensionDisplay();
        PylonUpdate();
        //StartButtonCoinVisual.SetActive(CoinManager.TotalEquipCost > 0);
        //Text.text = $"${CoinManager.TotalEquipCost}";
    }
    public void UpdateAscensionDisplay()
    {
        AscensionText.transform.parent.gameObject.SetActive(Player.PlayerHighestAscensionAvailable() != 0);
        if (LoadedAscLevel != Player.AscensionLevel)
        {
            LoadedAscLevel = Player.AscensionLevel;
            AscensionNum.text = LoadedAscLevel.ToString();
            AscensionText.text = $"<size=32><color={ColorHelper.AscColorHex}>{Localization.Get($"Common.Asc{LoadedAscLevel}Title")}</color></size>\n{(LoadedAscLevel <= 1 ? "" : "+")}{Localization.Get($"Common.Asc{LoadedAscLevel}Description")}";
        }
        if (Utils.IsMouseHoveringOverThis(true, AscensionText.transform.parent.GetComponent<RectTransform>(), 0, MyCanvas))
            Player.Instance.Control.BlockAttack = true;
    }
    public void PylonUpdate()
    {
        if (PylonVisual == null)
            return;
        if(Main.PlayerNearPylon)
            PylonVisual.color = Color.white;
        else
            PylonVisual.color = new Color(0.75f, 0.75f, 0.75f, 0.75f);
    }
}
