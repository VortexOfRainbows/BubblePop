using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderInputField : MonoBehaviour
{
    public int Type;
    public Slider Slider;
    public TMP_InputField InputField;
    public void TryParseSlider(float coefficient)
    {
        int percentString = (int)(coefficient * 100 + 0.05f);
        InputField.text = percentString.ToString() + '%';
        Slider.value = coefficient;
        UpdateSetting(coefficient);
    }
    public void TryParseInput(string Input)
    {
        try
        {
            float value = int.Parse(Input);
            if (value > 100)
                value = 100;
            InputField.text = value.ToString() + '%';
            Slider.value = value / 100f;
            UpdateSetting(Slider.value);
        }
        catch
        {
            Debug.Log("Failed to parse text input into num");
        }
    }
    public void Start()
    {
        LoadSetting();
    }
    public void LoadSetting()
    {
        if(Type == 0)
        {
            TryParseSlider(PlayerData.SFXVolume);
        }
        if (Type == 1)
        {
            TryParseSlider(PlayerData.MusicVolume);
        }
    }
    public void UpdateSetting(float value)
    {
        if(Type == 0)
        {
            PlayerData.SFXVolume = value;
            PlayerData.SaveSound();
        }
        if (Type == 1)
        {
            PlayerData.MusicVolume = value;
            PlayerData.SaveSound();
        }
    }
}
