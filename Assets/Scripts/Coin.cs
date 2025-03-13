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
            OnCollected();
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    public void FixedUpdate()
    {
        rb.velocity *= 0.985f;
        rb.rotation += rb.velocity.magnitude * 0.5f * -Mathf.Sign(rb.velocity.x);

        float attractDist = 4;
        Player p = Player.Instance;
        Vector2 toPlayer = p.transform.position - transform.position;
        float length = toPlayer.magnitude;
        if (length < attractDist)
        {
            rb.velocity += toPlayer.normalized * 0.15f * (0.3f + attractDist - length);
            //float speed = rb.velocity.magnitude;
            //if (speed > length)
            //{
            //    rb.velocity *= length / speed; //Max out the velocity at the length to player, so it doesn't move to fast when it is close
            //}
        }
    }
    public void OnCollected()
    {
        CoinManager.ModifyCurrent(Value);
    }
}
