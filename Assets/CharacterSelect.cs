using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public EquipmentUIElement EquipmentUISlotPrefab;
    public const int UILayer = 5;
    public GameObject[][] Equipments = new GameObject[4][];
    public GameObject[] Hats;
    public GameObject[] Accessories;
    public GameObject[] Weapons;
    public GameObject[] Characters;
    private Canvas myCanvas;
    public EquipmentUIElement[] UIElems;
    private List<EquipmentUIElement> TempSlots = new();
    private int prevPressedButton = -1;
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
        bool hasClickedAButtonAlready = false;
        for (int i = 0; i < 4; i++)
        {
            bool wasPressed = UIElems[i].UpdateActive(myCanvas);
            if(wasPressed && !hasClickedAButtonAlready)
            {
                hasClickedAButtonAlready = true;
                bool justClosed = false;
                if (TempSlots.Count > 0)
                {
                    justClosed = true;
                    CloseNewBoxes();
                }
                if(!justClosed || i != prevPressedButton)
                {
                    for (int j = 0; j < Equipments[i].Length; j++)
                    {
                        AddNewBox(UIElems[i], j);
                    }
                }
                prevPressedButton = i;
            }
        }
        for(int i = 0; i < TempSlots.Count; i++)
        {
            int slot = TempSlots[i].ParentEquipSlot;
            bool wasPressed = TempSlots[i].UpdateActive(myCanvas);
            if(wasPressed && !hasClickedAButtonAlready && TempSlots[i].Unlocked)
            {
                hasClickedAButtonAlready = true;
                UIElems[slot].ActiveEquipmentIndex = TempSlots[i].ActiveEquipmentIndex;
                RenderBox(UIElems[slot]);
                SwapPlayerEquipment(UIElems[slot].ParentEquipSlot);
                CloseNewBoxes();
            }
        }
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
        }
    }
    public void AddNewBox(EquipmentUIElement parent, int index)
    {
        EquipmentUIElement ui = Instantiate(EquipmentUISlotPrefab, transform);
        ui.transform.localPosition = parent.transform.localPosition + new Vector3(180 * TempSlots.Count, -210);
        ui.ParentEquipSlot = parent.ParentEquipSlot;
        ui.ActiveEquipmentIndex = index;
        ui.targetScale = new Vector3(0.75f, 0.75f, 0.75f);
        RenderBox(ui);
        TempSlots.Add(ui);
    }
    public void CloseNewBoxes()
    {
        for(int i = TempSlots.Count - 1; i >= 0; --i)
        {
            Destroy(TempSlots[i].gameObject);
            TempSlots.RemoveAt(i);
        }
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
}
