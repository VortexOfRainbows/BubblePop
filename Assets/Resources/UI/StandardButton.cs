using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StandardButton : Button
{
    public int Type = 0;
    public int VisualType = 0;
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
        if (VisualType == 0)
        {
            colors.normalColor = ColorHelper.UI.DefaultColor;
            colors.highlightedColor = ColorHelper.New255(0xFD, 0xFF, 0x4A);
            colors.pressedColor = ColorHelper.New255(0xD9, 0xC3, 0x3C);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = ColorHelper.UI.DarkGreyColor;
        }
        else if(VisualType == 1)
        {
            colors.normalColor = ColorHelper.New255(0x6E, 0xCB, 0xDC);
            colors.highlightedColor = ColorHelper.New255(0xFD, 0xFF, 0x4A);
            colors.pressedColor = ColorHelper.New255(0xD9, 0xC3, 0x3C);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = colors.normalColor * 0.6f;
        }
        else if (VisualType == 2)
        {
            colors.normalColor = ColorHelper.New255(0xBF, 0xB8, 0x34);
            colors.highlightedColor = ColorHelper.New255(0xFD, 0xFF, 0x4A);
            colors.pressedColor = ColorHelper.New255(0xD9, 0xC3, 0x3C);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = colors.normalColor * 0.6f;
        }
        base.colors = colors;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if(interactable && SoundOnHover)
        {
            if (Type == 1)
            {
                AudioManager.PlaySound(SoundID.BubblePop, CameraManager.MainCamera.transform.position, 1, 1.1f, 1);
            }
        }
    }
    public void Update()
    {
        if(Type == 1)
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