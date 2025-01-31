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
        inner.SetNativeSize();
        outer.SetNativeSize();
        inner.gameObject.SetActive(true);
    }
    public void TurnedOff()
    {
        Timer = 0;
        inner.gameObject.SetActive(false);
    }
    public void WhileOn()
    {
        Timer += 1;
        Count.text = MyPower.Stack.ToString();
    }
    public void FixedUpdate()
    {
        if (Type >= 0)
        {
            if ((!inner.gameObject.activeSelf || prevType != Type) && Timer > 1)
                TurnedOn();
            WhileOn();
        }
        else
            TurnedOff();
        prevType = Type;
    }
}
