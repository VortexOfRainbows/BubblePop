using UnityEngine;

public class NodeConnector : MonoBehaviour
{
    public bool ExitNode = true;
    public bool EntranceNode = true;
    public Vector2 Position => transform.position;
    public float Distance(NodeConnector other)
    {
        return Utils.Distance(Position, other.Position);
    }
}
