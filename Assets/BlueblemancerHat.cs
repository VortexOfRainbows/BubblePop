using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueblemancerHat : BubblemancerHat
{
    public override UnlockCondition UnlockCondition => UnlockCondition.Get<ScoreUnlock2000>();
    protected override string Description()
    {
        return "A suspiciously blue variation of Bubblemancer's classic hat\nNobody knows where it came from...";
    }
}
