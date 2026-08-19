using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu Instance { get; private set; }
    public CanvasGroup MyGroup;
    public Transform AudioLayout;
    public Transform GameplayLayout;
    public static void SwapToAudio()
    {
        Instance.AudioLayout.gameObject.SetActive(true);
        Instance.GameplayLayout.gameObject.SetActive(false);
    }
    public static void SwapToGameplay()
    {
        Instance.AudioLayout.gameObject.SetActive(false);
        Instance.GameplayLayout.gameObject.SetActive(true);
    }
    public static void ToggleVisibility(bool? state = null)
    {
        if(state.HasValue)
            Instance.gameObject.SetActive(state.Value);
        else
            Instance.gameObject.SetActive(!Instance.gameObject.activeSelf);
    }
    public static bool IsVisible => Instance.gameObject.activeSelf;
    public void Init()
    {
        Instance = this;
        EstablishConnection();
        SwapToAudio();
    }
    public void EstablishConnection()
    {
        SingleSetting.RequestDivider(AudioLayout, "Audio");
        SingleSetting.RequestNewSetting(AudioLayout).Assign("Sound", () => PlayerData.SFXVolume, val => PlayerData.SFXVolume = val, 0, 1);
        SingleSetting.RequestNewSetting(AudioLayout).Assign("Music", () => PlayerData.MusicVolume, val => PlayerData.MusicVolume = val, 0, 1);
        SingleSetting.RequestDivider(GameplayLayout, "Gameplay");
        SingleSetting.RequestNewSetting(GameplayLayout).Assign("Pause During Power Select", () => PlayerData.PauseDuringPowerSelect, val => PlayerData.PauseDuringPowerSelect = val);
        SingleSetting.RequestNewSetting(GameplayLayout).Assign("Pause During Card Select", () => PlayerData.PauseDuringCardSelect, val => PlayerData.PauseDuringCardSelect = val);
        SingleSetting.RequestNewSetting(GameplayLayout).Assign("Power Descriptions Brief By Default", () => PlayerData.BriefDescriptionsByDefault, val => PlayerData.BriefDescriptionsByDefault = val);
        SingleSetting.RequestNewSetting(GameplayLayout).Assign("Special Visual Opacity", () => PlayerData.SpecialVisualOpacity, val => PlayerData.SpecialVisualOpacity = val, 0.2f, 1);
    }
}
