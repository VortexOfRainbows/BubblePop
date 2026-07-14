using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingOil : Body
{
    public override float AbilityCD => 6;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<KingOilUnlock>();
    public override void Init()
    {
        Player.abilityTimer = 0;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add(PowerUp.Get<Burger>());
        powerPool.Add(PowerUp.Get<GlassShard>());
    }
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Ability);
    }
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {
        if (Player.Control.Ability && !Player.Control.LastAbility)
        {
            
        }
    }
    public override void FaceUpdate()
    {
        Vector2 toMouse = p.LookPosition - (Vector2)transform.position;
        Vector2 toMouse2 = toMouse.normalized;
        toMouse2.x += Mathf.Sign(toMouse2.x) * 4;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.08f, 0).RotatedBy(toMouseR);
        looking.x += 0.04f * p.Direction;
        Vector2 pos = looking;
        Face.transform.LerpLocalPosition(pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouse2.y * 12.5f * p.Direction);
        Face.transform.localScale = new Vector3(p.Direction, 1, 1);
    }
}