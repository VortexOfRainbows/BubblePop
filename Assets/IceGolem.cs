using UnityEngine;

public class IceGolem : Ent
{
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.03f;
        additiveColorPower += 0.9f;
    }
    public override float MoveSpeed => 0.1f;
    public override float InertiaMultiplier => 0.94f;
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
    public override void OnSpawn()
    {
        base.OnSpawn();
    }
}
