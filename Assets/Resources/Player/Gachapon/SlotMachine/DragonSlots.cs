using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSlots : SlotMachineWeapon
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<GachaponBubblebirb>();
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        offset.x -= 0.08f;
        scale *= 0.925f;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        base.ModifyPowerPool(powerPool);
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Dragon Slots").WithDescription("99% of bubble hit it big before popping");
        //\nGet increasingly inaccurate while firing, but non-winning spins spew fire instead of bubbles
    }
    public Transform Jaw;
    public Transform OpenMouth;
    public float InaccuracyMultiplier = 0.0f;
    protected override void AnimationUpdate()
    {
        base.AnimationUpdate(); //DO NOT REMOVE
        OpenMouth.gameObject.SetActive(true);
        Jaw.transform.LerpLocalPosition(new Vector2(0, 0.275f), 0.03f);
        if (AttackGamble <= 0 || AttackGamble >= 60)
            InaccuracyMultiplier *= 0.99f;
    }
    public override void Shoot(Vector2 shootFrom, Vector2 norm, float separation, int i, int t)
    {
        Jaw.transform.localPosition = new Vector3(0, -0.2f);
        norm = norm.RotatedBy(Mathf.Min(Utils.RandFloat(), Utils.RandFloat(0.2f, 2f)) * Utils.Rand1OrMinus1() * Mathf.Min(180, InaccuracyMultiplier) * Mathf.Deg2Rad);
        if(t == 0)
        {
            for(int j = 1; j < 6; ++j)
                Projectile.NewProjectile<GachaProj>(shootFrom, norm.RotatedBy(Mathf.Deg2Rad * separation * i + Utils.RandFloat(-5 * j, 5 * j) * Mathf.Deg2Rad) * (15 + 2 * j - Mathf.Abs(i) * 1f), 2, Player, 4);
        }
        else
        {
            base.Shoot(shootFrom, norm, separation, i, t);
        }
        InaccuracyMultiplier += 4;
    }
    public override int GetRarity()
    {
        return 2;
    }
}
