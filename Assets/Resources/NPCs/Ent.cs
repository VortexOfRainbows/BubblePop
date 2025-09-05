using System.IO;
using UnityEditor;
using UnityEngine;

public class Ent : Enemy
{
    private Vector2 targetedLocation;
    public float moveSpeed = 0.12f;
    public float inertiaMult = 0.96f;
    public override float CostMultiplier => 3.5f;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 27;
        data.BaseMaxCoin = 18;
    }
    public void UpdateDirection(float i)
    {
        if (i >= 0)
            i = 1;
        else
            i = -1;
        Visual.transform.localScale = new Vector3(i * 1.45f, 1.45f, 1);
    }
    public void MoveUpdate()
    {
        float dir = Utils.SignNoZero(Visual.transform.localScale.x);
        targetedLocation = Player.Position;
        Vector2 toTarget = targetedLocation - (Vector2)transform.position;
        RB.velocity += toTarget.normalized * moveSpeed;
        RB.velocity *= inertiaMult;
        if (Mathf.Abs(RB.velocity.x) > 0.1f)
            UpdateDirection(RB.velocity.x);
        float tilt = Mathf.Sqrt(Mathf.Abs(RB.velocity.x)) * dir * -1f;
        tilt += RB.velocity.y * 2.0f * dir;
        Visual.transform.localEulerAngles = Vector3.forward * Mathf.LerpAngle(Visual.transform.localEulerAngles.z, tilt, 0.05f);
    }
    public override void AI()
    {
        MoveUpdate();
    }
    public override void OnKill()
    {
        DeathParticles(20, 0.5f, new Color(.56f, .36f, .25f));
        AudioManager.PlaySound(SoundID.BathBombBurst, transform.position, 0.5f, 0.9f);
        for(int i = 0; i < 8; ++i)
        {
            Projectile.NewProjectile<Bullet>(transform.position, new Vector2(0, 7).RotatedBy(Mathf.PI * i / 4f));
        }
    }
}
