using UnityEngine;

public class RockGolem : RockSpider
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("David");
    }
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.04f;
        additiveColorPower += 0.2f;
    }
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 150;
        data.BaseMaxCoin = 50;
        data.BaseMinCoin = 5;
        data.BaseMaxGem = 3;
        data.Cost = 8f;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        scale *= 1.4f;
    }
    public override void OnSpawn()
    {
        //do not do base.OnSpawn()
    }
    public override void UpdateDirection(float i)
    {
        if (i >= 0)
            i = -1;
        else
            i = 1;
        Visual.transform.localScale = new Vector3(i * Mathf.Abs(Visual.transform.localScale.x), Visual.transform.localScale.y, 1);
    }
}
