using System.Collections.Generic;
using UnityEngine;

public class Gachapon : Body
{
    public List<ChipStack> stacks;
    public GameObject ChipPrefab;
    public GameObject ChipStackPrefab;
    public void AddChip()
    {
        List<int> possibleStacks = new List<int>();
        for (int i = 0; i < stacks.Count; ++i)
            possibleStacks.Add(i);
        int rand = Utils.RandInt(possibleStacks.Count);
        int index = possibleStacks[rand];
        while (stacks[index].IsFull())
        {
            possibleStacks.RemoveAt(rand);
            if (possibleStacks.Count <= 0)
                return;
            rand = Utils.RandInt(possibleStacks.Count);
            index = possibleStacks[rand];
        }
        int direction = index % 2 * 2 - 1;
        ChipStack stack = stacks[index];
        stack.Chips.Add(Instantiate(ChipPrefab, stack.Transform));
        int total = stacks[index].Chips.Count - 1;
        stack.Chips[total].transform.localPosition = new Vector3(total % 2 * -0.1f * direction, total * 0.15f);
    }
    public bool RemoveChip()
    {
        int totalCount = 0;
        foreach(ChipStack stack in stacks)
        {
            if(stack.Chips.Count > 0)
            {
                int top = stack.Chips.Count - 1;
                GameObject topChip = stack.Chips[top];
                Destroy(topChip);
                totalCount++;
                stack.Chips.RemoveAt(top);
            }
        }
        Vector2 toMouse = Utils.MouseWorld - (Vector2)transform.position;
        toMouse = toMouse.normalized * 14;
        float rotation = 12.5f * (totalCount - 1);
        float max = 180 * (1f - 1f / totalCount);
        if (rotation > max)
            rotation = max;
        for (int i = 0; i < totalCount; ++i)
        {
            float lerp = totalCount == 1 ? 0 : (float)i / (totalCount - 1);
            Vector2 rot = toMouse.RotatedBy(Mathf.Deg2Rad * Mathf.Lerp(-rotation, rotation, lerp));
            Projectile.NewProjectile<PokerChip>(transform.position,rot);
            for(int j = 0; j < player.DashSparkle; ++j)
            {
                Vector2 target = (Vector2)transform.position + rot * Utils.RandFloat(0.2f, 1.0f);
                Projectile.NewProjectile<StarProj>(transform.position, rot + Utils.RandCircle(5), target.x + Utils.RandFloat(-5, 5), target.y + Utils.RandFloat(-5, 5));
            }
        }
        return totalCount > 0;
    }
    public void AddStack()
    {
        int total = stacks.Count / 2 + 1;
        int direction = stacks.Count % 2 * 2 - 1;
        stacks.Add(Instantiate(ChipStackPrefab, transform).GetComponent<ChipStack>());
        float totalOffset = (0.575f + 0.525f * total) * direction;
        stacks[stacks.Count - 1].transform.localPosition = new Vector3(totalOffset, -0.78f);
    }
    public void RemoveStack()
    {
        ChipStack stack = stacks[stacks.Count - 1];
        foreach(GameObject chip in stack.Chips)
        {
            Destroy(chip);
        }
        stacks.RemoveAt(stacks.Count - 1);
    }
    public Sprite[] altFaces;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<GachaponUnlock>();
    protected override UnlockCondition CategoryUnlockCondition => UnlockCondition.Get<GachaponUnlock>();
    public override void Init()
    {
        PrimaryColor = new Color(0.95f, 1f, 0.6f);
        stacks = new List<ChipStack>();
    }
    //public SpriteRenderer MouthR;
    protected override float AngleMultiplier => 0.4f;
    protected override float RotationSpeed => 1f;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Choice>();
        powerPool.Add<BubbleBirb>();
        powerPool.Add<Overclock>();
        powerPool.Add<Raise>();
    }
    protected override string Description()
    {
        return "A greedy shopkeeper? No! I'm the most honest gal around!";
    }
    public GameObject top;
    public override void FaceUpdate()
    {
        Vector2 toMouse = p.LookPosition - (Vector2)transform.position;
        Vector2 toMouse2 = toMouse.normalized;
        if (p.IsMainPlayerAnimator)
        {
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && Player.Instance.Weapon.IsAttacking())
                FaceR.sprite = altFaces[1];
            else
                FaceR.sprite = altFaces[0];
        }
        else 
        {
            if (toMouse.magnitude < 5)
                FaceR.sprite = altFaces[1];
            else
                FaceR.sprite = altFaces[0];
        }
        toMouse2.x += Mathf.Sign(toMouse2.x) * 4;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.16f, 0).RotatedBy(toMouseR);
        looking.y *= 0.8f;
        if (looking.x < 0)
            toMouseR += Mathf.PI;
        Vector2 pos = new Vector2(0.08f * p.Direction, 0.3f) + looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouseR * Mathf.Rad2Deg);
        FaceR.flipX = toMouse.x > 0;
        FaceR.enabled = true;
        top.SetActive(true);
        //MouthR.flipX = FaceR.flipX;
    }
    public override float AbilityCD => 2.5f;
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {
        PrimaryColor = new Color(0.95f, 1f, 0.6f);
        if(p.Accessory is Crystal)
        {
            PrimaryColor = new Color(0.6f, 0.933f, 0.255f);
        }
        while(stacks.Count < player.TotalChipStacks)
            AddStack();
        while (stacks.Count > player.TotalChipStacks)
            RemoveStack();
        if (Main.GameStarted)
        {
            if(player.AbilityReady)
            {
                player.abilityTimer = AbilityCD;
                AddChip();
            }
        }
        if(Control.Ability && !Control.LastAbility)
        {
            if (RemoveChip())
                player.abilityTimer = AbilityCD;
        }
        else if(Control.Ability)
        {
            while(player.abilityTimer < AbilityCD * 0.9f)
            {
                if (RemoveChip())
                    player.abilityTimer += AbilityCD * 0.15f * ((player.AbilityRecoverySpeed - 1) * 0.25f + Mathf.Sqrt(player.AbilityRecoverySpeed));
                else
                    break;
            }
        }
        //if (p.AbilityReady && Control.Ability && !Control.LastAbility)
        //{
        //    p.abilityTimer = p.AbilityCD * 2;
        //    p.Visual.SetActive(false);
        //}
        //if(p.AbilityOnCooldown)
        //{
        //    if (Control.Ability && !p.Visual.activeSelf)
        //    {
        //        p.Visual.SetActive(false);
        //        ParticleManager.NewParticle(transform.position, 1, playerVelo * Utils.RandFloat(-1f, 0.5f), 1, 0.5f, 2, PrimaryColor);
        //    }
        //    else
        //    {
        //        p.Visual.SetActive(true);
        //    }
        //}
        //else
        //{
        //    p.Visual.SetActive(true);
        //}
    }
    public override void ModifyDeathAnimation()
    {
        FaceR.enabled = false;
        top.SetActive(false);
        spriteRender.transform.localEulerAngles = Vector3.forward * Mathf.LerpAngle(spriteRender.transform.localEulerAngles.z, flipDir == -1 ? 180 : 0, 0.1f);
    }
}
