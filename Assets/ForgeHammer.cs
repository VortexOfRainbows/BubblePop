using System.Collections.Generic;
using UnityEngine;

public class ForgeHammer : GemUtility
{
    public List<ForgeCapsule> MyCapsules = new();
    public Transform HydraulicPress;
    public Transform GemCrushPosition;
    public float AnimationTimer = 0;
    public ForgeCapsule RCapsuel;
    public void Begin(ForgeCapsule requestingCapsule, int gems)
    {
        float baseTime = 1.0f;
        int gemAnimation = gems;
        for (int i = 0; i < gemAnimation; ++i)
            AddGem(0.5f * (1 - i * baseTime / gemAnimation));
        AnimationTimer = 1;
        RCapsuel = requestingCapsule;
    }
    public void Update()
    {
        AnimateGems(GemCrushPosition.position, -1, 2);
        if(AnimationTimer > 0)
        {
            AnimationTimer -= Time.deltaTime;
        }
        else
        {
            if(RCapsuel != null)
            {
                RCapsuel.DeployPower();
                RCapsuel = null;
            }
            foreach (ForgeCapsule c in MyCapsules)
                c.UpdateUI();
        }
    }
    public void Start()
    {
        foreach(ForgeCapsule c in MyCapsules)
            c.MyHammer = this;
    }
}
