using System.Collections.Generic;
using UnityEngine;

public class Cryskull : Crystal
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = Vector2.zero;
        scale = 2f;
    }
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Passive, Ability.ID.Passive);
    }
    public override void EquipUpdate()
    {
        Player.PersonalWaveCardBonus += 1f;
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<GachaponWave15AllSkullWaves>();
    public override int GetRarity()
    {
        return 4;
    }
}
