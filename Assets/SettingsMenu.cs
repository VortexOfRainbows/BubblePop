using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public Transform Layout;
    public void Start()
    {
        EstablishConnection();
    }
    public void EstablishConnection()
    {
        SingleSetting.RequestDivider(Layout, "Audio");
        SingleSetting.RequestNewSetting(Layout).Assign("Sound", () => PlayerData.SFXVolume, val => PlayerData.SFXVolume = val, 0, 1);
        SingleSetting.RequestNewSetting(Layout).Assign("Music", () => PlayerData.MusicVolume, val => PlayerData.MusicVolume = val, 0, 1);
        SingleSetting.RequestDivider(Layout, "Gameplay");
        SingleSetting.RequestNewSetting(Layout).Assign("Pause During Power Select", () => PlayerData.PauseDuringPowerSelect, val => PlayerData.PauseDuringPowerSelect = val);
        SingleSetting.RequestNewSetting(Layout).Assign("Pause During Card Select", () => PlayerData.PauseDuringCardSelect, val => PlayerData.PauseDuringCardSelect = val);
        SingleSetting.RequestNewSetting(Layout).Assign("Power Descriptions Brief By Default", () => PlayerData.BriefDescriptionsByDefault, val => PlayerData.BriefDescriptionsByDefault = val);
        SingleSetting.RequestNewSetting(Layout).Assign("Special Visual Opacity", () => PlayerData.SpecialVisualOpacity, val => PlayerData.SpecialVisualOpacity = val, 0.2f, 1);
    }
}
