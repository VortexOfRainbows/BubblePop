using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : Enemy
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Crow");
    }
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.02f;
        inlineColor.r *= 2f;
        additiveColorPower = 0.4f;
    }
    public JumpMotion JumpAnimation;
    private Vector2 targetedLocation;
    public float moveSpeed = 0.15f;
    public float inertiaMult = 0.9f;
    private float timer = 0;
    private float timer2 = 100;
    private float initialShootDelay = 100;
    public override float CostMultiplier => 2f;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 10;
        data.BaseMaxCoin = 15;
        data.CardBG = Resources.Load<Sprite>("NPCs/Card/CrowB");
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        offset.y += 0.2f;
        scale *= 1.2f;
    }
    public override void OnSpawn()
    {
        timer = 0;
    }
    public void UpdateDirection(float i)
    {
        if (i >= 0)
            i = 1;
        else
            i = -1;
        Visual.transform.localScale = new Vector3(i, 1, 1);
    }
    public void MoveUpdate()
    {
        targetedLocation = Player.Position;
        Vector2 toTarget = targetedLocation - (Vector2)transform.position;
        float dist = toTarget.magnitude;
        if(dist < 11)
        {
            toTarget = -toTarget;
        }
        if((dist < 11 || dist > 20 || timer != 0) && timer2 == 100)
        {
            timer++;
            if (timer >= 40)
            {
                timer = -70;
                RB.velocity *= 0.5f;
                RB.velocity += toTarget.normalized * moveSpeed * 45;
                //float tilt = Mathf.Sqrt(Mathf.Abs(RB.velocity.x)) * Visual.transform.localScale.x * -1.5f;
                //tilt += RB.velocity.y * 2.0f * Visual.transform.localScale.x;
                //Visual.transform.localEulerAngles = Vector3.forward * Mathf.LerpAngle(Visual.transform.localEulerAngles.z, tilt, 0.05f);
            }
            else
            {
                if (timer >= 0)
                {
                    JumpAnimation.JumpPercent = timer / 40f;
                    RB.velocity *= inertiaMult;
                    if(dist > 20)
                        RB.velocity += toTarget.normalized * moveSpeed * JumpAnimation.JumpPercent;
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
        else if(--initialShootDelay <= 0)
        {
            timer = 0;
            JumpAnimation.JumpPercent = 0;
            RB.velocity *= inertiaMult;
            if (dist > 12.5f)
            {
                RB.velocity += toTarget.normalized * moveSpeed * 0.5f;
            }
            if(dist >= 11)
            {
                if (Mathf.Abs(toTarget.x) > 0.1f)
                    UpdateDirection(toTarget.x);
            }
            timer2++;
            float sin = -Mathf.Sin((1 - Mathf.Sqrt(timer2 / 100f)) * Mathf.PI);
            JumpAnimation.BodyAnchor.localPosition = JumpAnimation.BodyAnchor.localPosition.Lerp( new Vector3(-0.15f * sin, -0.1f + sin * 0.1f, 0), 0.2f);
            JumpAnimation.BodyAnchor.LerpLocalEulerZ(20 * sin, 0.2f);
            JumpAnimation.ArmAnchors[0].LerpLocalEulerZ(-40 * sin, 0.2f);
            JumpAnimation.ArmAnchors[1].LerpLocalEulerZ(-30 * sin, 0.2f);
            if (timer2 >= 200)
            {
                timer2 = 0;
                Vector2 norm = (targetedLocation - (Vector2)transform.position).normalized;
                AudioManager.PlaySound(SoundID.FlamingoShot, transform.position, 0.8f, 0.9f);
                AudioManager.PlaySound(SoundID.LenardLaser, transform.position, 0.5f, 1.2f, 0);
                for (int i = -1; i <= 1; ++i)
                {
                    Vector2 v = norm.RotatedBy(i * Mathf.Deg2Rad * 35f);
                    Projectile.NewProjectile<Bullet>((Vector2)transform.position + v * 1.5f, v * 6);
                }
            }
        }
        else
        {
            RB.velocity *= inertiaMult;
        }
    }
    public override void AI()
    {
        MoveUpdate();
    }
    public override void OnKill()
    {
        DeathParticles(20, 0.5f, new Color(0.2f, 0.2f, 0.2f));
        AudioManager.PlaySound(SoundID.FlamingoNoise, transform.position, 0.25f, 1.2f);
    }
}
