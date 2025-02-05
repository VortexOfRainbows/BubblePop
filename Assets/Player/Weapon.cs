using UnityEngine;

public class Weapon : Equipment
{
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
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0.4f, -0.6f), 0.1f);
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.transform.eulerAngles.z, Mathf.Sign(p.lastVelo.x) == 1 ? 0 : 180, 0.1f));
    }
}
