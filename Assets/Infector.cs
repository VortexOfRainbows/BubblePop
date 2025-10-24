using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Infector : Enemy
{
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.Card = Resources.Load<Sprite>("NPCs/Infectors/Infector");
        data.BaseMinCoin = 5;
        data.BaseMaxCoin = 10;
        data.BaseMaxLife = 15;
        data.Rarity = 5;
    }
    public SpriteRenderer[] Shards;
    public SpriteRenderer[] Glows;
    public float AnimationTimer;
    public Transform Eye;
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
        Eye.transform.localPosition = Eye.transform.localPosition.Lerp(norm * 0.175f, 0.1f);

        float bobbing = Mathf.Deg2Rad + AnimationTimer * Mathf.PI / 90f;
        Visual.transform.localPosition = Visual.transform.localPosition.Lerp(new Vector3(0, 0.25f + Mathf.Sin(bobbing) * 0.05f, 0), 0.1f);
    }
    public override void AI()
    {
        UpdateShards();
        UpdateEye();
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
                ParticleManager.NewParticle(pos + Utils.RandCircle(0.1f), Utils.RandFloat(0.1f, 0.2f), Utils.RandCircle(5), 0.3f, Utils.RandFloat(0.6f, 1.1f), ParticleManager.ID.Square, Color.black);
            for (int i = 0; i < 6; ++i)
                ParticleManager.NewParticle(pos + Utils.RandCircle(0.1f), 0.25f, Utils.RandCircle(3, 6), 0.1f, Utils.RandFloat(0.5f, 0.7f), ParticleManager.ID.Trail, Color.red);
        }
        Vector2 pos2 = (Vector2)Visual.transform.position;
        for (int i = 0; i < 15; ++i)
        {
            ParticleManager.NewParticle(pos2 + Utils.RandCircle(0.6f), Utils.RandFloat(3, 5), Utils.RandCircle(5), 0.5f, Utils.RandFloat(0.9f, 1.3f), ParticleManager.ID.Pixel, Color.red);
            ParticleManager.NewParticle(pos2 + Utils.RandCircle(0.6f), Utils.RandFloat(0.1f, 0.2f), Utils.RandCircle(5), 0.3f, Utils.RandFloat(0.6f, 1.1f), ParticleManager.ID.Square, Color.black);
        }
        for (int i = 0; i < 8; ++i)
            ParticleManager.NewParticle(pos2 + Utils.RandCircle(0.6f), 0.25f, Utils.RandCircle(3, 6), 0.1f, Utils.RandFloat(0.5f, 0.7f), ParticleManager.ID.Trail, Color.red);
    }
}
