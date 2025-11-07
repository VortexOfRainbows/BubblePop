using System.Threading;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class RockSpider : Enemy
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Rock Spider");
    }
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.04f;
        additiveColorPower += 0.2f;
    }
    private Vector2 targetedLocation;
    public float moveSpeed = 0.12f;
    public float inertiaMult = 0.96f;
    public override float CostMultiplier => 1;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 8;
        data.BaseMaxCoin = 5;
        data.Cost = 1f;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        scale *= 1.1f;
    }
    public void UpdateDirection(float i)
    {
        if (i >= 0)
            i = -1;
        else
            i = 1;
        Visual.transform.localScale = new Vector3(i * 1.2f, 1.2f, 1);
    }
    protected float Timer = 150;
    public Transform Eye;
    public float ShotRecoil = 1;
    public void UpdateEye()
    {
        Vector2 toPlayer = Player.Position - (Vector2)transform.position;
        Vector2 norm = toPlayer.normalized;
        norm.x *= Utils.SignNoZero(Visual.transform.localScale.x);
        float f = 1;
        Vector3 e = Eye.transform.localEulerAngles;
        Eye.transform.localPosition = Eye.transform.localPosition.Lerp(0.06f * f * ShotRecoil * norm, 0.11f);
        ShotRecoil = Mathf.Lerp(ShotRecoil, 1, 0.08f);
    }
    public void MoveUpdate()
    {
        Vector2 toPlayer = Player.Position - (Vector2)transform.position;
        Vector2 norm = toPlayer.normalized;
        UpdateEye();
        Timer++;
        if (Timer > 160)
        {
            if(Utils.RandFloat(1) < 0.4f)
            {
                AudioManager.PlaySound(SoundID.ElectricZap, Eye.transform.position, 0.7f, 1.7f, 0);
                int c = 1;
                for (int i = 0; i < c; i++)
                {
                    float otherMult = (i + 0.5f - c / 2f);
                    float j = otherMult * 25f;
                    Vector2 spread = norm.RotatedBy(j * Mathf.Deg2Rad);
                    Projectile.NewProjectile<Bullet>((Vector2)Eye.transform.position + spread * 0.5f, spread * 5.5f, 1, 1.15f);
                }
                ShotRecoil = -1.7f;
            }
            Timer = 0;
            targetedLocation = FindTargetedPlayerPosition();
        }
        if (Timer > 50 && Timer < 120)
        {
            if (targetedLocation == Vector2.zero)
                targetedLocation = FindTargetedPlayerPosition();
            toPlayer = targetedLocation - (Vector2)transform.position;
            if (Timer == 51)
            {
                RB.velocity *= 0.4f;
                RB.velocity += toPlayer.normalized * 5f;
            }
            norm = RB.velocity.normalized;
            RB.velocity += toPlayer.normalized * Mathf.Min(0.225f, toPlayer.magnitude);
            if (Mathf.Abs(RB.velocity.x) > 0.1f)
                UpdateDirection(RB.velocity.x);
            RB.velocity *= 0.9875f;
        }
        if (Timer > 120)
            RB.velocity *= 0.8f;
    }
    private Vector2 FindTargetedPlayerPosition()
    {
        Vector2 toPlayer = Player.Position - RB.position;
        float magnitude = Mathf.Min(5, toPlayer.magnitude);
        toPlayer = toPlayer.normalized;
        return RB.position + toPlayer * magnitude + new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
    }
    public override void AI()
    {
        MoveUpdate();
    }
    public override void OnKill()
    {
        DeathParticles(20, 0.5f, new Color(60 / 255f, 70 / 255f, 92 / 255f));
        AudioManager.PlaySound(SoundID.DuckDeath, transform.position, 0.2f, 0.7f);
    }
}
