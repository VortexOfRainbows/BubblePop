using UnityEngine;
public class EnemyDuck : Entity
{
    //aiState 0 = choosing location
    //aiState 1 = going to location
    private int aiState = 0;
    private Vector2 targetedLocation;
    private int aimingTimer;
    const int baseAimingTimer = 400;
    private int movingTimer;
    const int baseMovingTimer = 3000;
    private float moveSpeed = 0.1f;


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
            transform.position = Vector2.Lerp(transform.position, targetedLocation, moveSpeed * Time.deltaTime);
            if (movingTimer <= 0) {
                movingTimer = baseMovingTimer;
                aiState = 0;
            }
            else {
                movingTimer--;
            }
        }
    }

    private Vector2 FindLocation() {
        return new Vector2 (Random.Range(-50, 50), Random.Range(-50, 50));
    }
}
