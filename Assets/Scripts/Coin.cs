using UnityEditor;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public Rigidbody2D rb;
    public int Value;
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeSelf && collision.CompareTag("Player"))
        {
            CoinManager.Gain(Value);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    public void FixedUpdate()
    {
        rb.velocity *= 0.985f;
        rb.rotation += rb.velocity.magnitude * 0.5f * -Mathf.Sign(rb.velocity.x);
    }
}
