using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementTextTracker : MonoBehaviour
{
    public int Type = 0;
    public TextMeshProUGUI text;
    public void Update()
    {
        if (Type == UnlockCondition.RegularAchievement)
            text.text = $"{PlayerData.MetaProgression.AchievementStars}/{PlayerData.MetaProgression.TotalAchievementStars}";
        else if (Type == UnlockCondition.Meadows)
            text.text = $"{PlayerData.MetaProgression.MeadowsStars}/{PlayerData.MetaProgression.TotalMeadowsStars}";
    }
}
