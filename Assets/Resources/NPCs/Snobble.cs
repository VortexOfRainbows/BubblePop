using UnityEngine;

public class Snobble : Enemy
{
    public Transform Snow;
    public Transform Face;
    public Transform TopJaw;
    public Transform BotJaw;
    public Transform BackJaw;
    public Transform Mouth;
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.1f;
    }
    public virtual float MoveSpeed => 0.35f;
    public virtual float Inertia => 0.9625f;
    protected float MovementTimer;
    public bool PlayerNearby { get; set; } = false;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 12;
        data.BaseMaxCoin = 4;
        data.BaseMinCoin = 2;
        data.BaseMaxGem = 2;
        data.Cost = 2;
        data.WaveNumber = 4;
        data.Rarity = 2;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        scale *= 1.1f;
    }
    public void UpdateDirection(float i)
    {
        i = i >= 0 ? -1 : 1;
        Visual.transform.localScale = new Vector3(i * Mathf.Abs(Visual.transform.localScale.x), Mathf.Abs(Visual.transform.localScale.y), 1);
    }
    public override void AI()
    {
        Player.FindClosest(transform.position, out Vector2 _, out float dist);
        MovementTimer++;
        float trueMoveSpeed = MoveSpeed * 2;
        if (MovementTimer < 100)
        {
            float percent = MovementTimer / 100f;
            percent = 0.5f * percent + 0.5f * percent * percent;
            float rate = Mathf.Sin(percent * Mathf.PI * 0.5f);
            Snow.transform.LerpLocalScale(new Vector2(1.0f + 0.05f * rate, 1.1f - 0.125f * rate), 0.1f);
            Face.transform.LerpLocalPosition(new Vector2(0.15f * rate, -0.1f + 0.1f * rate), 0.1f);
            Face.transform.LerpLocalEulerZ(6 * rate, 0.1f);
            Mouth.transform.LerpLocalScale(new Vector2(1.0f - 0.1f * rate, 1.0f), 0.1f);
            trueMoveSpeed *= 0.0f;

            TopJaw.LerpLocalPosition(new Vector2(0, 0), 0.1f);
            BotJaw.LerpLocalPosition(new Vector2(0, 0), 0.1f);
            BackJaw.LerpLocalScale(new Vector2(1.0f, 1f), 0.1f);
            PlayerNearby = dist < 9;
            if (PlayerNearby && MovementTimer > 30)
                MovementTimer++;
        }
        else if (MovementTimer < 150)
        {
            if(MovementTimer == 110)
            {
                if(PlayerNearby)
                    AudioManager.PlaySound(SoundID.DuckDeath, transform.position, 0.7f, Utils.RandFloat(2.4f, 2.6f));
                else
                    AudioManager.PlaySound(SoundID.SoapSlide, transform.position, 0.5f, 1.7f, 0);
            }
            float percent = (MovementTimer - 100) / 50f;
            float rate = Mathf.Sin(percent * Mathf.PI);
            rate = Mathf.Sqrt(Mathf.Abs(rate));
            Snow.transform.LerpLocalScale(new Vector2(1.05f + 0.1f * rate, 0.9f + 0.25f * rate), 0.1f);
            Face.transform.LerpLocalPosition(new Vector2(-0.25f * rate, PlayerNearby ? 0.2f * rate : 0.0f), 0.1f);
            Face.transform.LerpLocalEulerZ(-6 * rate, 0.1f);
            Mouth.transform.LerpLocalScale(new Vector2(0.9f + 0.1f * rate, 1.0f - 0.1f * rate), 0.1f);
            trueMoveSpeed *= rate;
            if (MovementTimer > 140)
                trueMoveSpeed *= 0.1f;
            if(PlayerNearby)
            {
                TopJaw.LerpLocalPosition(new Vector2(0, 0.2f * rate), 0.1f);
                BotJaw.LerpLocalPosition(new Vector2(0, -0.3f * rate), 0.1f);
                BackJaw.LerpLocalScale(new Vector2(1.0f + 0.1f * rate, 1f + rate * 1.75f), 0.1f);
                trueMoveSpeed *= 2.75f;
            }
            if((Utils.RandBool(2) || PlayerNearby) && MovementTimer > 110)
                ParticleManager.NewParticle((Vector2)transform.position + new Vector2(0, -0.1f) + Utils.RandCircle(.5f), Utils.RandFloat(0.3f, 0.5f), Vector2.up * Utils.RandFloat(3, 6) - RB.velocity * Utils.RandFloat(0.4f, 0.6f), 0.5f, Utils.RandFloat(0.5f, 1.0f), ParticleManager.ID.SnowBG, Color.white);
        }
        else
        {
            MovementTimer = -Utils.RandInt(7);
        }
        Vector2 moveDir = GetPathfindingToPlayerNorm();
        RB.velocity += moveDir * trueMoveSpeed;
        RB.velocity *= Inertia;
        if (Mathf.Abs(RB.velocity.x) > 0.1f)
            UpdateDirection(RB.velocity.x);
        float tilt = Mathf.Sqrt(Mathf.Abs(RB.velocity.x)) * Visual.transform.localScale.x * -0.75f;
        tilt += Mathf.Sqrt(Mathf.Abs(RB.velocity.y)) * -1.5f * Visual.transform.localScale.x;
        Visual.transform.localEulerAngles = Vector3.forward * Mathf.LerpAngle(Visual.transform.localEulerAngles.z, tilt, 0.05f);
    }
    public override void OnKill()
    {
        for(int i = 0; i < 20; ++i)
            ParticleManager.NewParticle((Vector2)transform.position + new Vector2(0, -0.3f) + Utils.RandCircle(.7f), Utils.RandFloat(0.6f, 1.0f), Vector2.up * Utils.RandFloat(5, 7.5f) + Utils.RandCircle(4), 0.5f, Utils.RandFloat(0.8f, 1.4f), ParticleManager.ID.SnowBG, Color.white);
        AudioManager.PlaySound(SoundID.DuckNoise, transform.position, 0.65f, 2.5f);
    }
}
