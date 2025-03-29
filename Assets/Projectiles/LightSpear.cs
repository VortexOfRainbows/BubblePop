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
        SpawnParticles();
    }
    public void SetSize()
    {
        Vector2 destination = new Vector2(Data1, Data2);
        Vector2 toDest = destination - (Vector2)transform.position;
        float scaleX = toDest.magnitude;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        transform.eulerAngles = new Vector3(0, 0, toDest.ToRotation() * Mathf.Rad2Deg);
    }
    public void SpawnParticles()
    {
        Vector2 destination = new Vector2(Data1, Data2);
        Vector2 toDest = destination - (Vector2)transform.position;
        float scaleX = toDest.magnitude;
        for(int j = 0; j < 12; ++j)
        {
            ParticleManager.NewParticle(transform.position, Utils.RandFloat(0.2f, 0.4f), RB.velocity * Utils.RandFloat(0.8f, 4.5f), 3.6f, Utils.RandFloat(0.45f, 0.6f), 2, SpriteRenderer.color);
        }
        for(float i = 0; i < scaleX; i += 0.33f)
        {
            Vector2 inBetween = Vector2.Lerp(transform.position, destination, i / scaleX);
            ParticleManager.NewParticle(inBetween, Utils.RandFloat(0.1f, 0.3f), RB.velocity * Utils.RandFloat(0.1f, 0.7f), 2, Utils.RandFloat(0.3f, 0.45f), 2, SpriteRenderer.color);
        }
        for (int j = 0; j < 20; ++j)
        {
            ParticleManager.NewParticle(destination, Utils.RandFloat(0.2f, 0.4f), RB.velocity * Utils.RandFloat(0.4f, 1.6f), 8f, Utils.RandFloat(0.45f, 0.6f), 2, SpriteRenderer.color);
        }
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
