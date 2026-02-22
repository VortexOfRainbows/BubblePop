using System.Collections.Generic;
using UnityEngine;

public class Fizzy : Body
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyUnlock>();
    public override void Init()
    {
        Player.abilityTimer = 0;
        PrimaryColor = new Color(0.8f, 0.7f, 0.56f, 0.45f);
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add(PowerUp.Get<WeaponUpgrade>());
        powerPool.Add(PowerUp.Get<WeaponUpgrade>());
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Fizzy").WithDescription("He's a pretty cool bubble");
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
        Vector2 toMouse2 = toMouse.normalized;
        toMouse2.x += Mathf.Sign(toMouse2.x) * 4;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.17f, 0).RotatedBy(toMouseR);
        looking.y *= 0.8f;
        if (looking.x < 0)
            toMouseR += Mathf.PI;
        Vector2 pos = new Vector2(0.17f * p.Direction, 0.05f) + looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouseR * Mathf.Rad2Deg);
        Face.transform.localScale = new Vector3(toMouse.x > 0 ? 1 : -1, 1, 1);
    }
}
