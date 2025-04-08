using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SliderInputField : MonoBehaviour
{
    public Slider Slider;
    public TMP_InputField InputField;
    public void TryParseSlider(float coefficient)
    {
        int percentString = (int)(coefficient * 100 + 0.05f);
        InputField.text = percentString.ToString() + '%';
    }
    public void TryParseInput(string Input)
    {
        float value = int.Parse(Input);
        if (value > 100)
            value = 100;
        InputField.text = value.ToString() + '%';
        Slider.value = value / 100f;
    }
}
