using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Entity : MonoBehaviour
{
    public readonly Dictionary<Type, int> UniqueBuffTypes = new();
    public readonly List<Buff> buffs = new();
    private GameObject BuffIconParent = null;
    private readonly List<BuffIcon> BuffIcons = new();
    public void AddBuff<T>(float time) where T: Buff, new()
    {
        if(BuffIconParent == null)
            BuffIconParent = Instantiate(BuffIcon.BuffIconParentPrefab, transform, false);
        Buff b = new T
        {
            timeLeft = time,
            initiallyAppliedDuration = time,
            owner = this,
        };
        buffs.Add(b);
        Type t = b.GetType();
        if (!UniqueBuffTypes.ContainsKey(t))
        {
            SpawnBuffIcon(b);
            UniqueBuffTypes.Add(t, 1);
        }
        else
            UniqueBuffTypes[t]++;
        Debug.Log($"Added buff: {b.GetType().Name} for {b.timeLeft} seconds");
    }
    public void UpdateBuffs()
    {
        for(int i = buffs.Count - 1; i >= 0; --i)
        {
            Buff b = buffs[i];
            b.Update();
            if (!b.active)
            {
                buffs.RemoveAt(i);
                Type t = b.GetType();
                if(UniqueBuffTypes.ContainsKey(t))
                {
                    if (--UniqueBuffTypes[t] <= 0)
                    {
                        UniqueBuffTypes.Remove(t);
                    }
                }
            }
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
