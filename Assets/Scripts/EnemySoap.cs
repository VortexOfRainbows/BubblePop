using System.IO.Pipes;
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
    void FixedUpdate()
    {
        IFrame--;
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
                rb.velocity *= 0.5f;
                rb.velocity += toPlayer.normalized * 6f;
            }
            rb.velocity += toPlayer.normalized * 0.5f;
        }
        if(timer > 90)
            rb.velocity *= 0.91f;
    }
    public void Kill()
    {
        GameObject.Instantiate(GlobalDefinitions.TinySoap, transform.position, Quaternion.identity).GetComponent<EnemySoapTiny>().sRender.sprite = Soap1;
        GameObject.Instantiate(GlobalDefinitions.TinySoap, transform.position, Quaternion.identity).GetComponent<EnemySoapTiny>().sRender.sprite = Soap2;
    }
    private Vector2 FindTargetedPlayerPosition() {
        float offsetX = Random.Range(-5f, 5f);
        float offsetY = Random.Range(-5f, 5f);
        return new Vector2 (Player.Position.x + offsetX, Player.Position.y + offsetY);
    }
}
