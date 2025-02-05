using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpButton : MonoBehaviour
{
    public const int RerollAttempsForSamePowerInPicker = 3;
    public static PowerUpButton[] buttons = new PowerUpButton[3];
    [SerializeField] public PowerUpUIElement PowerUI;
    [SerializeField] private int index = 0;
    public bool Active => gameObject.activeSelf;
    public void Start()
    {
        if (Active)
            PowerUp.PickingPowerUps = true;
        buttons[index] = this;
        TurnOff();
    }
    public void OnButtonPress()
    {
        GrantPower();
    }
    public void GrantPower()
    {
        PowerUI.MyPower.PickUp();
        PowerUp.TurnOffPowerUpSelectors();
    }
    public void FixedUpdate()
    {
        PowerUI.PickerElement = true;
    }
    private bool SameTypeAsOthers()
    {
        PowerUpButton other1 = buttons[(index + 1) % 3];
        PowerUpButton other2 = buttons[(index + 2) % 3];
        return other1.PowerUI.Index == PowerUI.Index || other2.PowerUI.Index == PowerUI.Index;
    }
    public void TurnOn()
    {
        PowerUp.PickingPowerUps = true;
        for (int i = RerollAttempsForSamePowerInPicker; i > 0; --i)
        {
            SetType(PowerUp.RandomFromPool(0.04f)); //This needs to happen first, before the button is turned on
            if (!SameTypeAsOthers())
                break;
        }
        PowerUI.TurnedOn();
        gameObject.SetActive(true);
    }
    public void TurnOff()
    {
        PowerUp.PickingPowerUps = false;
        PowerUI.TurnedOff();
        gameObject.SetActive(false);
    }
    public void SetType(int type)
    {
        PowerUI.Index = type;
    }
}
