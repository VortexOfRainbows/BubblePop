using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PricedButton : MonoBehaviour
{
    public Button StartButton;
    public Image StartButtonImage;
    public GameObject StartButtonCoinVisual;
    public TextMeshProUGUI Text;
    public Image PylonVisual;
    public Image InteractVisual;
    public Canvas MyCanvas;
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
            InteractVisual.color = ColorHelper.UIGreyColor;
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
        PylonUpdate();
        //StartButtonCoinVisual.SetActive(CoinManager.TotalEquipCost > 0);
        //Text.text = $"${CoinManager.TotalEquipCost}";
    }
    public void FixedUpdate()
    {
        if (Utils.IsMouseHoveringOverThis(true, StartButton.GetComponent<RectTransform>(), 0, MyCanvas))
            Main.MouseHoveringOverButton = true;
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
