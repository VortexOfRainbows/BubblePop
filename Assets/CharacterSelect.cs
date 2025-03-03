using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public class EquipmentPage
    {
        public bool IsPrimary = false;
        public int Count => Equips.Count;
        public EquipmentPage(CharacterSelect charSelect, bool isPrimary)
        {
            this.charSelect = charSelect;
            IsPrimary = isPrimary;
        }
        public CharacterSelect charSelect;
        public List<EquipmentUIElement> Equips = new List<EquipmentUIElement>();
        public bool IsOpen = false;
        public int PreviousType = -1;
        public GameObject hoveringElement, prevHoveringElement;
        public bool NewHovering => hoveringElement != prevHoveringElement;
        /// </summary>
        /// <param name="equipmentType"></param>
        public void Open(EquipmentUIElement parent, List<GameObject> Equipments)
        {
            Close();
            for (int j = 0; j < Equipments.Count; j++)
            {
                Add(parent, Equipments[j].GetComponent<Equipment>());
            }
            IsOpen = true;
        }
        /// <summary>
        /// Closes the equipment page.
        /// </summary>
        public void Close()
        {
            for (int i = Equips.Count - 1; i >= 0; --i)
            {
                Destroy(Equips[i].gameObject);
                Equips.RemoveAt(i);
            }
            IsOpen = false;
            if(this == charSelect.PrimaryPage)
            {
                charSelect.SecondaryPage.Close();
            }
        }
        public void Add(EquipmentUIElement parent, Equipment equipment)
        {
            EquipmentUIElement ui = Instantiate(charSelect.EquipmentUISlotPrefab, charSelect.visual.transform);
            if(IsPrimary)
                ui.transform.localPosition = parent.transform.localPosition + new Vector3(210 + 180 * Count, 0);
            else
                ui.transform.localPosition = parent.transform.localPosition + new Vector3(180 * Count, -180);
            ui.targetScale = new Vector3(0.75f, 0.75f, 0.75f);
            charSelect.RenderBox(ui,  equipment);
            Equips.Add(ui);
        }
    }
    public void SaveData()
    {
        PlayerData.SaveInt("LastSelectedChar", LastSelectedBody);
    }
    public void LoadData()
    {
        LastSelectedBody = PlayerData.GetInt("LastSelectedChar");
    }
    public int LastSelectedBody = 0;
    public GameObject topCanvas;
    public EquipmentUIElement EquipmentUISlotPrefab;
    public PowerUpLayout PowerLayout;
    public GameObject visual;
    public const int UILayer = 5;
    public EquipmentUIElement[] DisplayBoxes;
    public List<GameObject>[] PrimaryEquipments = new List<GameObject>[4];
    public List<GameObject> Hats;
    public List<GameObject> Accessories;
    public List<GameObject> Weapons;
    public List<GameObject> Characters;
    public List<GameObject> AllEquipmentsList = new List<GameObject>();
    private Canvas myCanvas;
    private EquipmentPage SecondaryPage;
    private EquipmentPage PrimaryPage;
    private List<PowerUpUIElement> AvailablePowersUI = new();
    private bool PowerUpPageIsOpen = false;
    public bool HasLoaded = false;
    public void Start()
    {
        SecondaryPage = new EquipmentPage(this, false);
        PrimaryPage = new EquipmentPage(this, true);
        PowerUpLayout.MenuLayout = PowerLayout;
        PrimaryEquipments[0] = Hats;
        PrimaryEquipments[1] = Accessories;
        PrimaryEquipments[2] = Weapons;
        PrimaryEquipments[3] = Characters;
        myCanvas = GetComponent<Canvas>();
        HasLoaded = false;
        for(int j = 0; j < PrimaryEquipments.Length; j++)
        {
            for(int i = 0; i < PrimaryEquipments[j].Count; ++i)
            {
                Equipment equip = PrimaryEquipments[j][i].GetComponent<Equipment>();
                equip.IndexInTheAllEquipPool = AllEquipmentsList.Count;
                AllEquipmentsList.Add(equip.gameObject);
                if (equip.SubEquipment != null)
                {
                    for(int k = 0; k < equip.SubEquipment.Count; k++)
                    {
                        equip.SubEquipment[k].GetComponent<Equipment>().IndexInTheAllEquipPool = AllEquipmentsList.Count;
                        AllEquipmentsList.Add(equip.SubEquipment[k]);
                    }
                }
            }
        }
        InitializeMainButtons();
    }
    public void Update()
    {
        if(!HasLoaded)
        {
            HasLoaded = true;
            LoadData();
            UpdateSelectedEquipmentBox(LastSelectedBody);
        }
        if (UIManager.StartingScreen)
        {
            //topCanvas.SetActive(false);
            visual.SetActive(true);
        }
        else
        {
            if(visual.activeSelf)
            {
                //topCanvas.SetActive(true);
                visual.SetActive(false);
                SaveData();
                Player.Instance.Body.SaveData();
            }
            return;
        }
        bool hasOpenPageAlready = false;
        for (int i = 0; i < 4; i++) 
        {
            if (UISlotUpdate(DisplayBoxes[i], PrimaryPage, i, !hasOpenPageAlready))
                hasOpenPageAlready = true;
        }
        for(int i = 0; i < PrimaryPage.Count; i++)
        {
            if (UISlotUpdate(PrimaryPage.Equips[i], SecondaryPage, i, !hasOpenPageAlready))
                hasOpenPageAlready = true;
        }
        for (int i = 0; i < SecondaryPage.Count; i++)
        {
            if (UISlotUpdate(SecondaryPage.Equips[i], null, i, false))
                hasOpenPageAlready = true;
        }
        PrimaryPage.prevHoveringElement = PrimaryPage.hoveringElement;
        SecondaryPage.prevHoveringElement = SecondaryPage.hoveringElement;
        if(!PowerUpPageIsOpen)
        {
            PowerLayout.Generate(PowerUp.AvailablePowers);
            PowerUpPageIsOpen = true;
        }
    }
    public bool UISlotUpdate(EquipmentUIElement slot, EquipmentPage page, int index, bool AllowOpeningPage)
    {
        slot.UpdateActive(myCanvas, out bool hovering, out bool clicked);
        bool openPage = page == PrimaryPage ? clicked : false;
        if(page != null)
        {
            if (hovering)
            {
                page.hoveringElement = slot.gameObject;
                if (page.NewHovering)
                    openPage = true;
            }
            else if (page.hoveringElement == slot.gameObject && !page.IsOpen)
                page.hoveringElement = null;
        }
        if (openPage && AllowOpeningPage)
        {
            if(page != null)
            {
                bool justClosed = false;
                if (page.Count > 0)
                {
                    justClosed = true;
                    page.Close();
                }
                if (!justClosed || index != page.PreviousType)
                {
                    if (page == PrimaryPage)
                        page.Open(slot, PrimaryEquipments[index]);
                    else
                        page.Open(slot, slot.ActiveEquipment.OriginalPrefab.SubEquipment);
                }
                page.PreviousType = index;
            }
            return true;
        }
        if(page != PrimaryPage)
        {
            if(clicked && slot.Unlocked)
            {
                UpdateSelectedEquipmentBox(slot.ActiveEquipment.OriginalPrefab);
                PrimaryPage.Close();
            }
        }
        return false;
    }
    public void UpdateSelectedEquipmentBox(Equipment equipPrefab)
    {
        int i = -1;
        if (equipPrefab is Hat)
            i = 0;
        else if (equipPrefab is Accessory)
            i = 1;
        else if (equipPrefab is Weapon)
            i = 2;
        else if (equipPrefab is Body)
            i = 3;
        SwapPlayerEquipment(equipPrefab);
        RenderBox(DisplayBoxes[i], equipPrefab);
    }
    public void UpdateSelectedEquipmentBox(int AllIndex)
    {
        Equipment equipPrefab = AllEquipmentsList[AllIndex].GetComponent<Equipment>();
        int i = -1;
        if (equipPrefab is Hat)
            i = 0;
        else if (equipPrefab is Accessory)
            i = 1;
        else if (equipPrefab is Weapon)
            i = 2;
        else if (equipPrefab is Body)
            i = 3;
        SwapPlayerEquipment(equipPrefab);
        RenderBox(DisplayBoxes[i], equipPrefab);
    }
    public void SwapPlayerEquipment(Equipment equipPrefab)
    {
        Equipment equip = null;
        int i = -1;
        if (equipPrefab is Hat)
        {
            i = 0;
            equip = Player.Instance.Hat;
        }
        else if (equipPrefab is Accessory)
        {
            i = 1;
            equip = Player.Instance.Cape;
        }
        else if (equipPrefab is Weapon)
        {
            i = 2;
            equip = Player.Instance.Wand;
        }
        else if (equipPrefab is Body)
        {
            i = 3;
            equip = Player.Instance.Body;
        }

        GameObject oldEquipment = equip.gameObject;
        equip = Instantiate(equipPrefab, Player.Instance.transform).GetComponent<Equipment>();
        equip.OriginalPrefab = equipPrefab;
        Debug.Log(equip);
        equip.AliveUpdate();
        if (i == 0)
        {
            Player.Instance.Hat = equip as Hat;
            Player.Instance.Body.LastSelectedHat = equip.IndexInTheAllEquipPool;
        }
        if (i == 1)
        {
            Player.Instance.Cape = equip as Accessory;
            Player.Instance.Body.LastSelectedAcc = equip.IndexInTheAllEquipPool;
        }
        if (i == 2)
        {
            Player.Instance.Wand = equip as Weapon;
            Player.Instance.Body.LastSelectedWep = equip.IndexInTheAllEquipPool;
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
        UpdateSelectedEquipmentBox(newBody.LastSelectedHat);
        UpdateSelectedEquipmentBox(newBody.LastSelectedAcc);
        UpdateSelectedEquipmentBox(newBody.LastSelectedWep);
        LastSelectedBody = newBody.IndexInTheAllEquipPool;
        SaveCurrentSelects();
    }
    public void SaveCurrentSelects()
    {
        Player.Instance.Body.SaveData();
        SaveData();
    }
    public void InitializeMainButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            RenderBox(DisplayBoxes[i], PrimaryEquipments[i][0].GetComponent<Equipment>());
        }
    }
    public void RenderBox(EquipmentUIElement uiElem, Equipment equipmentToRender)
    {
        if(uiElem.ActiveEquipment != null)
            Destroy(uiElem.ActiveEquipment.gameObject);
        uiElem.ActiveEquipment = GenerateEquipment(equipmentToRender, uiElem.Visual);
        uiElem.UpdateOrientation();
    }
    public Equipment GenerateEquipment(Equipment prefab, GameObject parent)
    {
        Equipment obj = Instantiate(prefab, parent.transform);
        obj.gameObject.layer = UILayer;
        obj.transform.localPosition = Vector3.zero;
        obj.OriginalPrefab = prefab;
        foreach(Transform t in obj.GetComponentsInChildren<Transform>())
            t.gameObject.layer = UILayer;
        return obj;
    }
}
