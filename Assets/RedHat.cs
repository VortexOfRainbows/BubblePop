using System.Linq;

public class RedHat : BubblemancerHat
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<StartsUnlocked>();
    protected override string Description()
    {
        return "Start with the rarest powerup in the availability pool";
    }
    public override void OnStartWith()
    {
        PowerUp.Spawn(PowerUp.AvailablePowers.Last(), Player.Position, 0);
    }
}
