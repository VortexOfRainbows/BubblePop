using System.Collections.Generic;

public class RedCape : BubblemancerCape
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubbleBirbUnlock10>();
    public override void InitializeAbilities(ref List<Ability> abilities)
    {
        abilities.Add(new Ability(Ability.ID.Passive, $"Gain a {PowerUp.Get<Contract>().UnlockedName} everytime you resurrect"));
    }
    public override int GetRarity()
    {
        return 4;
    }
}
