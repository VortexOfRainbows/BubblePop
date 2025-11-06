using UnityEngine;

public class EnemyDuck : Enemy
{
    public SpriteRenderer sRender;
    //aiState 0 = choosing location
    //aiState 1 = going to location
    private int aiState = 0;
    private Vector2 targetedLocation;
    private int aimingTimer;
    const int baseAimingTimer = 240;
    private int movingTimer;
    const int baseMovingTimer = 300;
    private float moveSpeed = 0.1f;
    protected float bobbingTimer = 0;
    public override float CostMultiplier => 1;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxCoin = 8;
        data.BaseMaxLife = 10;
    }
    // Update is called once per frame
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

        if (aiState == 1)
        {
            Vector2 toTarget = targetedLocation - (Vector2)transform.position;
            bobbingTimer += Mathf.Sqrt(toTarget.magnitude);
            sRender.flipX = toTarget.x > 0;
            if (this is EnemyFlamingo)
                sRender.flipX = !sRender.flipX;
            transform.position = Vector2.Lerp(transform.position, targetedLocation, moveSpeed * Time.deltaTime);
            if (movingTimer <= 0)
            {
                movingTimer = baseMovingTimer;
                aiState = 0;
            }
            else
            {
                movingTimer--;
            }
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
        return (Vector2)transform.position + new Vector2(Random.Range(-50f, 50f), Random.Range(-50f, 50f));
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
}
