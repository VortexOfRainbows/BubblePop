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
    protected int aiState = 0;
    protected Vector2 targetedPlayerPosition;
    protected int aimingTimer;
    const int baseAimingTimer = 400;
    protected int attackingTimer;
    const int baseAttackingTimer = 300;
    protected float moveSpeed = 3f;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Life = 5;
    }
    // Update is called once per frame
    void Update()
    {
        if (aiState == 0) {
            if (aimingTimer <= 0) {
                aimingTimer = baseAimingTimer;
                targetedPlayerPosition = FindTargetedPlayerPosition();
                aiState = 1;
            }
            else {
                aimingTimer--;
            }
        }

        if (aiState == 1) 
        {
            Vector2 toPlayer = targetedPlayerPosition - (Vector2)transform.position;
            rb.rotation = toPlayer.ToRotation() * Mathf.Rad2Deg;
            if (rb.rotation > 90 || rb.rotation < -90)
                rb.rotation -= 180;
            transform.position = Vector2.Lerp(transform.position, targetedPlayerPosition, 0.04f);
            if (attackingTimer <= 0) {
                attackingTimer = 12;
                aiState = 0;
            }
            else {
                attackingTimer--;
            }
        }
    }
    public void Kill()
    {
        GameObject.Instantiate(GlobalDefinitions.TinySoap, transform.position, Quaternion.identity).GetComponent<EnemySoapTiny>().sRender.sprite = Soap1;
        GameObject.Instantiate(GlobalDefinitions.TinySoap, transform.position, Quaternion.identity).GetComponent<EnemySoapTiny>().sRender.sprite = Soap2;
    }
    private Vector2 FindTargetedPlayerPosition() {
        float offsetX = Random.Range(-3f, 3f);
        float offsetY = Random.Range(-3f, 3f);
        return new Vector2 (Player.Position.x + offsetX, Player.Position.y + offsetY);
    }
}
