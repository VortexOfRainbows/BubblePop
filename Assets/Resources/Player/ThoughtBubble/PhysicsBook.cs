using System.Collections.Generic;
using UnityEngine;

public class PhysicsBook : Book
{
    public List<GameObject> MagnetBalls { get; set; } = new();
    public int MagnetBallCounter = 0;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<ThoughtBubbleIndistinguishable>();
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Guide to Physics").WithDescription("A encyclopedia of everything physics'");
    }
    public override void InitializeAbilities(ref List<Ability> abilities)
    {
        base.InitializeAbilities(ref abilities);
        abilities.Add(new Ability(Ability.ID.Passive, "Y:[Thunder bubbles] have Y:[magnetic properties] and can pick up Y:coins"));
    }
    public override int GetRarity() => 2;
    public override Sprite OpenBook => Resources.Load<Sprite>("Player/ThoughtBubble/Book2Open");
    public override Sprite ClosedBook => Resources.Load<Sprite>("Player/ThoughtBubble/Book2Closed");
    protected override void AnimationUpdate()
    {
        base.AnimationUpdate();
        if(!Open && MagnetBalls.Count > 0)
            MagnetBalls.Clear();
    }
    public override void EquipUpdate()
    {
        base.EquipUpdate();
        Player.HasMagnetBook = true;
    }
}
