using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Infector : Enemy
{
    public SpriteRenderer[] Shards;
    public SpriteRenderer[] Glows;
    public float AnimationTimer;
    public Transform Eye;
    public void UpdateShards(float lerp = 0.1f)
    {
        AnimationTimer++;
        int c = Shards.Length;
        float rad = Mathf.PI / c * 2f;
        for (int i = 0; i < c; ++i)
        {
            float rot = rad * i + AnimationTimer * Mathf.PI / 240f;
            float bobbing = rad * i + AnimationTimer * Mathf.PI / 54f;
            Vector3 circular = new Vector2(0.6f, 0).RotatedBy(rot);
            circular.y *= -0.225f;
            circular.z = Mathf.Sin(rot) * 0.4f;
            circular.y += Mathf.Sin(bobbing) * 0.04f;
            float scale = 1 - circular.z * 0.2f;
            Shards[i].transform.localPosition = Shards[i].transform.localPosition.Lerp(circular, lerp);
            Shards[i].transform.localEulerAngles = Mathf.LerpAngle(Shards[i].transform.localEulerAngles.z, circular.x * -30, lerp) * Vector3.forward;
            Shards[i].transform.LerpLocalScale(Vector2.one * scale * 0.9f, lerp);
            Glows[i].color = Glows[i].color.WithAlpha(Mathf.Lerp(Glows[i].color.a, 1f, 0.08f));
            //velocities[i] *= 1 - lerp;
        }
    }
    public void UpdateEye()
    {
        Vector2 toPlayer = Player.Position - (Vector2)transform.position;
        Vector2 norm = toPlayer.normalized;
        Eye.transform.localPosition = Eye.transform.localPosition.Lerp(norm * 0.16f, 0.1f);
    }
    public override void AI()
    {
        UpdateShards();
        UpdateEye();
    }
    public override void OnKill()
    {

    }
}
