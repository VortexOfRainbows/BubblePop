using UnityEngine;

public class Boxer : Enemy
{
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        outlineSize = 0.015f;
        inlineThreshold = 0.02f;
        additiveColorPower = 0.3f;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {

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
    public class BoxerFist
    {
        public BoxerFist(SpriteRenderer fist, Transform parent, Vector2 velo, Collider2D c2D, Vector2 restPos)
        {
            Fist = fist;
            transform = parent;
            Velo = velo;
            Collider = c2D;
            RestPosition = restPos;
        }
        public SpriteRenderer Fist;
        public Transform transform;
        public Vector2 Velo;
        public Collider2D Collider;
        public Vector2 RestPosition;
        public Vector2 GetRestPos(float dir) => new(RestPosition.x * dir, RestPosition.y);
        public void LerpToRestPosition(float dir, float lerpT)
        {
            transform.LerpLocalPosition(GetRestPos(dir), lerpT);
        }
        public void Update(float inertia)
        {
            transform.localPosition += (Vector3)(Velo * Time.fixedDeltaTime);
            Velo *= inertia;
        }
    }
    public BoxerFist Left, Right;
    public Transform Chassis;
    public override float MoveSpeed => 0.4f;
    public override float Inertia => 0.95f;
    public float Timer = 0;
    public SpriteRenderer Blade1, Blade2, Blade3, Blade4;
    public SpriteRenderer FistL, FistR;
    public Collider2D FistLC2D, FistRC2D;
    public float Dir { get; set; } = 1;
    public float AI1 { get; set; }
    public float AI2 { get; set; }
    public bool UseLeftFist = true;
    public override void OnSpawn()
    {
        Left = new(FistL, FistL.transform, Vector2.zero, FistLC2D, new Vector2(0.45f, -0.125f));
        Right = new(FistR, FistR.transform, Vector2.zero, FistRC2D, new Vector2(-0.9f, -0.75f));
    }
    public override void AI()
    {
        float punchRate = 50f;
        float punchRecovery = 20;
        float punchReturn = 60;
        BoxerFist currentFist = UseLeftFist ? Left : Right;
        BoxerFist otherFist = UseLeftFist ? Right : Left; 
        Timer += Time.fixedDeltaTime;
        float distToPlayer = Target.Distance(transform.gameObject);
        Vector2 toTarget = GetPathfindingToPlayerNorm();
        if (Mathf.Abs(RB.velocity.x) > 0.2f)
            Dir = Utils.SignNoZero(toTarget.x);
        else if(AI1 != 0)
            Dir = Utils.SignNoZero(Target.transform.position.x - transform.position.x);
        otherFist.Fist.flipY = currentFist.Fist.flipY = Dir == 1;
        if (distToPlayer > 8 && AI1 != 0)
        {
            RB.velocity += toTarget * MoveSpeed;
            if(UseLeftFist || AI2 <= 0)
            {
                Vector2 fistToPlayerL = Target.transform.position - FistL.transform.position;
                fistToPlayerL.y *= 0.25f;
                FistL.transform.LerpLocalEulerZ((-fistToPlayerL).ToRotation() * Mathf.Rad2Deg, 0.04f);
                Left.LerpToRestPosition(Dir, 0.04f);
            }
            if(!UseLeftFist || AI2 <= 0)
            {
                Vector2 fistToPlayerR = Target.transform.position - FistR.transform.position;
                fistToPlayerR.y *= 0.25f;
                FistR.transform.LerpLocalEulerZ((-fistToPlayerR).ToRotation() * Mathf.Rad2Deg, 0.04f);
                Right.LerpToRestPosition(Dir, 0.04f);
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
                    currentFist.Velo += norm * punchSpeed;
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
                currentFist.Velo += 1.75f * percent * -windup * norm;
            }
            currentFist.transform.LerpLocalEulerZ((-fistToPlayer).ToRotation() * Mathf.Rad2Deg, 0.1f);
        }
        Vector2 otherToPlayer = Target.transform.position - otherFist.transform.position;
        Vector2 returnPos = otherFist.GetRestPos(Dir);
        if(AI2 > 0)
        {
            otherFist.Collider.enabled = AI2 > punchReturn / 2;
            float fromCenter = otherFist.transform.localPosition.magnitude;
            --AI2;
            float returnPercent = 1 - AI2 / punchReturn;
            Vector2 toReturn = returnPos - (Vector2)otherFist.transform.localPosition;
            if(fromCenter < 8)
            {
                float percent2 = 1 - fromCenter / 8f;
                otherFist.Velo += 0.5f * percent2 * otherToPlayer;
                otherFist.transform.LerpLocalEulerZ((-otherFist.Velo).ToRotation() * Mathf.Rad2Deg, 0.1f);
            }
            otherFist.Velo += 0.1f * returnPercent * toReturn;
            otherFist.LerpToRestPosition(Dir, returnPercent * 0.05f);
        }
        else
            otherFist.LerpToRestPosition(Dir, 0.05f);
        //Return punches to the default position after switching fists
        if (currentFist == Left)
            Left.LerpToRestPosition(Dir, .1f);
        if (currentFist == Right)
            Right.LerpToRestPosition(Dir, .1f);
        if (otherFist.Velo.sqrMagnitude < 1)
        {
            Vector2 fistToPlayer = otherToPlayer;
            otherFist.transform.LerpLocalEulerZ((-fistToPlayer).ToRotation() * Mathf.Rad2Deg, 0.1f);
        }
        RB.velocity *= Inertia;
        Left.Update(Inertia);
        Right.Update(Inertia);
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
