using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpLayout : MonoBehaviour
{
    public static PowerUpLayout InGameLayout;
    public static PowerUpLayout MenuLayout;
    public GameObject PowerUpUISlotPrefab;
    public List<PowerUpUIElement> PowerUpElems;
    public GridLayoutGroup layout;
    public Canvas myCanvas;
    public Image bg;
    public bool isInGameLayout;
    public bool isHovering;
    public void FixedUpdate()
    {
        if(isInGameLayout)
        {
            InGameLayout = this;
            GenerateInventory();
        }
        else
        {
            MenuLayout = this;
        }
    }
    public void Update()
    {
        UpdateSizing();
        if(Input.GetKeyDown(KeyCode.V) && !isInGameLayout)
        {
            AddNewPower(PowerUpUISlotPrefab.gameObject, gameObject, 0);
        }
        isHovering = false;
        for (int i = PowerUpElems.Count - 1; i >= 0; --i)
        {
            PowerUpElems[i].OnUpdate();
        }
    }
    public void UpdateSizing()
    {
        int rowCount = 1;
        int powerupSize = 130;
        int padding = 20;
        layout.padding = new RectOffset(padding, padding, padding, padding);
        float resolution = Screen.width - padding * 2;
        int amountFittable = (int)(resolution / powerupSize);
        int amountINeedToFit = (PowerUpElems.Count + rowCount - 1) / rowCount;
        if (amountINeedToFit < amountFittable)
            amountINeedToFit = amountFittable;

        //float scaleChange = amountFittable / (float)amountINeedToFit;

        float actualSize = resolution / amountINeedToFit;
        layout.cellSize = new Vector2(actualSize, powerupSize + 10);
        layout.constraintCount = amountINeedToFit;

        //layout.transform.localScale = Vector3.one * scaleChange;
    }
    public void Generate(List<int> AvailablePowers)
    {
        foreach (PowerUpUIElement pUI in PowerUpElems)
        {
            Destroy(pUI.gameObject);
        }
        PowerUpElems.Clear();
        Equipment.ModifyPowerPoolAll();
        //PowerUp.SortAvailablePowers();
        for (int i = 0; i < PowerUp.AvailablePowers.Count; ++i)
        {
            AddNewPower(PowerUpUISlotPrefab.gameObject, gameObject, AvailablePowers[i]);
        }
    }
    public void GenerateInventory()
    {
        while (PowerUpElems.Count < Player.Instance.PowerCount)
        {
            AddNewPower(PowerUpUISlotPrefab, gameObject, PowerUpElems.Count, true);
        }
        while (PowerUpElems.Count > Player.Instance.PowerCount)
        {
            PowerUpUIElement obj = PowerUpElems.Last();
            PowerUpElems.RemoveAt(PowerUpElems.Count - 1);
            Destroy(obj.gameObject);
        }
    }
    public PowerUpUIElement AddNewPower(GameObject prefab, GameObject parent, int index, bool inventory = false)
    {
        PowerUpUIElement powerUI = Instantiate(prefab.GetComponent<PowerUpUIElement>(), transform);
        //powerUI.transform.localPosition = UIElems[3].transform.localPosition + new Vector3(150 * AvailablePowersUI.Count, -190);
        powerUI.Index = index;
        powerUI.InventoryElement = inventory;
        powerUI.Count.gameObject.SetActive(inventory);
        powerUI.myCanvas = myCanvas;
        powerUI.MenuElement = !inventory;
        powerUI.myLayout = this;
        powerUI.TurnedOn();
        PowerUpElems.Add(powerUI);
        UpdateSizing();
        return powerUI;
    }
}
