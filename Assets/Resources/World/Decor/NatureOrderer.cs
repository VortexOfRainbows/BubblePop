using System.Collections.Generic;
using UnityEngine;

public class NatureOrderer : MonoBehaviour
{
    public List<Transform> Objects { get; private set; } = new();
    private float Lowest;
    private float Highest;
    public void Init()
    {
        GatherObjects();
        OrderObjects();
    }
    public void GatherObjects()
    {
        Lowest = float.MaxValue;
        Highest = float.MinValue;
        for (int i = 0; i < transform.childCount; ++i)
        {
            var t = transform.GetChild(i);
            Objects.Add(t);
            Lowest = Mathf.Min(Lowest, t.position.y);
            Highest = Mathf.Max(Highest, t.position.y);
        }
    }
    public void OrderObjects()
    {
        float diff = Highest - Lowest;
        for (int i = 0; i < Objects.Count; ++i)
        {
            Transform t = Objects[i];
            float percent = (t.position.y - Lowest) / diff;
            t.position = new Vector3(t.position.x, t.position.y, percent);
        }
    }
}
public class YPosComparer : IComparer<GameObject>
{
    public int Compare(GameObject x, GameObject y)
    {
        float diff = x.transform.position.y - y.transform.position.y;
        return Mathf.FloorToInt(diff);
    }
}
