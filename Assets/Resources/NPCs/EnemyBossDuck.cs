using UnityEngine;

public class EnemyBossDuck : EnemyDuck
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Leonard");
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        //scale *= 2;
        //offset = Vector2.one * -1.25f;
    }
    public override float HealthBarOffset => -3f;
    private float projectileSpeed = 3f;
    private int projectileTimer = -50;
    public override float CostMultiplier => 10;
    public override float SkullPowerDropChance => 1f;
    public override float PowerDropChance => 0.2f;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 45;
        data.BaseMaxCoin = 50;
    }
    public override void AI()
    {
        PlayMusic = true;
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
            //GameObject.Instantiate(EnemyID.OldDuck, transform.position, Quaternion.identity);
            projectileTimer = -200;
        }
        else
        {
            if(projectileTimer % 33 == 0 && projectileTimer >= 0)
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
    public override void OnKill()
    {
        DeathParticles(40, 0.7f, new Color(0.1f, 0.1f, 0.1f));
        DeathParticles(80, 0.9f, new Color(1, .97f, .52f));
        AudioManager.PlaySound(SoundID.DuckDeath, transform.position, 0.3f, 0.5f);
    }
    protected override Vector2 FindLocation()
    {
        return (Vector2)transform.position.Lerp(Player.Position, Utils.RandFloat(0.1f, 0.5f)) + Utils.RandCircleEdge(Utils.RandFloat(5f, 15f));
    }
    public bool PlayMusic = false;
    new public void Update()
    {
        base.Update();
        if(PlayMusic)
            AudioManager.SetMusic(AudioManager.LeonardTheme, 1);
    }
}
