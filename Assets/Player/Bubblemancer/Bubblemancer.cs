using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubblemancer : Body
{
    protected override UnlockCondition CategoryUnlockCondition => UnlockCondition.Get<BubblemancerUnlock>();
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
    }
    protected override string Description()
    {
        return "A humble shepard from the quaint Bubble Fields";
    }
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {
        if (Control.Ability && !Control.LastAbility && moveSpeed.magnitude > 0 && p.abilityTimer <= 0)
        {
            Dash(ref playerVelo, moveSpeed);
        }
    }
    public void Dash(ref Vector2 velocity, Vector2 moveSpeed)
    {
        float speed = Player.DashDefault;
        p.abilityTimer = p.abilityCD;
        velocity = velocity * p.MaxSpeed + moveSpeed * speed;
        p.squash = p.SquashAmt;
        transform.eulerAngles = new Vector3(0, 0, velocity.ToRotation() * Mathf.Rad2Deg);
        AudioManager.PlaySound(SoundID.Dash.GetVariation(3), transform.position, 1f, Utils.RandFloat(1.2f, 1.3f));
        p.OnDash(velocity);
    }
}
