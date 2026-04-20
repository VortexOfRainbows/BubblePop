using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

public class WaypointGraphManager : MonoBehaviour
{
    public Tilemap nodeTilemap;
    public GameObject waypointPrefab;
    

    void Start()
    {
        GenerateGraphFromTiles();
        ConnectWaypoints();
    }

    private void GenerateGraphFromTiles()
    {
        BoundsInt bounds = nodeTilemap.cellBounds;

        bool[][] usedCells = new bool[bounds.size.x][]; // To prevent setting multiple waypoints in the same spot
        for (int i = 0; i < bounds.size.x; i++)
        {
            usedCells[i] = new bool[bounds.size.y];
        }

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

                int i = x - bounds.xMin;
                int j = y - bounds.yMin;
                if (!BL && !B && !L && isTile(bottomLeft) && !usedCells[i - 1][j - 1])
                {
                    Instantiate(waypointPrefab, ManualCellToWorld(bottomLeft), Quaternion.identity).transform.parent = transform;
                    usedCells[i - 1][j - 1] = true;
                }
                if (!TL && !T && !L && isTile(topLeft) && !usedCells[i - 1][j + 1])
                {
                    Instantiate(waypointPrefab, ManualCellToWorld(topLeft), Quaternion.identity).transform.parent = transform;
                    usedCells[i - 1][j + 1] = true;
                }
                if (!TR && !T && !R && isTile(topRight) && !usedCells[i + 1][j + 1])
                {
                    Instantiate(waypointPrefab, ManualCellToWorld(topRight), Quaternion.identity).transform.parent = transform;
                    usedCells[i + 1][j + 1] = true;
                }
                if (!BR && !B && !R && isTile(bottomRight) && !usedCells[i + 1][j - 1])
                {
                    Instantiate(waypointPrefab, ManualCellToWorld(bottomRight), Quaternion.identity).transform.parent = transform;
                    usedCells[i + 1][j - 1] = true;
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

        return new Vector3(x, y, 0) + transform.position;
    }

    private Vector3Int ManualWorldToCell(Vector3 worldPos)
    {
        Vector3 size = new Vector3(2, 2, 0);

        float x = (worldPos.x - transform.position.x) / size.x;
        float y = (worldPos.y - transform.position.y) / size.y;

        return new Vector3Int((int)x, (int)y, 0);
    }

    private void ConnectWaypoints()
    {
        Debug.Log("Connecting");
        for (int i = 0; i < transform.childCount; i++)
        {
            Waypoint curWaypoint = transform.GetChild(i).GetComponent<Waypoint>();

            for (int j = i + 1; j < transform.childCount - 1; j++)
            {
                Waypoint checkWaypoint = transform.GetChild(j).GetComponent<Waypoint>();

                RaycastHit2D hit = Physics2D.Raycast(curWaypoint.transform.position, checkWaypoint.transform.position - curWaypoint.transform.position, LayerMask.GetMask("World"));
                if (hit.collider == null)
                {
                    Debug.Log("Hit: " + hit.collider);
                    curWaypoint.listNeighbors.Add(checkWaypoint);
                    checkWaypoint.listNeighbors.Add(curWaypoint);
                }
            }
        }

        foreach (Transform child in transform) // At the end turns all waypoint neighbor information into arrays for faster processing during runtime
        {
            child.GetComponent<Waypoint>().convertToArray();
        }
    }
}