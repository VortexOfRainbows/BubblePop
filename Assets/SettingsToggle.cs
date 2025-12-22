using UnityEngine;
using UnityEngine.UI;

public class SettingsToggle : MonoBehaviour
{
    public ref bool TargetSetting
    {
        get
        {
            if (Type == -4)
                return ref Main.DebugSettings.SkipWaves;
            if (Type == -3)
                return ref Main.DebugSettings.ForceUnlockAll;
            if (Type == -2)
                return ref Main.DebugSettings.PowerUpCheat;
            if (Type == -1)
                return ref Main.DebugSettings.DirectorView;
            if(Type == 0)
                return ref PlayerData.PauseDuringPowerSelect;
            if (Type == 1)
                return ref PlayerData.BriefDescriptionsByDefault;
            if (Type == 2)
                return ref PlayerData.PauseDuringCardSelect;
            throw new System.Exception("Settings Toggle Has Invalid Type");
        }
    }
    public Toggle Toggle;
    public int Type;
    public void Start()
    {
        LoadSetting();
    }
    public void Update()
    {
        if(Type < 0)
            LoadSetting();
    }
    public void LoadSetting()
    {
        Toggle.isOn = TargetSetting;
    }
    public void UpdateSetting(bool value)
    {
        TargetSetting = value;
        if (Type == 0 || Type == 1 || Type == 2)
            PlayerData.SaveSettingsToggles();
    }
}
