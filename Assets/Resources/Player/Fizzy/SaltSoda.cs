using UnityEngine;

public class SaltSoda : Cola
{
    public override void ModifyLifeStats(ref int MaxLife, ref int Life, ref int Shield)
    {
        if(Life > 1)
            Life -= Life - 1;
    }
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        scale *= 1.025f;
    }
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Primary, Ability.ID.Secondary, Ability.ID.Passive);
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyFakeDoctor>();
    public override void EquipUpdate()
    {
        base.EquipUpdate();
        Player.ChoiceOnHeal += 1;
    }
    protected override int AttackRate => 11;
    protected override float AttackCooldown => base.AttackCooldown - 5;
    public override int GetRarity()
    {
        return 2;
    }
}