using System.Collections.Generic;
using UnityEngine;

public class Crown : Bulb
{
    public override void Init()
    {
        velocities = new Vector2[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
        UpdateShards(1);
    }
    public GameObject[] Shards;
    public SpriteRenderer[] Glows;
    private Vector2[] velocities;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ThoughtBubbleWave15NoAttack>();
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        scale *= 1.4f;
        offset.y = -0.5f;
        offset.x = 0f;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        base.ModifyPowerPool(powerPool);
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Crown of Command").WithDescription("Choice powers have 2 more options");
    }
    public override void EquipUpdate()
    {
        player.BonusChoices = true;
    }
    private float AnimationTimer = 0;
    protected override void AnimationUpdate()
    {
        float r = p.MoveDashRotation();
        spriteRender.flipX = !p.Body.Flipped;
        spriteRender.sprite = OnBulb;
        light2d.color = light2d.color.WithAlpha(Mathf.Lerp(light2d.color.a, 1f, 0.08f));
        light2d.gameObject.SetActive(true);
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, r, 0.05f));
        transform.localPosition = Vector2.Lerp((Vector2)transform.localPosition,
            new Vector2(0, 0.5f + 0.5f * p.Bobbing * p.squash - 0.2f * (1 - p.squash)).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad) + velocity,
            0.25f);
        bounceCount = 0.7f;
        UpdateShards();
        velocity *= 0.5f;
    }
    public void UpdateShards(float lerp = 0.1f)
    {
        AnimationTimer++;
        int c = Shards.Length;
        float rad = Mathf.PI / c * 2f;
        for (int i = 0; i < c; ++i)
        {
            float rot = rad * i + AnimationTimer * Mathf.PI / 240f;
            float bobbing = rad * i + AnimationTimer * Mathf.PI / 54f;
            Vector3 circular = new Vector2(1, 0).RotatedBy(rot);
            circular.y *= 0.35f;
            circular.z = Mathf.Sin(rot) * 0.4f;
            circular.y += Mathf.Sin(bobbing) * 0.04f;
            float scale = 1 - circular.z * 0.2f;
            Shards[i].transform.localPosition = Shards[i].transform.localPosition.Lerp(circular, lerp);
            Shards[i].transform.localEulerAngles = Mathf.LerpAngle(Shards[i].transform.localEulerAngles.z, circular.x * -15, lerp) * Vector3.forward;
            Shards[i].transform.LerpLocalScale(Vector2.one * scale, lerp);
            Glows[i].color = Glows[i].color.WithAlpha(Mathf.Lerp(Glows[i].color.a, 1f, 0.08f));
            velocities[i] *= 1 - lerp;
        }
    }
    protected override void DeathAnimation()
    {
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, 0, 0.03f));
        int c = Shards.Length;
        for (int i = 0; i < c; ++i)
        {
            Transform t = Shards[i].transform;
            float z = t.localPosition.z;
            float toBody = t.localPosition.y - p.Body.transform.localPosition.y;
            if (p.DeathKillTimer <= 0)
            {
                velocities[i] *= 0.0f;
                velocities[i].y += 0.03f * Utils.RandFloat(1f, 2f);
                velocities[i].x += 0.05f * p.Direction * Utils.RandFloat(-0.35f, 1.85f);
            }
            if (toBody < -1.5f)
            {
                velocities[i] *= -bounceCount;
                velocities[i] += Utils.RandCircle(0.05f) * Mathf.Abs(bounceCount);
                t.localPosition = (Vector2)t.localPosition + new Vector2(0, -1.5f - toBody);
                bounceCount *= 0.95f;
            }
            else
            {
                velocities[i].x *= 0.998f;
                velocities[i].y -= 0.005f;
            }
            Glows[i].color = Glows[i].color.WithAlpha(Mathf.Lerp(Glows[i].color.a, 0, 0.01f));
            t.localPosition = (Vector2)t.localPosition + velocities[i];
            t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, z);
        }
    }
}
