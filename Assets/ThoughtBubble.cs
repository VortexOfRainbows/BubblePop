using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class ThoughtBubble : Body
{
    public GameObject TailPrefab;
    protected List<GameObject> Tails;
    protected float TailAddTimer = 0;
    protected int CurrentMarkedTail = -1;
    protected float TailTravelTimer = 0;
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
        powerPool.Add<Choice>();
        powerPool.Add<BubbleBirb>();
    }
    protected override string Description()
    {
        return "A mysterious Bubbletech Scientist. His origin is unknown; he appeared to Bubblemancer shortly after the first waves... why was he there?";
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
    public void TailUpdate(ref Vector2 playerVelo)
    {
        //p.TrailOfThoughts = 10;
        int maxTail = p.TrailOfThoughts * 3 + 15;
        if (Tails == null)
        {
            Tails = new List<GameObject>();
            for(int i = 0; i < maxTail; ++i)
            {
                TryAddingTail();
            }
        }
        if(Control.Ability && (!Control.LastAbility || CurrentMarkedTail != -1) && CurrentMarkedTail != Tails.Count - 1)
        {
            if(TailTravelTimer >= 4)
            {
                TailTravelTimer = 0;
                TravelDownTail();
            }
            if (CurrentMarkedTail >= Tails.Count)
            {
                CurrentMarkedTail = Tails.Count - 1;
            }
            TailTravelTimer++;
            playerVelo *= 0.25f;
        }
        else
        {
            if(Control.LastAbility && CurrentMarkedTail != -1) //This will only be true upon releasing the button
            {
                p.transform.position = Tails[CurrentMarkedTail].transform.position;
                for(int i = CurrentMarkedTail; i >= 0; --i)
                {
                    TryRemovingTail();
                }
                CurrentMarkedTail = -1;
            }
            else
            {
                if (Tails.Count < maxTail)
                {
                    TailAddTimer += Time.deltaTime * p.TrailOfThoughtsRecoverySpeed;
                    while (TailAddTimer > 0.5f)
                    {
                        TailAddTimer -= 0.5f;
                        TryAddingTail();
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
        float tailSeparation = 1.25f;
        GameObject Tail = Tails[i];
        Vector2 tailToBody = previousPos - Tail.transform.position;
        tailToBody = tailToBody.normalized * tailSeparation;

        float deg = tailToBody.ToRotation() * Mathf.Rad2Deg;
        if (deg > 360)
            deg %= 360;
        else if (deg < 0)
            deg += 360;
        Tail.GetComponent<SpriteRenderer>().flipY = deg > 90 && deg < 270;
        Tail.transform.eulerAngles = new Vector3(0, 0, deg);
        Vector2 targetPos = (Vector2)previousPos - tailToBody;
        Tail.transform.position = Vector2.Lerp(Tail.transform.position, targetPos, CurrentMarkedTail == -1 ? 0.75f : 0.2f);
        previousPos = Tail.transform.position;
        Tail.transform.localScale = Vector3.Lerp(Tails[i].transform.localScale, new Vector3(1.1f, 1.1f, 1.1f), 0.05f);
    }
    /// <summary>
    /// Optimizations to be made: Instead of removing trails, just mark them as deleted. This will make preserving positions and stuff easier, and it will make it more consistent
    /// </summary>
    public void TryAddingTail()
    {
        int i = Tails.Count;
        Vector2 circular = i > 0 ? new Vector2(-1, 0).RotatedBy(Tails[i - 1].transform.eulerAngles.z * Mathf.Deg2Rad) : new Vector2(0, 1);
        Vector3 spawnPos = i > 0 ? Tails[i - 1].transform.position : transform.position;
        Tails.Add(Instantiate(TailPrefab, (Vector2)spawnPos + circular, Quaternion.identity));
        Tails[i].transform.localScale = new Vector3(2f, 2f, 2f);
        UpdateTailPos(Tails.Count - 1, ref spawnPos);
    }
    public void TryRemovingTail()
    {
        if(Tails.Count > 0)
        {
            int end = Tails.Count - 1;
            Destroy(Tails[end].gameObject);
            Tails.RemoveAt(end);
        }
    }
    public void TravelDownTail()
    {
        CurrentMarkedTail++;
        GameObject current = Tails[CurrentMarkedTail];
        Projectile.NewProjectile<SmallBubble>(current.transform.position, Utils.RandCircle(1));
    }
}
