using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peaclock : Crow
{
    public Transform TailGear;
    public Transform[] Feathers;
    public float TailRotationSpeed { get; set; } = 1.0f;
    public float TailDefaultRotationSpeed { get; set; } = 5.0f;
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.02f;
        inlineColor.r *= 2f;
        additiveColorPower = 0.1f;
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
        //if you do not have line of sight with the player, this behavior should be changed
        Vector2 toTarget = targetedLocation - (Vector2)transform.position;
        float dist = toTarget.magnitude;
        toTarget = toTarget.normalized;
        float tailRotateSpeed = TailDefaultRotationSpeed;
        if ((dist > 12 || JumpTimer != 0) && AttackTimer == 100)
        {
            JumpTimer++;
            if (JumpTimer >= 40) //Airborn
            {
                JumpTimer = -70;
                RB.velocity *= 0.5f;
                RB.velocity += 45 * MoveSpeed * toTarget;
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
                    if (dist > 20)
                        RB.velocity += JumpAnimation.JumpPercent * MoveSpeed * toTarget;
                    tailRotateSpeed *= 0.1f;
                }
                else //Airborn
                {
                    tailRotateSpeed *= 3;
                    JumpAnimation.JumpPercent = JumpTimer / 70f;
                }
            }
            if (Mathf.Abs(RB.velocity.x) > 0.1f)
                UpdateDirection(RB.velocity.x);
            AttackTimer = 100;
        }
        else if (--initialShootDelay <= 0)
        {
            JumpTimer = 0;
            JumpAnimation.JumpPercent = 0;
            RB.velocity *= InertiaMult;
            AttackTimer++;
            if(Mathf.Sign(toTarget.x) != Mathf.Sign(Visual.transform.localScale.x))
            {
                RB.velocity += 0.1f * MoveSpeed * toTarget;
                UpdateDirection(RB.velocity.x);
            }
            float percent = AttackTimer / 200f;
            float sin = Mathf.Sin(percent * Mathf.PI * 4);
            float sin2 = Mathf.Sin(percent * Mathf.PI * 2);
            Vector2 bob = new(sin2 * 0.07f, sin * 0.07f);
            JumpAnimation.BodyAnchor.LerpLocalPosition(bob, 0.2f);
            JumpAnimation.BodyAnchor.LerpLocalEulerZ(2 * sin2, 0.2f);
            JumpAnimation.ArmAnchors[0].LerpLocalEulerZ(-10 * sin2, 0.2f);
            JumpAnimation.ArmAnchors[1].LerpLocalEulerZ(-10 * sin2, 0.2f);
            JumpAnimation.LegAnchors[0].GetChild(0).localScale = new Vector2(1, 1.05f + sin * 0.05f);
            JumpAnimation.LegAnchors[1].GetChild(0).localScale = new Vector2(1, 1.05f + sin * 0.05f);
            JumpAnimation.LegAnchors[0].GetChild(0).localPosition = new Vector2(0, sin * 0.05f);
            JumpAnimation.LegAnchors[1].GetChild(0).localPosition = new Vector2(0, sin * 0.05f);
            if (AttackTimer >= 200)
            {
                AttackTimer = 0;
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
        TailRotationSpeed = Mathf.Lerp(TailRotationSpeed, tailRotateSpeed, 0.1f);
        float degrees = TailGear.localEulerAngles.z - TailRotationSpeed * Time.fixedDeltaTime * 10;
        TailGear.SetLocalEulerZ(degrees);
        for (int i = 0; i < Feathers.Length; i++)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad * 2 + i * Mathf.PI / 3f);
            Feathers[i].localScale = new Vector3(1, 1 + sin * 0.1f, 1);
        }
    }
    public override void OnKill()
    {
        DeathParticles(20, 0.5f, new Color(0.588f, 0.424f, 0.216f));
        AudioManager.PlaySound(SoundID.FlamingoNoise, transform.position, 0.25f, 1.2f);
    }
}
