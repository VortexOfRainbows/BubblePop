using System.Linq;

public class RedHat : BubblemancerHat
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubbleBirbUnlock10>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Red Bubblemancy Hat").WithDescription("Start with the rarest powerup in the availability pool");
    }
    public override void OnStartWith()
    {
        PowerUp.Spawn(PowerUp.AvailablePowers.Last(), Player.Position, 0);
    }
}
