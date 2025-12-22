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
    private bool Loaded = false;
    public void Update()
    {
        if(!Loaded)
        {
            LoadSetting();
            Loaded = true;
        }
    }
    public void LoadSetting()
    {
        if(Type == 0)
        {
            TryParseSlider(PlayerData.SFXVolume);
        }
        else if (Type == 1)
        {
            TryParseSlider(PlayerData.MusicVolume);
        }
        else if (Type == 2)
        {
            TryParseSlider(PlayerData.SpecialVisualOpacity);
        }
    }
    public void UpdateSetting(float value)
    {
        if(Type == 0)
        {
            PlayerData.SFXVolume = value;
            PlayerData.SaveSettingSliders();
        }
        else if (Type == 1)
        {
            PlayerData.MusicVolume = value;
            PlayerData.SaveSettingSliders();
        }
        else if (Type == 2)
        {
            if (value < 0.2f)
            {
                value = 0.2f;
                TryParseSlider(value);
            }
            else
            {
                PlayerData.SpecialVisualOpacity = value;
                PlayerData.SaveSettingSliders();
            }
        }
    }
}
