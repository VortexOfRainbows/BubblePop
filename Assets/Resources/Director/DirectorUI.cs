using TMPro;
using UnityEngine;

public class DirectorUI : MonoBehaviour
{
    public TextMeshProUGUI DirectorText;
    public TextMeshProUGUI DirectorPlayDelay;
    public TextMeshProUGUI CardsPlayed;
    public TextMeshProUGUI Points;
    public DirectorCardVisualizer[] cards;
    public void FixedUpdate()
    {
        DirectorText.text = $"Director Credits: {(int)WaveDirector.Credits}";
        DirectorPlayDelay.text = $"Play Delay: {(int)WaveDirector.PlayRecoil}";
        CardsPlayed.text = $"Cards This Wave: {(int)WaveDirector.CardsPlayed}";
        Points.text = $"Points: N/A";
        for (int i = 0; i < cards.Length; ++i)
        {
            cards[i].UpdateVisual(i);
        }
    }
}
