using UnityEngine;

public class IceGolem : Ent
{
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.1f;
        outlineSize *= 1.5f;
    }
    public override float MoveSpeed => 0.11f;
    public override float InertiaMultiplier => 0.95f;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 30;
        data.BaseMaxCoin = 12;
        data.BaseMinCoin = 3;
        data.BaseMaxGem = 2;
        data.Cost = 4f;
        data.WaveNumber = 5;
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
            AICounter++;
            if (AICounter > 250)
            {
                float percent = (AICounter - 250) / 60f;
                float iPer = 1 - percent;
                RightArmAnchor.LerpLocalEulerZ(60 * iPer, percent);
                LeftArmAnchor.LerpLocalEulerZ(90 * iPer, percent);
                if (AICounter > 310)
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
            }
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
}
