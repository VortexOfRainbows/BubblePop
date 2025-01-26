using UnityEngine;
public class EnemySoapTiny : EnemySoap
{
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Life = 3;
        aiState = 1;
    }
}
