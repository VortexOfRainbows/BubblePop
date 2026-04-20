using UnityEngine;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> listNeighbors = new List<Waypoint>();
    public Waypoint[] neighbors;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        if (neighbors != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Waypoint neighbor in neighbors)
            {
                if (neighbor != null)
                {
                    Gizmos.DrawLine(transform.position, neighbor.transform.position);
                }
            }
        }
    }

    public void convertToArray() // Turns all node information into arrays for faster processing at the end
    {
        neighbors = new Waypoint[listNeighbors.Count];
        int index = 0;
        foreach (Waypoint neighbor in listNeighbors)
        {
            neighbors[index++] = neighbor;
        }
    }
}
