using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUIElement : MonoBehaviour
{
    public static readonly Color DefaultBubbleColor = new(1, 1, 1, 0.627451f);
    public static readonly Color GrayBubbleColor = new(.4f, .4f, .4f, 0.627451f);
    public static readonly Color GrayColor = new(.4f, .4f, .4f, 0.8f);
    public void SetPowerType(int PowerID)
    {
        if (PickerElement)
            Index = PowerID;
        else
            throw new Exception("Cannot set the power type of a power in the inventory!");
    }
    public Canvas myCanvas = null;
    public Image outer;
    public Image inner;
    public Image adornment;
    [SerializeField] private GameObject visual;
    public PowerUp MyPower => PowerUp.Get(Type);
    public Sprite Sprite => MyPower.sprite;
    private int prevType = -1;
    public int Type => InventoryElement ? (PlayerHasPower() ? Player.Instance.GetPower(Index) : -1) : Index;
    public TMPro.TextMeshProUGUI Count;
    public int Index = 0;
    public bool InventoryElement = true;
    /// <summary>
    /// Whether this element appears in the char select screen or compendium. Determines whether it can be locked (blacked out)
    /// </summary>
    public bool MenuElement = false;
    /// <summary>
    /// Whether this element appears in the compendium. If true, does not show a description
    /// </summary>
    public bool CompendiumElement = false;
    private float Timer;
    public PowerUpLayout myLayout;
    public bool GrayOut { get; set; }
    public bool PickerElement
    {
        get => !InventoryElement;
        set => InventoryElement = !value;
    }
    public bool PlayerHasPower()
    {
        return Index < Player.Instance.PowerCount;
    }
    public void TurnedOn()
    {
        MyPower.AliveUpdate(inner.gameObject, outer.gameObject, true);

        inner.sprite = Sprite;
        RectTransform rect = inner.transform as RectTransform;
        Rect rectangle = inner.sprite.rect;
        rect.pivot = inner.sprite.pivot / new Vector2(rectangle.width, rectangle.height);
        inner.SetNativeSize();

        Sprite adornSprite = MyPower.GetAdornment();
        if (adornSprite != null)
        {
            adornment.gameObject.SetActive(true);
            adornment.sprite = adornSprite;
            rect = adornment.transform as RectTransform;
            rectangle = adornment.sprite.rect;
            rect.pivot = adornment.sprite.pivot / new Vector2(rectangle.width, rectangle.height);
            adornment.SetNativeSize();
        }
        else
            adornment.gameObject.SetActive(false);

        outer.SetNativeSize();
        visual.SetActive(true);
        SetBorderColors();
        WhileOn();
    }
    public void SetBorderColors()
    {
        outer.material = adornment.material = MyPower.GetBorder(true);
        inner.material = MyPower.GetBorder();
    }
    public void TurnedOff()
    {
        Timer = 0;
        visual.SetActive(false);
    }
    public bool AppearLocked => MenuElement && MyPower.PickedUpCountAllRuns <= 0;
    public bool PreventHovering;
    public bool ForceHideCount = false;
    public float HoverRadius { get; set; } = 64;
    public void WhileOn()
    {
        Timer += 1;
        if(ForceHideCount)
            Count.gameObject.SetActive(false);
        else if(!CompendiumElement)
            Count.text = MyPower.Stack.ToString();
        else
            Count.gameObject.SetActive(!AppearLocked && Compendium.ShowCounts && !PreventHovering);
        bool canHover = !PreventHovering && (myLayout == null || !myLayout.isHovering);
        if(CompendiumElement && !Compendium.MouseInCompendiumArea)
            canHover = false;
        float size = CompendiumElement ? 96 + HoverRadius - outer.rectTransform.rect.width : HoverRadius * transform.localScale.x;
        bool rectangular = CompendiumElement;
        if (canHover && Utils.IsMouseHoveringOverThis(rectangular, outer.rectTransform, size, myCanvas))
        {
            if(myLayout != null)
                myLayout.isHovering = true;
            string name = AppearLocked ? PowerUp.LockedName : MyPower.UnlockedName;
            string desc = AppearLocked ? PowerUp.LockedDescription : CompendiumElement ? "" : MyPower.FullDescription;
            PopUpTextUI.Enable(name, desc);
            float scaleUP = 1.125f;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * scaleUP, 0.16f);

            if(CompendiumElement && Control.LeftMouseClick)
            {
                Compendium.SelectedType = Index;
            }
        }
        else
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.16f);

        if(GrayOut && !AppearLocked)
        {
            inner.color = adornment.color = GrayColor;
            outer.color = GrayBubbleColor;
        }
        else
        {
            if (AppearLocked)
                inner.color = adornment.color = Color.black;
            else if (MenuElement)
                inner.color = adornment.color = Color.white;
            outer.color = DefaultBubbleColor;
        }
    }
    public void Update()
    {
        if(!InventoryElement && !MenuElement)
            OnUpdate();
    }
    public void OnUpdate()
    {
        if (Type >= 0)
        {
            if ((!visual.activeSelf || prevType != Type) && Timer > 1)
            {
                TurnedOn();
                prevType = Type;
            }
            else
                WhileOn();
        }
        else
            TurnedOff();
    }
}
