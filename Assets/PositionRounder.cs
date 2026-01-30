using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class PositionRounder : MonoBehaviour
{
    #if UNITY_EDITOR
    public void Update() => RoundPosition();
    #endif
    public void Start() => RoundPosition();
    public void RoundPosition()
    {
        Vector3Int transformPos = new((int)transform.position.x / 2, (int)transform.position.y / 2);
        transform.position = transformPos * 2 + Vector3Int.one;
    }
}
