using UnityEngine;
public class RockGolem : RockSpider
{
    private Color ShotColor => InfectionTarget ? new Color(1, .1f, .1f, 1.0f) : new Color(.2f, .3f, 1f, 1.0f);
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Rock Golem");
    }
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.05f;
        additiveColorPower += 0.4f;
    }
    public override float HealthBarOffset => -1.1f;
    public override float HealthBarSizeModifier => .7f;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 18;
        data.BaseMaxCoin = 3;
        data.BaseMinCoin = 1;
        data.BaseMaxGem = 1;
        data.Cost = 10f;
        data.Rarity = 5;
        data.WaveNumber = 9;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        scale *= 1.7f;
        offset.y += 0.55f;
    }
    public GameObject Parent { get; set; } = null;
    public GameObject Child { get; set; } = null;
    public override void OnSpawn()
    {
        HealthBarAlpha = 0;
        Timer = 0;
    }
    public bool hasSpawned = false;
    public float SpawnedInAlpha = 0;
    public override void UIAI()
    {
        SpawnedInAlpha = 1;
        UpdateDirection(-1);
        //UpdateLegRotations();
        //Animate();
    }
    public float TiltCounter { get; set; } = 0;
    public static readonly float ShotCooldownSlowdown = 100;
    public static readonly float ShotCooldown = 500;
    public static readonly float ShotWindup = 150;
    public static readonly float ShotChainRate = 0.1f;
    public int AttackNum = 0;
    public override bool ViableInfectionTarget()
    {
        return Parent == null;
    }
    public override void OnImplantChampion(Infector Infector)
    {
        if(Child != null)
        {
            var spider = Child.GetComponent<RockGolem>();
            spider.ImplantChampion(Infector);
            spider.InfectionTarget = true;
        }
    }
    public override void AI()
    {
        if (Parent == null)
        {
            HealthBarAlpha = Mathf.Lerp(HealthBarAlpha, 1, 0.05f);
            GetComponent<CircleCollider2D>().enabled = true;
            SpawnedInAlpha += 0.05f;
            if (!hasSpawned)
            {
                RockGolem prevP = this;
                for(int i = 1; i < 10; ++i)
                {
                    RockGolem r = Instantiate(EnemyID.RockGolem, transform.position, Quaternion.identity).GetComponent<RockGolem>();
                    r.Parent = prevP.gameObject;
                    r.Visual.transform.localPosition = new Vector3(r.Visual.transform.localPosition.x, r.Visual.transform.localPosition.y, 0.07f * i);
                    prevP.Child = r.gameObject;
                    prevP = r;
                    r.transform.localScale *= 1f;
                    r.hasSpawned = true;
                    r.UpdateRendererColor(Color.red.WithAlpha(0), 1);
                    if (IsSkull)
                        r.SetSkullEnemy();
                }
            }
            Head.gameObject.SetActive(true);
            Vector2 toPlayer = Player.Position - (Vector2)transform.position;
            float beginShootingRange = ShotCooldown - ShotCooldownSlowdown;
            if (toPlayer.magnitude < 16 || Timer < 60 || Timer > beginShootingRange)
                ++Timer;
            float percent = 1;
            float speed = 0.24f * Mathf.Clamp(Timer / 60f, 0, 1) * percent;
            if (Timer > beginShootingRange)
            {
                percent = 1 - ((Timer - beginShootingRange) / ShotCooldownSlowdown);
                if(percent <= 0)
                {
                    if ((Timer - ShotCooldown) == 70)
                        AudioManager.PlaySound(SoundID.ChargePoint, transform.position, 0.5f, 1.1f, 0);
                    percent = Mathf.Clamp01(percent);
                    speed *= percent;
                    if (AttackNum % 2 == 0)
                    {
                        percent = ((Timer - ShotCooldown) / ShotWindup);
                        if (percent > ShotChainRate && Child != null && Child.GetComponent<RockGolem>().SpawnedInAlpha >= 0.95f)
                        {
                            var child = Child.GetComponent<RockGolem>();
                            if (child.Timer <= 0)
                            {
                                child.Timer = 1;
                            }
                        }
                        if (percent >= 1)
                        {
                            ShootProj();
                            AttackNum++;
                        }
                    }
                    else
                    {
                        percent = ((Timer - ShotCooldown) / ShotWindup);
                        if (percent >= 1)
                        {
                            Vector2 headToPlayer = Player.Position - (Vector2)Head.transform.position;
                            Vector2 norm2 = headToPlayer.normalized;
                            for(int i = -4; i <= 4; ++i)
                            {
                                Projectile.NewProjectile<Bullet>(Head.transform.position, 
                                    norm2.RotatedBy(Mathf.PI * i / 4f * 0.125f) * 12, 1, 1.25f - Mathf.Abs(i) * 0.125f, InfectionTarget ? 0 : 1);
                            }
                            AudioManager.PlaySound(SoundID.BathBombBurst, Head.transform.position, 1.1f, 1.3f, 0);
                            Head.transform.position -= (Vector3)(norm2 * 0.35f);
                            AttackNum++;
                            Timer = 0;
                        }
                    }
                }
                else
                {
                    percent = Mathf.Clamp01(percent);
                    speed *= percent;
                }
            }
            Vector2 norm = toPlayer.normalized;
            RB.velocity += norm * speed;
            RB.velocity *= 0.95f;

            norm *= 0.5f + 0.5f * percent;
            Head.transform.SetEulerZ(norm.x * 20 * norm.y);
            norm.x *= 0.25f;
            norm.y *= 0.20f;
            norm.y += 0.12f;
            Head.transform.localPosition = Head.transform.localPosition.Lerp(norm, 0.05f);
        }
        else
        {
            HealthBarAlpha = Mathf.Lerp(HealthBarAlpha, 0, 0.05f);
            if(Timer > 0 && SpawnedInAlpha >= 1)
            {
                Timer++;
                if(Timer == 70)
                    AudioManager.PlaySound(SoundID.ChargePoint, transform.position, 0.5f, 1.1f, 0);
                float percent = Timer / ShotWindup;
                if (percent > ShotChainRate && Child != null)
                {
                    var child = Child.GetComponent<RockGolem>();
                    if (child.Timer <= 0)
                    {
                        child.Timer = 1;
                    }
                }
                if (Timer > ShotWindup)
                    ShootProj();
            }
            else
                Timer = 0;
            GetComponent<CircleCollider2D>().enabled = false;
            Head.gameObject.SetActive(false);
            Vector2 pos = transform.position;
            Vector2 parent = Parent.transform.position;
            Vector2 parentToMe = pos - parent;
            float magnitude = parentToMe.magnitude;
            float snapDist = 2.05f * transform.localScale.x;
            if(magnitude > snapDist)
            {
                Vector2 old = transform.position;
                transform.position = parent + parentToMe.normalized * snapDist;
                Vector2 oldToNew = (Vector2)transform.position - old;
                RB.velocity = Vector2.Lerp(RB.velocity, oldToNew * magnitude / Time.fixedDeltaTime, 0.05f);
            }
            SpawnedInAlpha += RB.velocity.magnitude * Time.fixedDeltaTime * 0.5f;
            RB.position -= RB.velocity * Time.fixedDeltaTime;
        }
        float magnitude2 = RB.velocity.magnitude;
        if(magnitude2 > 0)
            TiltCounter += Mathf.Sqrt(magnitude2) * Time.fixedDeltaTime * 0.4f;
        Body.LerpLocalEulerZ(Mathf.Sin(TiltCounter * Mathf.PI) * 6, 0.5f);
        hasSpawned = true;
        UpdateDirection(Utils.SignNoZero(RB.velocity.x));
        UpdateLegRotations();
        SpawnedInAlpha = Mathf.Clamp01(SpawnedInAlpha);
    }
    public void ShootProj()
    {
        Vector2 pos = (Vector2)Body.transform.position + new Vector2(0, 3.5f).RotatedBy(Body.transform.localEulerAngles.z * Mathf.Deg2Rad);
        Vector2 toPlayer = Player.Position - pos;
        Vector2 tnorm = toPlayer.normalized;
        for (int i = 0; i < 16; ++i)
        {
            ParticleManager.NewParticle(pos, Utils.RandFloat(2, 4), Utils.RandCircle(5) - tnorm * Utils.RandFloat(3), .2f, Utils.RandFloat(0.8f, 1.5f), 
                ParticleManager.ID.Pixel, Color.Lerp(ShotColor, Color.white, Utils.RandFloat()));
        }
        Projectile.NewProjectile<Bullet>(pos, tnorm * 12, 1, 1.25f, InfectionTarget ? 0 : 1);
        AudioManager.PlaySound(SoundID.BathBombBurst, pos, 1.0f, 1.4f, 0);
        Timer = 0;
    }
    public new void Update()
    {
        if (IsDummy)
            return;
        if (DamageTaken > 0)
        {
            UpdateRendererColor(Color.Lerp(Color.white, Color.red, 0.8f).WithAlpha(SpawnedInAlpha), Utils.DeltaTimeLerpFactor(0.05f + DamageTaken / 500f));
            DamageTaken -= 20f * Time.deltaTime;
        }
        else
        {
            DamageTaken = 0;
            UpdateRendererColor(Color.Lerp(Color.red, Color.white, SpawnedInAlpha).WithAlpha(SpawnedInAlpha), Utils.DeltaTimeLerpFactor(0.1f));
        }
        float offset = Parent == null ? ShotCooldown : 0;
        if (Timer > offset)
        {
            float percent = (Timer - offset) / ShotWindup;
            float per2 = percent * percent;
            Vector2 pos = (Vector2)Body.transform.position + new Vector2(0, 1.0f + per2 * 2.5f).RotatedBy(Body.transform.localEulerAngles.z * Mathf.Deg2Rad);
            if (AttackNum % 2 == 0)
            {
                SpriteBatch.Draw(Main.TextureAssets.Shadow,
                    pos, Vector2.one * (1 + percent), 0, ShotColor * percent, 1);
                SpriteBatch.Draw(Main.TextureAssets.Shadow,
                    pos, Vector2.one * per2, 0, Color.white * percent, 1);
            }
            else
            {
                SpriteBatch.Draw(Main.TextureAssets.Shadow,
                    Head.transform.position + new Vector3(0, -0.24f), Vector2.one * (1 + percent), 0, ShotColor * percent, 1);
                SpriteBatch.Draw(Main.TextureAssets.Shadow,
                    Head.transform.position + new Vector3(0, -0.24f), Vector2.one * percent, 0, ShotColor * percent, 1);
            }
        }
        lastPos = transform.position;
    }
    public Transform[] LegAnchors;
    public float movementOffset = 0;
    public void UpdateLegRotations()
    {
        float[] radians = new float[] {Mathf.PI, Mathf.PI * 3 / 2, Mathf.PI / 2, 0};
        movementOffset = Mathf.LerpAngle(movementOffset, (RB.velocity.ToRotation() + Mathf.PI / 2) * Mathf.Rad2Deg, 0.05f);
        Color c = IsDummy ? Color.white : Color.Lerp(Color.red, Color.white, SpawnedInAlpha);
        for (int i = 0; i < LegPoints.Length; ++i)
        {
            float r = radians[i] + movementOffset * Mathf.Deg2Rad;
            float sin = Mathf.Sin(r - 3 * Mathf.PI / 4);
            Vector2 v = new Vector2(1, 1).RotatedBy(r); 
            Vector3 defaultPos = v;
            defaultPos.x *= 0.75f;
            defaultPos.y *= 0.425f;
            defaultPos.y += 0.025f;

            Vector3 anchorPos = v;
            anchorPos.x *= 0.2f;
            anchorPos.y *= 0.2f;
            anchorPos.y -= 0.175f;

            LegPoints[i].color = Color.Lerp(Color.black, c, 0.8f + 0.2f * sin).WithAlpha(SpawnedInAlpha);

            bool isBackLeg = sin < 0;
            if (isBackLeg)
                anchorPos.z = defaultPos.z = 1.05f;
            float scaleBack = 0.85f + 0.15f * sin;
            LegAnchors[i].localPosition = LegAnchors[i].localPosition = defaultPos * scaleBack;
            LegConnectors[i].transform.localPosition = anchorPos * scaleBack;
            LegAnchors[i].localScale = Vector3.one * scaleBack;
            LegConnectors[i].transform.localScale = LegConnectors[i].transform.localScale.SetXY(LegConnectors[i].transform.localScale.x, 1.0f + 0.2f * sin);
        }
    }
    public override void UpdateDirection(float i)
    {
        if (i >= 0)
            i = -1;
        else
            i = 1;
        Body.transform.localScale = new Vector3(i * Mathf.Abs(Body.transform.localScale.x), Body.transform.localScale.y, 1);
    }
    public override void OnKill()
    {
        DeathParticles(40, 0.6f, new Color(60 / 255f, 70 / 255f, 92 / 255f));
        AudioManager.PlaySound(SoundID.DuckDeath, transform.position, 0.1f, 0.5f);
    }
    public override Vector3 CrownPositionOffset()
    {
        return new Vector3(0, 0, -1);
    }
}
