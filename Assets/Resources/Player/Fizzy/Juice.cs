using UnityEngine;

public class Juice : Cola
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
    }
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Primary, Ability.ID.Secondary, Ability.ID.Passive);
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyThirdDay>();
    protected override int AttackRate => 25;
    protected override float RightAttackSpeed => base.RightAttackSpeed - 10;
    public override int GetRarity()
    {
        return 5;
    }
    protected override float SpreadDegrees => -360;
    public override void OnStartWith()
    {
        for(int i = 0; i < 12; ++i)
            CoinManager.SpawnKey(p.transform.position, 0);
    }
    public override void EquipUpdate()
    {
        Player.HasJesusJuice = true;
    }
}