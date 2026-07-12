using System.Collections.Generic;
using UnityEngine;

public class FocusFizzSoda : Cola
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        scale *= 1.05f;
    }
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Primary, Ability.ID.Secondary, Ability.ID.Passive);
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyFocus>();
    public override void Init()
    {
        transform.localScale = Vector3.zero;
    }
    public override void EquipUpdate()
    {
        base.EquipUpdate();
        Player.SecondaryAttackSpeedModifier += 0.1f;
    }
    protected override int AttackRate => 8;
    protected override float AttackCooldown => base.AttackCooldown + 5;
    protected override float SpreadDegrees => 45;
    public override void OnStartWith()
    {
        base.OnStartWith();
        int i = Utils.RandInt(PowerUp.AvailablePowers.Count);
        for (int j = 0; j < 50; ++j)
        {
            if (PowerUp.Get(PowerUp.AvailablePowers[i]).Rarity == 3)
                break;
            i = Utils.RandInt(PowerUp.AvailablePowers.Count);
        }
        PowerUp.Spawn(PowerUp.AvailablePowers[i], Player.Position);
    }
}
