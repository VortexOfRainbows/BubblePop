using UnityEngine;

public class Sentinel : Enemy
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Sentinel");
    }
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.1f;
    }
    public Transform[] Rings;
    private static readonly Vector2[] DefaultPositions = new Vector2[] { new(0, -0.5f), new(0, -0.875f), new(0, -1.025f), new(0, 1.4f) };
    public Transform Head => Rings[3];
    private Vector2 targetedLocation;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 25;
        data.BaseMaxCoin = 20;
        data.BaseMinCoin = 1;
        data.BaseMinGem = 1;
        data.Rarity = 3;
        data.Cost = 4f;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        scale *= 1.1f;
    }
    public float AnimCounter = 0;
    public int DustCount = 0;
    public float AttackCounter = 0;
    public SpecialTrail MyTrail { get; private set; }
    public override void OnSpawn()
    {
        MyTrail = SpecialTrail.NewTrail(Rings[0], ColorHelper.SentinelBlue.WithAlpha(0.75f), 0.5f, 0.75f);
        MyTrail.Trail.sortingOrder = -1;
        MyTrail.Trail.endColor = ColorHelper.SentinelGreen.WithAlpha(0);
    }
    public override void AI()
    {
        float moveSpeed = 1.5f;
        float inertiaMult = 0.9825f;
        targetedLocation = Player.Position;
        Vector2 toTarget = targetedLocation - (Vector2)transform.position;
        float magnitude = toTarget.magnitude;
        if (magnitude > 16 && AttackCounter <= 0)
        {
            RB.velocity += toTarget / magnitude * moveSpeed;
            inertiaMult = 0.95f;
        }
        else
        {
            AttackCounter++;
            if (AttackCounter >= 200)
            {
                AttackCounter = 0;
            }
            RB.velocity += toTarget.RotatedBy(Mathf.PI * 0.4f) / magnitude * moveSpeed * 0.0225f;
        }
        RB.velocity *= inertiaMult;
        float mainDir = toTarget.x > 0 ? 1 : -1;
        float tilt = Mathf.Sqrt(Mathf.Abs(RB.velocity.x)) * Visual.transform.localScale.x * -3.0f;

        Visual.transform.localEulerAngles = Vector3.forward * Mathf.LerpAngle(Visual.transform.localEulerAngles.z, tilt, 0.01f);

        Vector2 specialAngleVector = toTarget;
        specialAngleVector.x = mainDir * 10 + specialAngleVector.x * 10;
        float headAngle = specialAngleVector.ToRotation() * Mathf.Rad2Deg + (mainDir == -1 ? 180 : 0);
        Head.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(Head.transform.eulerAngles.z, headAngle, 0.05f));
        AnimCounter += Time.fixedDeltaTime;
        for (int i = 0; i < Rings.Length; ++ i)
        {
            if(i != 0)
            {
                Vector2 old = Rings[i].transform.position;
                Vector2 newPos = old;
                newPos.x -= RB.velocity.x * Time.fixedDeltaTime;
                newPos.y -= RB.velocity.y * Time.fixedDeltaTime * 0.2f;
                Rings[i].transform.position = newPos;
            }
            Vector3 pos = DefaultPositions[i];
            pos.x += Mathf.Sin((AnimCounter + i * 0.5f) * Mathf.PI * 2) * 0.0175f;
            pos.y += Mathf.Sin((AnimCounter + i * 0.1f) * Mathf.PI) * 0.2f;
            Rings[i].LerpLocalPosition(pos, i == 3 ? 0.4f : 0.3f - i * 0.05f);
            Rings[i].transform.localScale = new Vector3(mainDir * Mathf.Abs(Rings[i].transform.localScale.x), Mathf.Abs(Rings[i].transform.localScale.y), 1);
        }
        ++DustCount;
        int c = DustCount / 2;
        if(DustCount % 2 == 0 && Utils.RandFloat() <= 0.1f)
        {
            for (int i = 0; i < 2; i++)
            {
                if (c % 2 == i)
                {
                    float r = AnimCounter * Mathf.PI + i * Mathf.PI;
                    Vector2 circular = new Vector2(0.5f, 0).RotatedBy(r);
                    circular.y *= 0.25f;
                    circular.y += 0.05f;
                    ParticleManager.NewParticle(Rings[0].position + (Vector3)circular, Utils.RandFloat(1.25f, 1.5f), Vector2.up * 8 + circular * 10 + RB.velocity, 0.1f, 1f, ParticleManager.ID.Pixel, ColorHelper.SentinelColorsLerp(Mathf.Sin(r)).WithAlpha(0.5f));
                }
            }
        }
        if(RB.velocity.magnitude > 6 && DustCount % 3 == 0)
        {
            ParticleManager.NewParticle(Rings[0].position - (Vector3)(RB.velocity.normalized * 0.65f + Utils.RandCircle(0.25f)), Utils.RandFloat(1.5f, 2.5f), -RB.velocity * 0.2f, 1f, Utils.RandFloat(0.8f, 1.2f), ParticleManager.ID.Pixel, ColorHelper.SentinelColorsLerp(Utils.RandFloat()).WithAlpha(0.5f));
        }
    }
    public new void Update()
    {
        base.Update();
        float percent = AttackCounter / 200f;
        float iPer = 1 - percent;
        iPer *= iPer;
        float sqrt = Mathf.Sqrt(percent);
        Vector2 toTarget = targetedLocation - (Vector2)Head.position;
        float magnitude = toTarget.magnitude;
        for (int i = -1; i <= 1; i += 2)
            SpriteBatch.Draw(Main.TextureAssets.GradientLine, new(Head.position.x, Head.position.y, 0.5f),
                new Vector2(magnitude * 0.3f * percent, 6 + percent * 10f),
                toTarget.ToRotation() * Mathf.Rad2Deg + i * iPer * 90, ColorHelper.SentinelGreen * sqrt * 0.9f);
        for (int i = -1; i <= 1; i += 2)
            SpriteBatch.Draw(Main.TextureAssets.GradientLine, new(Head.position.x, Head.position.y, 0.5f),
                new Vector2(magnitude * 0.3f * percent, 3 + percent * 5f),
                toTarget.ToRotation() * Mathf.Rad2Deg + i * iPer * 90, ColorHelper.SentinelBlue * sqrt);
    }
    public override void OnKill()
    {
        float amt = 60;
        for (int i = 0; i < amt; ++i)
        {
            float p = i / amt * Mathf.PI * 2;
            Vector2 circular = new Vector2(0, 1.5f + 0.5f * Mathf.Sin(p * 5)).RotatedBy(p - Mathf.PI / 10f);
            ParticleManager.NewParticle((Vector2)Head.position + circular * 0.1f, Utils.RandFloat(2.5f, 4.0f), circular * Utils.RandFloat(4, 6), 2f, Utils.RandFloat(0.6f, 2.5f), ParticleManager.ID.Pixel, ColorHelper.SentinelColorsLerp(Mathf.Sin(p)));
        }
        AudioManager.PlaySound(SoundID.ElectricCast, transform.position, 0.5f, 1.2f);
    }
}