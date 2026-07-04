using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilePathfinding : MonoBehaviour
{
    // Debugging to draw flowfield
    public bool draw = true;

    // Full tilemap to reference for making maps of zones
    [SerializeField] private Tilemap nodeTilemap;

    // Struct that locally only stores needed information for speed and space
    public struct pathData
    {
        // Distance from closest player
        public float distance;
        // Direction of shortest path to player
        public Vector2 direction;
    }
    // 1D array operating as 2D
    private pathData[] zoneTiles = { };
    // Used to index through 1D array
    private int zoneWidth;
    // Used for array size and bounds checking
    private int zoneHeight;
    // Offset of zone from full tile map
    private Vector3 zoneOffset;
    private Vector3Int zoneIntOffset;

    // Tiles that still need to be checked
    private Vector3Int[] openSet;
    // Index of final item in openSet, used as openSet is larger than needed
    private int openSetEnd;

    // Pairs of differences to get all 8 neighboring positions
    private int[] OffsetX = { -1, 0, 1, -1, 1, -1, 0, 1 };
    private int[] OffsetY = { 1, 1, 1, 0, 0, -1, -1, -1 };


    #region HelperFunctions
    private bool isFloor(Vector3Int cellPos) // TODO: Check zone, may not be needed though with barrier
    {
        return ((nodeTilemap.GetColor(cellPos) == Color.white) && (World.GetTileData(cellPos).IsRoadblock == false)); // If white and not roadblock then floor
    }

    private bool isTile(Vector3Int cellPos)
    {
        return (nodeTilemap.GetTile(cellPos) != null); // If not null then tile
    }

    private Vector3 tileSize = new Vector3(2, 2, 0);
    private Vector3 ManualCellToWorld(Vector3Int cellPos)
    {
        float x = cellPos.x * tileSize.x;
        float y = cellPos.y * tileSize.y;

        x += tileSize.x * 0.5f;
        y += tileSize.y * 0.5f;

        return new Vector3(x, y, 0) + transform.position;
    }

    private Vector3Int ManualWorldToCell(Vector3 worldPos)
    {
        float x = (worldPos.x - transform.position.x) / tileSize.x;
        float y = (worldPos.y - transform.position.y) / tileSize.y;

        return new Vector3Int((int)x, (int)y, 0);
    }
    #endregion

    public void SetZone(Vector3 startPos)
    {
        Vector3Int startTile = ManualWorldToCell(startPos);

        // Min and max x and y of zone to find size
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        // List of all nodes to return
        List<Vector3Int> validTiles = new List<Vector3Int>();

        // Queue of tiles to check
        Queue<Vector3Int> tilesToCheck = new Queue<Vector3Int>();

        // Quick lookup to prevent revising tiles
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        // Target zone
        int targetZoneID = World.GetTileData(startTile).ProgressionNumber;
        Debug.Log("Prog target: " + targetZoneID);

        // Sets start node
        tilesToCheck.Enqueue(startTile);
        visited.Add(startTile);

        // Loops through all tiles to check
        while (tilesToCheck.Count > 0)
        {
            // Gets next tile
            Vector3Int currentTile = tilesToCheck.Dequeue();

            // Sets new min and max x values
            if (currentTile.x < minX)
            {
                minX = currentTile.x;
            }
            else if (currentTile.x > maxX)
            {
                maxX = currentTile.x;
            }
            // Sets new min and max y values
            if (currentTile.y < minY)
            {
                minY = currentTile.y;
            }
            else if (currentTile.y > maxY)
            {
                maxY = currentTile.y;
            }

            // Add it to the list
            validTiles.Add(currentTile);

            // Check all 8 neighbors of the current tile
            for (int i = 0; i < 8; ++i)
            {
                // Finds position of neighbor
                int neighborX = currentTile.x + OffsetX[i];
                int neighborY = currentTile.y + OffsetY[i];

                // Neighbor to check
                Vector3Int neighborTile = new Vector3Int (neighborX, neighborY, 0);

                // Check if neighbor hasn't been checked and if it's in the right zone
                if (!visited.Contains(neighborTile) && World.GetTileData(neighborTile).ProgressionNumber == targetZoneID)
                {
                    Debug.Log("ProgNum: " + World.GetTileData(neighborTile).ProgressionNumber);
                    // Marks as visited to avoid checking again
                    visited.Add(neighborTile);

                    //Add to queue to have neighbors checked later
                    tilesToCheck.Enqueue(neighborTile);
                }
                else
                {
                    //Debug.Log("ProgNum: " + World.GetTileData(neighborTile).ProgressionNumber);
                }
            }
        }

        zoneWidth = maxX - minX;
        zoneHeight = maxY - minY;
        zoneTiles = new pathData[zoneWidth * zoneHeight];
        openSet = new Vector3Int[zoneWidth * zoneHeight];
        zoneOffset = ManualCellToWorld(new Vector3Int(minX, minY, 0));
        zoneIntOffset = new Vector3Int(minX, minY, 0);

        if (draw)
        {
            Gizmos.color = Color.white;
        }
    }

    private void openNeighbors(Vector3Int tilePos)
    {
        // TODO (DAVID): Look into if corner tiles should need both connecting tiles to be valid as well, such that at the corner of a wall would fail as they may run into it otherwise
        // (Will swap to hard code checking directions if needed)

        for (int i = 0; i < 8; ++i) // Loops through all 8 directions of neighbors
        {
            int neighborX = tilePos.x + OffsetX[i];
            int neighborY = tilePos.y + OffsetY[i];

            if (neighborX >= 0 && neighborX < zoneWidth && neighborY >= 0 && neighborY < zoneHeight) // Neighbor bounds checking
            {
                Vector3Int neighborTilePos = new Vector3Int(neighborX, neighborY, 0);
                if (isFloor(neighborTilePos)) // Is valid floor tile TODO: Is this needed here? Maybe 
                {
                    float neighborDistance = zoneTiles[neighborX + zoneWidth * neighborY].distance;

                    if (neighborDistance > 0f)
                    {
                        zoneTiles[neighborX + zoneWidth * neighborY].distance = -1f; // Used to prevent tiles in openSet from being added again
                        openSet[openSetEnd++] = neighborTilePos;
                    }
                    else // Has a value
                    {
                        if (neighborX == 0 || neighborY == 0) // Is a straight distance neighbor
                        {
                            if (neighborDistance + 1f < zoneTiles[tilePos.x + zoneWidth * tilePos.y].distance)
                            {
                                zoneTiles[tilePos.x + zoneWidth * tilePos.y].distance = neighborDistance + 1f;
                                zoneTiles[tilePos.x + zoneWidth * tilePos.y].direction = new Vector2(neighborX, neighborY);
                            }
                        }
                        else // Is a diagonal neighbor
                        {
                            if (neighborDistance + 1.41f < zoneTiles[tilePos.x + zoneWidth * tilePos.y].distance) // TODO: Check if 1.41 is a good enough approximation of sqrt(2)
                            {
                                zoneTiles[tilePos.x + zoneWidth * tilePos.y].distance = neighborDistance + 1.41f;
                                zoneTiles[tilePos.x + zoneWidth * tilePos.y].direction = new Vector2(neighborX, neighborY);
                            }
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        foreach (Player plr in Player.AllPlayers)
        {
            Vector3Int tilePos = ManualWorldToCell(plr.Position) - zoneIntOffset;
            zoneTiles[tilePos.x + zoneWidth * tilePos.y].distance = 1;
            zoneTiles[tilePos.x + zoneWidth * tilePos.y].direction = Vector2.zero;
            //openNeighbors(tilePos);
        }

        /*
        for (int i = 0; i < openSet.Length; ++i)
        {
            Vector3Int tilePos = openSet[i];

            openNeighbors(tilePos);
        }
        */
    }

    private void OnDrawGizmos()
    {
        if (!draw) return;

        Debug.Log(zoneWidth + "x" + zoneHeight);
        for (int i = 0; i < zoneTiles.Length; ++i)
        {
            if (zoneTiles[i].distance > 0f)
            {
                int x = i % zoneWidth;
                int y = i / zoneWidth;
                Debug.Log("First Real Pos: " + (ManualCellToWorld(new Vector3Int(0, 0, 0))));
                Gizmos.color = Color.red;
                Gizmos.DrawSphere((ManualCellToWorld(new Vector3Int(0, 0, 0))), 0.5f);
                Debug.Log("Tile Pos: " + new Vector3Int(x, y, 0));
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(new Vector3Int(x, y, 0), 0.5f);
                Debug.Log("Real Pos: " + (new Vector3(x, y, 0) + zoneOffset));
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(new Vector3(x, y, 0) + zoneOffset, 0.5f);
                //Gizmos.DrawRay(new Vector3(x, y, 0) + zoneOffset, new Vector3(zoneTiles[i].direction.x, zoneTiles[i].direction.y, 0));
                //Gizmos.DrawSphere(new Vector3(x, y, 0) + zoneOffset, 0.5f);
            }
        }
    }
}
