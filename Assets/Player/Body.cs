using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : Equipment
{
    public GameObject Face;
    public SpriteRenderer FaceR;
    protected sealed override void AnimationUpdate()
    {
        float angleMult = 0.5f + (p.squash < 0.9f ? 0.5f : 0);
        if (p.lastVelo.sqrMagnitude > 0.10f)
        {
            float r = transform.eulerAngles.z;
            float angle = p.lastVelo.ToRotation() * Mathf.Rad2Deg;
            if (angle < 0)
                angle += 360;
            if (angle > 90 && angle <= 270)
            {
                angle = angle - 180;
                angle = 180 + angle * angleMult;
            }
            else
            {
                if (angle >= 270)
                    angle -= 360;
                angle *= angleMult;
            }
            r = Mathf.LerpAngle(r, angle, 0.12f);
            spriteRender.flipY = r >= 90 && r < 270;
            bool flip = !spriteRender.flipY;
            transform.eulerAngles = new Vector3(0, 0, r);
        }
        transform.localScale = new Vector3(1 + (1 - p.squash) * 2.5f + 0.1f * (1 - p.Bobbing), p.Bobbing * p.squash, 1);
        transform.localPosition = new Vector2(0, Mathf.Sign(p.lastVelo.x) * ((p.Bobbing * p.squash) - 1)).RotatedBy(p.lastVelo.ToRotation());
        FaceUpdate();
    }
    protected sealed override void DeathAnimation()
    {
        if (p.DeathKillTimer <= 0)
        {
            AudioManager.PlaySound(GlobalDefinitions.audioClips[23], transform.position, 0.21f, 0.4f);
            for (int i = 0; i < 100; i++)
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * i / 25f);
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1),
                    Utils.RandFloat(0.5f, 1.1f), circular * Utils.RandFloat(0, 24) + new Vector2(0, Utils.RandFloat(-2, 4)), 4f, Utils.RandFloat(1, 3));
            }
            gameObject.SetActive(false);
        }
    }
    public void FaceUpdate()
    {
        Vector2 toMouse = Utils.MouseWorld - (Vector2)transform.position;
        toMouse *= Mathf.Sign(p.lastVelo.x);
        Vector2 pos = new Vector2(0.15f, 0) + toMouse.normalized * 0.25f;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        FaceR.flipY = spriteRender.flipY;
    }
}
