using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementTextTracker : MonoBehaviour
{
    public TextMeshProUGUI text;
    public void Update()
    {
        text.text = $"{PlayerData.MetaProgression.AchievementStars}/{PlayerData.MetaProgression.TotalAchievementStars}";
    }
}
