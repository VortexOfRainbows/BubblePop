using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peaclock : Crow
{
    public Transform TailGear;
    public float TailRotationSpeed { get; set; } = 1.0f;
    public float TailDefaultRotationSpeed { get; set; } = 5.0f;
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.02f;
        inlineColor.r *= 2f;
        additiveColorPower = 0.4f;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        offset.y += 0.2f;
        scale *= 1.2f;
    }
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 10;
        data.BaseMaxCoin = 5;
        data.BaseMinCoin = 2;
        data.Cost = 2;
        data.WaveNumber = 100; //temp
        data.Rarity = 2;
    }
    public override void AI()
    {
        targetedLocation = Target.Position;
        Vector2 toTarget = targetedLocation - (Vector2)transform.position;
        float dist = toTarget.magnitude;
        if ((dist > 20 || timer != 0) && timer2 == 100)
        {
            timer++;
            if (timer >= 40)
            {
                timer = -70;
                RB.velocity *= 0.5f;
                RB.velocity += 45 * MoveSpeed * toTarget.normalized;
                //float tilt = Mathf.Sqrt(Mathf.Abs(RB.velocity.x)) * Visual.transform.localScale.x * -1.5f;
                //tilt += RB.velocity.y * 2.0f * Visual.transform.localScale.x;
                //Visual.transform.localEulerAngles = Vector3.forward * Mathf.LerpAngle(Visual.transform.localEulerAngles.z, tilt, 0.05f);
            }
            else
            {
                if (timer >= 0)
                {
                    JumpAnimation.JumpPercent = timer / 40f;
                    RB.velocity *= InertiaMult;
                    if (dist > 20)
                        RB.velocity += JumpAnimation.JumpPercent * MoveSpeed * toTarget.normalized;
                }
                else
                {
                    JumpAnimation.JumpPercent = timer / 70f;
                }
            }
            if (Mathf.Abs(RB.velocity.x) > 0.1f)
                UpdateDirection(RB.velocity.x);
            timer2 = 100;
        }
        else if (--initialShootDelay <= 0)
        {
            timer = 0;
            JumpAnimation.JumpPercent = 0;
            RB.velocity *= InertiaMult;
            timer2++;
            //if (dist > 12.5f)
            //    RB.velocity += toTarget.normalized * MoveSpeed * 0.5f;
            //float sin = -Mathf.Sin((1 - Mathf.Sqrt(timer2 / 100f)) * Mathf.PI);
            //JumpAnimation.BodyAnchor.localPosition = JumpAnimation.BodyAnchor.localPosition.Lerp(new Vector3(-0.15f * sin, -0.1f + sin * 0.1f, 0), 0.2f);
            //JumpAnimation.BodyAnchor.LerpLocalEulerZ(20 * sin, 0.2f);
            //JumpAnimation.ArmAnchors[0].LerpLocalEulerZ(-40 * sin, 0.2f);
            //JumpAnimation.ArmAnchors[1].LerpLocalEulerZ(-30 * sin, 0.2f);
            if (timer2 >= 200)
            {
                timer2 = 0;
                //Vector2 norm = (targetedLocation - (Vector2)transform.position).normalized;
                //AudioManager.PlaySound(SoundID.FlamingoShot, transform.position, 0.8f, 0.9f);
                //AudioManager.PlaySound(SoundID.LenardLaser, transform.position, 0.5f, 1.2f, 0);
                //for (int i = -1; i <= 1; ++i)
                //{
                //    Vector2 v = norm.RotatedBy(i * Mathf.Deg2Rad * 35f);
                //    Projectile.NewProjectile<Bullet>((Vector2)transform.position + v * 1.5f, v * 6, 1, this);
                //}
            }
        }
        else
        {
            RB.velocity *= InertiaMult;
        }
    }
    public override void OnKill()
    {
        DeathParticles(20, 0.5f, new Color(0.588f, 0.424f, 0.216f));
        AudioManager.PlaySound(SoundID.FlamingoNoise, transform.position, 0.25f, 1.2f);
    }
}
