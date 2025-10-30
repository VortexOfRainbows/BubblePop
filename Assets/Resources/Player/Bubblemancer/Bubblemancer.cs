using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubblemancer : Body
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubblemancerUnlock>();
    public override void Init()
    {
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
        if (Control.Ability && !Control.LastAbility && moveSpeed.magnitude > 0 && player.AbilityReady)
        {
            Dash(ref playerVelo, moveSpeed);
        }
    }
    public void Dash(ref Vector2 velocity, Vector2 moveSpeed)
    {
        float speed = Player.DashDefault;
        player.abilityTimer = player.AbilityCD;
        velocity = velocity * player.MaxSpeed + moveSpeed * speed;
        p.squash = player.SquashAmt;
        spriteRender.transform.eulerAngles = new Vector3(0, 0, velocity.ToRotation() * Mathf.Rad2Deg);
        FaceR.transform.eulerAngles = new Vector3(0, 0, p.Direction < 0 ? (Mathf.PI + velocity.ToRotation()) * Mathf.Rad2Deg : velocity.ToRotation() * Mathf.Rad2Deg);
        AudioManager.PlaySound(SoundID.Dash.GetVariation(3), transform.position, 1f, Utils.RandFloat(1.2f, 1.3f));
        player.OnDash(velocity);
    }
    public override void FaceUpdate()
    {
        Vector2 toMouse = p.LookPosition - (Vector2)transform.position;
        Vector2 pos = new Vector2(0.175f * p.Direction, 0) + toMouse.normalized * 0.175f;
        pos.y *= 0.7f;
        pos *= p.squash;

        if (player.rb.velocity.magnitude < player.MaxSpeed + 11.5f * player.MoveSpeedMod)
        {
            float r = Mathf.LerpAngle(FaceR.transform.eulerAngles.z, 0, 0.0425f);
            FaceR.transform.eulerAngles = new Vector3(0, 0, r);
        }
        else
        {
            float r = player.rb.velocity.ToRotation();
            FaceR.transform.eulerAngles = new Vector3(0, 0, p.Direction < 0 ? (Mathf.PI + r) * Mathf.Rad2Deg : r * Mathf.Rad2Deg);
            pos *= 0.5f;
            pos += new Vector2(0.0f, -0.3f * p.Direction).RotatedBy(r);
        }

        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.065f);
        FaceR.flipX = !spriteRender.flipY;
    }
}
