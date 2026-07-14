using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblemancerUnlock : UnlockCondition
{
    protected override bool TryUnlockCondition => true;
}
public class ThoughtBubbleUnlock : UnlockCondition
{
    protected override bool TryUnlockCondition => WaveDirector.HighScoreWaveNum > 15;
}
public class GachaponUnlock : UnlockCondition
{
    protected override bool TryUnlockCondition => false;
}
public class FizzyUnlock : UnlockCondition
{
    protected override bool TryUnlockCondition => false;
}
public class KingOilUnlock : UnlockCondition
{
    protected override bool TryUnlockCondition => false;
}