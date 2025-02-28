using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public void SaveData()
    {
        PlayerData.SaveInt("LastSelectedChar", LastSelectedBody);
    }
    public void LoadData()
    {
        LastSelectedBody = PlayerData.GetInt("LastSelectedChar");
    }
    public int LastSelectedBody = 0;

    public EquipmentUIElement EquipmentUISlotPrefab;
    public PowerUpLayout PowerLayout;
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
        PowerUpLayout.MenuLayout = PowerLayout;
        Equipments[0] = Hats;
        Equipments[1] = Accessories;
        Equipments[2] = Weapons;
        Equipments[3] = Characters;
        myCanvas = GetComponent<Canvas>();
        RenderBoxes();
        HasLoaded = false;
        for(int j = 0; j < 4; j++)
        {
            for(int i = 0; i < Equipments[j].Length; ++i)
            {
                Equipments[j][i].GetComponent<Equipment>().myEquipmentIndex = i;
            }
        }
    }
    public bool HasLoaded = false;
    public void Update()
    {
        if(!HasLoaded)
        {
            HasLoaded = true;
            LoadData();
            UpdateSelectedEquipmentBox(3, LastSelectedBody);
        }
        if (UIManager.StartingScreen)
            visual.SetActive(true);
        else
        {
            if(visual.activeSelf)
            {
                visual.SetActive(false);
                SaveData();
                Player.Instance.Body.SaveData();
            }
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
                UpdateSelectedEquipmentBox(parent, slot.ActiveEquipmentType);
                CloseEquipmentPage();
            }
        }
        prevHoveringElement = hoveringElement;
        if(!PowerUpPageIsOpen)
        {
            PowerLayout.Generate(PowerUp.AvailablePowers);
            PowerUpPageIsOpen = true;
        }
    }
    public void UpdateSelectedEquipmentBox(int equipSlot, int newType)
    {
        UIElems[equipSlot].ActiveEquipmentType = newType;
        SwapPlayerEquipment(UIElems[equipSlot].ParentEquipSlot);
        RenderBox(UIElems[equipSlot]);
    }
    /// </summary>
    /// <param name="equipmentType"></param>
    public void OpenEquipmentPage(int equipmentType)
    {
        for (int j = 0; j < Equipments[equipmentType].Length; j++)
        {
            AddNewEquipmentBox(UIElems[equipmentType], j);
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

        int equipmentType = UIElems[i].ActiveEquipmentType;
        GameObject equipmentPrefab = Equipments[i] //Select whether this is a hat, accessory, weapon, or body element
            [equipmentType]; //Which type of hat, accessory, weapon, or body element is this?
        GameObject oldEquipment = equip.gameObject;
        equip = Instantiate(equipmentPrefab, Player.Instance.transform).GetComponent<Equipment>();
        equip.myEquipmentIndex = equipmentType;
        Debug.Log(equip);
        equip.AliveUpdate();
        if (i == 0)
        {
            Player.Instance.Hat = equip as Hat;
            Player.Instance.Body.LastSelectedHatType = equipmentType;
        }
        if (i == 1)
        {
            Player.Instance.Cape = equip as Accessory;
            Player.Instance.Body.LastSelectedAccType = equipmentType;

        }
        if (i == 2)
        {
            Player.Instance.Wand = equip as Weapon;
            Player.Instance.Body.LastSelectedWepType = equipmentType;
        }
        if (i == 3)
        {
            SwapBody(oldEquipment.GetComponent<Equipment>() as Body, equip as Body);
        }
        SaveCurrentSelects();
        Destroy(oldEquipment);
        PowerLayout.Generate(PowerUp.AvailablePowers);
    }
    public void SwapBody(Body oldBody, Body newBody)
    {
        Player.Instance.Body = newBody;
        newBody.LoadData();
        UpdateSelectedEquipmentBox(0, newBody.LastSelectedHatType);
        UpdateSelectedEquipmentBox(1, newBody.LastSelectedAccType);
        UpdateSelectedEquipmentBox(2, newBody.LastSelectedWepType);
        LastSelectedBody = newBody.myEquipmentIndex;
        SaveCurrentSelects();
    }
    public void SaveCurrentSelects()
    {
        Body body = Player.Instance.Body;
        //body.LastSelectedHatType = Player.Instance.Hat.myEquipmentIndex;
        //body.LastSelectedAccType = Player.Instance.Cape.myEquipmentIndex;
        //body.LastSelectedWepType = Player.Instance.Wand.myEquipmentIndex;
        body.SaveData();
        SaveData();
    }
    public void AddNewEquipmentBox(EquipmentUIElement parent, int index)
    {
        EquipmentUIElement ui = Instantiate(EquipmentUISlotPrefab, visual.transform);
        ui.transform.localPosition = parent.transform.localPosition + new Vector3(210 + 180 * EquipmentPage.Count, 0);
        ui.ParentEquipSlot = parent.ParentEquipSlot;
        ui.ActiveEquipmentType = index;
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
        uiElem.ActiveEquipment = RenderSingular(Equipments[uiElem.ParentEquipSlot][uiElem.ActiveEquipmentType], uiElem.Visual);
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
}
