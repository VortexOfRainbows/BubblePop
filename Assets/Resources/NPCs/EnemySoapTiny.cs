using UnityEngine;
public class EnemySoapTiny : EnemySoap
{
    public override void Init()
    {
        Life = 3;
        timer = 60;
        UniversalImmuneFrames = 80;
        MaxCoins = 5;
    }
    public override void OnKill()
    {
        DeathParticles(10, 0.4f, new Color(1, 0.85f, 0.99f));
        AudioManager.PlaySound(SoundID.SoapDie, transform.position, 0.9f, 1.1f);
    }
}
