using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Gachapon : Body
{
    public static Sprite BlueChip => bChip == null ? bChip = Resources.Load<Sprite>("Projectiles/BlueChip") : bChip;
    private static Sprite bChip = null;
    public List<ChipStack> stacks;
    private List<GameObject> notYetResizedCips = new();
    public GameObject ChipPrefab;
    public GameObject ChipStackPrefab;
    public void AddChip()
    {
        List<int> possibleStacks = new List<int>();
        int stacksWithAppropriateChips = 2;
        for (int i = 0; i < stacks.Count; ++i)
        {
            if (stacks[i].Chips.Count >= 3)
                stacksWithAppropriateChips++;
            possibleStacks.Add(i);
        }
        if (stacksWithAppropriateChips > possibleStacks.Count)
            stacksWithAppropriateChips = possibleStacks.Count;
        stacksWithAppropriateChips = stacksWithAppropriateChips / 2 * 2;
        int rand = Utils.RandInt(stacksWithAppropriateChips);
        int index = possibleStacks[rand];
        while (stacks[index].IsFull())
        {
            --stacksWithAppropriateChips;
            possibleStacks.RemoveAt(rand);
            if (possibleStacks.Count <= 0)
                return;
            rand = Utils.RandInt(stacksWithAppropriateChips);
            index = possibleStacks[rand];
        }
        int direction = index % 2 * 2 - 1;
        ChipStack stack = stacks[index];
        SpriteRenderer r = Instantiate(ChipPrefab, stack.Transform).GetComponent<SpriteRenderer>();
        if (Utils.RandFloat() < player.BlueChipChance){
            r.sprite = BlueChip;
        }
        r.gameObject.transform.localScale = Vector3.zero;
        notYetResizedCips.Add(r.gameObject);
        stack.Chips.Add(r.gameObject);
        int total = stacks[index].Chips.Count - 1;
        stack.Chips[total].transform.localPosition = new Vector3(total % 2 * -0.1f * direction, total * 0.15f, total * -0.025f);
    }
    public bool RemoveChip()
    {
        int totalCount = 0;
        int blueCount = 0;
        foreach(ChipStack stack in stacks)
        {
            if(stack.Chips.Count > 0)
            {
                int top = stack.Chips.Count - 1;
                GameObject topChip = stack.Chips[top];
                if (topChip.GetComponent<SpriteRenderer>().sprite == BlueChip)
                    blueCount++;
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
        int half = totalCount / 2;
        int bHalf = blueCount / 2;
        for (int i = 0; i < totalCount; ++i)
        {
            bool blue = i >= half - bHalf && i <= half + bHalf;
            float lerp = totalCount == 1 ? 0 : (float)i / (totalCount - 1);
            Vector2 rot = toMouse.RotatedBy(Mathf.Deg2Rad * Mathf.Lerp(-rotation, rotation, lerp));
            if(blue && blueCount-- > 0)
                Projectile.NewProjectile<BlueChip>(transform.position, rot);
            else
                Projectile.NewProjectile<PokerChip>(transform.position, rot);
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
        float totalOffset = (0.575f + 0.625f * total) * direction;
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
    public void ResizeChips()
    {
        for(int i = notYetResizedCips.Count - 1; i >= 0; --i)
        {
            GameObject chip = notYetResizedCips[i];
            if(chip == null || chip.transform.localScale.x >= 1)
            {
                if(chip != null)
                    chip.transform.localScale = Vector3.one;
                notYetResizedCips.RemoveAt(i);
            }
            else
                chip.transform.localScale = Vector3.Lerp(chip.transform.localScale * 1.01f, Vector3.one, 0.08f);
        }
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
        powerPool.Add<AllIn>();
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
        ResizeChips();
        PrimaryColor = new Color(0.95f, 1f, 0.6f);
        if(p.Accessory is Crystal)
        {
            PrimaryColor = new Color(0.6f, 0.933f, 0.255f);
        }
        while(stacks.Count < player.ChipStacks)
            AddStack();
        while (stacks.Count > player.ChipStacks)
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
            while(player.abilityTimer < AbilityCD)
            {
                if (RemoveChip())
                    player.abilityTimer += AbilityCD * 0.125f * ((player.AbilityRecoverySpeed - 1) * 0.2f + Mathf.Sqrt(player.AbilityRecoverySpeed));
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
