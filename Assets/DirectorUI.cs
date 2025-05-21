using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DirectorUI : MonoBehaviour
{
    public TextMeshProUGUI DirectorText;
    public TextMeshProUGUI DirectorPlayDelay;
    public void FixedUpdate()
    {
        DirectorText.text = $"Director Credits: {(int)WaveDirector.Credits}";
        DirectorPlayDelay.text = $"Play Delay: {(int)WaveDirector.PlayRecoil}";
    }
}
