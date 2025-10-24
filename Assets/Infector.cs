using UnityEngine;

public class Infector : Enemy
{
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.Card = Resources.Load<Sprite>("NPCs/Infectors/Infector");
        data.BaseMinCoin = 5;
        data.BaseMaxCoin = 25;
        data.BaseMaxLife = 20;
        data.Rarity = 5;
        data.Cost = 8f;
    }
    public SpriteRenderer[] Shards;
    public SpriteRenderer[] Glows;
    public float AnimationTimer;
    public Transform Crown;
    public Transform Eye;
    public float ShotRecoil = 1;
    private float Counter = -280;
    private float Counter2 = 0;

    private Vector2 TargetPos = Vector2.zero;
    private Vector2 PrevPlayerPos = Vector2.zero;
    public void UpdateShards(float lerp = 0.1f)
    {
        AnimationTimer++;
        int c = Shards.Length;
        float rad = Mathf.PI / c * 2f;
        for (int i = 0; i < c; ++i)
        {
            float rot = rad * i + AnimationTimer * Mathf.PI / 240f;
            float bobbing = rad * i + AnimationTimer * Mathf.PI / 54f;
            Vector3 circular = new Vector2(0.6f, 0).RotatedBy(rot);
            circular.y *= -0.225f;
            circular.z = Mathf.Sin(rot) * 0.4f;
            circular.y += Mathf.Sin(bobbing) * 0.03f;
            float scale = 1 - circular.z * 0.2f;
            Shards[i].transform.localPosition = Shards[i].transform.localPosition.Lerp(circular, lerp);
            Shards[i].transform.localEulerAngles = Mathf.LerpAngle(Shards[i].transform.localEulerAngles.z, circular.x * -30, lerp) * Vector3.forward;
            Shards[i].transform.LerpLocalScale(Vector2.one * scale * 0.9f, lerp);
            Glows[i].color = Glows[i].color.WithAlpha(Mathf.Lerp(Glows[i].color.a, 1f, 0.08f));
            //velocities[i] *= 1 - lerp;
        }
    }
    public void UpdateEye()
    {
        Vector2 toPlayer = Player.Position - (Vector2)transform.position;
        Vector2 norm = toPlayer.normalized;
        Eye.transform.localPosition = Eye.transform.localPosition.Lerp(norm * 0.175f * ShotRecoil, 0.11f);

        float bobbing = Mathf.Deg2Rad + AnimationTimer * Mathf.PI / 90f;
        Visual.transform.localPosition = Visual.transform.localPosition.Lerp(new Vector3(0, 0.25f + Mathf.Sin(bobbing) * 0.05f, 0), 0.1f);
        ShotRecoil = Mathf.Lerp(ShotRecoil, 1, 0.09f);

        Crown.transform.LerpLocalEulerZ(RB.velocity.x, 0.25f);
        Visual.transform.LerpLocalEulerZ(RB.velocity.x * 0.5f, 0.15f);
    }
    public override void AI()
    {
        Vector2 playerPos = Player.Position;
        Vector2 toPlayer = playerPos - (Vector2)transform.position;
        Vector2 norm = toPlayer.normalized;
        float dist = toPlayer.magnitude;
        UpdateShards();
        UpdateEye();
        RB.velocity *= 0.925f;
        if (Counter < -120)
        {
            Counter++;
            return;
        }
        if (TargetPos == Vector2.zero)
        {
            Vector2 circular = new Vector2(-Mathf.Clamp(dist - 3, 10, 14.5f), 0).RotatedBy(toPlayer.ToRotation() + Utils.RandFloat(Mathf.PI / 2f, Mathf.PI) * Utils.Rand1Or0());
            TargetPos = Player.Position + circular;
            PrevPlayerPos = Player.Position;
            AudioManager.PlaySound(SoundID.ElectricCast, transform.position, 0.7f, 0.8f, 0);
        }
        else
        {
            Vector2 toTarget = PrevPlayerPos - TargetPos;
            float targetMagnitude = toTarget.magnitude;
            float a = Utils.LerpAngleRadians((PrevPlayerPos - (Vector2)transform.position).ToRotation(), toTarget.ToRotation(), 0.09f);

            Vector2 interTarget = PrevPlayerPos + new Vector2(-targetMagnitude, 0).RotatedBy(a);
            Vector2 playerToInter = interTarget - playerPos;
            float diff = playerToInter.magnitude - 6;
            if (diff < 0 && Counter < 0) 
            {
                interTarget -= playerToInter.normalized * diff;
            }
            toTarget = interTarget - (Vector2)transform.position;
            if(toTarget.magnitude > 0.5f)
            {
                RB.velocity += toTarget.normalized * 4f;
                Vector2 veloNorm = RB.velocity.normalized;
                if (Counter <= 0)
                    ParticleManager.NewParticle((Vector2)transform.position + new Vector2(0, -1) - veloNorm * 0.6f, 2.5f, Utils.RandCircle(1, 4) - veloNorm * 5, 0f, Utils.RandFloat(0.6f, 0.7f), ParticleManager.ID.Pixel, Color.red);
            }
            else
            {
                transform.position = transform.position.Lerp(interTarget, 0.1f);
            }
        }
        if (dist < 30 || Counter2 > 0)
        {
            Counter++;
            if(Counter > 75 && Counter2 <= 2)
            {
                Counter = 0;
                AudioManager.PlaySound(SoundID.ElectricZap, Eye.transform.position, 0.8f, 1.7f, 0);
                int c = (int)Counter2 % 2 + 2;
                for(int i = 0; i < c; i++)
                {
                    float otherMult = (i + 0.5f - c / 2f);
                    float j = otherMult * 25f;
                    Vector2 spread = norm.RotatedBy(j * Mathf.Deg2Rad);
                    Projectile.NewProjectile<Bullet>((Vector2)Eye.transform.position + spread * 0.5f, spread * (10 - Mathf.Abs(otherMult) * 2), 1, 1.3f);
                }
                ShotRecoil = -0.6f;
                RB.velocity -= norm * 2.0f;
                Counter2++;
            }
            if (Counter2 > 2 && Counter > 105)
            {
                Counter2 = 0;
                Counter = -120;
                TargetPos = Vector2.zero;
            }
        }
        else if (dist < 40)
        {
            if(Counter > 0 || Counter2 > 0)
            {
                Counter2 = 0;
                Counter = 0;
                TargetPos = Vector2.zero;
            }
        }
    }
    public override void OnInjured(float damage, int damageType)
    {
        if(Life > 0)
        {
            Vector2 pos2 = (Vector2)Visual.transform.position;
            AudioManager.PlaySound(SoundID.ElectricZap, transform.position, 0.8f, 1.5f, 1);
            for (int i = 0; i < damage; ++i)
            {
                ParticleManager.NewParticle(pos2 + Utils.RandCircleEdge(0.8f), Utils.RandFloat(3, 5), Utils.RandCircle(5), 0.5f, Utils.RandFloat(0.9f, 1.3f), ParticleManager.ID.Pixel, Color.red);
                ParticleManager.NewParticle(pos2 + Utils.RandCircleEdge(0.8f), Utils.RandFloat(0.1f, 0.2f), Utils.RandCircle(5), 0.3f, Utils.RandFloat(0.6f, 1.1f), ParticleManager.ID.Square, Color.black);
            }
            for (int i = 0; i < damage / 2; ++i)
                ParticleManager.NewParticle(pos2 + Utils.RandCircleEdge(0.6f), 0.25f, Utils.RandCircle(3, 6), 0.1f, Utils.RandFloat(0.5f, 0.7f), ParticleManager.ID.Trail, Color.red);
        }
    }
    public override void OnKill()
    {
        AudioManager.PlaySound(SoundID.ElectricZap, transform.position, 0.5f, 0.6f, 0);
        foreach(SpriteRenderer glow in Glows)
        {
            Vector2 pos = (Vector2)glow.transform.position;
            for (int i = 0; i < 12; ++i)
                ParticleManager.NewParticle(pos + Utils.RandCircle(0.1f), Utils.RandFloat(2, 4), Utils.RandCircle(4), 0.5f, Utils.RandFloat(0.7f, 1.0f), ParticleManager.ID.Pixel, glow.color);
            for (int i = 0; i < 8; ++i)
                ParticleManager.NewParticle(pos + Utils.RandCircle(0.1f), Utils.RandFloat(0.3f, 0.4f), Utils.RandCircle(5), 0.3f, Utils.RandFloat(0.6f, 1.1f), ParticleManager.ID.Square, Color.black);
            for (int i = 0; i < 6; ++i)
                ParticleManager.NewParticle(pos + Utils.RandCircle(0.1f), 0.25f, Utils.RandCircle(3, 6), 0.1f, Utils.RandFloat(0.5f, 0.7f), ParticleManager.ID.Trail, Color.red);
        }
        Vector2 pos2 = (Vector2)Visual.transform.position;
        for (int i = 0; i < 15; ++i)
        {
            ParticleManager.NewParticle(pos2 + Utils.RandCircle(0.6f), Utils.RandFloat(3, 5), Utils.RandCircle(5), 0.5f, Utils.RandFloat(0.9f, 1.3f), ParticleManager.ID.Pixel, Color.red);
            ParticleManager.NewParticle(pos2 + Utils.RandCircle(0.6f), Utils.RandFloat(0.3f, 0.4f), Utils.RandCircle(5), 0.3f, Utils.RandFloat(0.6f, 1.1f), ParticleManager.ID.Square, Color.black);
        }
        for (int i = 0; i < 8; ++i)
            ParticleManager.NewParticle(pos2 + Utils.RandCircle(0.6f), 0.25f, Utils.RandCircle(3, 6), 0.1f, Utils.RandFloat(0.5f, 0.7f), ParticleManager.ID.Trail, Color.red);
    }
}
