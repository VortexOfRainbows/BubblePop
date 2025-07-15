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
        {
            Toggle.isOn = PlayerData.PauseDuringPowerSelect;
        }
    }
    public void UpdateSetting(bool value)
    {
        if (Type == 0)
        {
            PlayerData.PauseDuringPowerSelect = value;
            PlayerData.SaveSettings();
        }
    }
}
