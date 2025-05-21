using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DirectorUI : MonoBehaviour
{
    public TextMeshProUGUI DirectorText;
    public TextMeshProUGUI DirectorPlayDelay;
    public TextMeshProUGUI CardsPlayed;
    public DirectorCardVisualizer[] cards;
    public void FixedUpdate()
    {
        DirectorText.text = $"Director Credits: {(int)WaveDirector.Credits}";
        DirectorPlayDelay.text = $"Play Delay: {(int)WaveDirector.PlayRecoil}";
        CardsPlayed.text = $"Cards This Wave: {(int)WaveDirector.CardsPlayed}";
        for (int i = 0; i < cards.Length; ++i)
        {
            cards[i].UpdateVisual(i);
        }
    }
}
