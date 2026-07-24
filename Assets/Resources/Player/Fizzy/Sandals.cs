using System.Collections.Generic;
using UnityEngine;

public class Sandals : Kicks
{
    public override int GetRarity() => 2;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzySpeedrun>();
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Passive);
    }
    public override void EquipUpdate()
    {
        Player.IgnoreMovespeed = true;
    }
}
