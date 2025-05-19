using TMPro;
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
    public void Start()
    {
        inner.sprite = Sprite;
        Sprite adornmentSprite = MyPower.GetAdornment();
        if (adornmentSprite != null)
        {
            adornment.gameObject.SetActive(true);
            adornment.sprite = adornmentSprite;
        }
        else
            adornment.gameObject.SetActive(false);
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
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.1f);
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
        if(collision.tag == "Player" && !PickedUp)
        {
            if(CoinManager.Current >= Cost && transform.lossyScale.x > 0.8f)
            {
                PickUp();
            }
        }
    }
    public void PickUp()
    {
        CoinManager.ModifyCurrent(-Cost);
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
        //Debug.Log($"Player has picked up {MyPower.Name()}");
    }
}
