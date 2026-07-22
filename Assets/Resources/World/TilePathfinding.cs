using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePathfinding : MonoBehaviour
{
    [Header("Pathfinding Settings")]
    public float maxDistance = 160;
    [SerializeField] Tilemap nodeTilemap;

    [Header("Debugging")]
    public bool draw = true;

    // Used for gizmos to draw
    private List<Vector3Int> visitedList = new List<Vector3Int>();

    // Adds nodes neighbors to check
    private Queue<Vector3Int> openSet = new Queue<Vector3Int>();

    // Allows for a quick check if node has been visited
    private int currentRunID = 0;

    // Stores player positions to detect when they move
    private Vector3Int[] plrPositions = new Vector3Int[0];

    // Offsets 0-3 are Cardinal and 4-7 are Diagonal
    private static readonly int[] offsetX = { 0, 0, 1, -1, 1, -1, 1, -1 };
    private static readonly int[] offsetY = { 1, -1, 0, 0, 1, 1, -1, -1 };
    private static readonly Vector2[] neighborDirection = {
        new Vector2(0, 1),
        new Vector2(0, -1),
        new Vector2(1, 0),
        new Vector2(-1, 0),
        new Vector2(1, 1).normalized,
        new Vector2(-1, 1).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(-1, -1).normalized,
    };

    private void Update()
    {
        // Check if player count has changed and resize array as needed
        if (plrPositions.Length != Player.AllPlayers.Count)
        {
            plrPositions = new Vector3Int[Player.AllPlayers.Count];
            pathfind();
            return;
        }

        // Check if any player has moved to a different tile
        for (int i = 0; i < Player.AllPlayers.Count; ++i)
        {
            Vector3Int plrTilePos = worldToCell(Player.AllPlayers[i].Position);

            if (plrTilePos != plrPositions[i])
            {
                pathfind();
                plrPositions[i] = plrTilePos;
                return;
            }
        }
    }

    // Function to generate flood fill on tile map
    private void pathfind()
    {
        // Sets data sets for use
        openSet.Clear();
        visitedList.Clear();
        ++currentRunID;

        // Loops through each player for start tiles
        foreach (Player plr in Player.AllPlayers)
        {
            Vector3Int plrTilePos = worldToCell(plr.Position);
            ref World.TileData plrTile = ref World.UnsafeGetTileData(plrTilePos);

            // Check if node is visited, only matters if multiple players are on the same tile
            if (plrTile.runID != currentRunID)
            {
                plrTile.runID = currentRunID;
                plrTile.distance = 0f;
                plrTile.direction = Vector2.zero;

                openSet.Enqueue(plrTilePos);
            }
        }

        // Flood-Fills open set until empty
        while (openSet.Count > 0)
        {
            Vector3Int curTilePos = openSet.Dequeue();
            ref World.TileData curTile = ref World.UnsafeGetTileData(curTilePos);

            // Check all 8 neighbors of the current tile
            for (int i = 0; i < 8; ++i)
            {
                // Finds position of neighbor
                int neighborX = curTilePos.x + offsetX[i];
                int neighborY = curTilePos.y + offsetY[i];
                Vector3Int neighborTilePos = new Vector3Int(neighborX, neighborY, 0);

                // Skips if not a valid tile
                if (!isFloor(neighborTilePos)) continue;

                // Sets distance of tile if it is straight or diagonal
                float stepCost = (i < 4) ? 1.0f : 1.414214f;
                float newDistance = curTile.distance + stepCost;

                // Kills check if too far
                if (newDistance > maxDistance) continue;

                ref World.TileData neighborTile = ref World.UnsafeGetTileData(neighborTilePos);
                bool isUnvisited = neighborTile.runID != currentRunID;

                // If a shorter path to neighbor is found
                if (isUnvisited || newDistance < neighborTile.distance)
                {
                    neighborTile.distance = newDistance;
                    // Set's neighbor's flow direction to current tile
                    neighborTile.direction = -neighborDirection[i];

                    if (isUnvisited)
                    {
                        neighborTile.runID = currentRunID;
                        visitedList.Add(neighborTilePos);
                        openSet.Enqueue(neighborTilePos);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!draw) return;

        Gizmos.color = Color.yellow;
        foreach (Vector3Int visitedPos in visitedList)
        {
            ref World.TileData tile = ref World.UnsafeGetTileData(visitedPos);

            if (tile.direction != Vector2.zero)
            {
                Vector3 worldPos = cellToWorld(visitedPos);
                Gizmos.DrawRay(worldPos, (Vector3)tile.direction * tileSize.x * 0.4f);
            }
        }
    }

    #region HelperFunctions

    public static Vector3 tileSize = new Vector3(2, 2, 0);
    private Vector3 cellToWorld(Vector3Int cellPos)
    {
        float x = cellPos.x * tileSize.x;
        float y = cellPos.y * tileSize.y;

        x += tileSize.x * 0.5f;
        y += tileSize.y * 0.5f;

        return new Vector3(x, y, 0);
    }

    private Vector3Int worldToCell(Vector3 worldPos)
    {
        float x = (worldPos.x) / tileSize.x;
        float y = (worldPos.y) / tileSize.y;

        return new Vector3Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0);
    }

    private bool isFloor(Vector3Int cellPos)
    {
        return nodeTilemap.GetColliderType(cellPos) == Tile.ColliderType.None;
    }
    #endregion
}