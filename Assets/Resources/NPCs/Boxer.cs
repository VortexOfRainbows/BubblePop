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
    public SpriteRenderer FistL, FistR;
    public Vector2 FistLVelo, FistRVelo;
    public float Dir { get; set; } = 1;
    public float AI1 { get; set; }
    public bool UseLeftFist = true;
    public override void AI()
    {
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
        if(distToPlayer > 8 && AI1 <= 0)
        {
            RB.velocity += toTarget * MoveSpeed;
        }
        else //Attack range
        {
            Vector2 fistToPlayer = Target.transform.position - currentFist.transform.position;
            Vector2 norm = fistToPlayer.normalized;
            AI1++;
            if (AI1 > 100)
            {
                if(AI1 < 105)
                {
                    //throw the punch
                    currentVelo += norm * 8;
                }
                else
                {
                    //switch the punching fist to the other one
                    AI1 = -20;
                    UseLeftFist = !UseLeftFist;
                }
            }
            else if(AI1 >= 0)//wind up the punch
            {
                float percent = AI1 / 100f;
                float windup = Mathf.Sin(percent * Mathf.PI * 1.5f);
                currentVelo += -windup * norm;
            }
        }
        if(true) //Return punches to the default position after switching fists
        {
            FistL.transform.LerpLocalPosition(new Vector2(-0.5095f, -0.1346f), 0.1f);
            FistR.transform.LerpLocalPosition(new Vector2(0.947f, -0.834f), 0.1f);
        }
        FistLVelo *= Inertia;
        FistRVelo *= Inertia;
        RB.velocity *= Inertia;
        FistL.transform.localPosition += (Vector3)(FistLVelo * Time.fixedDeltaTime);
        FistR.transform.localPosition += (Vector3)(FistRVelo * Time.fixedDeltaTime);
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
