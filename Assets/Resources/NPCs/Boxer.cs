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
    public Transform Chassis;
    public override float MoveSpeed => 0.3f;
    public override float Inertia => 0.95f;
    public float Timer = 0;
    public SpriteRenderer Blade1, Blade2, Blade3, Blade4;
    public SpriteRenderer FistL, FistR;
    public Vector2 FistLVelo, FistRVelo;
    public float Dir { get; set; } = 1;
    public float AI1 { get; set; }
    public float AI2 { get; set; }
    public bool UseLeftFist = true;
    public override void OnSpawn()
    {

    }
    public override void AI()
    {
        float punchRate = 50f;
        float punchRecovery = 20;
        float punchReturn = 60;
        SpriteRenderer currentFist = UseLeftFist ? FistL : FistR; 
        SpriteRenderer otherFist = UseLeftFist ? FistR : FistL; 
        ref Vector2 currentVelo = ref FistRVelo; // Default fallback
        ref Vector2 otherVelo = ref FistLVelo;
        if (UseLeftFist)
        {
            currentVelo = ref FistLVelo;
            otherVelo = ref FistRVelo;
        }
        Timer += Time.fixedDeltaTime;
        float distToPlayer = Target.Distance(transform.gameObject);
        Vector2 toTarget = GetPathfindingToPlayerNorm();
        if (Mathf.Abs(RB.velocity.x) > 0.2f)
            Dir = Utils.SignNoZero(toTarget.x);
        else if(AI1 != 0)
            Dir = Utils.SignNoZero(Target.transform.position.x - transform.position.x);
        otherFist.flipY = currentFist.flipY = Dir == 1;
        if (distToPlayer > 8 && AI1 != 0)
        {
            RB.velocity += toTarget * MoveSpeed;
            if(UseLeftFist || AI2 <= 0)
            {
                Vector2 fistToPlayerL = Target.transform.position - FistL.transform.position;
                fistToPlayerL.y *= 0.25f;
                FistL.transform.LerpLocalEulerZ((-fistToPlayerL).ToRotation() * Mathf.Rad2Deg, 0.05f);
                FistL.transform.LerpLocalPosition(new Vector2(0.45f * Dir, -0.135f), 0.05f);
            }
            if(!UseLeftFist || AI2 <= 0)
            {
                Vector2 fistToPlayerR = Target.transform.position - FistR.transform.position;
                fistToPlayerR.y *= 0.25f;
                FistR.transform.LerpLocalEulerZ((-fistToPlayerR).ToRotation() * Mathf.Rad2Deg, 0.05f);
                FistR.transform.LerpLocalPosition(new Vector2(-0.9f * Dir, -0.835f), 0.05f);
            }
        }
        else //Attack range
        {
            RB.velocity += 0.01f * MoveSpeed * toTarget;
            Vector2 fistToPlayer = Target.transform.position - currentFist.transform.position;
            Vector2 norm = fistToPlayer.normalized;
            AI1++;
            if (AI1 > punchRate)
            {
                if (AI1 < punchRate + 3)
                {
                    //throw the punch
                    float punchSpeed = 10f;
                    currentVelo += norm * punchSpeed;
                }
                else
                {
                    //switch the punching fist to the other one
                    AI1 = -punchRecovery;
                    AI2 = punchReturn;
                    UseLeftFist = !UseLeftFist;
                }
            }
            else if (AI1 >= 0)//wind up the punch
            {
                float percent = AI1 / punchRate;
                float windup = Mathf.Sin(percent * Mathf.PI * 1.45f);
                currentVelo += 1.5f * percent * -windup * norm;
            }
            currentFist.transform.LerpLocalEulerZ((-fistToPlayer).ToRotation() * Mathf.Rad2Deg, 0.1f);
        }
        Vector2 otherToPlayer = Target.transform.position - otherFist.transform.position;
        Vector2 returnPos = otherFist == FistL ? new Vector2(0.45f * Dir, -0.135f) : new Vector2(-0.9f * Dir, -0.835f);
        if(AI2 > 0)
        {
            if(AI2 > punchReturn / 2)
            {
                //hitbox on;
            }
            float fromCenter = otherFist.transform.localPosition.magnitude;
            --AI2;
            float returnPercent = 1 - AI2 / punchReturn;
            Vector2 toReturn = returnPos - (Vector2)otherFist.transform.localPosition;
            if(fromCenter < 8)
            {
                float percent2 = 1 - fromCenter / 8f;
                otherVelo += otherToPlayer * percent2 * 0.5f;
                otherFist.transform.LerpLocalEulerZ((-otherVelo).ToRotation() * Mathf.Rad2Deg, 0.1f);
            }
            otherVelo += 0.1f * returnPercent * toReturn;
            otherFist.transform.LerpLocalPosition(returnPos, returnPercent * 0.04f);
        }
        else
            otherFist.transform.LerpLocalPosition(returnPos, 0.05f);
        if (true) //Return punches to the default position after switching fists
        {
            if (currentFist == FistL)
                FistL.transform.LerpLocalPosition(new Vector2(0.45f * Dir, -0.135f), .1f);
            if (currentFist == FistR)
                FistR.transform.LerpLocalPosition(new Vector2(-0.9f * Dir, -0.835f), .1f);
            FistLVelo *= Inertia;
            FistRVelo *= Inertia;
        }
        if (otherVelo.sqrMagnitude < 1)
        {
            Vector2 fistToPlayer = otherToPlayer;
            otherFist.transform.LerpLocalEulerZ((-fistToPlayer).ToRotation() * Mathf.Rad2Deg, 0.1f);
        }
        RB.velocity *= Inertia;
        FistL.transform.localPosition += (Vector3)(FistLVelo * Time.fixedDeltaTime);
        FistR.transform.localPosition += (Vector3)(FistRVelo * Time.fixedDeltaTime);
        Visual.transform.localPosition = new Vector3(0, 1 + 0.1f * Mathf.Sin(Timer * Mathf.PI), 0);
        Chassis.transform.localScale = new Vector3(-Dir * Mathf.Abs(Chassis.transform.localScale.x), Chassis.transform.localScale.y, 1);
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
