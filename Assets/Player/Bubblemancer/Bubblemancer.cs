using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubblemancer : Body
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<BubblemancerUnlock>();
    public override void Init()
    {
        PrimaryColor = ParticleManager.DefaultColor;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Choice>();
        powerPool.Add<BubbleBirb>();
        powerPool.Add<BubbleTrail>();
        powerPool.Add<Overclock>();
        powerPool.Add<BubbleMitosis>();
        powerPool.Add<Coalescence>();
    }
    protected override string Description()
    {
        return "A humble shepard from the quaint Bubble Fields.";
    }
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {
        if (Control.Ability && !Control.LastAbility && moveSpeed.magnitude > 0 && player.AbilityReady)
        {
            Dash(ref playerVelo, moveSpeed);
        }
        if (playerVelo.magnitude <= player.MaxSpeed + 12f)
        {
            float r = Mathf.LerpAngle(FaceR.transform.eulerAngles.z, 0, 0.04f);
            FaceR.transform.eulerAngles = new Vector3(0, 0, r);
        }
    }
    public void Dash(ref Vector2 velocity, Vector2 moveSpeed)
    {
        float speed = Player.DashDefault;
        player.abilityTimer = player.AbilityCD;
        velocity = velocity * player.MaxSpeed + moveSpeed * speed;
        p.squash = player.SquashAmt;
        spriteRender.transform.eulerAngles = new Vector3(0, 0, velocity.ToRotation() * Mathf.Rad2Deg);
        FaceR.transform.eulerAngles = new Vector3(0, 0, p.Direction < 0 ? (Mathf.PI +velocity.ToRotation()) * Mathf.Rad2Deg : velocity.ToRotation() * Mathf.Rad2Deg);
        AudioManager.PlaySound(SoundID.Dash.GetVariation(3), transform.position, 1f, Utils.RandFloat(1.2f, 1.3f));
        player.OnDash(velocity);
    }
    public override void FaceUpdate()
    {
        Vector2 toMouse = p.LookPosition - (Vector2)transform.position;
        Vector2 pos = new Vector2(0.2f * p.Direction, 0) + toMouse.normalized * 0.2f;
        pos.y *= 0.7f;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.05f);
        FaceR.flipX = spriteRender.flipY;
    }
}
