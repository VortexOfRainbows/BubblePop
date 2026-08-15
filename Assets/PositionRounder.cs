using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class PositionRounder : MonoBehaviour
{
    public bool HalfStep = false;
    #if UNITY_EDITOR
    public void Update() => RoundPosition();
    #endif
    public void Start() => RoundPosition();
    public void RoundPosition()
    {
        if(HalfStep)
        {
            Vector3Int transform2 = new((int)transform.position.x, (int)transform.position.y);
            transform.position = transform2;
            return;
        }
        Vector3Int transformPos = new((int)(transform.position.x - 1) / 2, (int)(transform.position.y - 1) / 2);
        transform.position = transformPos * 2 + Vector3Int.one;
    }
}
