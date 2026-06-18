using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueblemancerHat : BubblemancerHat
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<StarbarbUnlock5>();
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Passive);
    }
    public override void EquipUpdate()
    {
        Player.OrbitalStars = true;
    }
    public override int GetRarity()
    {
        return 3;
    }
}
