using System.Collections.Generic;
using UnityEngine;

public class Cap : BubblemancerHat
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(0.32f, -0.16f);
        scale = 1.2f;
        rotation = 16f;
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Dash>();
        powerPool.Add<LuckyStar>();
        powerPool.Add<BinaryStars>();
        powerPool.Add<Starbarbs>();
        powerPool.Add<Supernova>();
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Rad Cap").WithDescription("Now this is a cool hat!");
    }
}
