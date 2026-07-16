using System.Collections.Generic;
using UnityEngine;

public class Bubblemancer : Body
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubblemancerUnlock>();
    public override void Init()
    {
        Player.abilityTimer = 0;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<BubbleTrail>();
        powerPool.Add<Coalescence>();
    }
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Ability);
    }
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {
        if (Player.Control.Ability && !Player.Control.LastAbility && moveSpeed.magnitude > 0 && Player.AbilityReady)
        {
            Dash(ref playerVelo, moveSpeed);
        }
    }
    public void Dash(ref Vector2 velocity, Vector2 moveSpeed)
    {
        float speed = Player.DashDefault;
        Player.abilityTimer = Player.AbilityCD;
        velocity = velocity * Player.MaxSpeed + moveSpeed * speed;
        p.Squash = Player.SquashAmt;
        Vector2 velocityForRotationPurposes = velocity;
        velocityForRotationPurposes.x = Mathf.Abs(velocityForRotationPurposes.x);
        spriteRender.transform.eulerAngles = new Vector3(0, 0, velocityForRotationPurposes.ToRotation() * Mathf.Rad2Deg * FlipDir);
        FaceR.transform.eulerAngles = new Vector3(0, 0, p.Direction < 0 ? (Mathf.PI + velocity.ToRotation()) * Mathf.Rad2Deg : velocity.ToRotation() * Mathf.Rad2Deg);
        AudioManager.PlaySound(SoundID.Dash.GetVariation(3), transform.position, 1f, Utils.RandFloat(1.2f, 1.3f));
        Player.OnDash(velocity);
    }
    public override void FaceUpdate()
    {
        Vector2 toMouse = p.LookPosition - (Vector2)transform.position;
        Vector2 pos = new Vector2(0.175f * p.Direction, 0) + toMouse.normalized * 0.175f;
        pos.y *= 0.7f;
        pos *= p.Squash;

        if (Player.RB.velocity.magnitude < Player.MaxSpeed + 11.5f * Player.MoveSpeedMod)
        {
            float r = Mathf.LerpAngle(FaceR.transform.eulerAngles.z, 0, 0.0425f);
            FaceR.transform.eulerAngles = new Vector3(0, 0, r);
        }
        else
        {
            float r = Player.RB.velocity.ToRotation();
            FaceR.transform.eulerAngles = new Vector3(0, 0, p.Direction < 0 ? (Mathf.PI + r) * Mathf.Rad2Deg : r * Mathf.Rad2Deg);
            pos *= 0.5f;
            pos += new Vector2(0.0f, -0.3f * p.Direction).RotatedBy(r);
        }

        Face.transform.LerpLocalPosition(pos, 0.065f);
        FaceR.flipX = !Flipped;
    }
}
