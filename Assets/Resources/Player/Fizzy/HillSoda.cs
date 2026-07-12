using UnityEngine;

public class HillSoda : Cola
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        scale *= 1.05f;
        offset.x += 0.025f;
        offset.y += 0.025f;
    }
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Primary, Ability.ID.Secondary, Ability.ID.Passive);
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyStuffed>();
    protected override int AttackRate => 30;
    protected override float AttackCooldown => Mathf.Max(40, 83f - Player.AttackSpeedModifier * 3);
    protected override float RightAttackSpeed => base.RightAttackSpeed + 20;
    public override int GetRarity()
    {
        return 4;
    }
    protected override float SpreadDegrees => -180;
    public override void OnStartWith()
    {
        base.OnStartWith();
        PowerUp.Spawn<Burger>(Player.Position);
    }
}