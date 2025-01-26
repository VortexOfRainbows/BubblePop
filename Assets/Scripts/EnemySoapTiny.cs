using UnityEngine;
public class EnemySoapTiny : MonoBehaviour
{
    //aiState 0 = aiming
    //aiState 1 = attacking
    private int aiState = 0;
    private Vector2 targetedPlayerPosition;
    private int aimingTimer;
    const int baseAimingTimer = 300;
    private int attackingTimer;
    const int baseAttackingTimer = 250;
    private float moveSpeed = 3f;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite[] soapPieceSprites;

    private void Awake()
    {
        spriteRenderer.sprite = soapPieceSprites[Random.Range(0, soapPieceSprites.Length)];
    }
    
    // Update is called once per frame
    private void Update()
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

        if (aiState == 1) {
            transform.position = Vector2.Lerp(transform.position, targetedPlayerPosition, moveSpeed * Time.deltaTime);
            if (attackingTimer <= 0) {
                attackingTimer = baseAttackingTimer;
                aiState = 0;
            }
            else {
                attackingTimer--;
            }
        }
    }

    private Vector2 FindTargetedPlayerPosition() {
        float offsetX = Random.Range(-4f, 4f);
        float offsetY = Random.Range(-4f, 4f);
        return new Vector2 (Player.Position.x + offsetX, Player.Position.y + offsetY);
    }
}
