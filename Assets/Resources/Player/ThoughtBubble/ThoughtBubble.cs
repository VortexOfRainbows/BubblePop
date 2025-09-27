using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngineInternal;

public class ThoughtBubble : Body
{
    public override void ModifyLifeStats(ref int MaxLife, ref int Life, ref int MaxShield, ref int Shield)
    {
        MaxLife += 2;
        Life += 2;
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ThoughtBubbleUnlock>();
    public const float TailRegenTime = 0.3f;
    public const float TailTravelTime = 4f;
    public GameObject TailPrefab;
    protected List<GameObject> Tails;
    protected float TailAddTimer = 0;
    protected int CurrentMarkedTail = -1;
    protected float TailTravelTimer = 0;
    protected int StartTail = 0;
    public int TailCount = 0;
    public int LastTail => StartTail == 0 ? Tails.Count - 1 : StartTail - 1;
    public GameObject CurrentTail => Tails[(CurrentMarkedTail + StartTail) % Tails.Count];
    public override void Init()
    {
        PrimaryColor = new Color(1.00f, 1.05f, 1.1f);
        Tails = null;
    }
    protected override float AngleMultiplier => 0.1f;
    protected override float RotationSpeed => 1f;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Choice>();
        powerPool.Add<TrailOfThoughts>();
        powerPool.Add<BubbleBirb>();
        powerPool.Add<Overclock>();
        powerPool.Add<BrainBlast>();
        powerPool.Add<BubbleMitosis>();
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Thought Bubble").WithDescription("A mysterious Bubbletech Scientist. His origin is unknown. He appeared before Bubblemancer shortly after the first waves... why was he there?");
    }
    public override void FaceUpdate()
    {
        Vector2 toMouse = p.LookPosition - (Vector2)transform.position;
        Vector2 toMouse2 = toMouse.normalized;
        toMouse2.x += Mathf.Sign(toMouse2.x) * 4;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.18f, 0).RotatedBy(toMouseR);
        looking.y *= 0.8f;
        if (looking.x < 0)
            toMouseR += Mathf.PI;
        Vector2 pos = new Vector2(0.16f * p.Direction, 0.02f) + looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouseR * Mathf.Rad2Deg);
        FaceR.flipX = toMouse.x > 0;
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
                DetonateTail(Tails[orig], true);
            }
            for (int i = 0; i < Tails.Count; ++i)
                TryTurningOffTail();
        }
        TailUpdate(ref empty);
    }
    public void TailUpdate(ref Vector2 playerVelo)
    {
        //p.TrailOfThoughts = 10;
        int maxTail = player.TrailOfThoughts * 3 + 12;
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
            if(Control.Ability && !Control.LastAbility)
            {
                FinishedTeleport = false;
                AudioManager.PlaySound(SoundID.TeleportCharge, transform.position, 1f, 1);
            }
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
                Teleport(CurrentTail.transform.position);
                for(int i = CurrentMarkedTail; i >= 0; --i)
                {
                    TryTurningOffTail();
                }
                StartTail = (CurrentMarkedTail + StartTail + 1) % Tails.Count;
                CurrentMarkedTail = -1;
            }
            else
            {
                if (TailCount < maxTail && p.DeathKillTimer <= 0 && playerVelo.magnitude > 1.5f)
                {
                    TailAddTimer += Time.deltaTime * player.TrailOfThoughtsRecoverySpeed;
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
    public static bool FinishedTeleport = false;
    public void Teleport(Vector2 pos)
    {
        player.OnUseAbility();
        player.transform.position = pos;
        AudioManager.PlaySound(SoundID.Teleport, pos, 2f, 1);
        FinishedTeleport = true;
        int amt = (int)BrainBlastNum;
        while (BrainBlastNum >= 1)
        {
            float speed = 5f + Player.Instance.FasterBulletSpeed * 2.5f + player.BrainBlast * Utils.RandFloat(1.5f, 2.5f);
            Projectile.NewProjectile<SmallBubble>(transform.position, new Vector2(speed * Mathf.Sqrt(Utils.RandFloat(0.2f, 1.5f)), 0).RotatedBy((BrainBlastNum + Utils.RandFloat(1)) / (int)amt * Mathf.PI * 2f), 1); 
            --BrainBlastNum;
        }
        ThunderAuraNum = 0;
        if (player.UniversalImmuneFrames < 7)
            player.UniversalImmuneFrames = 7;
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
        SpriteRenderer sr = Tails[i].GetComponent<SpriteRenderer>();
        sr.enabled = true;
        sr.color = new Color(1, 1, 1, 0.6f - 0.2f * percent);
        Tails[i].transform.localScale = Vector3.Lerp(Tails[i].transform.localScale, targetScale, 0.05f);
        //if(CurrentMarkedTail >= 0 && CurrentTail == Tails[i])
        //{
        //}
        //else
        //{
        //}
    }
    public void UpdateTailOff(int i)
    {
        Tails[i].transform.localScale = new Vector3(0f, 0f, 0f);
        Tails[i].GetComponent<SpriteRenderer>().enabled = false;
    }
    public float sparkleSparkleNum = 0.0f;
    public void TravelDownTail()
    {
        CurrentMarkedTail++;
        GameObject current = CurrentTail;
        DetonateTail(current);
    }
    public float BrainBlastNum = 0;
    public float BrainBlastTrailNum = 0;
    public int ThunderAuraNum = 0;
    public void DetonateTail(GameObject current, bool death = false)
    {
        for(int i = 0; i < 5; ++i)
        {
            ParticleManager.NewParticle(current.transform.position, Utils.RandFloat(0.4f, 0.5f), Vector2.zero, 4.5f, Utils.RandFloat(0.4f, 0.6f), 2, new Color(1, 0.8377f, 0f));
        }
        int amt = 1 + (int)BrainBlastTrailNum;
        for(int i = 0; i < amt; ++i)
        {
            Projectile.NewProjectile<SmallBubble>(current.transform.position, UnityEngine.Random.insideUnitCircle * Utils.RandFloat(0.1f + i * 0.05f, 1.0f + i * 0.2f), 1);
            if (i > 0)
                BrainBlastTrailNum--;
        }
        if(!death)
            Projectile.NewProjectile<ThoughtBubbleThunderAura>(current.transform.position, Vector2.zero, 1.0f * (1 + player.BrainBlast * 0.2f), ++ThunderAuraNum % 3);
        if (player.BrainBlast > 0)
        {
            BrainBlastNum += 0.5f + player.BrainBlast * 0.5f;
            BrainBlastTrailNum += 0.1f + player.BrainBlast * 0.1f;
        }
        if (player.DashSparkle > 0)
        {
            sparkleSparkleNum += (player.DashSparkle + 1) / 6f; //33% + 16.67% per stack
            while (sparkleSparkleNum > 1f)
            {
                Vector2 target = Utils.RandCircle(1).normalized * 20;
                Vector2 target2 = Utils.RandCircle(1).normalized * (14 + 0.5f * player.FasterBulletSpeed);
                Projectile.NewProjectile<StarProj>(current.transform.position, target2, 2, current.transform.position.x + target.x, current.transform.position.y + target.y, Utils.RandInt(2) * 2 - 1);
                sparkleSparkleNum -= 1.0f;
            }
        }
        player.OnUseAbility();
    }
    public void TryAddingTail()
    {
        int i = Tails.Count;
        Vector3 spawnPos = LastTail >= 0 ? Tails[LastTail].transform.position : transform.position;
        //Debug.Log(LastTail);
        if(LastTail == Tails.Count - 1)
        {
            Tails.Add(Instantiate(TailPrefab, (Vector2)spawnPos, Quaternion.identity));
        }
        else
        {
            Tails.Insert(LastTail, Instantiate(TailPrefab, (Vector2)spawnPos, Quaternion.identity));
            ++StartTail;
        }
        Tails[LastTail].transform.localScale = new Vector3(0f, 0f, 0f);
        //UpdateTailPos(LastTail, ref spawnPos);
        //Debug.Log(StartTail);
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
    private void OnDestroy()
    {
        if (Tails == null)
            return;
        while (Tails.Count > 0)
        {
            int end = Tails.Count - 1;
            Destroy(Tails[end].gameObject);
            Tails.RemoveAt(end);
        }
    }
}
