using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class NodeSnapParent : MonoBehaviour
{
    public void Start()
    {
        RoundPositions();
    }
    public void RoundPositions()
    {
        transform.localPosition = Vector3.zero;
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            Vector3Int transformPos = new((int)child.localPosition.x, (int)child.localPosition.y);
            child.localPosition = new Vector3(transformPos.x, transformPos.y, child.localPosition.z);
        }
    }
    #if UNITY_EDITOR
    public void Update() => RoundPositions();
    #endif
}
