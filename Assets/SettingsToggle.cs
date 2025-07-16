using UnityEngine;
using UnityEngine.UI;

public class SettingsToggle : MonoBehaviour
{
    public Toggle Toggle;
    public int Type;
    public void Start()
    {
        LoadSetting();
    }
    public void LoadSetting()
    {
        if (Type == 0)
            Toggle.isOn = PlayerData.PauseDuringPowerSelect;
        if (Type == 1)
            Toggle.isOn = PlayerData.BriefDescriptionsByDefault;
    }
    public void UpdateSetting(bool value)
    {
        if (Type == 0)
        {
            PlayerData.PauseDuringPowerSelect = value;
            PlayerData.SaveSettings();
        }
        if (Type == 1)
        {
            PlayerData.BriefDescriptionsByDefault = value;
            PlayerData.SaveSettings();
        }
    }
}
