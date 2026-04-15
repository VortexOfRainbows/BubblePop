using System.Collections.Generic;
using UnityEngine;

public class CatEars : Dice
{
    public override int GetRarity()
    {
        return 4;
    }
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(0.0825f, -0.75f);
        scale = 1.8f;
        rotation = 5f;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Cat Ears").WithDescription("Tail not included");
    }
    public override void InitializeAbilities(ref List<Ability> abilities)
    {
        abilities.Add(new Ability(Ability.ID.Passive, "Gain Y:[5 coins] when a Y:power is acquired"));
    }
    public override void EquipUpdate()
    {
        p.MyPlayer.CoinsOnPowerPickup += 5;
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<GachaponBlackjack>();
}
