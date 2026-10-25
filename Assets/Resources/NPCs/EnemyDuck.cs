using UnityEngine;

public class EnemyDuck : Enemy
{
    public override bool Amphibious => true;
    public const int baseAimingTimer = 240;
    public SpriteRenderer sRender;
    public int aiState = 0;
    public Vector2 targetedLocation;
    public int aimingTimer = baseAimingTimer;
    public int movingTimer;
    public const int baseMovingTimer = 300;
    protected float bobbingTimer = 0;
    public override float Inertia => 0.88f;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxCoin = 3;
        data.BaseMaxLife = 10;
        data.Cost = 1;
    }
    public void MoveUpdate()
    {
        float inertia = 1 - Inertia;
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
        if (aiState == 2)
            inertia *= 0.2f;
        RB.velocity *= 1 - inertia;
        if (aiState == 1 || aiState == 2)
        {
            Vector2 toTarget = targetedLocation - (Vector2)transform.position;
            Vector2 movementDir = Life >= MaxLife ? toTarget.normalized : GetPathfindingToPlayerNorm();
            bobbingTimer += Mathf.Sqrt(toTarget.magnitude);
            if(Mathf.Abs(movementDir.x) > 0.2f)
                sRender.flipX = movementDir.x > 0;
            float speedScaling = aiState == 2 ? 0.2f : 0.1f;
            float BaseSpeed = aiState == 2 ? 4.4f : 1.0f;
            float speed = Mathf.Min(BaseSpeed + speedScaling * toTarget.magnitude, 16);
            RB.velocity += inertia * speed * movementDir;// * Time.fixedDeltaTime;
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
        WaterUpdate();
    }
    public bool InWater = false;
    public float WaterTransitionAI = 0;
    public void WaterUpdate()
    {
        Vector2 waterDetectPos = MyShadow.transform.position; //Since the water shadow is moved relative to the visual, while the default shadow is not moved, the default is more accurate for this purpose
        waterDetectPos += 5 * Time.fixedDeltaTime * RB.velocity;
        float y = MyAmphibiousShadowRenderer.transform.localScale.y;
        float x = MyAmphibiousShadowRenderer.transform.localScale.x;
        //Check tiles in a thin vertical band of 0.5, for transitions to look better
        if(World.GetTile(World.RealPosToTilePos(waterDetectPos + new Vector2(0, 0.5f))).IsLiquid && World.GetTile(World.RealPosToTilePos(waterDetectPos)).IsLiquid)
        {
            WaterTransitionAI += Time.fixedDeltaTime * 2;
            if (WaterTransitionAI > 1)
            {
                WaterTransitionAI = 1;
                if(!InWater)
                {
                    InWater = true;
                    AudioManager.PlaySound(SoundID.BathBombSplash, waterDetectPos, 0.5f, 2.0f, 0);
                    float totalParticles = 12;
                    for(int i = 0; i < totalParticles; i++)
                    {
                        Vector2 circular = new Vector2(Utils.RandFloat(0.4f, 0.5f), 0).RotatedBy(i * Utils.TwoPI / totalParticles + Utils.RandFloat(-0.4f, 0.4f) / totalParticles);
                        circular.y *= y;
                        circular.x *= x;
                        ParticleManager.NewParticle(waterDetectPos + circular, Utils.RandFloat(0.3f, 0.4f), Vector2.up * 2 + circular * 4, 1, Utils.RandFloat(0.3f, 0.4f), ParticleManager.ID.Bubble, ColorHelper.WaterOutline.WithAlpha(0.95f));
                    }
                }
                else if(Utils.RandBool(5))
                {
                    Vector2 circular = new Vector2(Utils.RandFloat(0.4f, 0.5f), 0).RotatedBy(Utils.RandFloat(Utils.TwoPI));
                    circular.y *= y;
                    circular.x *= x;
                    ParticleManager.NewParticle(waterDetectPos + circular, Utils.RandFloat(0.3f, 0.4f), Vector2.up * 2 + circular * 4 - Time.fixedDeltaTime * Utils.RandFloat(8, 12) * RB.velocity, 1, Utils.RandFloat(0.3f, 0.4f), ParticleManager.ID.Bubble, ColorHelper.WaterOutline.WithAlpha(0.95f));
                }
            }
            else
                RB.velocity *= 0.915f;
        }
        else
        {
            WaterTransitionAI -= Time.fixedDeltaTime * 2;
            if (WaterTransitionAI < 0)
            {
                WaterTransitionAI = 0;
                if (InWater)
                {
                    InWater = false;
                    AudioManager.PlaySound(SoundID.ChestDrop, waterDetectPos, 0.5f, 2.0f, 0);
                }
            }
            else
                RB.velocity *= 0.915f;
        }
        MyAmphibiousShadowRenderer.transform.localPosition = new Vector3(0, -0.5f);
        Visual.transform.localPosition = new Vector2(0,  Mathf.Lerp(0, -0.5f, WaterTransitionAI) + Mathf.Sin(Mathf.PI * WaterTransitionAI));
        ToggleShadowWater();
    }
    public void ToggleShadowWater()
    {
        MyAmphibiousShadowRenderer.color = ColorHelper.WaterOutline.WithAlpha(WaterTransitionAI);
        MyShadowRenderer.color = MyShadowDefaultColor.WithAlphaMultiplied(1 - WaterTransitionAI);
    }
    protected virtual Vector2 FindLocation() {
        return (Vector2)transform.position.Lerp(Target.Position, Utils.RandFloat(0.25f, 0.75f)) + Utils.RandCircleEdge(Utils.RandFloat(2.5f, 10f));
    }
    public override void OnKill()
    {
        DeathParticles(20, 0.5f, new Color(1, .97f, .52f));
        AudioManager.PlaySound(SoundID.DuckDeath, transform.position, 0.25f, 1.2f);
    }
    public override void UIAI()
    {
        sRender.flipX = true;
    }
}
