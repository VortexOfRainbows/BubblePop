using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.UIElements;
public class LightSpear : Projectile
{
    public override void Init()
    {
        transform.localScale = Vector3.one * 0.5f;
        SpriteRenderer.color = new Color(1, 1, .9f, 0.5f);
        SpriteRendererGlow.transform.localPosition = new Vector3(0.2f, 0, 0);
        SpriteRendererGlow.transform.localScale = new Vector3(0.4f, 3f, 3f);
        SpriteRendererGlow.color = new Color(1.2f, 1.2f, 1.2f);
        SpriteRenderer.sprite = Resources.Load<Sprite>("Projectiles/LaserSquare");
        Damage = 1;
        Friendly = true;
        Hostile = false;
        cmp.c2D.offset = new Vector2(1, 0);
        cmp.c2D.radius = 0.02f;
        transform.localScale = new Vector3(1, 0.1f, transform.localScale.z);
        immunityFrames = 5;
        Penetrate = -1;
        SetSize();
    }
    public void SetSize()
    {
        Vector2 destination = new Vector2(Data1, Data2);
        Vector2 toDest = destination - (Vector2)transform.position;
        float scaleX = toDest.magnitude;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        transform.eulerAngles = new Vector3(0, 0, toDest.ToRotation() * Mathf.Rad2Deg);
    }
    public override void AI()
    {
        ++timer;
        if(timer <= 7)
        {
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 0.4f, 0.2f), 1);
            SpriteRendererGlow.transform.localPosition = new Vector3(0.2f + 0.6f * timer / 7f, 0, 0);
        }
        else
        {
            SpriteRendererGlow.enabled = false;
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 0f, 0.1f), 1);
            if (transform.lossyScale.y < 0.01f)
                timer = 101;
            if (timer > 14)
                Friendly = false; //turn off hitbox after a bit
        }
        //RB.position -= RB.velocity * Time.fixedDeltaTime * 0.9f; //basically this should not be very effected by velocity
        if (timer > 100)
        {
            Kill();
            return;
        }
        SetSize();
    }
}
