using UnityEngine;

public class Gatligator : Enemy
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gatligator");
    }
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.01f;
        additiveColorPower = 0.5f;
    }
    public override float HealthBarOffset => -0.25f;
    public Transform GunTip, Head;
    public GameObject Gun;
    public Transform[] Barrels = new Transform[6];
    private Vector2 targetedLocation;
    private readonly float moveSpeed = 0.18f;
    private readonly float inertiaMult = 0.9875f;
    public float direction = 1;
    private float ShootTimer = 0;
    private float ShootSpeed = 0.5f;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 25;
        data.BaseMaxCoin = 40;
        data.Rarity = 4;
        data.BaseMaxGem = 3;
        data.Cost = 5;
        data.WaveNumber = 6;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        offset.x -= 0.125f;
        offset.y += 0.55f;
        scale *= 0.6125f;
    }
    public override void OnSpawn()
    {
        ShootTimer = -3f;
        ShootSpeed = 0.38f;
        GunAnimationUpdate();
    }
    public override void UIAI()
    {
        if (ShootSpeed != 5)
        {
            ShootSpeed = 5;
            GunAnimationUpdate();
        }
    }
    public void UpdateDirection(float i)
    {
        if (i >= 0)
            i = 1;
        else
            i = -1;
        Visual.transform.localScale = new Vector3(i * 1.1f, 1.1f, 1);
    }
    private int movingMode = 0;
    public void MoveUpdate()
    {
        Vector2 toTarget = targetedLocation - (Vector2)transform.position;
        if(movingMode == 0)
        {
            targetedLocation = Target.Position;
        }
        else if(toTarget.magnitude < 1)
        {
            targetedLocation = Target.Position + Utils.RandCircle(18);
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
                targetedLocation = Target.Position + Utils.RandCircle(18);
        }
    }
    public void GunUpdate()
    {
        GunAnimationUpdate();
        Vector2 toTarget = Target.Position - (Vector2)Gun.transform.position;
        toTarget.x *= Visual.transform.localScale.x;
        float lerp = 0.03f;
        if (direction != Utils.SignNoZero(Visual.transform.localScale.x))
        {
            direction = Utils.SignNoZero(Visual.transform.localScale.x);
            Gun.transform.localEulerAngles = Vector3.forward * (180 - Gun.transform.localEulerAngles.z);
        }
        float angle = Mathf.LerpAngle(Gun.transform.localEulerAngles.z, toTarget.ToRotation() * Mathf.Rad2Deg, lerp);
        float dir2 = angle < 90 || angle > 270 ? 1 : -1;
        Gun.transform.localScale = new Vector3(0.97f, 0.97f * dir2, 1f);
        Gun.transform.localEulerAngles = Vector3.forward * angle;

        ShootTimer += Time.fixedDeltaTime;
        if(ShootTimer > ShootSpeed)
        {
            ShootTimer -= ShootSpeed;
            Vector2 norm = (GunTip.position - Gun.transform.position).normalized;
            Projectile.NewProjectile<Gatorade>(GunTip.transform.position, norm * 16f, 1, this);
            AudioManager.PlaySound(SoundID.ShootBubbles, GunTip.transform.position, 0.5f, 1.5f);
        }
    }
    public void GunAnimationUpdate()
    {
        float windUpPercent = Mathf.Clamp01((ShootTimer + 3) / 3f);
        float rollPercent = windUpPercent * Mathf.Clamp01(ShootTimer / ShootSpeed);
        Head.transform.SetEulerZ(Mathf.Sin(rollPercent * Mathf.PI * 2) * -15);
        for(int i = 0; i < 6; ++i)
        {
            var b = Barrels[i];
            float r = i * Mathf.PI / 3f;
            Vector2 circular = new Vector2(0, 1).RotatedBy(r + rollPercent * Mathf.PI);
            float scaleFactor = circular.x;
            circular.x *= 0.05f;
            circular.y *= 0.2f;
            b.localPosition = new Vector3(circular.x, circular.y, scaleFactor * 0.005f);
            b.localScale = Vector3.one * (1 - 0.1f * scaleFactor);
            var sr = b.GetComponent<SpriteRenderer>();
            if (!IsDummy)
                sr.color = Color.white;
            sr.color = Color.Lerp(sr.color, Color.black, 0.4f + 0.4f * scaleFactor);
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
