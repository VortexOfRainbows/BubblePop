using UnityEngine;

public class PowerUpObject : MonoBehaviour
{
    public SpriteRenderer outer;
    public SpriteRenderer inner;
    public SpriteRenderer glow;
    public int Type;
    public PowerUp MyPower => PowerUp.Get(Type);
    public Sprite Sprite => MyPower.sprite;
    private int timer;
    private bool PickedUp = false;
    public void Start()
    {
        inner.sprite = Sprite;
    }
    public void FixedUpdate()
    {
        timer++;
        float scale = 1.0f + 0.1f * Mathf.Sin(Mathf.Deg2Rad * timer * 2f);
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2f / scale, 2f * scale, 2), 0.1f);
        if (Utils.RandFloat(1) < 0.4f)
        {
            Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * Utils.RandFloat(2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.5f, 0.6f), circular * Utils.RandFloat(3, 6) + new Vector2(0, Utils.RandFloat(-1, 2)),
                2f, Utils.RandFloat(0.3f, .5f), 0, glow.color * 0.9f);
        }
    }
    public void OnTriggerStay2D(Collider2D collision) => OnTrigger(collision);
    public void OnTriggerEnter2D(Collider2D collision) => OnTrigger(collision);
    private void OnTrigger(Collider2D collision)
    {
        if(collision.tag == "Player" && !PickedUp)
        {
            Kill();
            PickedUp = true;
        }
    }
    private void Kill()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 circular = new Vector2(.5f, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.6f, 0.7f), circular * Utils.RandFloat(3, 6), 4f, Utils.RandFloat(0.4f, 0.6f), 0, glow.color);
        }
        AudioManager.PlaySound(GlobalDefinitions.audioClips[37], transform.position, 1.2f, 0.9f);

        MyPower.PickUp();
        //Debug.Log($"Player has picked up {MyPower.Name()}");
        Destroy(gameObject);
    }
}
