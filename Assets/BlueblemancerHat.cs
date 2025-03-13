using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueblemancerHat : BubblemancerHat
{
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ScoreUnlock1000>();
    protected override string Description()
    {
        return "A suspiciously blue variation of Bubblemancer's classic hat\n" +
            "Nobody knows where it came from...\n" +
            "\n" +
            "Start with Choice";
    }
    public override void OnStartWith()
    {
        PowerUp.Spawn<Choice>(Player.Position, 0);
    }
    public override int GetPrice() => 5;
}
