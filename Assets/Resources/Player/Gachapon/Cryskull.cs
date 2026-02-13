using UnityEngine;

public class Cryskull : Crystal
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = Vector2.zero;
        scale = 2f;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Cryskull").WithDescription("Grants an additional heart\nIncreases wave card difficulty and rewards");
    }
    public override void EquipUpdate()
    {
        Player.PersonalWaveCardBonus += 1f;
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<GachaponWave15AllSkullWaves>();
    public override int GetRarity()
    {
        return 4;
    }
}
