using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBubbleUI : MonoBehaviour
{
    public Ability MyAbilityToDisplay_TEMP()
    {
        return Player.Instance.Body.GetAbility()[0];
    }
    public Ability MyAbility => MyAbilityToDisplay_TEMP();
    public float FillPercent = 0;
    public Image BarFill;
    public Image BottleFill;
    public TextMeshProUGUI CountText;
    public Image[] PrimaryColorImages;
    public Image BarTop;
    public Image BarBottom;
    public float OscillationTimer = 0;
    public void Update()
    {
        FillPercent = Mathf.Lerp(FillPercent, MyAbility.ProgressDisplay, Utils.DeltaTimeLerpFactor(Mathf.Clamp01(0.1f + 2f * Mathf.Abs(FillPercent - MyAbility.ProgressDisplay))));
        FillPercent = Mathf.Clamp01(FillPercent);
        BarFill.fillAmount = 1 - FillPercent;
        BottleFill.fillAmount = FillPercent;
        int num = MyAbility.NumberDisplay;
        if (num >= 0)
        {
            CountText.gameObject.SetActive(true);
            CountText.text = num.ToString();
        }
        else
            CountText.gameObject.SetActive(false);
        UpdateColor();
    }
    public void UpdateColor()
    {
        Player localPlayer = Player.Instance;
        Color borderColor = Player.SecondaryProjectileColor;
        Color barColor = new(1, 1, 0f, 1);
        Color barBottomColor = new(barColor.r * .9f, barColor.g * .75f, barColor.b * .4f, 1);
        if(Player.Instance != null)
        {
            if (localPlayer.Body is Gachapon)
            {
                borderColor = ColorHelper.RarityColors[4].Lerp(Color.yellow, 0.5f);
                barColor = ColorHelper.ChipColor * 1.4f;
                barBottomColor = ColorHelper.ChipColor * 0.9f;
            }
            else if (localPlayer.Body is Fizzy)
            {
                borderColor = ColorHelper.ColaColor * 0.85f;
            }
            else if (localPlayer.Body is Bubblemancer)
            {
                borderColor = ColorHelper.RarityColors[2];
            }
            else if (localPlayer.Body is KingOil)
            {
                borderColor = ColorHelper.KingOilColor.Lerp(Color.black, 0.2f);
                barColor = ColorHelper.SentinelPurple;
                barBottomColor = ColorHelper.SentinelPurple * 0.7f;
            }
            else if (localPlayer.Body is ThoughtBubble)
            {
                borderColor = ColorHelper.RarityColors[3] * .8f;
                barColor = Color.white * 0.95f;
                barBottomColor = Color.white * 0.7f;
            }
        }
        foreach (Image i in PrimaryColorImages)
            i.color = borderColor.WithAlpha(1);
        BarTop.color = barColor.WithAlpha(1);
        BarBottom.color = barBottomColor.WithAlpha(1);
        if(MyAbility.ProgressDisplay <= 0)
        {
            float osc = -Mathf.Cos(OscillationTimer * Mathf.PI) * 0.2f + 0.2f;
            BarTop.color = barColor.WithAlpha(1).Lerp(Color.white, osc);
            BarBottom.color = barBottomColor.WithAlpha(1).Lerp(Color.white, osc);
            OscillationTimer += Time.deltaTime * 5;
        }
        else
        {  
            OscillationTimer = 0; 
        }
    }
}
