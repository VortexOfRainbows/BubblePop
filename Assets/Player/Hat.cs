using UnityEngine;

public class Hat : Equipment
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(0.1f, -0.2f);
        scale = 0.95f;
        rotation = 15f;
    }
    protected override void AnimationUpdate()
    {

    }
    protected override void DeathAnimation()
    {

    }
}
