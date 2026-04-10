using UnityEngine;

public class SuperSentinel : Sentinel
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Super Sentinel");
    }
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.03f;
        additiveColorPower += 0.9f;
    }
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 25;
        data.BaseMaxCoin = 35;
        data.BaseMinCoin = 5;
        data.BaseMinGem = 2;
        data.BaseMaxGem = 5;
        data.Rarity = 4;
        data.Cost = 8.0f;
        data.WaveNumber = 12;
    }
    public override void OnSpawn()
    {
        UsePurpleColors = true;
        base.OnSpawn();
    }
}
