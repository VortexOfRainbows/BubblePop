using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpObject : MonoBehaviour
{
    public SpriteRenderer outer;
    public SpriteRenderer adornment;
    public SpriteRenderer inner;
    public SpriteRenderer glow;
    public int Type;
    public int Cost;
    public GameObject CostObj;
    public TextMeshPro CostText;
    public PowerUp MyPower => PowerUp.Get(Type);
    public Sprite Sprite => MyPower.sprite;
    private int timer;
    private bool PickedUp = false;

    public float VeloEndTimer = 0.0f;
    public Vector2 velocity = Vector2.zero;
    public Vector2 finalPosition;
    public void Start()
    {
        inner.sprite = Sprite;
        Sprite adornmentSprite = MyPower.GetAdornment();
        if (adornmentSprite != null)
        {
            adornment.gameObject.SetActive(true);
            adornment.sprite = adornmentSprite;
            inner.material = MyPower.GetBorder();
            outer.material = adornment.material = MyPower.GetBorder(true);
        }
        else
        {
            inner.material = MyPower.GetBorder();
            outer.material = MyPower.GetBorder(true);
            adornment.gameObject.SetActive(false);
        }
        adornment = null;
        MyPower.AliveUpdate(inner.gameObject, outer.gameObject, false);

    }
    public void FixedUpdate()
    {
        if (inner.sprite == null)
            inner.sprite = Sprite;
        MyPower.AliveUpdate(inner.gameObject, outer.gameObject, false);
        timer++;
        float scale = 1.0f + 0.1f * Mathf.Sin(Mathf.Deg2Rad * timer * 2f);
        if (velocity != Vector2.zero)
        {
            VeloEndTimer += Time.fixedDeltaTime;
            if (VeloEndTimer > 1)
                VeloEndTimer = 1;
            transform.position += (Vector3)velocity * Time.fixedDeltaTime;
            transform.position = transform.position.Lerp(finalPosition, VeloEndTimer);
            transform.localScale = Vector3.Lerp(transform.localScale, (0.25f + 0.75f * Mathf.Sqrt(Mathf.Min(1, VeloEndTimer * 2f))) * Vector3.one, 0.1f);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.1f);
        }
        outer.transform.localScale = Vector3.Lerp(outer.transform.localScale, new Vector3(2f / scale, 2f * scale, 2), 0.1f);
        if (Utils.RandFloat(1) < 0.4f)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(0, 1) * transform.localScale.x, 0).RotatedBy(Mathf.PI * Utils.RandFloat(2));
            ParticleManager.NewParticle((Vector2)transform.position + circular, Utils.RandFloat(0.5f, 0.6f), circular * Utils.RandFloat(3, 6) + new Vector2(0, Utils.RandFloat(-1, 2)),
                1.6f, Utils.RandFloat(0.3f, 0.4f), 0, glow.color * 0.8f);
        }
        if(Cost > 0)
        {
            CostObj.SetActive(true);
            CostText.text = $"${Cost}";
        }
        else
        {
            CostObj.SetActive(false);
        }
    }
    public void OnTriggerStay2D(Collider2D collision) => OnTrigger(collision);
    public void OnTriggerEnter2D(Collider2D collision) => OnTrigger(collision);
    private void OnTrigger(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !PickedUp)
        {
            if(CoinManager.Current >= Cost && transform.lossyScale.x > 0.8f && (VeloEndTimer == 0 || VeloEndTimer >= 0.9f))
            {
                PickUp();
            }
        }
    }
    public void PickUp()
    {
        if(Cost > 0)
        {
            CoinManager.ModifyCurrent(-Cost);
            int charisma = Player.Instance.RollChar;
            if(charisma > 0)
            {
                if (charisma >= 81 || Utils.RandFloat(1) < 0.19f + charisma * 0.01f)
                {
                    if(Player.Instance.Life < Player.Instance.MaxLife)
                        CoinManager.SpawnHeart(transform.position, 0.2f);
                    else
                        CoinManager.SpawnCoin(transform.position, charisma * 25, 0.5f);
                }
            }
        }
        PickedUp = true;
        MyPower.PickUp();
        Kill();
    }
    private void Kill()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 circular = new Vector2(.5f, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.6f, 0.7f), circular * Utils.RandFloat(3, 6), 4f, Utils.RandFloat(0.4f, 0.6f), 0, glow.color);
        }
        AudioManager.PlaySound(SoundID.PickupPower, transform.position, 1.2f, 0.9f);
        Destroy(gameObject);
    }
}
