using UnityEngine;

public class EnemyDuck : Enemy
{
    public const int baseAimingTimer = 240;
    public SpriteRenderer sRender;
    public int aiState = 0;
    public Vector2 targetedLocation;
    public int aimingTimer = baseAimingTimer;
    public int movingTimer;
    public const int baseMovingTimer = 300;
    protected float bobbingTimer = 0;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxCoin = 3;
        data.BaseMaxLife = 10;
        data.Cost = 1;
    }
    public void MoveUpdate()
    {
        float bobSpeed = 80f;
        if (this is EnemyFlamingo)
            bobSpeed = 100f;
        else if(this is not EnemyBossDuck)
        {
            if (Life < MaxLife)
            {
                if(aiState != 2)
                {
                    AudioManager.PlaySound(SoundID.DuckNoise, transform.position, 1.0f, 0.775f, 0);
                    aiState = 2;
                }
                targetedLocation = Player.FindClosest(transform.position, out Vector2 _, out float _).transform.position;
                if (Utils.RandFloat() < 0.25f)
                {
                    ParticleManager.NewParticle(transform.position + new Vector3(0, 0.5f, 0f), Utils.RandFloat(0.5f, 0.75f), Vector2.up * 2 + Utils.RandCircle(), 0.5f, Utils.RandFloat(0.4f, 0.5f), ParticleManager.ID.Bubble, Color.Lerp(Color.black, Color.red, Utils.RandFloat()).WithAlpha(0.7f));
                }
            }
        }
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
        if (aiState == 1 || aiState == 2)
        {
            Vector2 toTarget = targetedLocation - (Vector2)transform.position;
            bobbingTimer += Mathf.Sqrt(toTarget.magnitude);
            sRender.flipX = toTarget.x > 0;
            if (this is EnemyFlamingo)
                sRender.flipX = !sRender.flipX;
            float speedScaling = aiState == 2 ? 0.7f : 0.1f;
            float speed = Mathf.Min(0.9f + speedScaling + speedScaling * toTarget.magnitude, toTarget.magnitude);
            RB.velocity += inertia * speed * toTarget.normalized;// * Time.fixedDeltaTime;
            if(aiState != 2)
            {
                if (movingTimer <= 0)
                {
                    movingTimer = baseMovingTimer;
                    aiState = 0;
                }
                else
                    movingTimer--;
            }
        }
        bobbingTimer++;
        float sin = Mathf.Sin(bobbingTimer * Mathf.PI / bobSpeed);
        Visual.transform.eulerAngles = new Vector3(0, 0, sin * 15);
    }
    public override void AI()
    {
        if (Utils.RandBool(aiState == 2 ? 100 : 500))
            AudioManager.PlaySound(SoundID.DuckNoise, transform.position, 0.13f, 1.2f);
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
