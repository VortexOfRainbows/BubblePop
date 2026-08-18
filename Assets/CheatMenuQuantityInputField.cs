using TMPro;
using UnityEngine;

public class CheatMenuQuantityInputField : MonoBehaviour
{
    public int Type;
    public TMP_InputField InputField;
    public void TryParseInput(string Input)
    {
        TryParseInput(Input, true);
    }
    public void TryParseInput(string Input, bool integerOnly)
    {
        try
        {
            float value = int.Parse(Input);
            if (value > 100)
                value = 100;
            if (value < 1)
                value = 1;
            InputField.text = value.ToString();
            UpdateSetting(value);
        }
        catch
        {
            Debug.Log("Failed to parse text input into num");
        }
    }
    private bool Loaded = false;
    public void Update()
    {
        if(!Loaded)
        {
            LoadSetting();
            Loaded = true;
        }
    }
    public void LoadSetting()
    {
        TryParseInput(PowerUpCheatUI.ProcessQuantity.ToString(), true);
    }
    public void UpdateSetting(float value)
    {
        int val = (int)value;
        if (PowerUpCheatUI.ProcessQuantity != val)
        {
            PowerUpCheatUI.UpdatedProcessQuantity = 2;
            PowerUpCheatUI.ProcessQuantity = val;
        }
    }
}
