using System.Collections.Generic;
using UnityEngine;

public partial class Entity : MonoBehaviour
{
    private BuffAnchorLayoutGroup BuffIconParent = null;
    public readonly List<Buff> buffs = new();
    private readonly List<BuffIcon> BuffIcons = new();
    public void AddBuff<T>(float time, int stacks = 1) where T: Buff, new()
    {
        if(BuffIconParent == null)
            BuffIconParent = Instantiate(BuffIcon.BuffIconParentPrefab, transform, false).GetComponent<BuffAnchorLayoutGroup>();
        Buff existing = GetBuff<T>();
        if (existing == null)
        {
            Buff b = new T();
            b.Apply(this, time, stacks);
            buffs.Add(b);
            SpawnBuffIcon(b);
            //Debug.Log($"Added buff: {b.GetType().Name} for {time} seconds");
        }
        else
        {
            existing.Apply(this, time, stacks);
            //Debug.Log($"Incremented buff: {existing.GetType().Name} for {time} seconds");
        }
    }
    public bool RemoveBuff<T>(int stacks = 1) where T : Buff
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            Buff b = buffs[i];
            if (typeof(T) == b.GetType() && b.Active)
            {
                b.RemoveStack(stacks);
                return true;
            }
        }
        return false;
    }
    public Buff GetBuff<T>() where T : Buff
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            Buff b = buffs[i];
            if (typeof(T) == b.GetType())
                return b;
        }
        return null;
    }
    public bool HasBuff<T>() where T : Buff
    {
        return GetBuff<T>() != null;
    }
    public void UpdateBuffs()
    {
        if (BuffIconParent != null && !Main.GamePaused)
            BuffIconParent.UpdateLayout();
        for (int i = buffs.Count - 1; i >= 0; --i)
        {
            Buff b = buffs[i];
            b.Update();
            if (!b.Active)
                buffs.RemoveAt(i);
        }
        for(int i = BuffIcons.Count- 1; i >= 0; --i)
        {
            BuffIcon b = BuffIcons[i];
            if (!b.UpdateBuff(i))
                BuffIcons.RemoveAt(i);
        }
    }
    public void SpawnBuffIcon(Buff buff)
    {
        BuffIcon b = Instantiate(BuffIcon.Prefab, BuffIconParent.transform, false).GetComponent<BuffIcon>();
        b.owner = this;
        b.SetBuff(buff);
        BuffIcons.Add(b);
    }
}
