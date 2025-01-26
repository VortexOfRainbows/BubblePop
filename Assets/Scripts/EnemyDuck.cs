using Unity.VisualScripting;
using UnityEngine;
public class EnemyDuck : Entity
{
    public SpriteRenderer sRender;
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
        Life = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (aiState == 0) {
            if (aimingTimer <= 0) {
                aimingTimer = baseAimingTimer;
                targetedLocation = FindLocation();
                aiState = 1;
            }
            else {
                aimingTimer--;
            }
        }

        if (aiState == 1) {
            Vector2 toTarget = targetedLocation - (Vector2)transform.position;
            bobbingTimer += Mathf.Sqrt(toTarget.magnitude);
            sRender.flipX = toTarget.x > 0;
            transform.position = Vector2.Lerp(transform.position, targetedLocation, moveSpeed * Time.deltaTime);
            if (movingTimer <= 0) {
                movingTimer = baseMovingTimer;
                aiState = 0;
            }
            else {
                movingTimer--;
            }
        }
        bobbingTimer++;
        float sin = Mathf.Sin(bobbingTimer * Mathf.PI / 80f);
        transform.eulerAngles = new Vector3(0, 0, sin * 15);
    }

    private Vector2 FindLocation() {
        return (Vector2)transform.position + new Vector2(Random.Range(-50f, 50f), Random.Range(-50f, 50f));
    }
}
