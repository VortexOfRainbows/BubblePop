using UnityEngine;
public class Ent : Enemy
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Ent");
    }
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.06f; //This is intentionally very precise
        additiveColorPower = 0.4f;
    }
    private Vector2 targetedLocation;
    public float moveSpeed = 0.12f;
    public float inertiaMult = 0.96f;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 27;
        data.BaseMaxCoin = 18;
        data.BaseMaxGem = 2;
        data.Cost = 3.5f;
        data.WaveNumber = 4;
        data.Rarity = 3;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        offset.y += 0.15f;
        scale *= 1.4f;
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
