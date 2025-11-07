using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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
            int start = this == charSelect.PrimaryPage ? 0 : 1;
            for (int i = start; i < 2; ++i)
            {
                for (int j = 0; j < Equipments.Count; j++)
                {
                    Equipment e = Equipments[j].GetComponent<Equipment>();
                    if (start == 1 || (i == 0 && e.SameUnlockAsBody(Player.Instance.Body)) || (i == 1 && !e.SameUnlockAsBody(Player.Instance.Body)))
                    {
                        Add(parent, Equipments[j].GetComponent<Equipment>());
                    }
                }
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
            ui.targetScale = new Vector3(0.75f, 0.75f, 0.75f);
            if(IsPrimary)
                ui.transform.localPosition = parent.transform.localPosition + new Vector3(210 + 180 * Count, 0);
            else if(parent.ActiveEquipment is Body)
            {
                ui.DisplayOnly = true;
                ui.transform.localPosition = parent.transform.localPosition + new Vector3(0, 150 + 125 * Count);
                ui.targetScale *= 0.625f / 0.75f;
                ui.transform.localScale *= 0.65f;
            }
            else
                ui.transform.localPosition = parent.transform.localPosition + new Vector3(180 * Count, -180);
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
    public EquipmentUIElement[] DisplayBoxes;
    public List<GameObject>[] PrimaryEquipments => Main.GlobalEquipData.PrimaryEquipments;
    public List<GameObject> Hats => Main.GlobalEquipData.Hats;
    public List<GameObject> Accessories => Main.GlobalEquipData.Accessories;
    public List<GameObject> Weapons => Main.GlobalEquipData.Weapons;
    public List<GameObject> Characters => Main.GlobalEquipData.Characters;
    public List<GameObject> AllEquipmentsList => Main.GlobalEquipData.AllEquipmentsList;
    private Canvas myCanvas;
    private EquipmentPage SecondaryPage;
    private EquipmentPage PrimaryPage;
    private List<PowerUpUIElement> AvailablePowersUI = new();
    private bool PowerUpPageIsOpen = false;
    public bool HasLoaded = false;
    public static CharacterSelect Instance;
    public void Awake()
    {

    }
    public void Start()
    {
        Instance = this;
        SecondaryPage = new EquipmentPage(this, false);
        PrimaryPage = new EquipmentPage(this, true);
        PowerUpLayout.MenuLayout = PowerLayout;
        myCanvas = GetComponent<Canvas>();
        HasLoaded = false;
        InitializeMainButtons();
    }
    public void Update()
    {
        Instance = this;
        if(!HasLoaded)
        {
            HasLoaded = true;
            LoadData();
            UpdateSelectedEquipmentBox(LastSelectedBody);
        }
        if (!Main.WavesUnleashed)
        {
            visual.SetActive(true);
        }
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
        if (Player.Instance != null)
            CoinManager.TotalEquipCost = 0; //Player.Instance.Hat.GetPrice() + Player.Instance.Accessory.GetPrice() + Player.Instance.Weapon.GetPrice() + Player.Instance.Body.GetPrice(); 
    }
    public bool UISlotUpdate(EquipmentUIElement slot, EquipmentPage page, int index, bool AllowOpeningPage)
    {
        slot.UpdateActive(myCanvas, out bool hovering, out bool clicked, slot.GetComponent<RectTransform>());
        bool openPage = page == PrimaryPage ? clicked : false;
        bool justClosedFromDragOff = false;
        if (page != null)
        {
            if (hovering)
            {
                page.hoveringElement = slot.gameObject;
                if (page.NewHovering)
                    openPage = true;
            }
            else if (page.hoveringElement == slot.gameObject && !page.IsOpen)
            {
                page.hoveringElement = null;
            }
            else if(slot.ActiveEquipment is Body && page.IsOpen && page == SecondaryPage && page.PreviousType == index && page.hoveringElement == slot.gameObject)
            {
                openPage = true;
                page.hoveringElement = null; 
                justClosedFromDragOff = true;
            }
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
                if (((slot.Unlocked && slot.CanAfford) || page == PrimaryPage) && (!justClosed || index != page.PreviousType))
                {
                    if (page == PrimaryPage)
                        page.Open(slot, PrimaryEquipments[index]);
                    else
                    {
                        if(slot.ActiveEquipment is Body b)
                        {
                            b.LoadData();
                            List<GameObject> bodyPrevSelectedEquips = new List<GameObject>
                            {
                                AllEquipmentsList[b.LastSelectedWep],
                                AllEquipmentsList[b.LastSelectedAcc],
                                AllEquipmentsList[b.LastSelectedHat]
                            };
                            page.Open(slot, bodyPrevSelectedEquips);
                        }
                        else
                        {
                            page.Open(slot, slot.ActiveEquipment.OriginalPrefab.SubEquipment);
                        }
                    }
                }
                page.PreviousType = index;
            }
            return !justClosedFromDragOff;
        }
        if(page != PrimaryPage)
        {
            if(clicked && slot.Unlocked && slot.CanAfford)
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
            i = 1;
        else if (equipPrefab is Accessory)
            i = 2;
        else if (equipPrefab is Weapon)
            i = 3;
        else if (equipPrefab is Body)
            i = 0;
        SwapPlayerEquipment(equipPrefab);
        RenderBox(DisplayBoxes[i], equipPrefab);
    }
    public void UpdateSelectedEquipmentBox(int AllIndex)
    {
        Equipment equipPrefab = AllEquipmentsList[AllIndex].GetComponent<Equipment>();
        int i = -1;
        if (equipPrefab is Hat)
            i = 1;
        else if (equipPrefab is Accessory)
            i = 2;
        else if (equipPrefab is Weapon)
            i = 3;
        else if (equipPrefab is Body)
            i = 0;
        SwapPlayerEquipment(equipPrefab);
        RenderBox(DisplayBoxes[i], equipPrefab);
    }
    public void SwapPlayerEquipment(Equipment equipPrefab)
    {
        Equipment equip = null;
        int i = -1;
        if (equipPrefab is Hat)
        {
            i = 1;
            equip = Player.Instance.Hat;
        }
        else if (equipPrefab is Accessory)
        {
            i = 2;
            equip = Player.Instance.Accessory;
        }
        else if (equipPrefab is Weapon)
        {
            i = 3;
            equip = Player.Instance.Weapon;
        }
        else if (equipPrefab is Body)
        {
            i = 0;
            equip = Player.Instance.Body;
        }

        GameObject oldEquipment = equip.gameObject;
        equip = Instantiate(equipPrefab, Player.Instance.Visual.transform).GetComponent<Equipment>();
        equip.p = Player.Instance.Animator;
        equip.AliveUpdate();
        if (i == 1)
        {
            Player.Instance.Hat = equip as Hat;
            Player.Instance.Body.LastSelectedHat = equip.IndexInAllEquipPool;
        }
        if (i == 2)
        {
            Player.Instance.Accessory = equip as Accessory;
            Player.Instance.Body.LastSelectedAcc = equip.IndexInAllEquipPool;
        }
        if (i == 3)
        {
            Player.Instance.Weapon = equip as Weapon;
            Player.Instance.Body.LastSelectedWep = equip.IndexInAllEquipPool;
        }
        if (i == 0)
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
        LastSelectedBody = newBody.IndexInAllEquipPool;
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
        uiElem.UpdateEquipment(equipmentToRender);
    }
}
