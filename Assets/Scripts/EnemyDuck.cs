using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDuck : MonoBehaviour
{
    private float moveSpeed = 1f;

    // Update is called once per frame
    private void Update()
    {
        // move sprite towards the target location
        transform.position = Vector2.Lerp(transform.position, Player.Position, moveSpeed * Time.deltaTime);

        //if close to another enemy, move slightly away from that enemy

    }
}
