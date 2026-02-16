using UnityEngine;

public class EnemyDuck : Enemy
{
    public const int baseAimingTimer = 240;
    public SpriteRenderer sRender;
    private int aiState = 0;
    private Vector2 targetedLocation;
    private int aimingTimer = baseAimingTimer;
    private int movingTimer;
    const int baseMovingTimer = 300;
    protected float bobbingTimer = 0;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxCoin = 8;
        data.BaseMaxLife = 10;
        data.Cost = 1;
    }
    public void MoveUpdate()
    {
        if (aiState == 0)
        {
            if (aimingTimer <= 0)
            {
                aimingTimer = baseAimingTimer;
                targetedLocation = FindLocation();
                aiState = 1;
            }
            else
            {
                aimingTimer--;
            }
        }

        float inertia = 0.12f;
        RB.velocity *= 1 - inertia;
        if (aiState == 1)
        {
            Vector2 toTarget = targetedLocation - (Vector2)transform.position;
            bobbingTimer += Mathf.Sqrt(toTarget.magnitude);
            sRender.flipX = toTarget.x > 0;
            if (this is EnemyFlamingo)
                sRender.flipX = !sRender.flipX;
            float speed = Mathf.Min(1.0f + 0.1f * toTarget.magnitude, toTarget.magnitude);
            RB.velocity += toTarget.normalized * speed * inertia;// * Time.fixedDeltaTime;
            if (movingTimer <= 0)
            {
                movingTimer = baseMovingTimer;
                aiState = 0;
            }
            else
                movingTimer--;
        }
        bobbingTimer++;
        float bobSpeed = 80f;
        if (this is EnemyFlamingo)
        {
            bobSpeed = 100f;
        }
        float sin = Mathf.Sin(bobbingTimer * Mathf.PI / bobSpeed);
        Visual.transform.eulerAngles = new Vector3(0, 0, sin * 15);
    }
    public override void AI()
    {
        int soundChance = Random.Range(1, 500);
        if (soundChance == 1)
        {
            AudioManager.PlaySound(SoundID.DuckNoise, transform.position, 0.13f, 1.2f);
        }
        MoveUpdate();
    }
    protected virtual Vector2 FindLocation() {
        return (Vector2)transform.position.Lerp(Target.Position, Utils.RandFloat(0.25f, 0.75f)) + Utils.RandCircleEdge(Utils.RandFloat(2.5f, 10f));
    }
    public override void OnKill()
    {
        DeathParticles(20, 0.5f, new Color(1, .97f, .52f));
        AudioManager.PlaySound(SoundID.DuckDeath, transform.position, 0.25f, 1.2f);
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Duck");
    }
    public override void UIAI()
    {
        if (this is not EnemyFlamingo)
            sRender.flipX = true;
    }
}
