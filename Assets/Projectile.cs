using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static GameObject NewProjectile(Vector2 pos, Vector2 velo)
    {
        GameObject Proj = Instantiate(GlobalPrefabs.Projectile, pos, Quaternion.identity);
        Proj.GetComponent<Rigidbody2D>().velocity = velo;
        return Proj;
    }
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private CircleCollider2D c2D;
    private float timer = 0f;
    void FixedUpdate()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.6f, 0.085f);
        
        Vector2 velo = rb.velocity;
        velo *= 0.995f - timer / 5000f;
        velo.y += 0.005f;
        rb.velocity = velo;

        if (rb.velocity.magnitude < 0.5f || timer > 200)
        {
            Destroy(gameObject);
        }
        if((int)timer % 3 == 0)
        {
            Vector2 norm = rb.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f, .25f, norm * -.75f, 0.6f, .3f);
        }
        timer++;
    }
}
