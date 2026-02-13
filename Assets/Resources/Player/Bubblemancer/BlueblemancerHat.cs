using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueblemancerHat : BubblemancerHat
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<StarbarbUnlock5>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bluebblemancy Hat").WithDescription("Stars last longer and orbit you");
    }
    public override void EquipUpdate()
    {
        Player.OrbitalStars = true;
    }
    public override int GetRarity()
    {
        return 3;
    }
}
