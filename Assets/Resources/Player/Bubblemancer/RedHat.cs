using System.Collections.Generic;
using System.Linq;

public class RedHat : BubblemancerHat
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubbleBirbUnlock10>();
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Passive);
    }
    public override void OnStartWith()
    {
        PowerUp.Spawn<BubbleBirb>(Player.Position);
    }
    public override int GetRarity()
    {
        return 4;
    }
}
