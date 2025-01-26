using UnityEngine;

public class EnemyFlamingo : Entity
{
    //aiState 0 = choosing location
    //aiState 1 = going to location
    private int aiState = 0;
    private Vector2 targetedLocation;
    private int aimingTimer;
    const int baseAimingTimer = 400;
    private int movingTimer;
    const int baseMovingTimer = 3000;
    private float moveSpeed = 0.075f;
    private float projectileSpeed = 3f;
    private int projectileTimer;
    private void Start()
    {
        Life = 10;
        PointWorth = 15;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        IFrame--;
        if (aiState == 0)
        {
            if (aimingTimer <= 0) {
                aimingTimer = baseAimingTimer;
                targetedLocation = FindLocation();
                aiState = 1;
            }
            else {
                aimingTimer--;
            }
        }

        if (aiState == 1) {
            transform.position = Vector2.Lerp(transform.position, targetedLocation, moveSpeed * Time.deltaTime);
            if (movingTimer <= 0) {
                movingTimer = baseMovingTimer;
                aiState = 0;
            }
            else {
                movingTimer--;
            }
        }

        if (projectileTimer <= 0) {
            ShootProjectile();
            projectileTimer = UnityEngine.Random.Range(60, 300);
        } else {
            projectileTimer--;
        }
    }

    private Vector2 FindLocation() {
        return new Vector2(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50));
    }

    private void ShootProjectile() {
        Vector2 projectileDirection = (Player.Position - (Vector2)this.transform.position).normalized * projectileSpeed;
        Projectile.NewProjectile(this.transform.position, projectileDirection, 2);
    }

    public override void OnKill()
    {
        DeathParticles(30, 0.6f, new Color(1, 0.85f, 0.99f));
    }
}
