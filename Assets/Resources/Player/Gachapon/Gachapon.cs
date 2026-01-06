using System.Collections.Generic;
using UnityEngine;

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
        r.color = r.color.WithAlphaMultiplied(PlayerData.SpecialVisualOpacity);
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
                Projectile.NewProjectile<BlueChip>(transform.position, rot, 6 + 2 * Player.Instance.DoubleDownChip);
            else
                Projectile.NewProjectile<PokerChip>(transform.position, rot, 3 + 1 * Player.Instance.DoubleDownChip);
            int sparkle = player.DashSparkle;
            if(sparkle > 0)
            {
                Vector2 pos = (Vector2)transform.position;
                float approximate = player.DashSparkle * 0.25f + 0.25f;
                int whole = (int)approximate;
                approximate -= whole;
                for (int j = 0; j < whole; ++j)
                {
                    Vector2 target = pos + rot * Utils.RandFloat(0.2f, 1.0f);
                    Projectile.NewProjectile<StarProj>(pos, rot + Utils.RandCircle(5), 2, target.x + Utils.RandFloat(-5, 5), target.y + Utils.RandFloat(-5, 5), Utils.RandInt(2) * 2 - 1);
                }
                if(Utils.RandFloat(1) < approximate)
                {
                    Vector2 target = pos + rot * Utils.RandFloat(0.2f, 1.0f);
                    Projectile.NewProjectile<StarProj>(pos, rot + Utils.RandCircle(5), 2, target.x + Utils.RandFloat(-5, 5), target.y + Utils.RandFloat(-5, 5), Utils.RandInt(2) * 2 - 1);
                }
            }
        }
        bool ret = totalCount > 0;
        if(ret)
        {
            player.OnUseAbility();
        }
        return ret;
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
    public float PreviousSpecial = 1;
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
        if(PreviousSpecial != PlayerData.SpecialVisualOpacity)
        {
            PreviousSpecial = PlayerData.SpecialVisualOpacity;
            foreach(ChipStack stack in stacks)
            {
                foreach(GameObject chip in stack.Chips)
                {
                    var sp = chip.GetComponent<SpriteRenderer>();
                    sp.color = sp.color.WithAlpha(PreviousSpecial);
                }
            }
        }
    }
    public Sprite[] altFaces;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<GachaponUnlock>();
    public override void Init()
    {
        PrimaryColor = new Color(0.95f, 1f, 0.6f);
        stacks = new List<ChipStack>();
    }
    //public SpriteRenderer MouthR;
    protected override float AngleMultiplier => 0.25f;
    protected override float RotationSpeed => 1f;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Raise>();
        powerPool.Add<AllIn>();
        powerPool.Add<DoubleDown>();
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Gachapon").WithDescription("A greedy shopkeeper? No! I'm the most honest gal around!");
    }
    public GameObject top;
    public override void FaceUpdate()
    {
        Vector2 toMouse = p.LookPosition - (Vector2)transform.position;
        Vector2 toMouse2 = toMouse.normalized;
        if (p.IsMainPlayerAnimator)
        {
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && (Player.Instance.Weapon.IsAttacking() || Player.Instance.Weapon is BubbleGun || Player.Instance.Weapon is Book))
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
        toMouse2.x += Mathf.Sign(toMouse2.x) * 6;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.15f, 0).RotatedBy(toMouseR);
        looking.y *= 0.75f;
        if (looking.x < 0)
            toMouseR += Mathf.PI;
        Vector2 pos = new Vector2(0.07f * p.Direction, 0.325f) + looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouseR * Mathf.Rad2Deg);
        FaceR.flipX = toMouse.x > 0;
        FaceR.enabled = true;
        top.SetActive(true);
        //MouthR.flipX = FaceR.flipX;
    }
    public override float AbilityCD => 3.0f;
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
        if(Control.Ability && !Control.LastAbility)
        {
            if (RemoveChip())
                player.abilityTimer = AbilityCD;
        }
        else if(Control.Ability)
        {
            while(player.abilityTimer < AbilityCD * 0.8f)
            {
                if (RemoveChip())
                    player.abilityTimer = AbilityCD * 0.8f + AbilityCD * 0.125f * ((player.AbilityRecoverySpeed - 1) * 0.2f + Mathf.Sqrt(player.AbilityRecoverySpeed));
                else
                    break;
            }
        }
        while(player.AbilityReady)
        {
            player.abilityTimer += AbilityCD;
            AddChip();
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
        spriteRender.transform.localEulerAngles = Vector3.forward * Mathf.LerpAngle(spriteRender.transform.localEulerAngles.z, FlipDir == -1 ? 180 : 0, 0.1f);
    }
}
