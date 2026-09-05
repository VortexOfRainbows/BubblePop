using UnityEngine;
public class Crow : Enemy
{
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.02f;
        inlineColor.r *= 2f;
        additiveColorPower = 0.4f;
    }
    public JumpMotion JumpAnimation;
    public virtual float MoveSpeed => 0.15f;
    public virtual float InertiaMult => 0.9f;
    protected float JumpTimer = 0;
    protected float IdleTimer = 100;
    protected float initialShootDelay = 100;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 10;
        data.BaseMaxCoin = 4;
        data.BaseMinCoin = 2;
        data.Cost = 2;
        data.WaveNumber = 3;
        data.Rarity = 2;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        offset.y += 0.2f;
        scale *= 1.2f;
    }
    public override void OnSpawn()
    {
        JumpTimer = 0;
    }
    public void UpdateDirection(float i)
    {
        i = Utils.SignNoZero(i);
        Visual.transform.localScale = new Vector3(i * Mathf.Abs(Visual.transform.localScale.x), 1 * Mathf.Abs(Visual.transform.localScale.y), 1);
    }
    public override void AI()
    {
        Vector2 toTarget = GetPathfindingToPlayerNorm();
        float dist = Vector2.Distance(Target.Position, transform.position);
        if(dist < 11 && HasLineOfSightWithTarget)
            toTarget = -toTarget;
        if(((dist < 11 || dist > 20 || JumpTimer != 0) && IdleTimer == 100) || !HasLineOfSightWithTarget)
        {
            JumpTimer++;
            if (JumpTimer >= 40)
            {
                JumpTimer = -70;
                RB.velocity *= 0.5f;
                RB.velocity += toTarget * (MoveSpeed * 45);
                //float tilt = Mathf.Sqrt(Mathf.Abs(RB.velocity.x)) * Visual.transform.localScale.x * -1.5f;
                //tilt += RB.velocity.y * 2.0f * Visual.transform.localScale.x;
                //Visual.transform.localEulerAngles = Vector3.forward * Mathf.LerpAngle(Visual.transform.localEulerAngles.z, tilt, 0.05f);
            }
            else
            {
                if (JumpTimer >= 0)
                {
                    JumpAnimation.JumpPercent = JumpTimer / 40f;
                    RB.velocity *= InertiaMult;
                    if(dist > 20)
                        RB.velocity += toTarget * (MoveSpeed * JumpAnimation.JumpPercent);
                }
                else
                {
                    JumpAnimation.JumpPercent = JumpTimer / 70f;
                }
            }
            if (Mathf.Abs(RB.velocity.x) > 0.1f)
                UpdateDirection(RB.velocity.x);
            IdleTimer = 100;
        }
        else if(--initialShootDelay <= 0 && (HasLineOfSightWithTarget || IdleTimer != 100))
        {
            JumpTimer = 0;
            JumpAnimation.JumpPercent = 0;
            RB.velocity *= InertiaMult;
            if (dist > 12.5f)
            {
                RB.velocity += 0.5f * MoveSpeed * toTarget.normalized;
            }
            if(dist >= 11)
            {
                if (Mathf.Abs(toTarget.x) > 0.1f)
                    UpdateDirection(toTarget.x);
            }
            IdleTimer++;
            float sin = -Mathf.Sin((1 - Mathf.Sqrt(IdleTimer / 100f)) * Mathf.PI);
            JumpAnimation.BodyAnchor.localPosition = JumpAnimation.BodyAnchor.localPosition.Lerp( new Vector3(-0.15f * sin, -0.1f + sin * 0.1f, 0), 0.2f);
            JumpAnimation.BodyAnchor.LerpLocalEulerZ(20 * sin, 0.2f);
            JumpAnimation.ArmAnchors[0].LerpLocalEulerZ(-40 * sin, 0.2f);
            JumpAnimation.ArmAnchors[1].LerpLocalEulerZ(-30 * sin, 0.2f);
            if (IdleTimer >= 200)
            {
                IdleTimer = 0;
                Vector2 norm = (Target.Position - (Vector2)transform.position).normalized;
                AudioManager.PlaySound(SoundID.FlamingoShot, transform.position, 0.8f, 0.9f);
                AudioManager.PlaySound(SoundID.LenardLaser, transform.position, 0.5f, 1.2f, 0);
                for (int i = -1; i <= 1; ++i)
                {
                    Vector2 v = norm.RotatedBy(i * Mathf.Deg2Rad * 35f);
                    Projectile.NewProjectile<Bullet>((Vector2)transform.position + v * 1.5f, v * 6, 1, this);
                }
            }
        }
        else
        {
            RB.velocity *= InertiaMult;
        }
    }
    public override void OnKill()
    {
        DeathParticles(20, 0.5f, new Color(0.2f, 0.2f, 0.2f));
        AudioManager.PlaySound(SoundID.FlamingoNoise, transform.position, 0.25f, 1.2f);
    }
}