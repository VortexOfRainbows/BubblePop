using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUIElement : MonoBehaviour
{
    public void SetPowerType(int PowerID)
    {
        if (PickerElement)
            Index = PowerID;
        else
        {
            throw new Exception("Cannot set the power type of a power in the inventory!");
        }
    }
    public Image outer;
    public Image inner;
    [SerializeField] private GameObject visual;
    public PowerUp MyPower => PowerUp.Get(Type);
    public Sprite Sprite => MyPower.sprite;
    private int prevType;
    public int Type => InventoryElement ? (PlayerHasPower() ? Player.Instance.GetPower(Index) : -1) : Index;
    public TMPro.TextMeshProUGUI Count;
    public int Index = 0;
    public bool InventoryElement = true;
    private float Timer;
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
        inner.sprite = Sprite;
        RectTransform rect = inner.transform as RectTransform;
        Rect rectangle = inner.sprite.rect;
        rect.pivot = inner.sprite.pivot / new Vector2(rectangle.width, rectangle.height);
        inner.SetNativeSize();
        outer.SetNativeSize();
        visual.SetActive(true);
    }
    public void TurnedOff()
    {
        Timer = 0;
        visual.SetActive(false);
    }
    public void WhileOn()
    {
        Timer += 1;
        Count.text = MyPower.Stack.ToString();
        if(Utils.IsMouseHoveringOverThis(false, outer.rectTransform))
            PopUpTextUI.Enable(MyPower.Name(), MyPower.Description());
    }
    public void FixedUpdate()
    {
        if (Type >= 0)
        {
            if ((!visual.activeSelf || prevType != Type) && Timer > 1)
                TurnedOn();
            WhileOn();
        }
        else
            TurnedOff();
        prevType = Type;
    }
}
