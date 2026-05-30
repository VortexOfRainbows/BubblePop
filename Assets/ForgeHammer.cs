using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ForgeHammer : GemUtility
{
    public List<ForgeCapsule> MyCapsules = new();
    public Transform HydraulicPress;
    public Transform GemCrushPosition;
    public float AnimationTimer = 0;
    public ForgeCapsule RCapsuel { get; set; }
    public GameObject GemTemplate;
    public byte ProgressionNumber { get; set; } = 0;
    public Transform BackPipe1, BackPipe2;
    public float SpeedMultiplier { get; set; } = 0.95f;
    public float AnimateCounter = 0.0f;
    public override float SpeedMult => SpeedMultiplier;
    public float AudioSpeedMultiplier => 0.8f + 0.2f * SpeedMultiplier;
    public void Begin(ForgeCapsule requestingCapsule, int gems)
    {
        if (RCapsuel != null)
            return;
        SpeedMultiplier += 0.05f;
        if (SpeedMultiplier > 10)
            SpeedMultiplier = 10;
        CoinManager.ModifyGems(-gems);
        float baseTime = 1.0f;
        int gemAnimation = gems;
        for (int i = 0; i < gemAnimation; ++i)
            AddGem(0.5f * (1 - i * baseTime / gemAnimation));
        RCapsuel = requestingCapsule;
        if (RCapsuel != null)
            RCapsuel.DeployPower();
        AnimationTimer = 4;
    }
    public void Update()
    {
        AnimateGems(GemCrushPosition.position + new Vector3(0, -0.25f, 0), -1, 2);
    }
    public void FixedUpdate()
    {
        if (!HasDelayed)
            DelayedStart();
        AnimateCounter += AnimationTimer > 0 ? SpeedMultiplier : 1.0f;
        while(AnimateCounter >= 1)
        {
            UpdateMe();
            AnimateCounter -= 1;
        }
    }
    public void UpdateMe()
    {
        if(AnimationTimer > 0)
        {
            AnimationTimer -= Time.fixedDeltaTime;
            if(AnimationTimer <= 3)
            {
                if(AnimationTimer > 2.6f)
                {
                    float percent = 1 - ((AnimationTimer - 2.6f) / 0.4f);
                    percent = percent * percent * percent * percent * 0.95f + percent * 0.05f;
                    HydraulicPress.SetLocalXY(Vector2.Lerp(new Vector2(0, 2.9f), new Vector2(0, 2.175f), percent));
                }
                else if(AnimationTimer > 1.0f)
                {
                    HydraulicPress.SetLocalXY(new Vector2(0, 2.175f));
                    if (Gems.Count > 0)
                    {
                        AudioManager.PlaySound(SoundID.Starbarbs, GemCrushPosition.position, 1.4f, 0.7f * AudioSpeedMultiplier);
                        AudioManager.PlaySound(SoundID.WoodBreak, GemCrushPosition.position, 0.5f, 1.7f * AudioSpeedMultiplier);
                        AudioManager.PlaySound(SoundID.ElectricZap, GemCrushPosition.position, 0.6f, 0.45f * AudioSpeedMultiplier, 0);
                        foreach (GameObject gem in Gems)
                        {
                            for(int i = 0; i < 8; ++i)
                            {
                                Vector2 circular = new Vector2(0, -1).RotatedBy(Utils.RandFloat(-135, 135) * Mathf.Deg2Rad);
                                circular.y *= 0.5f;
                                ParticleManager.NewParticle((Vector2)GemCrushPosition.position + circular * 0.6f, Utils.RandFloat(1.2f, 1.5f), circular * Utils.RandFloat(2, 5), 1.0f, Utils.RandFloat(1.0f, 2.0f), ParticleManager.ID.Pixel, ColorHelper.SentinelGreen.WithAlpha(0.8f));
                            }
                            Destroy(gem);
                        }
                        Gems.Clear();
                        gemRemovalCount = 0;
                        nextGemThreshold = -5;
                    }
                    if (RCapsuel != null)
                    {
                        RCapsuel.PowerAnimation = -2.6f;
                        RCapsuel = null;
                    }
                }
                else if(AnimationTimer <= 1.0f)
                {
                    float percent = 1 - (AnimationTimer / 1.0f);
                    percent = percent * percent;
                    HydraulicPress.SetLocalXY(Vector2.Lerp(new Vector2(0, 2.175f), new Vector2(0, 2.9f), percent));
                }
            }
        }
        else
        {
            foreach (ForgeCapsule c in MyCapsules)
                c.UpdateUI();
        }
        float t = 0.1f;
        for (int i = 0; i < Gems.Count; ++i)
        {
            var gem = Gems[i];
            float dir = (i % 2 * 2 - 1);
            if (!gem.activeSelf)
            {
                gem.SetActive(true);
                gem.transform.SetLocalEulerZ(Mathf.Sqrt(Utils.RandFloat(1)) * 90 * dir);
                gem.transform.localScale = new Vector3(0, 0, 1);
            }
            gem.transform.LerpLocalScale(new Vector3(-dir, 1, 1), t);
        }
        if(AnimationTimer < 2.6f && AnimationTimer > 2.475f)
            BackPipe1.transform.localScale = new Vector3(1.1f, 1.0f, 1.0f);
        else
            BackPipe1.LerpLocalScale(Vector2.one, t);
        if (AnimationTimer < 2.575f && AnimationTimer > 2.45f)
            BackPipe2.transform.localScale = new Vector3(1.05f, 1.0f, 1.0f);
        else
            BackPipe2.LerpLocalScale(Vector2.one, t);
        foreach (ForgeCapsule c in MyCapsules)
            c.UpdateMe();
    }
    public bool HasDelayed { get; set; } = false;
    public void DelayedStart()
    {
        HasDelayed = true;
        foreach (ForgeCapsule c in MyCapsules)
        {
            Player.ObjectsConsideredForUIInteraction.Add(c.gameObject);
            c.MyHammer = this;
        }
        ProgressionNumber = World.GetTileData(World.RealTileMap.Map.WorldToCell(transform.position)).ProgressionNumber;
    }
    public List<GameObject> Gems;
    public int gemRemovalCount = 0;
    public int nextGemThreshold = -5;
    public override void OnGemRemoval()
    {
        ++gemRemovalCount;
        if (gemRemovalCount > nextGemThreshold)
        {
            gemRemovalCount = 0;
            nextGemThreshold++;
            Gems.Add(Instantiate(GemTemplate, GemTemplate.transform.parent, false));
        }
    }
}
