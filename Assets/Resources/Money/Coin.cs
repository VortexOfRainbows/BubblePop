using Unity.VisualScripting;
using UnityEngine;
public class Coin : MonoBehaviour
{
    public bool IsCoin => YieldType == 0;
    public bool IsHeart => YieldType == 1;
    public bool IsKey => YieldType == 2;
    public bool IsToken => YieldType == 3;
    public bool IsGem => YieldType == 4;
    public Rigidbody2D rb;
    public int YieldType = 0;
    public int Value;
    public float AttractTimer = 0;
    public Color PopupColor;
    public float BeforeCollectableTimer = 0;
    public GameObject HeartVisual;
    public bool CanCollect()
    {
        if (IsToken)
            return Player.Instance.MaxTokens > CoinManager.CurrentTokens;
        if (IsHeart)
            return Player.Instance.Life < Player.Instance.TotalMaxLife;
        if (IsKey || IsGem)
            return Main.WavesUnleashed;
        return true;
    }
    public void TryCollecting()
    {
        float radius = IsHeart ? 0.35f : 0.7025f;
        radius *= transform.localScale.x;
        radius += Player.Instance.transform.localScale.x * 0.7f;
        if (transform.position.Distance(Player.Position) < radius)
        {
            if (!CanCollect())
                return;
            OnCollected();
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    //public void OnTriggerStay2D(Collider2D collision)
    //{
    //}
    private float timer;
    public void FixedUpdate()
    {
        TryCollecting();
        Player p = Player.Instance;
        if (IsCoin || IsToken)
            rb.rotation += rb.velocity.magnitude * 0.5f * -Mathf.Sign(rb.velocity.x);
        else if(IsGem)
            HeartVisual.transform.localEulerAngles = new Vector3(HeartVisual.transform.localEulerAngles.x, HeartVisual.transform.localEulerAngles.y, rb.velocity.x - 70);
        float attractDist = IsHeart || IsKey ? 3.5f : 4 + p.Magnet * 3f;
        if(IsToken)
            attractDist *= 3;
        Vector2 toPlayer = p.transform.position - transform.position;
        float length = toPlayer.magnitude;
        if (length < attractDist && (BeforeCollectableTimer <= 0 || IsHeart) && CanCollect())
        {
            float attractSpeed = 3 + p.Magnet + (++AttractTimer) / 30f;
            if (IsToken)
                attractSpeed *= 3;
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
            rb.velocity *= IsKey ? 0.9725f : IsHeart ? 0.935f : 0.985f;
            AttractTimer = 0;
        }
        BeforeCollectableTimer -= Time.fixedDeltaTime;

        if(IsToken)
        {
            transform.LerpLocalScale(Vector3.one * 0.9f, 0.1f);
            timer++;
            float percent = timer / 400f;
            float deathPercent = 0.75f;
            if(percent > deathPercent)
            {
                percent = (percent - deathPercent) / (1 - deathPercent);
                percent *= percent;
                var renderer = GetComponent<SpriteRenderer>();
                renderer.color = renderer.color.WithAlpha(1 - percent);
                if (percent >= 1)
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }
        else if (IsHeart)
        {
            if (Utils.RandFloat(1) < 0.2f)
            {
                Vector2 circular = new Vector2(Utils.RandFloat(0.3f, 0.5f), 0).RotatedBy(Mathf.PI * Utils.RandFloat(2));
                circular.y -= 0.2f;
                ParticleManager.NewParticle((Vector2)transform.position + circular, Utils.RandFloat(0.2f, 0.25f), circular * Utils.RandFloat(1.5f, 2) + new Vector2(0, Utils.RandFloat(-1, 2)),
                    1.6f, Utils.RandFloat(0.3f, 0.4f), 0, PopupColor.WithAlpha(0.4f) * 1.2f);
            }
            HeartVisual.transform.localPosition = new Vector3(0, 0.1f * Mathf.Sin(++timer * Mathf.PI / 225f) - 0.1f);
        }
        else if(IsKey || IsGem)
        {
            if(IsKey)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    if (Utils.RandFloat(1) < 0.6f)
                    {
                        Vector2 circular = new Vector2(1.15f * i, 0).RotatedBy(Mathf.PI * Time.fixedTime);
                        circular.y *= 0.85f;
                        circular.y -= 0.025f;
                        ParticleManager.NewParticle((Vector2)transform.position + circular, Utils.RandFloat(0.2f, 0.25f), circular * Utils.RandFloat(0.1f, 0.2f) + new Vector2(0, Utils.RandFloat(0, 2)),
                            1.6f, Utils.RandFloat(0.3f, 0.4f), ParticleManager.ID.Circle, PopupColor.WithAlpha(0.2f) * 1.25f);
                    }
                }
            }
            else
            {
                if (Utils.RandFloat(1) < 0.1f)
                {
                    Vector2 circular = new Vector2(1, 0).RotatedBy(Utils.RandFloat(-Mathf.PI, Mathf.PI));
                    circular.x *= 0.75f;
                    circular = circular.RotatedBy(Mathf.Deg2Rad * (-40 + rb.rotation));
                    circular.y += 0.05f;
                    ParticleManager.NewParticle((Vector2)HeartVisual.transform.position + circular * 0.8f, Utils.RandFloat(1f, 2f), circular * Utils.RandFloat(0.1f, 1f),
                        0.2f, Utils.RandFloat(0.3f, 0.4f), ParticleManager.ID.Pixel, PopupColor.WithAlpha(0.2f) * 1.25f);
                }
            }
            HeartVisual.transform.localPosition = new Vector3(0, 0.1f * Mathf.Sin(++timer * Mathf.PI / 200f) + 0.1f);
        }
    }
    public void OnCollected()
    {
        if(IsGem)
        {
            CoinManager.ModifyGems(Value);
            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1.1f, 0.5f, 2);
            PopupText.NewPopupText(transform.position + (Vector3)Utils.RandCircle(0.5f) + Vector3.forward, Utils.RandCircle(2) + Vector2.up * 4, PopupColor, $"+{Value}", true, 1f);
        }
        else if(IsToken)
        {
            CoinManager.ModifyTokens(Value);
            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1.1f, 0.5f, 1);
            PopupText.NewPopupText(transform.position + (Vector3)Utils.RandCircle(0.5f) + Vector3.forward, Utils.RandCircle(2) + Vector2.up * 4, PopupColor, $"Token", true, 0.8f, 90);
        }
        else if(IsKey)
        {
            CoinManager.ModifyKeys(Value);
            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1.2f, 0.25f, 2);
            PopupText.NewPopupText(transform.position + (Vector3)Utils.RandCircle(0.5f) + Vector3.forward, Utils.RandCircle(2) + Vector2.up * 4, PopupColor, $"+{Value}");
        }
        else if (IsHeart)
        {
            Player.Instance.SetLife(Player.Instance.Life + Value);
            AudioManager.PlaySound(SoundID.PickupPower, transform.position, 1.1f, 0.76f, 0);
            AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 0.9f, 0.4f, 0);
            PopupText.NewPopupText(transform.position + (Vector3)Utils.RandCircle(0.5f) + Vector3.forward, Utils.RandCircle(2) + Vector2.up * 4, PopupColor, $"+{Value}");
        }
        else
        {
            CoinManager.ModifyCoins(Value);
            PopupText.NewPopupText(transform.position + (Vector3)Utils.RandCircle(0.5f) + Vector3.forward, Utils.RandCircle(2) + Vector2.up * 4, PopupColor, $"${Value}");
            if (Value <= 1)
                AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1, 0.85f, 0);
            else if (Value <= 5)
                AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1, 0.85f, 1);
            else
                AudioManager.PlaySound(SoundID.CoinPickup, transform.position, 1, 0.85f, 2);
        }
    }
}
