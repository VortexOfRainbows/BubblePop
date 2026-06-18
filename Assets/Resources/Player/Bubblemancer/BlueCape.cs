using System.Collections.Generic;

public class BlueCape : BubblemancerCape
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<WaveUnlock10>();
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Passive);
    }
    public override void OnStartWith()
    {
        int i = Utils.RandInt(PowerUp.AvailablePowers.Count);
        for(int j = 0; j < 50; ++j)
        {
            if (PowerUp.Get(PowerUp.AvailablePowers[i]).Rarity == 3)
                break;
            i = Utils.RandInt(PowerUp.AvailablePowers.Count);
        }
        PowerUp.Spawn(PowerUp.AvailablePowers[i], Player.Position);  
    }
    public override int GetRarity()
    {
        return 3;
    }
}
