using UnityEngine;

public class EnemyFlamingo : EnemyDuck
{
    private static readonly float ProjVelocity = 3f;
    private int projectileTimer = 300;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 12;
        data.BaseMaxCoin = 5;
        data.BaseMinCoin = 2;
        data.Cost = 3;
        data.WaveNumber = 3;
        data.Rarity = 3;
    }
    public override void AI()
    {
        if (Utils.RandBool(500))
            AudioManager.PlaySound(SoundID.FlamingoNoise, transform.position, 0.13f, 1.2f);
        MoveUpdate();
        if (projectileTimer <= 0)
        {
            ShootProjectile();
            projectileTimer = Random.Range(200, 300);
        }
        else
        {
            projectileTimer--;
        }
    }
    private void ShootProjectile() {
        Vector2 projectileDirection = (Target.Position - (Vector2)transform.position).normalized * ProjVelocity;
        Projectile.NewProjectile<FlamingoFeather>((Vector2)transform.position + projectileDirection * 0.5f, projectileDirection.RotatedBy(Mathf.Deg2Rad * Utils.RandFloat(-15, 15)), 1, this);
        AudioManager.PlaySound(SoundID.FlamingoShot.GetVariation(0), transform.position, 0.05f, 1.2f);
    }
    protected override Vector2 FindLocation()
    {
        return (Vector2)transform.position.Lerp(Target.Position, Utils.RandFloat(0.3f, 0.5f)) + Utils.RandCircleEdge(Utils.RandFloat(4f, 8f));
    }
    public override void OnKill()
    {
        DeathParticles(30, 0.6f, new Color(1, 0.85f, 0.99f));
        AudioManager.PlaySound(SoundID.FlamingoShot.GetVariation(1), transform.position, 0.9f, 1.2f);
    }
}
