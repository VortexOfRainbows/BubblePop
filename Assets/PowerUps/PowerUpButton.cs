using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpButton : MonoBehaviour
{
    public const int RerollAttempsForSamePowerInPicker = 3;
    public static PowerUpButton[] buttons = new PowerUpButton[5];
    [SerializeField] public PowerUpUIElement PowerUI;
    [SerializeField] private int index = 0;
    public bool IsCheatButton = false;
    public bool Active => gameObject.activeSelf;
    public void Start()
    {
        if(!IsCheatButton)
        {
            if (Active)
                PowerUp.PickingPowerUps = true;
            buttons[index] = this;
            TurnOff();
        }
        else
        {
            TurnOn();
        }
    }
    public void OnButtonPress()
    {
        GrantPower();
    }
    public void GrantPower()
    {
        PowerUI.MyPower.PickUp();
        if(!IsCheatButton)
            PowerUp.TurnOffPowerUpSelectors();
    }
    public void FixedUpdate()
    {
        PowerUI.PickerElement = true;
    }
    private bool SameTypeAsOthers()
    {
        PowerUpButton other1;
        PowerUpButton other2;
        if(Player.Instance.BonusChoices)
        {
            other1 = buttons[(index + 1) % 5];
            other2 = buttons[(index + 2) % 5];   
            PowerUpButton other3 = buttons[(index + 3) % 5];
            PowerUpButton other4 = buttons[(index + 4) % 5];
            return other1.PowerUI.Index == PowerUI.Index || 
                other2.PowerUI.Index == PowerUI.Index ||
                other3.PowerUI.Index == PowerUI.Index || 
                other4.PowerUI.Index == PowerUI.Index;
        }
        other1 = buttons[(index + 1) % 3];
        other2 = buttons[(index + 2) % 3];
        return other1.PowerUI.Index == PowerUI.Index || other2.PowerUI.Index == PowerUI.Index;
    }
    public void TurnOn()
    {
        float chanceForChoice = Player.Instance.BonusChoices ? 0.024f : 0.04f; //This should give roughly the same chance of seeing a choice power again in either case
        if(!IsCheatButton)
        {
            PowerUp.PickingPowerUps = true;
            for (int i = RerollAttempsForSamePowerInPicker; i > 0; --i)
            {
                SetType(PowerUp.RandomFromPool(chanceForChoice)); //This needs to happen first, before the button is turned on
                if (!SameTypeAsOthers())
                    break;
            }
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
