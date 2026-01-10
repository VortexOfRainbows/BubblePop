using System.Linq;

public class RedHat : BubblemancerHat
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubbleBirbUnlock10>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Red Bubblemancy Hat").WithDescription($"Start with {PowerUp.Get<BubbleBirb>().UnlockedName}");
    }
    public override void OnStartWith()
    {
        PowerUp.Spawn<BubbleBirb>(Player.Position);
        //PowerUp.Spawn(PowerUp.AvailablePowers.Last(), Player.Position, 0);
    }
    public override int GetRarity()
    {
        return 4;
    }
}
