using System.Collections.Generic;
using UnityEngine;

public class Tie : Accessory
{
    public override void ModifyLifeStats(ref int MaxLife, ref int Life, ref int Shield)
    {

    }
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(0, 0.2f);
        scale = 1.725f;
    }
    public override void ModifyDescription(ref EquipDescription description)
    {

    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<KingOilUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<BlackMarketDelivery>();
    }
    protected override void AnimationUpdate()
    {
        float dir = p.Body.Flipped ? -1 : 1;
        transform.localScale = new Vector3(p.Body.transform.localScale.x * dir, p.Body.transform.localScale.y * 0.5f + 0.475f, 1);
        transform.LerpLocalPosition(new Vector2(0.1625f * dir, -0.825f), 0.15f);
        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.zero, 0.15f);
        TiePart.LerpLocalScale(Vector2.one, 0.05f);
        TiePart.LerpLocalEulerZ(0, 0.05f);
    }
    public Transform TiePart;
    protected override void DeathAnimation()
    {
        float dir = p.Body.Flipped ? -1 : 1;
        transform.LerpLocalPosition(new Vector2(0.05f * dir, -1.55f), 0.1f);
        transform.LerpLocalScale(new Vector2(1.25f * dir, 0.8f), 0.1f);
        TiePart.LerpLocalScale(new Vector2(1 / 1.25f, 1), 0.1f);
        TiePart.LerpLocalEulerZ(15, 0.1f);
    }
}
