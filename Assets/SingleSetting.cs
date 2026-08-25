using System;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class SingleSetting : MonoBehaviour
{
    public Image BGImage;
    public TextMeshProUGUI Label;
    public Toggle Toggle;
    public Slider Slider;
    public StandardButton DiscreteButton;
    public TMP_InputField SliderInputField;
    public TextMeshProUGUI DiscreteValueDisplayField;
    public Image BoolModeImage;
    public static SingleSetting RequestNewSetting(Transform parent)
    {
        return Instantiate(Resources.Load<GameObject>("UI/Settings/SingleSetting"), parent).GetComponent<SingleSetting>();
    }
    public static GameObject RequestDivider(Transform parent, string text)
    {
        GameObject g = Instantiate(Resources.Load<GameObject>("UI/Settings/SettingsDivider"), parent);
        g.GetComponentInChildren<TextMeshProUGUI>().text = text;
        return g;
    }
    public class SettingBinder<T>
    {
        public SettingBinder(Func<T> get, Action<T> set)
        {
            Setter = set;
            Getter = get;
        }
        public T Setting { get => Getter(); set => Setter(value); }
        public Func<T> Getter;
        public Action<T> Setter;
    }
    public enum SettingType
    {
        Toggle = 0,
        Slider = 1,
        Discrete = 2,
    }
    public SettingBinder<bool> ToggleBinder;
    public SettingBinder<float> SliderBinder;
    public SettingBinder<int> DiscreteBinder;
    public SettingType MyType { get; private set; }
    public int DiscreteValueNonInclusiveUpperBound { get; private set; }
    //public void Assign(Func<int> get, Action<int> set, int totalValues = 3)
    //{
    //    MyType = SettingType.Discrete;
    //    DiscreteBinder = new(get, set);
    //    //not implemented exception
    //    throw new NotImplementedException();
    //}
    #region Toggle
    public void Assign(string Label, Func<bool> get, Action<bool> set)
    {
        this.Label.text = Label;
        BGImage.sprite = Main.TextureAssets.SecondaryUISquare;
        MyType = SettingType.Toggle;
        ToggleBinder = new(get, set);
        LoadToggleSetting();
        Toggle.gameObject.SetActive(true);
        BoolModeImage.gameObject.SetActive(true);
        BoolModeImage.color = ColorHelper.Cyan.WithAlpha(0.5f);
    }
    public void OnToggle(bool value)
    {
        AudioManager.PlaySound(SoundID.BubblePop, CameraManager.MainCamera.transform.position, 1, 1.1f, 1);
        ToggleBinder.Setting = value;
        PlayerData.SaveSettingsToggles();
    }
    public void LoadToggleSetting()
    {
        Toggle.isOn = ToggleBinder.Setting;
    }
    #endregion
    #region Slider
    public void Assign(string Label, Func<float> get, Action<float> set, float min = 0, float max = 1)
    {
        this.Label.text = Label;
        BGImage.sprite = Main.TextureAssets.SecondaryUISquare;
        MyType = SettingType.Slider;
        SliderBinder = new(get, set);
        Slider.minValue = min;
        Slider.maxValue = max;
        LoadSliderSetting(); //Retrieve setting from save
        Slider.gameObject.SetActive(true);
        SliderInputField.gameObject.SetActive(true);
    }
    public void LoadSliderSetting() => OnSliderChange(SliderBinder.Setting);
    public void UpdateSetting(float value)
    {
        SliderBinder.Setting = value;
        PlayerData.SaveSettingSliders();
    }
    public void OnSliderChange(float value)
    {
        int percentString = (int)(value * 100 + 0.05f);
        SliderInputField.text = percentString.ToString() + '%';
        Slider.value = value;
        UpdateSetting(value);
    }
    public void FinishSliderChange()
    {
        AudioManager.PlaySound(SoundID.BubblePop, CameraManager.MainCamera.transform.position, 1, 1.1f, 1);
    }
    public void OnSliderFieldChange(string input)
    {
        try
        {
            float value = int.Parse(input);
            if (value > 100)
                value = 100;
            Slider.value = value / 100f;
            SliderInputField.text = (Slider.value * 100).ToString() + '%';
            UpdateSetting(Slider.value);
            FinishSliderChange();
        }
        catch
        {
            Debug.Log("Failed to parse text input into num");
        }
    }
    #endregion
    #region Discrete
    public string[] DiscreteOptionsNameDisplay;
    public SingleSetting Assign(Func<int> get, Action<int> set, params string[] ModeNames)
    {
        MyType = SettingType.Discrete;
        BGImage.sprite = Main.TextureAssets.SecondaryUISquare; //move to other class laterAssets/Resources/UI/Boxes/ReverseUISquare.PNG
        DiscreteBinder = new(get, set);
        DiscreteButton.gameObject.SetActive(true);
        DiscreteButton.onClick.AddListener(OnButtonCycle);
        ResetDiscreteModeNames(ModeNames);
        //Make sure loading happens after the upper bound is initialized
        LoadDiscreteSetting();
        return this;
    }
    public void ResetDiscreteModeNames(string[] ModeNames)
    {
        DiscreteOptionsNameDisplay = ModeNames;
        DiscreteValueNonInclusiveUpperBound = ModeNames.Length;
    }
    public bool HasSpecialColors { get; private set; } = false;
    public SingleSetting WithSpecialColors()
    {
        HasSpecialColors = true;
        return this;
    }
    public void LoadDiscreteSetting()
    {
        DiscreteBinder.Setting %= DiscreteValueNonInclusiveUpperBound;
        if(DiscreteValueNonInclusiveUpperBound == 4 && HasSpecialColors) //THIS IS FOR TIME OF DAY SETTINGS
        {
            var colors = DiscreteButton.colors;
            colors.normalColor = ColorHelper.GetTimeOfDayUIColor(DiscreteBinder.Setting);
            colors.highlightedColor = ColorHelper.UI.SelectColor;
            colors.pressedColor = colors.highlightedColor * new Color(0.858f, 0.765f, 0.811f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = colors.normalColor * 0.6f;
            DiscreteButton.colors = colors;
        }
        DiscreteValueDisplayField.text = (DiscreteBinder.Setting + (HasSpecialColors ? 1 : 0)).ToString();
        Label.text = DiscreteOptionsNameDisplay[DiscreteBinder.Setting];
    }
    public void OnButtonCycle()
    {
        DiscreteBinder.Setting = DiscreteBinder.Setting + 1;
        LoadDiscreteSetting();
        PlayerData.SaveSettingSliders();
    }
    #endregion
}