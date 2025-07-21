using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Coin : MonoBehaviour
{
    public Rigidbody2D rb;
    public int Value;
    public float AttractTimer = 0;
    public Color PopupColor;
    public float BeforeCollectableTimer = 0;
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeSelf && collision.CompareTag("Player") && BeforeCollectableTimer <= 0)
        {
            OnCollected();
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    public void FixedUpdate()
    {
        Player p = Player.Instance;
        rb.rotation += rb.velocity.magnitude * 0.5f * -Mathf.Sign(rb.velocity.x);

        float attractDist = 4 + p.Magnet * 3f;
        Vector2 toPlayer = p.transform.position - transform.position;
        float length = toPlayer.magnitude;
        if (length < attractDist && BeforeCollectableTimer <= 0)
        {
            float attractSpeed = 3 + p.Magnet + (++AttractTimer) / 30f;
            float percent = length / attractDist;
            rb.velocity = Vector2.Lerp(rb.velocity, toPlayer.normalized * attractSpeed, (1 - percent) * 0.2f);
            float speed = rb.velocity.magnitude;
            float maxSpeed = attractDist + attractSpeed;
            if (speed > maxSpeed)
            {
                rb.velocity *= maxSpeed / speed; //Max out the velocity at the length to player, so it doesn't move to fast when it is close
            }
        }
        else
        {
            rb.velocity *= 0.985f;
            AttractTimer = 0;
            BeforeCollectableTimer -= Time.fixedDeltaTime;
        }
    }
    public void OnCollected()
    {
        CoinManager.ModifyCurrent(Value);
        PopupText.NewPopupText(transform.position + (Vector3)Utils.RandCircle(0.5f) + Vector3.forward, Utils.RandCircle(2) + Vector2.up * 4, PopupColor, $"${Value.ToString()}");
        if (Value <= 1)
            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1, 0.85f, 0);
        else if (Value <= 5)
            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1, 0.85f, 1);
        else
            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1, 0.85f, 2);
    }
}
