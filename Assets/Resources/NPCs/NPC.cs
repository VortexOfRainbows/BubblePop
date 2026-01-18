using UnityEngine;

public class NPC : Entity
{
    public PlayerAnimator p;
    public override void OnFixedUpdate()
    {
        p.Body.p = p.Hat.p = p.Accessory.p = p;
        p.Body.AliveUpdate();
        p.Hat.AliveUpdate();
        p.Accessory.AliveUpdate();
        if (p.LookPosition.x < transform.position.x)
            p.lastVelo.x = -1;
        else
            p.lastVelo.x = 1;
        p.PostUpdate();
    }
}
