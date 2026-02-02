using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpButton : MonoBehaviour
{
    public Crucible Crucible { get; set; }
    public const int RerollAttempsForSamePowerInPicker = 3;
    public Button SelectButton;
    [SerializeField] public PowerUpUIElement PowerUI;
    [SerializeField] private int index = 0;
    public bool NonChoiceButton { get; set; } = false;
    public bool CheatButton { get; set; } = false;
    public bool CrucibleButton => Crucible != null;
    public bool Active => gameObject.activeSelf;
    public TextMeshProUGUI HotkeyText;
    private KeyCode Hotkey;
    public void Start()
    {
        if (NonChoiceButton)
            Init();
    }
    public void Init()
    {
        SelectButton.onClick.AddListener(OnButtonPress);
        if (!NonChoiceButton)
        {
            if (Active)
                PowerUp.PickingPowerUps = true;
            TurnOff();
        }
        else
        {
            TurnOn();
        }
        UpdateHotkey();
    }
    public void OnButtonPress()
    {
        if (CrucibleButton)
        {
            int removeAmt = Mathf.Min(PowerUpCheatUI.ProcessQuantity, PowerUI.MyPower.Stack);
            Player.Instance.RemovePower(PowerUI.MyPower.Type, removeAmt);
            for(int i = 0; i < removeAmt; ++i)
                Crucible.PowerQueue.Enqueue(PowerUI.MyPower.Type);
            if(PowerUpCheatUI.AutoHide && !PowerUpCheatUI.Hide)
                PowerUpCheatUI.Instance.ToggleHide();
            Crucible.Text.text = Crucible.PowerQueue.Count.ToString();
            return;
        }
        GrantPower();
        Main.MouseHoveringOverButton = true;
    }
    public void GrantPower()
    {
        if(PowerUI.CrucibleElement)
        {
            if (!Main.DebugSettings.PowerUpCheat)
                CoinManager.ModifyShards(-PowerUI.Cost * PowerUpCheatUI.ProcessQuantity);
        }
        else if(ChoicePowerMenu.IsBlackMarket)
        {
            CoinManager.ModifyGems(-PowerUI.Cost);
        }
        int amt = NonChoiceButton ? Mathf.Clamp(PowerUpCheatUI.ProcessQuantity, 1, 100) : 1;
        PowerUI.MyPower.PickUp(amt);
        if (!NonChoiceButton)
        {
            PowerUp.TurnOffPowerUpSelectors();
        }
    }
    public void FixedUpdate()
    {
        PowerUI.PickerElement = true;
    }
    public void Update()
    {
        if (ChoicePowerMenu.Hide && !NonChoiceButton)
            return;
        if (CrucibleButton || PowerUI.CrucibleElement)
        {
            if(PowerUI.MyPower.Stack <= 0)
            {
                if (CrucibleButton || !Main.DebugSettings.PowerUpCheat)
                    Destroy(gameObject);
                else
                    PowerUI.Count.gameObject.SetActive(false);
            }
            else
            {
                PowerUI.Count.gameObject.SetActive(PowerUI.MyPower.Stack > 1 || Main.DebugSettings.PowerUpCheat);
                PowerUI.Count.text = PowerUI.MyPower.Stack.ToString();
            }
            if (!CrucibleButton)
            {
                int cost = PowerUI.Cost * PowerUpCheatUI.ProcessQuantity;
                bool canAfford = cost <= CoinManager.CurrentShards || Main.DebugSettings.PowerUpCheat;
                SelectButton.interactable = canAfford;
            }
            else
                SelectButton.interactable = true;
            return;
        }
        else if(ChoicePowerMenu.IsBlackMarket)
        {
            int cost = PowerUI.Cost;
            bool canAfford = cost <= CoinManager.CurrentGems;
            SelectButton.interactable = canAfford;
        }
        else
            SelectButton.interactable = true;
        if (Input.GetKeyDown(Hotkey) && !ChoicePowerMenu.Hide && SelectButton.interactable)
            GrantPower();
    }
    private bool SameTypeAsOthers()
    {
        PowerUpButton other1;
        PowerUpButton other2;
        if(Player.Instance.BonusChoices)
        {
            other1 = ChoicePowerMenu.PowerButtons[(index + 1) % 5];
            other2 = ChoicePowerMenu.PowerButtons[(index + 2) % 5];   
            PowerUpButton other3 = ChoicePowerMenu.PowerButtons[(index + 3) % 5];
            PowerUpButton other4 = ChoicePowerMenu.PowerButtons[(index + 4) % 5];
            return other1.PowerUI.Index == PowerUI.Index || 
                other2.PowerUI.Index == PowerUI.Index ||
                other3.PowerUI.Index == PowerUI.Index || 
                other4.PowerUI.Index == PowerUI.Index;
        }
        other1 = ChoicePowerMenu.PowerButtons[(index + 1) % 3];
        other2 = ChoicePowerMenu.PowerButtons[(index + 2) % 3];
        return other1.PowerUI.Index == PowerUI.Index || other2.PowerUI.Index == PowerUI.Index;
    }
    public void TurnOn()
    {
        if (!NonChoiceButton)
        {
            PowerUp.PickingPowerUps = true;
            float bm = ChoicePowerMenu.IsBlackMarket ? 1 : -1;
            for (int i = RerollAttempsForSamePowerInPicker; i > 0; --i)
            {
                int rare = -1;
                if (ChoicePowerMenu.IsBlackMarket && index <= 2)
                    rare = Utils.RandInt(index + 2);
                SetType(PowerUp.RandomFromPool(-1, bm, rare)); //This needs to happen first, before the button is turned on
                if (!SameTypeAsOthers())
                    break;
            }
            PowerUI.Cost = ChoicePowerMenu.IsBlackMarket ? PowerUI.MyPower.CrucibleGems() : -1;
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
    public void UpdateHotkey()
    {
        if(HotkeyText != null)
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
            if (index == 3)
            {
                Hotkey = KeyCode.Alpha4;
                HotkeyText.text = "4";
            }
            if (index == 4)
            {
                Hotkey = KeyCode.Alpha5;
                HotkeyText.text = "5";
            }
        }
    }
}
