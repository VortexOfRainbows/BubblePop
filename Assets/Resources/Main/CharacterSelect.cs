using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public class EquipmentPage
    {
        public int Index = -1;
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
        public void Open(EquipmentUIElement parent, List<GameObject> Equipments, int newIndex)
        {
            Close();
            int start = this == charSelect.PrimaryPage ? 0 : 1;
            for (int i = start; i < 2; ++i)
            {
                for (int j = 0; j < Equipments.Count; j++)
                {
                    Equipment e = Equipments[j].GetComponent<Equipment>();
                    bool sortSoCurrentCharacterEquipmentShowsFirst = (i == 0 && e.SameUnlockAsBody(Player.Instance.Body)) || (i == 1 && !e.SameUnlockAsBody(Player.Instance.Body));
                    if (start == 1 || sortSoCurrentCharacterEquipmentShowsFirst)
                    {
                        AddEquipToPage(parent, Equipments[j].GetComponent<Equipment>());
                    }
                }
            }
            Index = newIndex;
            IsOpen = true;
        }
        /// <summary>
        /// Closes the equipment page.
        /// </summary>
        public void Close()
        {
            Index = -1;
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
        public void AddEquipToPage(EquipmentUIElement parent, Equipment equipment)
        {
            EquipmentUIElement ui = Instantiate(charSelect.EquipmentUISlotPrefab, parent.transform);
            ui.GetComponent<Image>().maskable = false;
            ui.targetScale = new Vector3(0.85f, 0.85f, 0.85f);
            if(IsPrimary)
            {
                ui.transform.localPosition = new Vector3(150 + 150 * Count, 0);
                ui.transform.localScale *= 0.85f;
            }
            else if(parent.ActiveEquipment is Body)
            {
                ui.DisplayOnly = true;
                ui.transform.localPosition = new Vector3(0, 130 + 110 * Count);
                ui.targetScale *= 0.85f;
                ui.transform.localScale *= 0.85f;
            }
            else
                ui.transform.localPosition = new Vector3(150 * Count, -150);
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
    private Canvas myCanvas;
    public EquipmentPage SecondaryPage { get; private set; }
    public EquipmentPage PrimaryPage { get; private set; }
    private bool PowerUpPageIsOpen = false;
    public bool HasLoaded = false;
    public static CharacterSelect Instance;
    public RectTransform HangerButton, Slider, HoverMask;
    public Image HangerIcon;
    public bool selectMenuOpen = false;
    public void Start()
    {
        Instance = this;
        SecondaryPage = new EquipmentPage(this, false);
        PrimaryPage = new EquipmentPage(this, true);
        PowerUpLayout.MenuLayout = PowerLayout;
        myCanvas = GetComponent<Canvas>();
        HasLoaded = false;
        Utils.LerpSnap(Slider.transform, new Vector2(0, 645), 1f);
        InitializeMainButtons();
    }
    public void OnUpdate()
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
        if(Main.GamePaused)
        {
            return;
        }
        //hangarButtonImage = hangarButtonImage != null ? hangarButtonImage : HangerButton.GetComponent<Image>();
        float lerpFactor = Utils.DeltaTimeLerpFactor(0.1f);
        if (Utils.IsMouseHoveringOverThis(true, HangerButton, 0, myCanvas))
        {
            if(Control.LeftMouseClick)
                selectMenuOpen = !selectMenuOpen;
            if(!selectMenuOpen)
            {
                PrimaryPage.Close();
                SecondaryPage.Close();
                PrimaryPage.hoveringElement = PrimaryPage.prevHoveringElement = null;
                SecondaryPage.hoveringElement = SecondaryPage.prevHoveringElement = null;
            }
            HangerIcon.transform.LerpLocalScale(new Vector2(1.375f, 1.55f) * 1.1f, lerpFactor);
        }
        else
        {
            HangerIcon.transform.LerpLocalScale(new Vector2(1.375f, 1.55f), lerpFactor);
        }
        if(selectMenuOpen)
        {
            Utils.LerpSnap(HangerButton.transform, new Vector2(0, 600), lerpFactor);
            Utils.LerpSnap(Slider.transform, new Vector2(0, 45), lerpFactor);
        }
        else
        {
            Utils.LerpSnap(HangerButton.transform, new Vector2(0, 0), lerpFactor);
            Utils.LerpSnap(Slider.transform, new Vector2(0, 645), lerpFactor);
        }
        if (selectMenuOpen)
        {
            SelectEquipUpdate();
        }
    }
    public void SelectEquipUpdate()
    {
        bool hasOpenPageAlready = false;
        for (int i = 0; i < 4; i++)
            if (IsWithinMaskRange(DisplayBoxes[i]) && UISlotUpdate(DisplayBoxes[i], PrimaryPage, i, !hasOpenPageAlready))
                hasOpenPageAlready = true;
        for (int i = 0; i < PrimaryPage.Count; i++)
            if (IsWithinMaskRange(PrimaryPage.Equips[i]) && UISlotUpdate(PrimaryPage.Equips[i], SecondaryPage, i, !hasOpenPageAlready))
                hasOpenPageAlready = true;
        for (int i = 0; i < SecondaryPage.Count; i++)
            if (IsWithinMaskRange(SecondaryPage.Equips[i]) && UISlotUpdate(SecondaryPage.Equips[i], null, i, false))
                hasOpenPageAlready = true;
        PrimaryPage.prevHoveringElement = PrimaryPage.hoveringElement;
        SecondaryPage.prevHoveringElement = SecondaryPage.hoveringElement;
        if (!PowerUpPageIsOpen)
        {
            PowerLayout.Generate(PowerUp.AvailablePowers);
            PowerUpPageIsOpen = true;
        }
        //if (player != null)
        //    CoinManager.TotalEquipCost = 0; //player.Hat.GetPrice() + player.Accessory.GetPrice() +player.Weapon.GetPrice() + player.Body.GetPrice(); 
    }
    public bool IsWithinMaskRange(EquipmentUIElement t)
    {
        float relativeMaskPosition = HoverMask.transform.position.y;
        var r = t.GetComponent<RectTransform>();
        return relativeMaskPosition > r.position.y + r.sizeDelta.y * 0.5f * r.lossyScale.y;
    }
    public bool UISlotUpdate(EquipmentUIElement slot, EquipmentPage page, int index, bool AllowOpeningPage)
    {
        bool pageIsCurrentlyOpen = page != null && page.IsOpen && page.Index == index;
        slot.UpdateActive(myCanvas, out bool hovering, out bool clicked, slot.GetComponent<RectTransform>(), pageIsCurrentlyOpen ? 1.05f : 1.0f);
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
                        page.Open(slot, Main.GlobalEquipData.PrimaryEquipments[index], index);
                    else
                    {
                        if(slot.ActiveEquipment is Body b)
                        {
                            b.LoadData();
                            List<GameObject> bodyPrevSelectedEquips = new()
                            {
                                Main.GlobalEquipData.AllEquipmentsList[b.LastSelectedWep],
                                Main.GlobalEquipData.AllEquipmentsList[b.LastSelectedAcc],
                                Main.GlobalEquipData.AllEquipmentsList[b.LastSelectedHat]
                            };
                            page.Open(slot, bodyPrevSelectedEquips, index);
                        }
                        else
                        {
                            page.Open(slot, slot.ActiveEquipment.OriginalPrefab.SubEquipment, index);
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
        for(int j = 0; j < Player.AllPlayers.Count; ++j)
            SwapPlayerEquipment(Player.GetInstance(j), equipPrefab);
        RenderBox(DisplayBoxes[i], equipPrefab);
    }
    public void UpdateSelectedEquipmentBox(int AllIndex)
    {
        //Debug.Log($"Tried Accessing Equip Index: {AllEquipmentsList}, {AllIndex}");
        Equipment equipPrefab = Main.GlobalEquipData.AllEquipmentsList[AllIndex].GetComponent<Equipment>();
        int i = -1;
        if (equipPrefab is Hat)
            i = 1;
        else if (equipPrefab is Accessory)
            i = 2;
        else if (equipPrefab is Weapon)
            i = 3;
        else if (equipPrefab is Body)
            i = 0;
        for (int j = 0; j < Player.AllPlayers.Count; ++j)
            SwapPlayerEquipment(Player.GetInstance(j), equipPrefab);
        RenderBox(DisplayBoxes[i], equipPrefab);
    }
    public void SwapPlayerEquipment(Player player, Equipment equipPrefab)
    {
        Equipment equip = null;
        int i = -1;
        if (equipPrefab is Hat)
        {
            i = 1;
            equip = player.Hat;
        }
        else if (equipPrefab is Accessory)
        {
            i = 2;
            equip = player.Accessory;
        }
        else if (equipPrefab is Weapon)
        {
            i = 3;
            equip = player.Weapon;
        }
        else if (equipPrefab is Body)
        {
            i = 0;
            equip = player.Body;
        }

        Equipment oldEquipment = equip;
        equip = Instantiate(equipPrefab, player.Visual.transform).GetComponent<Equipment>();
        equip.p = player.Animator;
        if (i == 1)
        {
            player.Hat = equip as Hat;
            player.Body.LastSelectedHat = equip.IndexInAllEquipPool;
        }
        else if (i == 2)
        {
            player.Accessory = equip as Accessory;
            player.Body.LastSelectedAcc = equip.IndexInAllEquipPool;
        }
        else if (i == 3)
        {
            player.Weapon = equip as Weapon;
            player.Body.LastSelectedWep = equip.IndexInAllEquipPool;
        }
        if (i == 0)
            SwapBody(player, oldEquipment as Body, equip as Body);
        else
            equip.AliveUpdate();
        if (player.InstanceID == 0)
            SaveCurrentSelects();
        Destroy(oldEquipment.gameObject);
        if(player.InstanceID == 0)
            PowerLayout.Generate(PowerUp.AvailablePowers);
    }
    public void SwapBody(Player player, Body oldBody, Body newBody)
    {
        player.Body = newBody;
        player.Body.AliveUpdate();
        player.Body.FlipDir = oldBody.FlipDir;
        newBody.LoadData();
        UpdateSelectedEquipmentBox(newBody.LastSelectedHat);
        UpdateSelectedEquipmentBox(newBody.LastSelectedAcc);
        UpdateSelectedEquipmentBox(newBody.LastSelectedWep);
        LastSelectedBody = newBody.IndexInAllEquipPool;
        if(player.InstanceID == 0)
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
            RenderBox(DisplayBoxes[i], Main.GlobalEquipData.PrimaryEquipments[i][0].GetComponent<Equipment>());
        }
    }
    public void RenderBox(EquipmentUIElement uiElem, Equipment equipmentToRender)
    {
        if(uiElem.ActiveEquipment != null)
            Destroy(uiElem.ActiveEquipment.gameObject);
        uiElem.UpdateEquipment(equipmentToRender);
        uiElem.SetCompendiumLayering(Main.UICameraLayerID, 45);
    }
}
