using TMPro;
using UnityEngine;

public class PowerUpButton : MonoBehaviour
{
    public const int RerollAttempsForSamePowerInPicker = 3;
    public static PowerUpButton[] Buttons = new PowerUpButton[5];
    public static PowerUpButton RerollButton;
    [SerializeField] public PowerUpUIElement PowerUI;
    [SerializeField] private int index = 0;
    public bool IsCheatButton = false;
    public bool IsRerollButton = false;
    public bool Active => gameObject.activeSelf;
    public TextMeshProUGUI HotkeyText;
    private KeyCode Hotkey;
    public void Start()
    {
        if (IsRerollButton)
        {
            TurnOff();
            RerollButton = this;
        }
        else if (!IsCheatButton)
        {
            if (Active)
                PowerUp.PickingPowerUps = true;
            Buttons[index] = this;
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
        Main.MouseHoveringOverButton = true;
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
    public void Update()
    {
        if (Input.GetKeyDown(Hotkey) && PowerUp.PickingPowerUps)
        {
            GrantPower();
        }
    }
    private bool SameTypeAsOthers()
    {
        PowerUpButton other1;
        PowerUpButton other2;
        if(Player.Instance.BonusChoices)
        {
            other1 = Buttons[(index + 1) % 5];
            other2 = Buttons[(index + 2) % 5];   
            PowerUpButton other3 = Buttons[(index + 3) % 5];
            PowerUpButton other4 = Buttons[(index + 4) % 5];
            return other1.PowerUI.Index == PowerUI.Index || 
                other2.PowerUI.Index == PowerUI.Index ||
                other3.PowerUI.Index == PowerUI.Index || 
                other4.PowerUI.Index == PowerUI.Index;
        }
        other1 = Buttons[(index + 1) % 3];
        other2 = Buttons[(index + 2) % 3];
        return other1.PowerUI.Index == PowerUI.Index || other2.PowerUI.Index == PowerUI.Index;
    }
    public void TurnOn()
    {
        if(IsRerollButton)
        {
            SetType(PowerUp.Get<Choice>().Type); //This needs to happen first, before the button is turned on
        }
        else if(!IsCheatButton)
        {
            PowerUp.PickingPowerUps = true;
            for (int i = RerollAttempsForSamePowerInPicker; i > 0; --i)
            {
                SetType(PowerUp.RandomFromPool(-1)); //This needs to happen first, before the button is turned on
                if (!SameTypeAsOthers())
                    break;
            }
        }
        UpdateHotkey();
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
    public void UpdateHotkey()
    {
        if(HotkeyText != null)
        {
            if(IsRerollButton)
            {
                Hotkey = KeyCode.R;
                HotkeyText.text = "R";
            }
            else if (Player.Instance.BonusChoices)
            {
                if (index == 0)
                {
                    Hotkey = KeyCode.Alpha2;
                    HotkeyText.text = "2";
                }
                if (index == 1)
                {
                    Hotkey = KeyCode.Alpha3;
                    HotkeyText.text = "3";
                }
                if (index == 2)
                {
                    Hotkey = KeyCode.Alpha4;
                    HotkeyText.text = "4";
                }
                if (index == 3)
                {
                    Hotkey = KeyCode.Alpha1;
                    HotkeyText.text = "1";
                }
                if (index == 4)
                {
                    Hotkey = KeyCode.Alpha5;
                    HotkeyText.text = "5";
                }
            }
            else
            {
                if (index == 0)
                {
                    Hotkey = KeyCode.Alpha1;
                    HotkeyText.text = "1";
                }
                if (index == 1)
                {
                    Hotkey = KeyCode.Alpha2;
                    HotkeyText.text = "2";
                }
                if (index == 2)
                {
                    Hotkey = KeyCode.Alpha3;
                    HotkeyText.text = "3";
                }
            }
        }
    }
}
