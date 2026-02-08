using System.Collections.Generic;
using UnityEngine;

public class UtilityBelt : Accessory
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        base.ModifyUIOffsets(isBubble, ref offset, ref rotation, ref scale);
        scale *= 1.075f;
        offset.x -= 0.07f;
    }
    public override int GetRarity()
    {
        return 2;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Calculator>();
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ThoughtBubbleArsenal>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Utility Belt").WithDescription($"Start with a {"Magnet".WithColor(DetailedDescription.Rares[0])}, a Y:Key, and Y:[100 coins]");
    }
    public override void OnStartWith()
    {
        PowerUp.Spawn<Magnet>(p.transform.position);
        CoinManager.SpawnKey(p.transform.position, 0);
        CoinManager.SpawnCoin(p.transform.position, 100, 0);
    }
    protected override void AnimationUpdate()
    {
        //Time.timeScale = 0.2f;
        Vector2 toMouse = p.LookPosition - (Vector2)p.Body.transform.position;
        float facingDir = p.Direction;
        Vector2 scale = new Vector2((1 - p.squash) * 2.5f + 0.1f * (1 - p.Bobbing), p.Bobbing * p.squash);
        scale.x *= 0.2f + 0.875f * Mathf.Cos(new Vector2(Mathf.Abs(p.lastVelo.x), p.lastVelo.y).ToRotation());
        Vector2 pos = p.Body is ThoughtBubble ? new Vector2(-0.04f, 0.475f) : new Vector2(-0.08f, 0.375f);
        if(p.Body is ThoughtBubble)
            scale.x += 0.07f;
        else if (p.Body is Gachapon)
            scale.x += 0.04f;
        transform.localPosition = new Vector3(pos.x * facingDir, p.Bobbing - pos.y, 0);
        transform.localScale = new Vector3((1 + scale.x) * facingDir * 1.02f, 0.55f + 0.425f * scale.y, transform.localScale.z);
    }
}
