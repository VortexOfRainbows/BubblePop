using UnityEngine;
using System.Collections.Generic;

public class NodeGraphManager : MonoBehaviour
{
    public List<Node> allNodes = new List<Node>();

    public Node NodeGetClosestNode(Vector2 position) // Will need to check via raycast if node can be reached
    {
        Node closest = null;
        float minDist = Mathf.Infinity;

        foreach (Node node in allNodes)
        {
            float dist = Vector2.Distance(position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }
        return closest;
    }
}