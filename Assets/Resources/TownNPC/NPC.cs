using UnityEngine;

public class NPC : Entity
{
    public enum NPCType
    {
        None,
        Gachapon,
    }
    public NPCType Type = NPCType.None;
    public PlayerAnimator p;
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
                AddEquip(Main.GlobalEquipData.GachaponHat);
                AddEquip(Main.GlobalEquipData.Gachapon);
                AddEquip(Main.GlobalEquipData.GachaponCape);
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
}
