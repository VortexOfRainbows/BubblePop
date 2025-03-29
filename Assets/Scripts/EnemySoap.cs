using UnityEngine;
public class EnemySoap : Entity
{
    public SpriteRenderer sRender;
    public Rigidbody2D rb;
    public Sprite Soap1;
    public Sprite Soap2;
    //aiState 0 = aiming
    //aiState 1 = attacking
    protected Vector2 targetedPlayerPosition = Vector2.zero;
    protected float timer = 0;
    protected float moveSpeed = 3f;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Life = 5;
        PointWorth = 5;
    }
    // Update is called once per frame
    public override void AI()
    {
        timer++;
        if (timer > 120) 
        {
            timer = 0;
            targetedPlayerPosition = FindTargetedPlayerPosition();
        }
        if (timer > 50 && timer < 90)
        {
            if(targetedPlayerPosition == Vector2.zero)
                targetedPlayerPosition = FindTargetedPlayerPosition();
            Vector2 toPlayer = targetedPlayerPosition - (Vector2)transform.position;
            rb.rotation = toPlayer.ToRotation() * Mathf.Rad2Deg;
            if (rb.rotation > 90 || rb.rotation < -90)
                rb.rotation -= 180;
            if(timer == 51)
            {
                if(this is EnemySoapTiny)
                    AudioManager.PlaySound(SoundID.SoapSlide, transform.position, 1f, 1.2f);
                else
                    AudioManager.PlaySound(SoundID.SoapSlide, transform.position, 1f, 1f);
                rb.velocity *= 0.5f;
                rb.velocity += toPlayer.normalized * 6f;
            }
            Vector2 norm = rb.velocity.normalized;
            if(Random.Range(0, 2) == 0)
                ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(1) - norm * 1.5f, .3f, norm * Utils.RandFloat(5f, 15f), 1.5f, 0.6f, 1, new Color(1, 0.85f, 0.99f));
            else if (Random.Range(0, 3) != 0)
                ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(1) - norm * 1.5f, Utils.RandFloat(0.45f, 0.75f), norm * Utils.RandFloat(5f, 15f), 1.5f, Utils.RandFloat(0.5f, 0.7f), 0, new Color(1, 0.85f, 0.99f));
            rb.velocity += toPlayer.normalized * 0.5f;
        }
        if(timer > 90)
            rb.velocity *= 0.91f;
    }
    public override void OnKill()
    {
        AudioManager.PlaySound(SoundID.SoapDie, sRender.transform.position, 1f, 1f);
        DeathParticles(15, 0.5f, new Color(1, 0.85f, 0.99f));
        if(this is not EnemySoapTiny)
        {
            Instantiate(Main.TinySoap, transform.position, Quaternion.identity).GetComponent<EnemySoapTiny>().sRender.sprite = Soap1;
            Instantiate(Main.TinySoap, transform.position, Quaternion.identity).GetComponent<EnemySoapTiny>().sRender.sprite = Soap2;
        }
    }
    private Vector2 FindTargetedPlayerPosition() {
        float offsetX = Random.Range(-5f, 5f);
        float offsetY = Random.Range(-5f, 5f);
        return new Vector2 (Player.Position.x + offsetX, Player.Position.y + offsetY);
    }
}
