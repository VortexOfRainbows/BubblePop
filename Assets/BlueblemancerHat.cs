using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueblemancerHat : BubblemancerHat
{
    public override UnlockCondition UnlockCondition => UnlockCondition.Get<ScoreUnlock1000>();
    protected override string Description()
    {
        return "A suspiciously blue variation of Bubblemancer's classic hat\n" +
            "Nobody knows where it came from...\n" +
            "\n" +
            "Start with 100 points and a choice power up";
    }
    public override void OnStartWith()
    {
        EventManager.Point += 100;
        PowerUp.Spawn<Choice>(Player.Position, 100);
    }
}
