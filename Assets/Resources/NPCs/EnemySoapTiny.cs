using UnityEngine;
public class EnemySoapTiny : EnemySoap
{
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Life = 3;
        timer  = 60;
        UniversalImmuneFrames = 80;
        PointWorth = 5;
    }
    public override void OnKill()
    {
        DeathParticles(10, 0.4f, new Color(1, 0.85f, 0.99f));
        AudioManager.PlaySound(SoundID.SoapDie, transform.position, 0.9f, 1.1f);
    }
}
