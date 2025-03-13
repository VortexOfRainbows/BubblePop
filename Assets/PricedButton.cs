using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PricedButton : MonoBehaviour
{
    public Button StartButton;
    public Image StartButtonImage;
    public GameObject StartButtonCoinVisual;
    public TextMeshProUGUI Text;
    public bool CanAfford => CoinManager.TotalEquipCost <= CoinManager.Savings || CoinManager.TotalEquipCost <= 0;
    public void Update()
    {
        StartButton.interactable = CanAfford;
        if (StartButton.interactable)
        {
            StartButtonImage.color = new Color(1, 1, 1, 0.8f);
            Text.color = Color.white;
        }
        else
        {
            StartButtonImage.color = Color.Lerp(new Color(1, 1, 1, 0.8f), new Color(0.9f, 0.0f, 0.0f, 0.8f), 0.5f);
            Text.color = Color.red;
        }
        StartButtonCoinVisual.SetActive(CoinManager.TotalEquipCost > 0);
        Text.text = CoinManager.TotalEquipCost.ToString();
    }
}
