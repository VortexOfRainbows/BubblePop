using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class CharacterSelect : MonoBehaviour
{
    public class EquipmentPage
    {
        public int Count => Equips.Count;
        public EquipmentPage(CharacterSelect charSelect)
        {
            this.charSelect = charSelect;
        }
        public CharacterSelect charSelect;
        public List<EquipmentUIElement> Equips = new List<EquipmentUIElement>();
        public bool IsOpen = false;
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
        }
        public void Add(EquipmentUIElement parent, Equipment equipment)
        {
            EquipmentUIElement ui = Instantiate(charSelect.EquipmentUISlotPrefab, charSelect.visual.transform);
            ui.transform.localPosition = parent.transform.localPosition + new Vector3(210 + 180 * Count, 0);
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

    public EquipmentUIElement EquipmentUISlotPrefab;
    public PowerUpLayout PowerLayout;
    public GameObject visual;
    public const int UILayer = 5;
    public EquipmentUIElement[] MainButtons;
    public List<GameObject>[] PrimaryEquipments = new List<GameObject>[4];
    public List<GameObject> Hats;
    public List<GameObject> Accessories;
    public List<GameObject> Weapons;
    public List<GameObject> Characters;
    private Canvas myCanvas;
    private EquipmentPage SubPage;
    private EquipmentPage PrimaryPage;
    private List<PowerUpUIElement> AvailablePowersUI = new();
    private int prevPressedButton = -1;
    private GameObject hoveringElement, prevHoveringElement;
    private bool NewHovering  => hoveringElement != prevHoveringElement;
    private bool PowerUpPageIsOpen = false;
    public bool HasLoaded = false;
    public void Start()
    {
        SubPage = new EquipmentPage(this);
        PrimaryPage = new EquipmentPage(this);
        PowerUpLayout.MenuLayout = PowerLayout;
        PrimaryEquipments[0] = Hats;
        PrimaryEquipments[1] = Accessories;
        PrimaryEquipments[2] = Weapons;
        PrimaryEquipments[3] = Characters;
        myCanvas = GetComponent<Canvas>();
        HasLoaded = false;
        //for(int j = 0; j < 4; j++)
        //{
        //    for(int i = 0; i < PrimaryEquipments[j].Length; ++i)
        //    {
        //        PrimaryEquipments[j][i].GetComponent<Equipment>().myEquipmentIndex = i;
        //    }
        //}
        InitializeMainButtons();
    }
    public void Update()
    {
        if(!HasLoaded)
        {
            HasLoaded = true;
            //LoadData();
            //UpdateSelectedEquipmentBox(3, LastSelectedBody);
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
            MainButtons[i].UpdateActive(myCanvas, out bool hovering, out bool clicked);
            bool openPage = clicked;
            if(hovering)
            {
                hoveringElement = MainButtons[i].gameObject;
                if (NewHovering)
                    openPage = true;
            }
            else if(hoveringElement == MainButtons[i].gameObject && !PrimaryPage.IsOpen)
                hoveringElement = null;
            if (openPage && !hasOpenPageAlready)
            {
                hasOpenPageAlready = true;
                bool justClosed = false;
                if (PrimaryPage.Count > 0)
                {
                    justClosed = true;
                    PrimaryPage.Close();
                }
                if(!justClosed || i != prevPressedButton)
                {
                    PrimaryPage.Open(MainButtons[i], PrimaryEquipments[i]);
                    Debug.Log("Opened Page");
                }
                prevPressedButton = i;
            }
        }
        for(int i = 0; i < PrimaryPage.Count; i++)
        {
            EquipmentUIElement slot = PrimaryPage.Equips[i];
            slot.UpdateActive(myCanvas, out bool hovering, out bool clicked);
            //if (hovering)
            //{
            //    hoveringElement = slot.gameObject;
            //    if (NewHovering)
            //        openPage = true;
            //}
            //else if (hoveringElement == slot.gameObject && !EquipmentPageOpen)
            //    hoveringElement = null;
            if (clicked && !hasOpenPageAlready && slot.Unlocked)
            {
                hasOpenPageAlready = true;
                UpdateSelectedEquipmentBox(slot.ActiveEquipment.OriginalPrefab);
                //OPEN SUBEQUIPMENT SLOTS
                PrimaryPage.Close();
            }
        }
        prevHoveringElement = hoveringElement;
        if(!PowerUpPageIsOpen)
        {
            PowerLayout.Generate(PowerUp.AvailablePowers);
            PowerUpPageIsOpen = true;
        }
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
        RenderBox(MainButtons[i], equipPrefab);
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
            Player.Instance.Hat = equip as Hat;
        if (i == 1)
            Player.Instance.Cape = equip as Accessory;
        if (i == 2)
            Player.Instance.Wand = equip as Weapon;
        if (i == 3)
            SwapBody(oldEquipment.GetComponent<Equipment>() as Body, equip as Body);
        SaveCurrentSelects();
        Destroy(oldEquipment);
        PowerLayout.Generate(PowerUp.AvailablePowers);
    }
    public void SwapBody(Body oldBody, Body newBody)
    {
        Player.Instance.Body = newBody;
        //newBody.LoadData();
        //UpdateSelectedEquipmentBox(0, newBody.LastSelectedHat.Type);
        //UpdateSelectedEquipmentBox(1, newBody.LastSelectedAcc.Type);
        //UpdateSelectedEquipmentBox(2, newBody.LastSelectedWep.Type);
        // LastSelectedBody = newBody.myEquipmentIndex;
        //SaveCurrentSelects();
    }
    public void SaveCurrentSelects()
    {
        Body body = Player.Instance.Body;
        //body.LastSelectedHatType = Player.Instance.Hat.myEquipmentIndex;
        //body.LastSelectedAccType = Player.Instance.Cape.myEquipmentIndex;
        //body.LastSelectedWepType = Player.Instance.Wand.myEquipmentIndex;
        //body.SaveData();
        SaveData();
    }
    public void InitializeMainButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            RenderBox(MainButtons[i], PrimaryEquipments[i][0].GetComponent<Equipment>());
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
    public void OpenSecondaryEquipmentSlots(EquipmentUIElement parent)
    {
        SubPage.Open(parent, parent.ActiveEquipment.SubEquipment);
    }
}
