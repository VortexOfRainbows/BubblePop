using Unity.VisualScripting;
using UnityEngine;
public class Coin : MonoBehaviour
{
    public Rigidbody2D rb;
    public int Value;
    public int Heal = 0;
    public float AttractTimer = 0;
    public Color PopupColor;
    public float BeforeCollectableTimer = 0;
    public GameObject HeartVisual;
    public bool CanCollectHeart()
    {
        return Player.Instance.Life < Player.Instance.TotalMaxLife;
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeSelf && collision.CompareTag("Player") && BeforeCollectableTimer <= 0)
        {
            if(Heal > 0 && !CanCollectHeart())
                return;
            OnCollected();
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    private float timer;
    public void FixedUpdate()
    {
        Player p = Player.Instance;
        bool isHeart = Heal > 0;
        if (!isHeart)
            rb.rotation += rb.velocity.magnitude * 0.5f * -Mathf.Sign(rb.velocity.x);
        float attractDist = isHeart ? 3.5f : 4 + p.Magnet * 3f;
        Vector2 toPlayer = p.transform.position - transform.position;
        float length = toPlayer.magnitude;
        if (length < attractDist && (BeforeCollectableTimer <= 0 || isHeart) && (!isHeart || CanCollectHeart()))
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
            rb.velocity *= isHeart ? 0.925f : 0.985f;
            AttractTimer = 0;
        }
        BeforeCollectableTimer -= Time.fixedDeltaTime;

        if (isHeart)
        {
            if (Utils.RandFloat(1) < 0.2f)
            {
                Vector2 circular = new Vector2(Utils.RandFloat(0.3f, 0.5f), 0).RotatedBy(Mathf.PI * Utils.RandFloat(2));
                circular.y -= 0.2f;
                ParticleManager.NewParticle((Vector2)transform.position + circular, Utils.RandFloat(0.2f, 0.25f), circular * Utils.RandFloat(1.5f, 2) + new Vector2(0, Utils.RandFloat(-1, 2)),
                    1.6f, Utils.RandFloat(0.3f, 0.4f), 0, PopupColor.WithAlpha(0.4f) * 1.2f);
                HeartVisual.transform.localPosition = new Vector3(0, 0.1f * Mathf.Sin(++timer * Mathf.PI / 45f) - 0.1f);
            }
        }
    }
    public void OnCollected()
    {
        if(Heal > 0)
        {
            Player.Instance.SetLife(Player.Instance.Life + Heal);
            AudioManager.PlaySound(SoundID.PickupPower, transform.position, 1.1f, 0.76f, 0);
            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 0.9f, 0.4f, 0);
            PopupText.NewPopupText(transform.position + (Vector3)Utils.RandCircle(0.5f) + Vector3.forward, Utils.RandCircle(2) + Vector2.up * 4, PopupColor, $"+{Heal}");
            return;
        }
        CoinManager.ModifyCurrent(Value);
        PopupText.NewPopupText(transform.position + (Vector3)Utils.RandCircle(0.5f) + Vector3.forward, Utils.RandCircle(2) + Vector2.up * 4, PopupColor, $"${Value}");
        if (Value <= 1)
            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1, 0.85f, 0);
        else if (Value <= 5)
            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1, 0.85f, 1);
        else
            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1, 0.85f, 2);
    }
}
