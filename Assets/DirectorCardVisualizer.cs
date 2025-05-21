using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DirectorCardVisualizer : MonoBehaviour
{
    public TextMeshProUGUI Cost;
    public TextMeshProUGUI Enemy;
    public Image Fill;
    public void UpdateVisual(int i)
    {
        if(WaveDirector.Deck.Count <= i)
        {
            Cost.text = "N/A";
            Enemy.text = "N/A";
            Fill.fillAmount = 1;
            return;
        }
        WaveCard card = WaveDirector.Deck[i];
        Cost.text = card.Cost.ToString();
        Enemy.text = card.Patterns[0].EnemyPrefabs[0].GetComponent<Enemy>().name;
        Fill.fillAmount = card.mulliganDelay / card.fullDelay;
    }
}
