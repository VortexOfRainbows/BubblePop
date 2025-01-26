using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossDuck : EnemyDuck
{
    private float projectileSpeed = 3f;
    private int projectileTimer;
    private void Start()
    {
        Life = 100;
        PointWorth = 50;
    }
    new public void FixedUpdate()
    {
        IFrame--;
        int soundChance = Random.Range(1, 400);
        if (soundChance == 1)
        {
            AudioManager.PlaySound(GlobalDefinitions.audioClips[Random.Range(28, 30)], transform.position, 0.3f, 0.8f);
        }
        MoveUpdate();
        projectileTimer++;
        if (projectileTimer >= 500)
        {
            AudioManager.PlaySound(GlobalDefinitions.audioClips[Random.Range(28, 30)], transform.position, 0.6f, 0.3f);
            ShootProjectile(12);
            GameObject.Instantiate(GlobalDefinitions.Ducky, transform.position, Quaternion.identity);
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
            Projectile.NewProjectile(this.transform.position, circular * 2.5f, 6);
        }
        AudioManager.PlaySound(GlobalDefinitions.audioClips[38], transform.position, 0.3f, 1.5f);
    }
    private Vector2 FindLocation()
    {
        return (Vector2)transform.position + new Vector2(Random.Range(-50f, 50f), Random.Range(-50f, 50f));
    }
    public override void OnKill()
    {
        DeathParticles(40, 0.7f, new Color(0.1f, 0.1f, 0.1f));
        DeathParticles(80, 0.9f, new Color(1, .97f, .52f));
        AudioManager.PlaySound(GlobalDefinitions.audioClips[27], transform.position, 0.3f, 0.5f);
    }
}
