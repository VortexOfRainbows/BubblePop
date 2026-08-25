using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu Instance { get; private set; }
    public static StandardButton UndoButton => Instance.BackButton;
    public StandardButton BackButton;
    public CanvasGroup MyGroup;
    public GameObject Visual;
    public Transform AudioLayout;
    public Transform GameplayLayout;
    public Transform GraphicsLayout;
    public static void SwapToAudio()
    {
        Instance.AudioLayout.gameObject.SetActive(true);
        Instance.GameplayLayout.gameObject.SetActive(false);
        Instance.GraphicsLayout.gameObject.SetActive(false);
    }
    public static void SwapToGameplay()
    {
        Instance.GameplayLayout.gameObject.SetActive(true);
        Instance.AudioLayout.gameObject.SetActive(false);
        Instance.GraphicsLayout.gameObject.SetActive(false);
    }
    public static void SwapToGraphics()
    {
        Instance.GraphicsLayout.gameObject.SetActive(true);
        Instance.AudioLayout.gameObject.SetActive(false);
        Instance.GameplayLayout.gameObject.SetActive(false);
    }
    public static void ToggleVisibility(bool? state = null)
    {
        if (state.HasValue)
            Instance.WantsVisible = state.Value;
        else
            Instance.WantsVisible = !Instance.WantsVisible;
        if (!IsVisible)
            Instance.MyGroup.alpha = 0;
    }
    public static bool IsVisible => Instance.Visual.activeSelf;
    public bool WantsVisible = false;
    public static void StaticUpdate()
    {
        Instance.SUpdate();
    }
    public void SUpdate()
    {
        float lerpFactor = Utils.DeltaTimeLerpFactor(0.1f);
        if (WantsVisible)
        {
            Utils.LerpSnap(Visual.transform, new Vector2(0, 0), lerpFactor, 1);
            MyGroup.alpha += 10 * Time.unscaledDeltaTime;
        }
        else
        {
            Utils.LerpSnap(Visual.transform, new Vector2(0, -20), lerpFactor, 1);
            Instance.MyGroup.alpha -= 10 * Time.unscaledDeltaTime;
        }
        MyGroup.alpha = Mathf.Clamp01(Instance.MyGroup.alpha);
        Visual.SetActive(Instance.MyGroup.alpha > 0);

        bool backButtonShouldAppear = Main.BackButtonShouldAppear();
        ////Probably change this later when compendium is more standardized?
        //if (BackButton.transform.localPosition.y > -50) //Only if the button is off screen do we want it to shift x positions
        //    BackButtonX = Compendium.Instance.Active ? 20 : 0;
        Vector2 pos = backButtonShouldAppear ? new Vector2(0, -200) : new Vector2(0, 0);
        Utils.LerpSnap(BackButton.transform, pos, lerpFactor);
    }
    public void Init()
    {
        Instance = this;
        WantsVisible = false;
        MyGroup.alpha = 0;
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
        SingleSetting.RequestDivider(GraphicsLayout, "Graphics");
        SingleSetting.RequestNewSetting(GraphicsLayout).Assign(() => PlayerData.LightingSetting, val => PlayerData.LightingSetting = val, "Sunlight: Standard", "Sunlight: Everday", "Sunlight: Evernight", "Sunlight: Real-time").WithSpecialColors().LoadDiscreteSetting();
        SingleSetting.RequestNewSetting(GraphicsLayout).Assign(() => PlayerData.ShadowBlurValue, val => PlayerData.ShadowBlurValue = val, "Shadow Blur: None", "Shadow Blur: Fast", "Shadow Blur: Soft", "Shadow Blur: Softest");
    }
}
