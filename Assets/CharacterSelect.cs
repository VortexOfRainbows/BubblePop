using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public EquipmentUIElement EquipmentUISlotPrefab;
    public PowerUpUIElement PowerUpUISlotPrefab;
    public GameObject visual;
    public const int UILayer = 5;
    public GameObject[][] Equipments = new GameObject[4][];
    public GameObject[] Hats;
    public GameObject[] Accessories;
    public GameObject[] Weapons;
    public GameObject[] Characters;
    private Canvas myCanvas;
    public EquipmentUIElement[] UIElems;
    private List<EquipmentUIElement> SubEquipmentPage = new();
    private List<EquipmentUIElement> EquipmentPage = new();
    private List<PowerUpUIElement> AvailablePowersUI = new();
    private int prevPressedButton = -1;
    private GameObject hoveringElement, prevHoveringElement;
    private bool NewHovering => hoveringElement != prevHoveringElement;
    private bool EquipmentPageOpen = false;
    private bool PowerUpPageIsOpen = false;
    public void Start()
    {
        Equipments[0] = Hats;
        Equipments[1] = Accessories;
        Equipments[2] = Weapons;
        Equipments[3] = Characters;
        myCanvas = GetComponent<Canvas>();
        RenderBoxes();
    }
    public void Update()
    {
        if (UIManager.StartingScreen)
            visual.SetActive(true);
        else
        {
            visual.SetActive(false);
            return;
        }
        bool hasOpenPageAlready = false;
        for (int i = 0; i < 4; i++) 
        {
            UIElems[i].UpdateActive(myCanvas, out bool hovering, out bool clicked);
            bool openPage = clicked;
            if(hovering)
            {
                hoveringElement = UIElems[i].gameObject;
                if (NewHovering)
                    openPage = true;
            }
            else if(hoveringElement == UIElems[i].gameObject && !EquipmentPageOpen)
                hoveringElement = null;
            if (openPage && !hasOpenPageAlready)
            {
                hasOpenPageAlready = true;
                bool justClosed = false;
                if (EquipmentPage.Count > 0)
                {
                    justClosed = true;
                    CloseEquipmentPage();
                }
                if(!justClosed || i != prevPressedButton)
                    OpenEquipmentPage(i);
                prevPressedButton = i;
            }
        }
        for(int i = 0; i < EquipmentPage.Count; i++)
        {
            EquipmentUIElement slot = EquipmentPage[i];
            int parent = slot.ParentEquipSlot;
            slot.UpdateActive(myCanvas, out bool hovering, out bool clicked);
            bool openPage = clicked;
            //if (hovering)
            //{
            //    hoveringElement = slot.gameObject;
            //    if (NewHovering)
            //        openPage = true;
            //}
            //else if (hoveringElement == slot.gameObject && !EquipmentPageOpen)
            //    hoveringElement = null;
            if (openPage && !hasOpenPageAlready && slot.Unlocked)
            {
                hasOpenPageAlready = true;
                UIElems[parent].ActiveEquipmentIndex = slot.ActiveEquipmentIndex;
                RenderBox(UIElems[parent]);
                SwapPlayerEquipment(UIElems[parent].ParentEquipSlot);
                CloseEquipmentPage();
            }
        }
        prevHoveringElement = hoveringElement;
        if(!PowerUpPageIsOpen)
        {
            ResetPowerUps();
            PowerUpPageIsOpen = true;
        }
    }
    /// <summary>
    /// Opens an equipment page. equipmentType: 0 = hat, 1 = accessory, 2 = weapon, 3 = character
    /// </summary>
    /// <param name="equipmentType"></param>
    public void OpenEquipmentPage(int equipmentType)
    {
        for (int j = 0; j < Equipments[equipmentType].Length; j++)
        {
            AddNewBox(UIElems[equipmentType], j);
        }
        EquipmentPageOpen = true;
    }
    /// <summary>
    /// Closes the equipment page.
    /// </summary>
    public void CloseEquipmentPage()
    {
        for (int i = EquipmentPage.Count - 1; i >= 0; --i)
        {
            Destroy(EquipmentPage[i].gameObject);
            EquipmentPage.RemoveAt(i);
        }
        EquipmentPageOpen = false;
    }
    public void SwapPlayerEquipment(int i)
    {
        Equipment equip = null;
        if (i == 0)
            equip = Player.Instance.Hat;
        if (i == 1)
            equip = Player.Instance.Cape;
        if (i == 2)
            equip = Player.Instance.Wand;
        if (i == 3)
            equip = Player.Instance.Body;
        if (equip.GetType() != UIElems[i].ActiveEquipment.GetType())
        {
            GameObject equipmentPrefab = Equipments[i][UIElems[i].ActiveEquipmentIndex];
            GameObject oldHat = equip.gameObject;
            equip = Instantiate(equipmentPrefab, Player.Instance.transform).GetComponent<Equipment>();
            Debug.Log(equip);
            equip.AliveUpdate();
            Destroy(oldHat);
            if (i == 0)
                Player.Instance.Hat  = equip as Hat; 
            if (i == 1)      
                Player.Instance.Cape = equip as Accessory; 
            if (i == 2)            
                Player.Instance.Wand = equip as Weapon; 
            if (i == 3)          
                Player.Instance.Body = equip as Body;
            ResetPowerUps();
        }
    }
    public void AddNewBox(EquipmentUIElement parent, int index)
    {
        EquipmentUIElement ui = Instantiate(EquipmentUISlotPrefab, visual.transform);
        ui.transform.localPosition = parent.transform.localPosition + new Vector3(210 + 180 * EquipmentPage.Count, 0);
        ui.ParentEquipSlot = parent.ParentEquipSlot;
        ui.ActiveEquipmentIndex = index;
        ui.targetScale = new Vector3(0.75f, 0.75f, 0.75f);
        RenderBox(ui);
        EquipmentPage.Add(ui);
    }
    public void RenderBoxes()
    {
        for (int i = 0; i < 4; i++)
        {
            UIElems[i].ParentEquipSlot = i;
            RenderBox(UIElems[i]);
        }
    }
    public void RenderBox(EquipmentUIElement uiElem)
    {
        if(uiElem.ActiveEquipment != null)
            Destroy(uiElem.ActiveEquipment.gameObject);
        uiElem.ActiveEquipment = RenderSingular(Equipments[uiElem.ParentEquipSlot][uiElem.ActiveEquipmentIndex], uiElem.Visual);
        uiElem.UpdateOrientation();
    }
    public Equipment RenderSingular(GameObject prefab, GameObject parent)
    {
        Equipment obj = Instantiate(prefab.GetComponent<Equipment>(), parent.transform);
        obj.gameObject.layer = UILayer;
        obj.transform.localPosition = Vector3.zero;
        foreach(Transform t in obj.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = UILayer;
        }
        return obj;
    }
    public void UpdatePowerUps()
    {
        for (int i = 0; i < AvailablePowersUI.Count; ++i)
        {

        }
    }
    public void ResetPowerUps()
    {
        foreach (PowerUpUIElement pUI in AvailablePowersUI) 
        {
            Destroy(pUI.gameObject);
        }
        AvailablePowersUI.Clear();
        Equipment.ModifyPowerPoolAll();
        //PowerUp.SortAvailablePowers();
        for(int i = 0; i < PowerUp.AvailablePowers.Count; ++i)
        {
            AddNewPower(PowerUpUISlotPrefab.gameObject, gameObject, PowerUp.AvailablePowers[i]);
        }
    }
    public PowerUpUIElement AddNewPower(GameObject prefab, GameObject parent, int index)
    {
        PowerUpUIElement powerUI = Instantiate(prefab.GetComponent<PowerUpUIElement>(), visual.transform);
        powerUI.transform.localPosition = UIElems[3].transform.localPosition + new Vector3(150 * AvailablePowersUI.Count, -190);
        powerUI.Index = index;
        powerUI.InventoryElement = false;
        powerUI.Count.gameObject.SetActive(false);
        powerUI.myCanvas = myCanvas;
        powerUI.MenuElement = true;
        powerUI.TurnedOn();
        AvailablePowersUI.Add(powerUI);
        return powerUI;
    }
}
