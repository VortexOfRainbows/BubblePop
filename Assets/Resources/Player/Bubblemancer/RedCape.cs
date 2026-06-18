using System.Collections.Generic;

public class RedCape : BubblemancerCape
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubbleBirbUnlock10>();
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Passive);
    }
    public override int GetRarity()
    {
        return 4;
    }
}
