using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StandardButton : Button
{
    public enum ButtonAnimationType
    {
        None = 0,
        ScaleUp = 1,
    }
    public enum ButtonColorType
    {
        WhiteToYellow = 0,
        CyanToYellow = 1,
        DarkYellowToYellow = 2,
    }
    public enum ButtonDestinationType
    {
        None = 0,
        Play = 1,
        LocalMultiplayer = 2,
        Resume = 3,
        ReturnToMenu = 4,
        Restart = 5,
        UnleashWave = 6,
        Settings = 7,
        Quit = 8,
        DebugMenu = 9,
        CloseTutorial = 10,
        OpenCompendium = 11,
    }
    public static Dictionary<ButtonDestinationType, UnityEngine.Events.UnityAction> ButtonActions;
    private static Dictionary<ButtonDestinationType, UnityEngine.Events.UnityAction> InitDict()
    {
        Dictionary<ButtonDestinationType, UnityEngine.Events.UnityAction> ButtonToActionDict = new();
        ButtonToActionDict[ButtonDestinationType.None] = DoNothing;
        ButtonToActionDict[ButtonDestinationType.Play] = Main.CanvasManager.Play;
        ButtonToActionDict[ButtonDestinationType.LocalMultiplayer] = Main.CanvasManager.PlayMult;
        ButtonToActionDict[ButtonDestinationType.Resume] = Main.CanvasManager.Resume;
        ButtonToActionDict[ButtonDestinationType.ReturnToMenu] = Main.CanvasManager.MainMenu;
        ButtonToActionDict[ButtonDestinationType.Restart] = Main.CanvasManager.Restart;
        ButtonToActionDict[ButtonDestinationType.UnleashWave] = Main.StartGame;
        ButtonToActionDict[ButtonDestinationType.Settings] = Main.CanvasManager.ToggleSettings;
        ButtonToActionDict[ButtonDestinationType.Quit] = Main.CanvasManager.QuitGame;
        ButtonToActionDict[ButtonDestinationType.DebugMenu] = Main.CanvasManager.OpenDebugMenu;
        ButtonToActionDict[ButtonDestinationType.CloseTutorial] = Main.CanvasManager.CloseMultiplayerMenu;
        ButtonToActionDict[ButtonDestinationType.OpenCompendium] = Compendium.StaticToggleActive;
        return ButtonToActionDict;
    }
    private static void DoNothing()
    {

    }
    public static void RegisterButtonBehavior(StandardButton button)
    {
        ButtonActions ??= InitDict();
        button.onClick.AddListener(ButtonActions[button.DestinationType]);
    }
    public ButtonAnimationType AnimationType = ButtonAnimationType.None;
    public ButtonColorType ColorType = ButtonColorType.WhiteToYellow;
    public ButtonDestinationType DestinationType = ButtonDestinationType.None;
    public bool SoundOnHover = true;
    public new void Awake()
    {
        base.Awake();
        SetDefaults();
    }
    public void SetDefaults()
    {
        base.transition = Transition.ColorTint;

        //Navigation OFF for now, though this may change to support controller navigation in the future
        Navigation nav = base.navigation;
        nav.mode = Navigation.Mode.None;
        base.navigation = nav;

        var colors = this.colors;
        colors.colorMultiplier = 1.0f;
        colors.fadeDuration = 0.1f;
        if (ColorType == ButtonColorType.WhiteToYellow)
        {
            colors.normalColor = ColorHelper.UI.DefaultColor;
            colors.highlightedColor = ColorHelper.New255(0xFD, 0xFF, 0x4A);
            colors.pressedColor = ColorHelper.New255(0xD9, 0xC3, 0x3C);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = ColorHelper.UI.DarkGreyColor;
        }
        else if(ColorType == ButtonColorType.CyanToYellow)
        {
            colors.normalColor = ColorHelper.New255(0x6E, 0xCB, 0xDC);
            colors.highlightedColor = ColorHelper.New255(0xFD, 0xFF, 0x4A);
            colors.pressedColor = ColorHelper.New255(0xD9, 0xC3, 0x3C);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = colors.normalColor * 0.6f;
        }
        else if (ColorType == ButtonColorType.DarkYellowToYellow)
        {
            colors.normalColor = ColorHelper.New255(0xBF, 0xB8, 0x34);
            colors.highlightedColor = ColorHelper.New255(0xFD, 0xFF, 0x4A);
            colors.pressedColor = ColorHelper.New255(0xD9, 0xC3, 0x3C);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = colors.normalColor * 0.6f;
        }
        base.colors = colors;

        RegisterButtonBehavior(this);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if(interactable && SoundOnHover)
        {
            if (AnimationType == ButtonAnimationType.ScaleUp)
            {
                AudioManager.PlaySound(SoundID.BubblePop, CameraManager.MainCamera.transform.position, 1, 1.1f, 1);
            }
        }
    }
    private static Button ArbitrarySceneResumeButton = null;
    public void Update()
    {
        if(AnimationType == ButtonAnimationType.ScaleUp)
        {
            if (IsHighlighted())
            {
                transform.LerpLocalScale(Vector2.one * 1.1f, Utils.DeltaTimeLerpFactor(0.16f));
            }
            else
            {
                transform.LerpLocalScale(Vector2.one, Utils.DeltaTimeLerpFactor(0.12f));
            }
        }
        if(DestinationType == ButtonDestinationType.DebugMenu && Main.DebugCheats)
        {
            interactable = true;
            GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }

        //ON GAME OVER (THIS IS PROBABLY UNNECESSARY AND SHOULD BE REWORKED)
        if (Player.Instance != null && Player.Instance.IsDead)
        {
            if (DestinationType == ButtonDestinationType.Settings)
            {
                gameObject.SetActive(false);
            }
            if (DestinationType == ButtonDestinationType.Resume)
            {
                gameObject.SetActive(false);
                ArbitrarySceneResumeButton = this;
            }
            if (DestinationType == ButtonDestinationType.Restart)
            {
                if (ArbitrarySceneResumeButton != null)
                {
                    GetComponent<RectTransform>().sizeDelta = ArbitrarySceneResumeButton.GetComponent<RectTransform>().sizeDelta;
                    GetComponent<RectTransform>().pivot = ArbitrarySceneResumeButton.GetComponent<RectTransform>().pivot;
                    transform.localPosition = ArbitrarySceneResumeButton.transform.localPosition;
                }
                GetComponentInChildren<TextMeshProUGUI>().text = "Try Again";
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(StandardButton))]
[CanEditMultipleObjects]
public class StandardButtonEditor : UnityEditor.UI.ButtonEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the original Button inspector (specifically onclick, which is the only non-standed field for now)
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Interactable"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TargetGraphic"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnClick"));

        //If other fields are needed look here: 
        //base.OnInspectorGUI();

        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;
        while (prop.NextVisible(enterChildren))
        {
            // Skip script reference and already drawn Button fields
            if (prop.name == "m_Script" || prop.name.StartsWith("m_") || prop.name == "size")
                continue;

            EditorGUILayout.PropertyField(prop, true);
            enterChildren = false;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif