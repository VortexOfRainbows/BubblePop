using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpLayout : MonoBehaviour
{
    public static PowerUpLayout InGameLayout;
    public static PowerUpLayout MenuLayout;
    public GameObject PowerUpUISlotPrefab;
    public List<PowerUpUIElement> PowerUpElems;
    public Canvas myCanvas;
    public bool isInGameLayout;
    public void Update()
    {
        if(isInGameLayout)
        {
            InGameLayout = this;
            GenerateInventory();
        }
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
        powerUI.TurnedOn();
        PowerUpElems.Add(powerUI);
        return powerUI;
    }
}
