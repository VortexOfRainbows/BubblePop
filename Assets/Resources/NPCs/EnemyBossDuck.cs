using UnityEngine;

public class EnemyBossDuck : EnemyDuck
{
    private float projectileSpeed = 3f;
    private int projectileTimer;
    public override float CostMultiplier => 10;
    public override float PowerDropChance => 1f;
    public override void Init()
    {
        Life = 80;
        MaxCoins = 50;
    }
    public override void AI()
    {
        int soundChance = Random.Range(1, 400);
        if (soundChance == 1)
        {
            AudioManager.PlaySound(SoundID.LenardNoise, transform.position, 0.3f, 0.9f);
        }
        MoveUpdate();
        projectileTimer++;
        if (projectileTimer >= 500)
        {
            AudioManager.PlaySound(SoundID.LenardNoise, transform.position, 0.6f, 0.9f);
            ShootProjectile(8);
            GameObject.Instantiate(EnemyID.OldDuck, transform.position, Quaternion.identity);
            projectileTimer = -200;
        }
        else
        {
            if(projectileTimer % 30 == 0 && projectileTimer >= 0)
            {
                int c = 1;
                ShootProjectile(c);
            }
        }
    }
    private void ShootProjectile(int c = 8)
    {
        Vector2 projectileDirection = (Player.Position - (Vector2)this.transform.position).normalized * projectileSpeed;
        for(int i = 0; i < c; i++)
        {
            Vector2 circular = projectileDirection.RotatedBy(i / (float)c * 2 * Mathf.PI);
            Projectile.NewProjectile<Laser>(this.transform.position, circular * 2.5f);
        }
        AudioManager.PlaySound(SoundID.LenardLaser.GetVariation(0), transform.position, 0.3f, 1.5f);
    }
    protected override Vector2 FindLocation()
    {
        return (Vector2)transform.position + new Vector2(Random.Range(-50f, 50f), Random.Range(-50f, 50f)) + (Player.Position - (Vector2)transform.position) * 0.1f;
    }
    public override void OnKill()
    {
        DeathParticles(40, 0.7f, new Color(0.1f, 0.1f, 0.1f));
        DeathParticles(80, 0.9f, new Color(1, .97f, .52f));
        AudioManager.PlaySound(SoundID.DuckDeath, transform.position, 0.3f, 0.5f);
    }
}
