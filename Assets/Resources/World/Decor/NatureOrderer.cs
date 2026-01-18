using System;
using System.Collections.Generic;
using UnityEngine;

public class NatureOrderer : MonoBehaviour
{
    public List<GameObject> Objects;
    public void Init()
    {
        GatherObjects();
        SortObjects();
        OrderObjects();
    }
    public void GatherObjects()
    {
        for (int i = 0; i < transform.childCount; ++i)
            Objects.Add(transform.GetChild(i).gameObject);
    }
    public void SortObjects()
    {
        Objects.Sort(new YPosComparer());
    }
    public void OrderObjects()
    {
        for (int i = 0; i < Objects.Count; ++i)
        {
            GameObject g = Objects[i];
            float percent = i / (float)Objects.Count;
            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, percent);
        }
    }
}
public class YPosComparer : IComparer<GameObject>
{
    public int Compare(GameObject x, GameObject y)
    {
        return (int)Math.Floor(x.transform.position.y - y.transform.position.y);
    }
}
