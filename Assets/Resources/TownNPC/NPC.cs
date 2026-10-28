using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC : Entity
{
    public static Dictionary<NPCType, string> DialogueVariantPair = new()
    {
        {NPCType.None, ""},
        {NPCType.Bubblemancer, "B"},
        {NPCType.ThoughtBubble, "T"},
        {NPCType.Gachapon, "G"},
        {NPCType.Fizzy, "F"},
        {NPCType.OilKing, "K"},
    };
    public static string GetDialogueVariant(NPCType type) => DialogueVariantPair[type];
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
        GachaponMansion,
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
    public bool AllowRepeatedDialogue = false;
    public bool HasOpenedDialogueAlready { get; private set; }
    public void UpdateUI()
    {
        Player p = Player.FindClosest(transform.position, out Vector2 norm, out float distance, 100);
        if (distance < 7 && p != null && p.ThisIsPlayerClosestInteractable(gameObject) && !DialogueController.Instance.WantsOpen && (!HasOpenedDialogueAlready || AllowRepeatedDialogue))
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
    //It might be good to make a version of this with (string, NPCType, out string EXPRESSION) where expression could be determined in the hjson file!
    public static DialogueController.DialogueEvent GetDialogueSegmentWithArguments(string segment, NPCType variant, NPCType Right = NPCType.None, string LeftExpression = null, string RightExpression = null)
    {
        string rawSegment = GetDialogueSegment(segment, variant);
        int argumentSectionEnd = rawSegment.LastIndexOf(')') + 1;
        string[] parts = rawSegment[..argumentSectionEnd].Split(' ');
        string expression = default;
        bool playerTalking = false;
        foreach (string part in parts)
        {
            if(part.StartsWith("("))
            {
                string arg = part[1..^1].ToLower(); //strip the parenthesis
                if (arg.Equals("skip"))
                    return null;
                else if (arg.Equals("player"))
                    playerTalking = true;
                else
                    expression = arg;
            }
        }
        //Might need to add an additional override for Left if the player is ever not the one in the left slot. Though this is probably not gonna happen.
        string dialogue = argumentSectionEnd >= rawSegment.Length ? string.Empty : rawSegment[(argumentSectionEnd != 0 ? argumentSectionEnd + 1 : 0)..];
        return new(dialogue, playerTalking, variant, Right, playerTalking ? expression : LeftExpression, playerTalking ? RightExpression : expression);
    }
    public static string GetDialogueSegment(string segment, NPCType variant) => Localization.TryGetVariant($"Dialogue.{segment}", GetDialogueVariant(variant));
    public void StartDialogue()
    {
        HasOpenedDialogueAlready = true;
        NPCType p = Player.Instance.Body.DialogueType;
        switch(Dialogue)
        {
            case DialogueType.GachaponShop:
                int rand = Utils.RandInt(4);
                if(rand == 0)
                {
                    DialogueController.EnqueueDialogueChain(
                        GetDialogueSegmentWithArguments("GachaponShop1_1", p, NPCType.Gachapon, "default", "default"),
                        GetDialogueSegmentWithArguments("GachaponShop1_2", p),
                        GetDialogueSegmentWithArguments("GachaponShop1_3", p)
                        );
                }
                else if(rand == 1)
                {
                    DialogueController.EnqueueDialogueChain(
                        GetDialogueSegmentWithArguments("GachaponShop2_1", p, NPCType.Gachapon, "default", "default"),
                        GetDialogueSegmentWithArguments("GachaponShop2_2", p),
                        GetDialogueSegmentWithArguments("GachaponShop2_3", p),
                        GetDialogueSegmentWithArguments("GachaponShop2_4", p),
                        GetDialogueSegmentWithArguments("GachaponShop2_5", p)
                        );
                }
                else if (rand == 2)
                {
                    DialogueController.EnqueueDialogueChain(
                        GetDialogueSegmentWithArguments("GachaponShop3_1", p, NPCType.Gachapon, "default", "default"),
                        GetDialogueSegmentWithArguments("GachaponShop3_2", p)
                        );
                }
                else if (rand == 3)
                {
                    DialogueController.EnqueueDialogueChain(
                        GetDialogueSegmentWithArguments("GachaponShop4_1", p, NPCType.Gachapon, "default", "default"),
                        GetDialogueSegmentWithArguments("GachaponShop4_2", p),
                        GetDialogueSegmentWithArguments("GachaponShop4_3", p)
                        );
                }
                break;
            case DialogueType.GachaponMansion:
                DialogueController.EnqueueDialogueChain(
                    GetDialogueSegmentWithArguments("GachaponMansion1_1", p, NPCType.Gachapon, "default", "default"),
                    GetDialogueSegmentWithArguments("GachaponMansion1_2", p),
                    GetDialogueSegmentWithArguments("GachaponMansion1_3", p)
                    );
                break;
        }
    }
}
