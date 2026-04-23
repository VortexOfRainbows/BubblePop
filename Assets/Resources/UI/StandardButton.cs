using UnityEngine.UI;

public class StandardButton : Button
{
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
        colors.normalColor = ColorHelper.UI.DefaultColor;
        colors.highlightedColor = ColorHelper.New255(0xFD, 0xFF, 0x4A);
        colors.pressedColor = ColorHelper.New255(0xD9, 0xC3, 0x3C);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = ColorHelper.UI.DarkGreyColor;
        base.colors = colors;
    }
}
