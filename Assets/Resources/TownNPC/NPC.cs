using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPC : Entity
{
    public enum NPCType
    {
        None,
        Gachapon,
        ThoughtBubble,
        OilKing,
        Fizzy,
        Bubblemancer,
    }
    public enum DialogueType
    {
        None,
        GachaponShop,
    }
    public NPCType Type = NPCType.None;
    public DialogueType Dialogue = DialogueType.None;
    public PlayerAnimator p;
    public InteractableWorldObject IWO;
    public override void Init()
    {
        InitializeEquips();
    }
    public void AddEquip(GameObject prefab)
    {
        Equipment e = Instantiate(prefab, transform).GetComponent<Equipment>();
        e.Player = null;
        e.p = p;
        if(e is Hat h)
            p.Hat = h;
        else if(e is Body b)
            p.Body = b;
        else if(e is Accessory a)
            p.Accessory = a;
        else if (e is Weapon) //NPCS SHOULD GENERALLY NOT HAVE WEAPONS
            throw new System.Exception("NPCs having weapons is not currently implemented");
    }
    public void InitializeEquips()
    {
        switch(Type)
        {
            case NPCType.Gachapon:
                AddEquip(Main.GlobalEquipData.Gachapon);
                AddEquip(Main.GlobalEquipData.GachaponHat);
                AddEquip(Main.GlobalEquipData.GachaponCape);
                break;
            case NPCType.ThoughtBubble:
                AddEquip(Main.GlobalEquipData.ThoughtBubble);
                AddEquip(Main.GlobalEquipData.ThoughtBubbleHat);
                AddEquip(Main.GlobalEquipData.ThoughtBubbleCape);
                break;
            case NPCType.OilKing:
                AddEquip(Main.GlobalEquipData.KingOil);
                AddEquip(Main.GlobalEquipData.OilHat);
                AddEquip(Main.GlobalEquipData.OilTie);
                break;
            case NPCType.Fizzy:
                AddEquip(Main.GlobalEquipData.Fizzy);
                AddEquip(Main.GlobalEquipData.Cap);
                AddEquip(Main.GlobalEquipData.Kicks);
                break;
            case NPCType.Bubblemancer:
                AddEquip(Main.GlobalEquipData.Bubblemancer);
                AddEquip(Main.GlobalEquipData.BubblemancerHat);
                AddEquip(Main.GlobalEquipData.BubblemancerCape);
                break;
        }
        p.Body.p = p.Hat.p = p.Accessory.p = p;
    }
    public override void OnFixedUpdate()
    {
        p.Body.AliveUpdate();
        p.Hat.AliveUpdate();
        p.Accessory.AliveUpdate();
        if (p.LookPosition.x < transform.position.x)
            p.lastVelo.x = -1;
        else
            p.lastVelo.x = 1;
        p.PostFixedUpdate();
    }
    public bool TryAddingToUIInteraction { get; private set; } = true;
    public new void Update()
    {
        base.Update(); //This is important to call

        //UI UPDATES HAPPEN HERE
        if (IWO != null && Dialogue != DialogueType.None)
        {
            if(TryAddingToUIInteraction)
            {
                Player.ObjectsConsideredForUIInteraction.Add(gameObject);
                TryAddingToUIInteraction = false;
            }
            UpdateUI();
        }
    }
    public void UpdateUI()
    {
        Player p = Player.FindClosest(transform.position, out Vector2 norm, out float distance, 100);
        if (distance < 7 && p != null && p.ThisIsPlayerClosestInteractable(gameObject) && !DialogueController.Instance.WantsOpen)
        {
            IWO.EnableUI();
            if (Input.GetKeyDown(KeyCode.E))
            {
                IWO.PopupUI.GetChild(0).gameObject.SetActive(false);
                StartDialogue();
            }
        }
        else
        {
            IWO.DisableUI();
        }
    }
    public void StartDialogue()
    {
        NPCType PlayerCharacter = Player.Instance.Body.DialogueType;
        switch(Dialogue)
        {
            case DialogueType.GachaponShop: //AI Generated this very funny.
                DialogueController.EnqueueDialogueChain(
                    new(Localization.Get("Dialogue.GachaponShopIntro1"), PlayerCharacter, 0, NPCType.Gachapon, 0, false),
                    new(Localization.Get("Dialogue.GachaponShopEnd1")));
                break;
        }
    }
}
