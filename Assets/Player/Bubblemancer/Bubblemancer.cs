using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubblemancer : Body
{
    public override void Init()
    {
        PrimaryColor = ParticleManager.DefaultColor;
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Choice>();
        powerPool.Add<BubbleBirb>();
    }
    protected override string Description()
    {
        return "A humble shepard from the quaint Bubble Fields";
    }
}
