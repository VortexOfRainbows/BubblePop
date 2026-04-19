using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaypointGraphManager : MonoBehaviour
{
    public Tilemap nodeTilemap;
    public GameObject waypointPrefab;
    
    public List<Waypoint> nodeWaypoints = new List<Waypoint>();
    public Waypoint[] waypointArr;
    private int numWaypoints;

    void Start()
    {
        GenerateGraphFromTiles();
    }

    void GenerateGraphFromTiles()
    {
        BoundsInt bounds = nodeTilemap.cellBounds;

        for (int x = bounds.xMin + 1; x < bounds.xMax - 1; x++) // This skips the outermost edge, I think all tiles are drawn so it should be okay
        {
            for (int y = bounds.yMin + 1; y < bounds.yMax - 1; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                if (!isTile(cellPos)) continue;
                if (!isWall(cellPos)) continue;

                // Check all 4 corners, skip generating waypoint in each if either adjacent slots are walls
                Vector3Int bottomLeft = cellPos + new Vector3Int(-1, -1, 0);
                Vector3Int left = cellPos + new Vector3Int(-1, 0, 0);
                Vector3Int topLeft = cellPos + new Vector3Int(-1, 1, 0);
                Vector3Int top = cellPos + new Vector3Int(0, 1, 0);
                Vector3Int topRight = cellPos + new Vector3Int(1, 1, 0);
                Vector3Int right = cellPos + new Vector3Int(1, 0, 0);
                Vector3Int bottomRight = cellPos + new Vector3Int(1, -1, 0);
                Vector3Int bottom = cellPos + new Vector3Int(0, -1, 0);

                bool BL = isWall(bottomLeft);
                bool L = isWall(left);
                bool TL = isWall(topLeft);
                bool T = isWall(top);
                bool TR = isWall(topRight);
                bool R = isWall(right);
                bool BR = isWall(bottomRight);
                bool B = isWall(bottom);

                if (!BL && !B && !L)
                {
                    Vector3 worldPos = ManualCellToWorld(bottomLeft);
                    Debug.Log("World Pos: " + worldPos);
                    Color cellColor = nodeTilemap.GetColor(cellPos);
                    Debug.Log("Color: " +  cellColor);

                    Instantiate(waypointPrefab, worldPos, Quaternion.identity);
                }
            }
        }
    }

    private bool isWall(Vector3Int cellPos)
    {
        Color cellColor = nodeTilemap.GetColor(cellPos);
        if (cellColor != Color.white) // If it is not a floor tile
        {
            return true;
        }
        return false;
    }

    private bool isTile(Vector3Int cellPos)
    {
        TileBase tileBase = nodeTilemap.GetTile(cellPos);
        if (tileBase == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private Vector3 ManualCellToWorld(Vector3Int cellPos)
    {
        Vector3 size = new Vector3(2, 2, 0);

        float x = cellPos.x * size.x;
        float y = cellPos.y * size.y;

        x += size.x * 0.5f;
        y += size.y * 0.5f;

        return new Vector3(x, y, 0) + new Vector3(2, 32, 0);
    }

    public Waypoint GetClosestWaypoint(Vector2 position) // Will need to check via raycast if waypoint can be reached
    {
        Waypoint closest = null;
        float minDist = Mathf.Infinity;

        foreach (Waypoint waypoint in nodeWaypoints)
        {
            float dist = Vector2.Distance(position, waypoint.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = waypoint;
            }
        }
        return closest;
    }
}