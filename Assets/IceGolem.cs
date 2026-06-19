using UnityEngine;

public class IceGolem : Ent
{
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.1f;
        outlineSize *= 1.5f;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        offset.y += 0.05f;
        scale *= 1.3f;
    }
    public override float MoveSpeed => 0.125f;
    public override float InertiaMultiplier => 0.965f;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 24;
        data.BaseMaxCoin = 12;
        data.BaseMinCoin = 3;
        data.BaseMaxGem = 3;
        data.Cost = 5f;
        data.WaveNumber = 7;
        data.Rarity = 3;
    }
    public float AICounter = 0;
    public Transform RightArmAnchor;
    public Transform LeftArmAnchor;
    public Transform Head;
    public override void AI()
    {
        if (AICounter > 100)
        {
            Vector2 toTarget = Target.Position - (Vector2)transform.position;
            RB.velocity *= InertiaMultiplier;
            UpdateDirection(Utils.SignNoZero(toTarget.x));
            if (AICounter > 250)
            {
                float percent = (AICounter - 250) / 70f;
                float iPer = 1 - percent;
                RightArmAnchor.LerpLocalEulerZ(60 * iPer, percent);
                LeftArmAnchor.LerpLocalEulerZ(90 * iPer, percent);
                if (AICounter > 320)
                    AICounter = -Utils.RandInt(200, 301);
                Head.LerpLocalPosition(new Vector2(0, 0.1f + 0.2f * iPer), 0.1f);
            }
            else //charge snowball
            {
                float percent = (AICounter - 100) / 150f;
                float iPer = 1 - percent;
                RightArmAnchor.LerpLocalEulerZ(-35 * iPer, 0.1f);
                LeftArmAnchor.LerpLocalEulerZ(35 * iPer, 0.1f);
                Head.LerpLocalPosition(new Vector2(0, percent * 0.3f), 0.1f);
                if(AICounter == 101)
                {
                    AudioManager.PlaySound(SoundID.GolemCharge, transform.position, 1, 0.9f * (1 + ChampionSpeedBonus), 0);
                    Projectile.NewProjectile<Snowball>(transform.position, Vector2.zero, 1, this, 150 / (1 + ChampionSpeedBonus));
                }
                else if(Utils.RandFloat() < 0.2f)
                {
                    Vector3 circle = Utils.RandCircleEdge() * (0.5f + percent);
                    ParticleManager.NewParticle(transform.position + new Vector3(Utils.SignNoZero(Visual.transform.localScale.x) * 0.1f, -0.5f)
                        + circle,
                        0.3f * percent, -circle * 4, 0.5f, 0.5f, ParticleManager.ID.Snow, Color.white);
                }
            }
            AICounter++;
        }
        else
        {
            RightArmAnchor.LerpLocalEulerZ(0, 0.1f);
            LeftArmAnchor.LerpLocalEulerZ(0, 0.1f);
            Head.LerpLocalPosition(Vector2.zero, 0.1f);
            float distanceToPlayer = Vector2.Distance(Target.Position, transform.position);
            if (distanceToPlayer < 18) //distance before starting to buffer ranged attack
                AICounter++;
            else if (AICounter > 0) //When far away stop charging aim buffer
                AICounter--;
            base.AI();
        }
    }
    public override void OnSpawn()
    {
        base.OnSpawn();
    }
    public override void OnKill()
    {
        AudioManager.PlaySound(SoundID.BathBombBurst, transform.position, 0.5f, 0.9f);
        for (int i = 0; i < 3; ++i)
        {
            Projectile.NewProjectile<Snowball>(transform.position, new Vector2(0, 3).RotatedBy(i / 3f * Utils.TwoPI), 1, null, 1, 0.2f + 0.2f * i);
        }
    }
}
