using System.Collections.Generic;
using UnityEngine;

public class Bubblemancer : Body
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubblemancerUnlock>();
    public override void Init()
    {
        Player.abilityTimer = 0;
        PrimaryColor = new Color(0.8f, 0.85f, 0.9f, 0.45f);
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<BubbleTrail>();
        powerPool.Add<Coalescence>();
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Bubblemancer").WithDescription("A humble shepard from the quaint Bubble Fields");
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
        p.squash = Player.SquashAmt;
        spriteRender.transform.eulerAngles = new Vector3(0, 0, velocity.ToRotation() * Mathf.Rad2Deg);
        FaceR.transform.eulerAngles = new Vector3(0, 0, p.Direction < 0 ? (Mathf.PI + velocity.ToRotation()) * Mathf.Rad2Deg : velocity.ToRotation() * Mathf.Rad2Deg);
        AudioManager.PlaySound(SoundID.Dash.GetVariation(3), transform.position, 1f, Utils.RandFloat(1.2f, 1.3f));
        Player.OnDash(velocity);
    }
    public override void FaceUpdate()
    {
        Vector2 toMouse = p.LookPosition - (Vector2)transform.position;
        Vector2 pos = new Vector2(0.175f * p.Direction, 0) + toMouse.normalized * 0.175f;
        pos.y *= 0.7f;
        pos *= p.squash;

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

        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.065f);
        FaceR.flipX = !spriteRender.flipY;
    }
}
