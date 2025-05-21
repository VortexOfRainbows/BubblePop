using System.IO;
using UnityEngine;

public class Walker : Enemy
{
    public GameObject Visual;
    public Rigidbody2D RB;
    //aiState 0 = choosing location
    //aiState 1 = going to location
    private int aiState = 0;
    private Vector2 targetedLocation;
    private int aimingTimer;
    const int baseAimingTimer = 240;
    private int movingTimer;
    const int baseMovingTimer = 300;
    private float moveSpeed = 0.1f;
    protected float bobbingTimer = 0;
    private void Start()
    {
        Life = 15;
        PointWorth = 20;
    }
    public void UpdateDirection(int i)
    {
        if (i >= 0)
            i = 1;
        else
            i = -1;
        Visual.transform.localScale = new Vector3(i, 1, 1);
    }
    // Update is called once per frame
    public void MoveUpdate()
    {
        if (aiState == 0)
        {
            if (aimingTimer <= 0)
            {
                aimingTimer = baseAimingTimer;
                targetedLocation = FindLocation();
                aiState = 1;
            }
            else
            {
                aimingTimer--;
            }
        }

        if (aiState == 1)
        {
            Vector2 toTarget = targetedLocation - (Vector2)transform.position;
            bobbingTimer += Mathf.Sqrt(toTarget.magnitude);
            UpdateDirection((int)Mathf.Sign(toTarget.x));
            transform.position = Vector2.Lerp(transform.position, targetedLocation, moveSpeed * Time.deltaTime);
            if (movingTimer <= 0)
            {
                movingTimer = baseMovingTimer;
                aiState = 0;
            }
            else
            {
                movingTimer--;
            }
        }
        bobbingTimer++;
        float bobSpeed = 80f;
        float sin = Mathf.Sin(bobbingTimer * Mathf.PI / bobSpeed);
        Visual.transform.localEulerAngles = new Vector3(0, 0, sin * 15);
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
    protected virtual Vector2 FindLocation() {
        return (Vector2)transform.position + new Vector2(Random.Range(-50f, 50f), Random.Range(-50f, 50f));
    }
    public override void OnKill()
    {
        DeathParticles(20, 0.5f, new Color(1, .97f, .52f));
        AudioManager.PlaySound(SoundID.DuckDeath, transform.position, 0.25f, 1.2f);
    }
}
