using System.Collections.Generic;
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
    public void Begin(ForgeCapsule requestingCapsule, int gems)
    {
        if (RCapsuel != null)
            return;
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
        if(!HasDelayed)
            DelayedStart();
        AnimateGems(GemCrushPosition.position, -1, 2);
        if(AnimationTimer > 0)
        {
            AnimationTimer -= Time.deltaTime;
            if(AnimationTimer <= 3)
            {
                if(AnimationTimer > 2.6f)
                {
                    float percent = 1 - ((AnimationTimer - 2.6f) / 0.4f);
                    percent = percent * percent * percent * percent * 0.9f + percent * 0.1f;
                    HydraulicPress.SetLocalXY(Vector2.Lerp(new Vector2(0, 2.9f), new Vector2(0, 2.175f), percent));
                }
                else if(AnimationTimer > 1.0f)
                {
                    HydraulicPress.SetLocalXY(new Vector2(0, 2.175f));
                    if (Gems.Count > 0)
                    {
                        foreach (GameObject gem in Gems)
                            Destroy(gem);
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
        float t = Utils.DeltaTimeLerpFactor(0.1f);
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
