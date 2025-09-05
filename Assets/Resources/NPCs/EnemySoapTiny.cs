using UnityEngine;
public class EnemySoapTiny : EnemySoap
{
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 3;
        data.BaseMaxCoin = 5;
        data.Card = Resources.Load<Sprite>("NPCs/Old/soap_tiny_1");
    }
    public override void OnSpawn()
    {
        timer = 60;
        UniversalImmuneFrames = 45;
    }
    public override void OnKill()
    {
        DeathParticles(10, 0.4f, new Color(1, 0.85f, 0.99f));
        AudioManager.PlaySound(SoundID.SoapDie, transform.position, 0.9f, 1.1f);
    }
    public override string Name()
    {
        return "Soap Fragment";
    }
}
