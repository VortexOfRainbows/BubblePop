using Unity.VisualScripting;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public virtual void ModifyUIOffsets(ref Vector2 offset, ref float rotation, ref float scale)
    {

    }
    public virtual string Name()
    {
        return this.GetType().HumanName(true);
    }
    public virtual string Description()
    {
        return "Temporary Equipment Description";
    }
    public static Player p => Player.Instance;
    public SpriteRenderer spriteRender;
    public Vector2 velocity;
    public void AliveUpdate()
    {
        AnimationUpdate();
    }
    public void DeadUpdate()
    {
        DeathAnimation();
    }
    protected virtual void AnimationUpdate()
    {

    }
    protected virtual void DeathAnimation()
    {

    }
}
