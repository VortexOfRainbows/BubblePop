using System.Collections.Generic;
using System.Linq;

public class RedHat : BubblemancerHat
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubbleBirbUnlock10>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Red Bubblemancy Hat").WithDescription($"Start with {PowerUp.Get<BubbleBirb>().UnlockedName}");
    }
    public override void InitializeAbilities(ref List<Ability> abilities)
    {
        abilities.Add(new Ability(Ability.ID.Passive, $"Start with {PowerUp.Get<BubbleBirb>().UnlockedName}"));
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
