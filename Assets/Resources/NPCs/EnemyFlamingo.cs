using UnityEngine;

public class EnemyFlamingo : EnemyDuck
{
    private float projectileSpeed = 3f;
    private int projectileTimer = 240;
    public override void Init()
    {
        Life = 10;
        MaxCoins = 15;
    }
    public override float CostMultiplier => 3;
    public override void AI()
    {
        int soundChance = Random.Range(1, 500);
        if (soundChance == 1)
        {
            AudioManager.PlaySound(SoundID.FlamingoNoise, transform.position, 0.13f, 1.2f);
        }
        MoveUpdate();
        if (projectileTimer <= 0)
        {
            ShootProjectile();
            projectileTimer = Random.Range(60, 300);
        }
        else
        {
            projectileTimer--;
        }
    }
    private void ShootProjectile() {
        Vector2 projectileDirection = (Player.Position - (Vector2)this.transform.position).normalized * projectileSpeed;
        Projectile.NewProjectile<FlamingoFeather>(this.transform.position, projectileDirection.RotatedBy(Mathf.Deg2Rad * Utils.RandFloat(-15, 15)));
        AudioManager.PlaySound(SoundID.FlamingoShot.GetVariation(0), transform.position, 0.05f, 1.2f);
    }
    public override void OnKill()
    {
        DeathParticles(30, 0.6f, new Color(1, 0.85f, 0.99f));
        AudioManager.PlaySound(SoundID.FlamingoShot.GetVariation(1), transform.position, 0.7f, 1.2f);
    }
}
