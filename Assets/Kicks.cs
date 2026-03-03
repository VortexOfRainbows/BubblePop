using System.Collections.Generic;
using UnityEngine;

public class Kicks : Crystal
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(-0.08f, 1.175f);
        scale = 1.8f;
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Fresh Kicks").WithDescription("Ain't got style without these!");
    }
    public override void InitializeAbilities(ref List<Ability> abilities)
    {
        //abilities.Add(new Ability(Ability.ID.Passive, $"Y:+1 Y:Heart"));
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<BubbleShield>();
    }
    public Transform LeftKickAnchor, RightKickAnchor;
    public Transform LeftKick, RightKick;
    protected override void AnimationUpdate()
    {
        bounceCount = 0.7f;
        velocity *= 0.9f;
        transform.localScale = new Vector3(Player.Body.FlipDir, 1, 1);
        LeftKickAnchor.transform.localPosition = new Vector3(-0.5f, -1f, 0.1f);
        RightKickAnchor.transform.localPosition = new Vector3(0.5f, -1, 0.1f);
        LeftKickAnchor.SetEulerZ(0);
        RightKickAnchor.SetEulerZ(0);
    }
    private float bounceCount = 0.7f;
    protected override void DeathAnimation()
    {
        float toBody = transform.localPosition.y - p.Body.transform.localPosition.y;
        if (p.DeathKillTimer <= 0)
        {
            velocity *= 0.1f;
            velocity.x += Utils.RandFloat(-1, 1) * 0.05f;
            velocity.y += 0.05f;
        }
        float bounceArea = -0.2f;
        if (toBody < bounceArea)
        {
            velocity *= -bounceCount;
            transform.localPosition = (Vector2)transform.localPosition + new Vector2(0, bounceArea - toBody);
            bounceCount *= 0.5f;
        }
        else
        {
            velocity.x *= 0.998f;
            velocity.y -= 0.005f;
        }
        transform.localPosition = (Vector2)transform.localPosition + velocity;
    }
}
