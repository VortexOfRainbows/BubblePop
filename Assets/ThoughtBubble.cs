using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngineInternal;

public class ThoughtBubble : Body
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ThoughtBubbleUnlock>();
    protected override UnlockCondition CategoryUnlockCondition => UnlockCondition.Get<ThoughtBubbleUnlock>();
    public const float TailRegenTime = 0.3f;
    public const float TailTravelTime = 4f;
    public GameObject TailPrefab;
    protected List<GameObject> Tails;
    protected float TailAddTimer = 0;
    protected int CurrentMarkedTail = -1;
    protected float TailTravelTimer = 0;
    public int StartTail = 0;
    public int TailCount = 0;
    public GameObject CurrentTail => Tails[(CurrentMarkedTail + StartTail) % Tails.Count];
    public override void Init()
    {
        PrimaryColor = new Color(1.00f, 1.05f, 1.1f);
        Tails = null;
    }
    public SpriteRenderer MouthR;
    protected override float AngleMultiplier => 0.1f;
    protected override float RotationSpeed => 1f;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Choice>();
        powerPool.Add<TrailOfThoughts>();
        powerPool.Add<BubbleBirb>();
    }
    protected override string Description()
    {
        return "A mysterious Bubbletech Scientist. His origin is unknown. He appeared before Bubblemancer shortly after the first waves... why was he there?";
    }
    public override void FaceUpdate()
    {
        Vector2 toMouse = Utils.MouseWorld - (Vector2)transform.position;
        toMouse *= Mathf.Sign(p.lastVelo.x);
        Vector2 toMouse2 = toMouse.normalized;
        toMouse2.x += Mathf.Sign(toMouse2.x) * 4;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.2f, 0).RotatedBy(toMouseR);
        looking.y *= 0.8f;
        if (looking.x < 0)
            toMouseR += Mathf.PI;
        Vector2 pos = new Vector2(0.1f, 0.16f * p.Direction) + looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouseR * Mathf.Rad2Deg);
        FaceR.flipX = !p.BodyR.flipY;
        MouthR.flipX = p.BodyR.flipY;
    }
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {
        TailUpdate(ref playerVelo);
    }
    protected override void DeathAnimation()
    {
        Vector2 empty = Vector2.zero;
        base.DeathAnimation();
        if(p.DeathKillTimer == 0)
        {
            for (int i = 0; i < TailCount; ++i)
            {
                int orig = (i + StartTail) % Tails.Count;
                DetonateTail(Tails[orig]);
            }
            for (int i = 0; i < Tails.Count; ++i)
                TryTurningOffTail();
        }
        TailUpdate(ref empty);
    }
    public void TailUpdate(ref Vector2 playerVelo)
    {
        //p.TrailOfThoughts = 10;
        int maxTail = p.TrailOfThoughts * 3 + 12;
        if (Tails == null)
        {
            Tails = new List<GameObject>();
        }
        while(Tails.Count < maxTail && TailCount == Tails.Count)
        {
            TryAddingTail();
        }
        if (Control.Ability && (!Control.LastAbility || CurrentMarkedTail != -1) && CurrentMarkedTail < TailCount - 1 && TailCount >= 3)
        {
            if(TailTravelTimer >= TailTravelTime)
            {
                TailTravelTimer = 0;
                TravelDownTail();
            }
            if (CurrentMarkedTail >= TailCount)
            {
                CurrentMarkedTail = TailCount - 1;
            }
            TailTravelTimer++;
            playerVelo *= 0.9f;
        }
        else
        {
            if(Control.LastAbility && CurrentMarkedTail != -1) //This will only be true upon releasing the button
            {
                p.transform.position = CurrentTail.transform.position;
                for(int i = CurrentMarkedTail; i >= 0; --i)
                {
                    TryTurningOffTail();
                }
                StartTail = (CurrentMarkedTail + StartTail) % Tails.Count;
                CurrentMarkedTail = -1;
            }
            else
            {
                if (TailCount < maxTail && p.DeathKillTimer <= 0 && playerVelo.magnitude > 1.5f)
                {
                    TailAddTimer += Time.deltaTime * p.TrailOfThoughtsRecoverySpeed;
                    while (TailAddTimer > TailRegenTime)
                    {
                        TailAddTimer -= TailRegenTime;
                        TryTurningOnTail();
                    }
                }
                else
                {
                    TailAddTimer = 0;
                }
            }
            TailTravelTimer = 20;
        }

        Vector3 previousPos = transform.position;
        for(int i = 0; i < Tails.Count; ++i)
        {
            UpdateTailPos(i, ref previousPos);
        }
    }
    public void UpdateTailPos(int i, ref Vector3 previousPos)
    {
        int orig = i;
        i = (i + StartTail) % Tails.Count;
        float tailSeparation = orig == 0 ? 1.5f : 1.25f;
        GameObject Tail = Tails[i];
        Vector2 tailToBody = previousPos - Tail.transform.position;
        tailSeparation = Mathf.Min(tailSeparation, tailToBody.magnitude);
        tailToBody = tailToBody.normalized * tailSeparation;

        float deg = tailToBody.ToRotation() * Mathf.Rad2Deg;
        if (deg > 360)
            deg %= 360;
        else if (deg < 0)
            deg += 360;
        if (orig < TailCount)
        {
            Tail.GetComponent<SpriteRenderer>().flipY = deg > 90 && deg < 270;
            Tail.transform.eulerAngles = new Vector3(0, 0, deg);
            Vector2 targetPos = (Vector2)previousPos - tailToBody;
            Tail.transform.position = Vector2.Lerp(Tail.transform.position, targetPos, CurrentMarkedTail == -1 ? 0.5f : 0.15f); //Only update position in here so the light position stays the same
            UpdateTailOn(orig, i);
        }
        else
        {
            UpdateTailOff(i);

        }
        previousPos = Tail.transform.position;
    }
    public void UpdateTailOn(int orig, int i)
    {
        int properIndex = orig % TailCount;
        float percent = 1f * properIndex / TailCount;
        Vector3 targetScale = new Vector3(1f, 1f) * (1f - 0.4f * percent);
        Light2D l2D = Tails[i].GetComponentInChildren<Light2D>();
        SpriteRenderer sr = Tails[i].GetComponent<SpriteRenderer>();
        sr.enabled = true;
        sr.color = new Color(1, 1, 1, 0.6f - 0.2f * percent);
        Tails[i].transform.localScale = Vector3.Lerp(Tails[i].transform.localScale, targetScale, 0.05f);
        if(CurrentMarkedTail >= 0 && CurrentTail == Tails[i])
        {
            l2D.intensity = Mathf.Lerp(l2D.intensity, 1.5f, 0.2f);
        }
        else
        {
            l2D.intensity = Mathf.Lerp(l2D.intensity, 0f, 0.1f);
            if (l2D.intensity <= 0.05f)
                l2D.intensity = 0;
        }
        l2D.pointLightOuterRadius = 1.0f + l2D.intensity;
    }
    public void UpdateTailOff(int i)
    {
        Light2D l2D = Tails[i].GetComponentInChildren<Light2D>();
        Tails[i].transform.localScale = new Vector3(0f, 0f, 0f);
        Tails[i].GetComponent<SpriteRenderer>().enabled = false;
        l2D.intensity = Mathf.Lerp(l2D.intensity, 0f, 0.1f);
        if (l2D.intensity <= 0.05f)
            l2D.intensity = 0;
    }
    public float sparkleSparkleNum = 0.0f;
    public void TravelDownTail()
    {
        CurrentMarkedTail++;
        GameObject current = CurrentTail;
        DetonateTail(current);
    }
    public void DetonateTail(GameObject current)
    {
        Projectile.NewProjectile<SmallBubble>(current.transform.position, Utils.RandCircle(1));
        if (p.DashSparkle > 0)
        {
            sparkleSparkleNum += (p.DashSparkle + 1) / 6f;
            while (sparkleSparkleNum > 1f)
            {
                Vector2 target = Utils.RandCircle(1).normalized * 20;
                Vector2 target2 = Utils.RandCircle(1).normalized * 10;
                Projectile.NewProjectile<StarProj>(current.transform.position, target2, current.transform.position.x + target.x, current.transform.position.y + target.y);
                sparkleSparkleNum -= 1.0f;
            }
        }
    }
    public void TryAddingTail()
    {
        int i = Tails.Count;
        Vector2 circular = i >= 12 ? new Vector2(-1, 0).RotatedBy(Tails[i - 1].transform.eulerAngles.z * Mathf.Deg2Rad) : new Vector2(0, 3 + .5f * i).RotatedBy(i * 33f * Mathf.Deg2Rad);
        Vector3 spawnPos = i >= 12 ? Tails[i - 1].transform.position : transform.position;
        Tails.Add(Instantiate(TailPrefab, (Vector2)spawnPos + circular, Quaternion.identity));
        Tails[i].transform.localScale = new Vector3(2f, 2f, 2f);
        UpdateTailPos(Tails.Count - 1, ref spawnPos);
    }
    public void TryRemovingTail()
    {
        if (TailCount > 0)
        {
            int end = Tails.Count - 1;
            Destroy(Tails[end].gameObject);
            Tails.RemoveAt(end);
            --TailCount;
        }
    }
    public void TryTurningOnTail()
    {
        if (TailCount < Tails.Count)
            ++TailCount;
    }
    public void TryTurningOffTail()
    {
        if (TailCount > 0)
            --TailCount;
    }
}
