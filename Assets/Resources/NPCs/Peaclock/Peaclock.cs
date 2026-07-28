using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.EditorTools;
using UnityEngine;

public class Peaclock : Crow
{
    public static readonly Vector2 TailAnchorDefaultPosition = new(-0.45f, 0.575f);
    public static readonly Vector2 TailAnchorCentralPosition = new(0f, 0.35f);
    public Transform TailCog;
    public Transform[] Feathers;
    public Transform[] Cogs;
    public float TailRotationSpeed { get; set; } = 1.0f;
    public float TailDefaultRotationSpeed { get; set; } = 5.0f;
    public float CogOutCounter { get; set; } = 0;
    public float TimeToCogOut => 50;
    public float AttackTimer { get; set; } = 0;
    public float BonusSpeed { get; set; } = 0;
    protected float InnateRotationDirection = 0;
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
    public new void UpdateDirection(float i)
    {
        Transform flipBody = Visual.transform;
        i = Utils.SignNoZero(i);
        bool thisIsAFlip = i != Utils.SignNoZero(flipBody.localScale.x);
        flipBody.localScale = new Vector3(i * Mathf.Abs(flipBody.localScale.x), 1 * Mathf.Abs(flipBody.localScale.y), 1);
        if (thisIsAFlip)
        {
            TailCog.parent.localPosition = new Vector3(-TailCog.parent.localPosition.x, TailCog.parent.localPosition.y);
            for (int a = 0; a < Cogs.Length; a++)
            {
                Cogs[a].localPosition = new Vector3(-Cogs[a].localPosition.x, Cogs[a].localPosition.y);
            }
        }

    }
    public override void AI()
    {
        if(InnateRotationDirection == 0)
            InnateRotationDirection = Utils.RandBool(2) ? 1 : -1;
        targetedLocation = Target.Position;
        //if you do not have line of sight with the player, this behavior should be changed
        Vector2 toTarget = targetedLocation - (Vector2)transform.position;
        float dist = toTarget.magnitude;
        toTarget = toTarget.normalized;
        float tailRotateSpeed = TailDefaultRotationSpeed;
        bool requestAttack = false;
        if ((dist > 10 || JumpTimer != 0) && CogOutCounter <= 0)
        {
            JumpTimer++;
            if (JumpTimer >= 40) //Airborn
            {
                JumpTimer = -70;
                RB.velocity *= 0.5f;
                RB.velocity += 45 * MoveSpeed * toTarget;
                
                float tilt = Mathf.Sqrt(Mathf.Abs(RB.velocity.x)) * Visual.transform.localScale.x * -1.5f;
                tilt += RB.velocity.y * 2.0f * Visual.transform.localScale.x;
                Visual.transform.localEulerAngles = Vector3.forward * Mathf.LerpAngle(Visual.transform.localEulerAngles.z, tilt, 0.05f);

                AudioManager.PlaySound(SoundID.ChestDrop, transform.position, 0.5f, 1.4f);
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
            CogOutCounter = -50;
        }
        else
        {
            JumpTimer = 0;
            JumpAnimation.JumpPercent = 0;
            RB.velocity *= InertiaMult;
            IdleTimer++;
            if(Mathf.Sign(toTarget.x) != Mathf.Sign(Visual.transform.localScale.x))
            {
                RB.velocity += 0.1f * MoveSpeed * toTarget;
                UpdateDirection(RB.velocity.x);
            }
            float percent = IdleTimer / 200f;
            float sin = Mathf.Sin(percent * Mathf.PI * 4);
            float sin2 = Mathf.Sin(percent * Mathf.PI * 2);
            Vector2 bob = new(sin2 * 0.07f, sin * 0.07f);
            JumpAnimation.BodyAnchor.LerpLocalPosition(bob, 0.2f);
            JumpAnimation.BodyAnchor.LerpLocalEulerZ(2 * sin2, 0.2f);
            if(CogOutCounter <= 0)
            {
                JumpAnimation.ArmAnchors[0].LerpLocalEulerZ(-10 * sin2, 0.04f);
                JumpAnimation.ArmAnchors[1].LerpLocalEulerZ(-10 * sin2, 0.04f);
            }
            JumpAnimation.LegAnchors[0].GetChild(0).LerpLocalScale(new Vector2(1, 1.05f + sin * 0.05f), 0.2f);
            JumpAnimation.LegAnchors[1].GetChild(0).LerpLocalScale(new Vector2(1, 1.05f + sin * 0.05f), 0.2f);
            JumpAnimation.LegAnchors[0].GetChild(0).LerpLocalPosition(new Vector2(0, sin * 0.05f), 0.2f);
            JumpAnimation.LegAnchors[1].GetChild(0).LerpLocalPosition(new Vector2(0, sin * 0.05f), 0.2f);
            if (IdleTimer >= 200)
                IdleTimer = 0;
            if (CogOutCounter < TimeToCogOut && dist < 20)
            {
                if((int)CogOutCounter == 10)
                {
                    AudioManager.PlaySound(SoundID.Infect, transform.position, 0.5f, 2.4f);
                    AudioManager.PlaySound(SoundID.Teleport, transform.position, 0.6f, 1.4f);
                }
                CogOutCounter++;
            }
            else if (CogOutCounter > 0 && dist >= 20)
            {
                if (CogOutCounter >= TimeToCogOut)
                    CogOutCounter = TimeToCogOut;
                if ((int)CogOutCounter == 40)
                {
                    AudioManager.PlaySound(SoundID.Infect, transform.position, 0.5f, 3f);
                    AudioManager.PlaySound(SoundID.Teleport, transform.position, 0.6f, 1.8f);
                }
                CogOutCounter--;
            }
            if (CogOutCounter < TimeToCogOut && dist < 20)
            {
                percent = CogOutCounter / TimeToCogOut;
                sin = Mathf.Sin(percent * Mathf.PI / 2f);
                JumpAnimation.ArmAnchors[0].LerpLocalEulerZ(-40 * sin * percent, 0.04f);
                JumpAnimation.ArmAnchors[1].LerpLocalEulerZ(40 * sin * percent, 0.04f);
            }
            else if (CogOutCounter >= TimeToCogOut) //Attack timer will go in here
            {
                CogOutCounter++;
                percent = (CogOutCounter - TimeToCogOut) / TimeToCogOut;
                sin = Mathf.Sin(percent * Mathf.PI * 2);
                JumpAnimation.ArmAnchors[0].LerpLocalEulerZ(-40 + 20 * sin2 + sin * 10, 0.04f);
                JumpAnimation.ArmAnchors[1].LerpLocalEulerZ(40 + 20 * sin2 + sin * 10, 0.04f);
                if (CogOutCounter >= TimeToCogOut * 2)
                    CogOutCounter -= TimeToCogOut;

                AttackTimer += (TailDefaultRotationSpeed + BonusSpeed) * 0.2f;
                if (AttackTimer >= 120)
                {
                    AttackTimer -= 120;
                    requestAttack = true;
                    ++BonusSpeed;
                }
            }
        }
        if (CogOutCounter < TimeToCogOut)
        {
            AttackTimer = 40;
            BonusSpeed *= 0.95f;
        }
        if (BonusSpeed > 10)
            BonusSpeed = 10;
        tailRotateSpeed += BonusSpeed;
        TailRotationSpeed = Mathf.Lerp(TailRotationSpeed, tailRotateSpeed, 0.1f);
        float degrees = TailCog.localEulerAngles.z * InnateRotationDirection - TailRotationSpeed * Time.fixedDeltaTime * 10;
        TailCog.SetLocalEulerZ(degrees * InnateRotationDirection);
        for (int i = 0; i < Feathers.Length; i++)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad * 2 + i * Mathf.PI / 3f);
            Feathers[i].localScale = new Vector3(1, 1 + sin * 0.1f, 1);
        }
        float cPercent = Mathf.Clamp01(CogOutCounter / TimeToCogOut);
        cPercent *= cPercent;
        float dir = Utils.SignNoZero(Visual.transform.localScale.x);
        TailCog.parent.LerpLocalPosition(Vector2.Lerp(TailAnchorDefaultPosition, TailAnchorCentralPosition, cPercent), 0.1f);
        float bulletVelocity = (5 + BonusSpeed * 0.34f) * 1.75f;
        Color c = IsInfected ? new Color(1, 0.1f, 0.1f, 1f) : new Color(.8f, 0.7f, 0.1f, 1);
        for (int i = 0; i < Cogs.Length; i++)
        {
            Vector2 circ = new Vector2(1.0f * cPercent, 0).RotatedBy(Mathf.Deg2Rad * degrees * -InnateRotationDirection + i * Mathf.PI * 0.5f);
            if (requestAttack)
            {
                Projectile.NewProjectile<Bullet>((Vector2)Cogs[i].transform.position + circ * 0.1f, circ * bulletVelocity, 1, this, 1, c.r, c.g, c.b, c.a, TailRotationSpeed * 0.2f * InnateRotationDirection);
            }
            circ.x *= dir;
            Cogs[i].LerpLocalPosition(circ * 1.75f, 0.1f);
            Cogs[i].SetLocalEulerZ(-4 * degrees * InnateRotationDirection);
        }
        if (requestAttack)
            AudioManager.PlaySound(SoundID.GolemShoot, transform.position, 1f, 1.4f + BonusSpeed * 0.05f);
    }
    public override void OnKill()
    {
        DeathParticles(30, 0.5f, new Color(0.588f, 0.424f, 0.216f));
        for (int i = 0; i < 20; ++i)
            ParticleManager.NewParticle(transform.position, Utils.RandFloat(0.3f, 0.6f), Utils.RandCircle(4), 2, Utils.RandFloat(0.4f, 0.8f), ParticleManager.ID.Trail, Color.yellow);
        AudioManager.PlaySound(SoundID.FlamingoNoise, transform.position, 0.25f, 0.9f);
        AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 0.25f, 1.2f);
    }
}
