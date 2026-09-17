using UnityEngine;

public class Boxer : Enemy
{
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        base.ModifyInfectionShaderProperties(ref outlineColor, ref inlineColor, ref inlineThreshold, ref outlineSize, ref additiveColorPower);
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        base.ModifyUIOffsets(ref offset, ref scale);
    }
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 12;
        data.BaseMaxCoin = 4;
        data.BaseMinCoin = 2;
        data.Cost = 2;
        data.WaveNumber = 4;
        data.Rarity = 2;
    }
    public override float MoveSpeed => 0.3f;
    public override float Inertia => 0.95f;
    public float Timer = 0;
    public SpriteRenderer Blade1, Blade2, Blade3, Blade4;
    public float Dir { get; set; } = 1;
    public override void AI()
    {
        Timer += Time.fixedDeltaTime;
        float distToPlayer = Target.Distance(transform.gameObject);
        Vector2 toTarget = GetPathfindingToPlayerNorm();
        if (Mathf.Abs(RB.velocity.x) > 0.2f)
            Dir = Utils.SignNoZero(toTarget.x);
        if(distToPlayer > 8)
        {
            RB.velocity += toTarget * MoveSpeed;
        }
        RB.velocity *= Inertia;
        Visual.transform.localPosition = new Vector3(0, 1 + 0.1f * Mathf.Sin(Timer * Mathf.PI), 0);
        Visual.transform.localScale = new Vector3(-Dir * Mathf.Abs(Visual.transform.localScale.x), Visual.transform.localScale.y, 1);
        Visual.transform.LerpLocalEulerZ(Mathf.Clamp(RB.velocity.x * -3, -25, 25), 0.1f);
        BladeUpdate(Blade1, 0);
        BladeUpdate(Blade2, 1);
        BladeUpdate(Blade3, 2);
        BladeUpdate(Blade4, 3);
    }
    public void BladeUpdate(SpriteRenderer blade, float offset)
    {
        float r = Timer * Mathf.PI * 2.5f + offset * Utils.HalfPI;
        Vector2 circleSimulator = new Vector2(1, 0).RotatedBy(r);
        circleSimulator.y *= 0.35f;
        blade.transform.SetLocalEulerZ(circleSimulator.ToRotation() * Mathf.Rad2Deg);
        blade.transform.localScale = new Vector3(circleSimulator.magnitude * 1.1f, 1);
        blade.sortingOrder = (int)(-30 - 20 * circleSimulator.y);
    }
    public override void UIAI()
    {
        base.UIAI();
    }
    public override void OnKill()
    {
        DeathParticles(20, 0.5f, new Color(0.655f, 0.4745f, 0.2431373f));
        AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 0.4f, 2.5f);
    }
    //TODO:
    //1. Enemy flying offset and shadow handling (this is the part that makes it more unique from other enemies from an implementation point of view
    //2. Enemy fist behavior (maybe just attach a hitbox?)
}
