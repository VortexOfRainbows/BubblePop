using System.Collections.Generic;
using UnityEngine;

public class Kicks : Accessory
{
    public override void Init()
    {
        velocities = new Vector2[] { Vector2.zero, Vector2.zero };
        base.Init();
    }
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(-0.12f, 1.15f);
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
    public LegMotion LeftKick, RightKick;
    protected override void AnimationUpdate()
    {
        bounceCount = 0.7f;
        velocity *= 0.9f;
        transform.localScale = new Vector3(Player.Body.FlipDir, 1, 1);
        transform.localPosition = new Vector3(-0.15f * Player.Body.FlipDir, 0, 0);
        LeftKickAnchor.transform.localPosition = new Vector3(-0.4325f, -1.05f, -0.1f);
        RightKickAnchor.transform.localPosition = new Vector3(0.4325f, -1.05f, 0.1f);
        float directionOffset = 0.025f;
        LeftKick.ReAnchorOffset = new Vector2(0.325f - directionOffset, -0.05f);
        RightKick.ReAnchorOffset = new Vector2(-0.325f - directionOffset, -0.05f);
        LeftKickAnchor.SetLocalEulerZ(0);
        RightKickAnchor.SetLocalEulerZ(0);
        LeftKick.Parent = RightKick.Parent = Player;
        LeftKick.Animate();
        RightKick.Animate();
    }
    private float bounceCount = 0.7f;
    public GameObject[] Shoes => new GameObject[] { LeftKick.gameObject, RightKick.gameObject };
    private Vector2[] velocities;
    protected override void DeathAnimation()
    {
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, 0, 0.03f));
        int c = Shoes.Length;
        for (int i = 0; i < c; ++i)
        {
            Transform t = Shoes[i].transform;
            float z = t.localPosition.z;
            float toBody = t.localPosition.y - p.Body.transform.localPosition.y;
            if (p.DeathKillTimer <= 0)
            {
                velocities[i] *= 0.0f;
                velocities[i].y += 0.05f * Utils.RandFloat(1f, 2f);
                velocities[i].x += Utils.RandFloat(-0.04f, 0.04f);
            }
            if (toBody < -0.2f)
            {
                velocities[i] *= -bounceCount;
                velocities[i] += Utils.RandCircle(0.05f) * Mathf.Abs(bounceCount);
                t.localPosition = (Vector2)t.localPosition + new Vector2(0, -0.2f - toBody);
                bounceCount *= 0.95f;
            }
            else
            {
                velocities[i].x *= 0.998f;
                velocities[i].y -= 0.005f;
            }
            t.localPosition = (Vector2)t.localPosition + velocities[i];
            t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, z);
        }
    }
}
