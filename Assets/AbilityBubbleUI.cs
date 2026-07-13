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
    public Image RadialFill;
    public Image LinearFill;
    public TextMeshProUGUI CountText;
    public void Update()
    {
        FillPercent = MyAbility.ProgressDisplay;
        FillPercent = Mathf.Clamp01(FillPercent);
        RadialFill.fillAmount = 1 - FillPercent;
        LinearFill.fillAmount = FillPercent;
        int num = MyAbility.NumberDisplay;
        if (num >= 0)
        {
            CountText.gameObject.SetActive(true);
            CountText.text = num.ToString();
        }
        else
            CountText.gameObject.SetActive(false);
    }
}
