using UnityEngine;

public class Weapon : Equipment
{
    public override void ModifyUIOffsets(ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(-0.7f, -0.7f);
        rotation = 45f;
    }
    public float AttackLeft = 0;
    public float AttackRight = 0;
    public Vector3 WandEulerAngles = new Vector3(0, 0, 0);
    public virtual void StartAttack(bool alternate)
    {

    }
    protected override void AnimationUpdate()
    {

    }
    protected sealed override void DeathAnimation()
    {
        AttackLeft = 0;
        AttackRight = 0;
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, -0.6f), 0.1f);
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.transform.eulerAngles.z, Mathf.Sign(p.lastVelo.x) == 1 ? 0 : 180, 0.1f));
    }
    public virtual bool IsAttacking()
    {
        return IsSecondaryAttacking() || IsPrimaryAttacking();
    }
    public virtual bool IsSecondaryAttacking()
    {
        return AttackRight > 0;
    }
    public virtual bool IsPrimaryAttacking()
    {
        return AttackLeft > 0;
    }
}
