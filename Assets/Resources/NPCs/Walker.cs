using System.IO;
using UnityEngine;

public class Walker : Enemy
{
    private Vector2 targetedLocation;
    public float moveSpeed = 0.12f;
    public float inertiaMult = 0.96f;
    private void Start()
    {
        Life = 7;
        MaxCoins = 5;
    }
    public void UpdateDirection(float i)
    {
        if (i >= 0)
            i = 1;
        else
            i = -1;
        Visual.transform.localScale = new Vector3(i, 1, 1);
    }
    public void MoveUpdate()
    {
        targetedLocation = Player.Position;
        Vector2 toTarget = targetedLocation - (Vector2)transform.position;
        RB.velocity += toTarget.normalized * moveSpeed;
        RB.velocity *= inertiaMult;
        if (Mathf.Abs(RB.velocity.x) > 0.1f)
            UpdateDirection(RB.velocity.x);
    }
    public override void AI()
    {
        int soundChance = Random.Range(1, 500);
        if (soundChance == 1)
        {
            AudioManager.PlaySound(SoundID.DuckNoise, transform.position, 0.13f, 1.2f);
        }
        MoveUpdate();
    }
    public override void OnKill()
    {
        DeathParticles(20, 0.5f, Color.white);
        AudioManager.PlaySound(SoundID.DuckDeath, transform.position, 0.25f, 1.2f);
    }
}
