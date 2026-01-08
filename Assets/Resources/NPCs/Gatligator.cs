using UnityEngine;

public class Gatligator : Enemy
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gatligator");
    }
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.05f;
        additiveColorPower = 0.5f;
    }
    public override float HealthBarOffset => -0.25f;
    public Transform GunTip;
    public GameObject Gun;
    private Vector2 targetedLocation;
    private readonly float moveSpeed = 0.18f;
    private readonly float inertiaMult = 0.9875f;
    public float direction = 1;
    private float ShootTimer = 0;
    private float ShootSpeed = 0.5f;
    public override float SkullPowerDropChance => 0.2f;
    public override float PowerDropChance => 0.05f;
    public override float CostMultiplier => 5;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 25;
        data.BaseMaxCoin = 40;
        data.Rarity = 4;
        data.BaseMaxGem = 3;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        offset.x -= 0.1f;
        scale *= 0.45f;
    }
    public override void OnSpawn()
    {
        ShootTimer = -3f;
        ShootSpeed = 0.38f;
    }
    public void UpdateDirection(float i)
    {
        if (i >= 0)
            i = 1;
        else
            i = -1;
        Visual.transform.localScale = new Vector3(i, 1, 1);
    }
    private int movingMode = 0;
    public void MoveUpdate()
    {
        Vector2 toTarget = targetedLocation - (Vector2)transform.position;
        if(movingMode == 0)
        {
            targetedLocation = Player.Position;
        }
        else if(toTarget.magnitude < 1)
        {
            targetedLocation = Player.Position + Utils.RandCircle(18);
        }
        RB.velocity += toTarget.normalized * moveSpeed;
        RB.velocity *= inertiaMult;
        if(RB.velocity.magnitude > 18)
            RB.velocity *= inertiaMult;
        if (Mathf.Abs(RB.velocity.x) > 0.1f)
            UpdateDirection(RB.velocity.x);
        float tilt = Mathf.Sqrt(Mathf.Abs(RB.velocity.x)) * Visual.transform.localScale.x * -1.5f;
        tilt += RB.velocity.y * 0.5f * Visual.transform.localScale.x;
        Visual.transform.localEulerAngles = Vector3.forward * Mathf.LerpAngle(Visual.transform.localEulerAngles.z, tilt, 0.05f);
        if(Utils.RandFloat(1) < 0.006f) //Small chance to switch move modes every frame
        {
            movingMode++;
            movingMode %= 2;
            if(movingMode == 1)
                targetedLocation = Player.Position + Utils.RandCircle(18);
        }
    }
    public void GunUpdate()
    {
        Vector2 toTarget = Player.Position - (Vector2)Gun.transform.position;
        toTarget.x *= Visual.transform.localScale.x;
        float lerp = 0.02f;
        if (direction != Visual.transform.localScale.x)
        {
            direction = Visual.transform.localScale.x;
            Gun.transform.localEulerAngles = Vector3.forward * (180 - Gun.transform.localEulerAngles.z);
        }
        Gun.transform.localEulerAngles = Vector3.forward * Mathf.LerpAngle(Gun.transform.localEulerAngles.z, toTarget.ToRotation() * Mathf.Rad2Deg, lerp);

        ShootTimer += Time.fixedDeltaTime;
        if(ShootTimer > ShootSpeed)
        {
            ShootTimer -= ShootSpeed;
            Vector2 norm = (GunTip.position - Gun.transform.position).normalized;
            Projectile.NewProjectile<Gatorade>(GunTip.transform.position, norm * 16f);
            AudioManager.PlaySound(SoundID.ShootBubbles, GunTip.transform.position, 0.5f, 1.5f);
        }
    }
    public override void AI()
    {
        MoveUpdate();
        GunUpdate();
    }
    public override void OnKill()
    {
        DeathParticles(30, 0.7f, new Color(105 / 255f, 141 / 255f, 72 / 255f));
        AudioManager.PlaySound(SoundID.DuckDeath, transform.position, 0.25f, 0.4f);
    }
}
