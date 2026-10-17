using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePathfinding : MonoBehaviour
{
    [Header("Pathfinding Settings")]
    public float maxDistance = 160;
    [SerializeField] Tilemap nodeTilemap;

    [Header("Debugging")]
    public bool DrawWater = false;
    public bool DrawLand = true;

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
                plrTile.groundPathDistance = 0f;
                plrTile.groundPathDirection = Vector2.zero;
                plrTile.liquidPathDistance = 0f;
                plrTile.liquidPathDirection = Vector2.zero;
                plrTile.totalLiquidDistance = 0f;

                openSet.Enqueue(plrTilePos);
            }
        }

        // Flood-Fills open set until empty
        while (openSet.Count > 0)
        {
            Vector3Int curTilePos = openSet.Dequeue();
            ref World.TileData curTile = ref World.UnsafeGetTileData(curTilePos);

            // Check all 8 neighbors of the current tile
            for (int i = 0; i < 8; ++i) {
                // Finds position of neighbor
                int neighborX = curTilePos.x + offsetX[i];
                int neighborY = curTilePos.y + offsetY[i];
                Vector3Int neighborTilePos = new Vector3Int(neighborX, neighborY, 0);

                // Skips if not a valid tile
                ref World.TileData neighborTile = ref World.UnsafeGetTileData(neighborTilePos);
                if (neighborTile.IsSolid) continue;

                // Sets distance of tile if it is straight or diagonal
                float stepCost = (i < 4) ? 1.0f : 1.414214f;
                float newGroundDistance = curTile.groundPathDistance + stepCost;
                float newLiquidDistance = curTile.liquidPathDistance + stepCost;

                bool isUnvisited = neighborTile.runID != currentRunID;

                bool passed = false;

                // Kills if liquid and if too far
                if (!neighborTile.IsLiquid && newGroundDistance < maxDistance)
                {
                    // If a shorter path to neighbor is found
                    if (isUnvisited || newGroundDistance < neighborTile.groundPathDistance)
                    {
                        neighborTile.groundPathDistance = newGroundDistance;
                        // Set's neighbor's flow direction to current tile
                        neighborTile.groundPathDirection = -neighborDirection[i];

                        passed = true;
                    }
                }

                // Kills if too far
                if (newLiquidDistance < maxDistance)
                {
                    // If a shorter path to neighbor is found
                    if (isUnvisited || newLiquidDistance < neighborTile.liquidPathDistance)
                    {
                        neighborTile.liquidPathDistance = newLiquidDistance;
                        // Set's neighbor's flow direction to current tile
                        neighborTile.liquidPathDirection = -neighborDirection[i];

                        passed = true;
                    }
                }


                if (passed && isUnvisited) {
                    neighborTile.runID = currentRunID;
                    visitedList.Add(neighborTilePos);
                    openSet.Enqueue(neighborTilePos);
                }
            }
        }
    }

    public enum PathfindingType
    {
        Ground = 0,
        Liquid = 1,
        Air = 2,
    }

    public static Vector2 BestPathToPlayer(Vector3 tilePos, PathfindingType pathingType, float liquidModifier)
    {
        // Raycast to player over ground
        //  Go if able

        // Get tile to compare distances
        ref World.TileData curTile = ref World.UnsafeGetTileData((int) tilePos.x, (int) tilePos.y);

        Vector2 bestDirection = curTile.groundPathDirection;

        switch ((int)pathingType)
        {
            case 0: // Ground only Traversal
                
                Debug.Log("Current tile ground: " + curTile.groundPathDirection);
                break;
            case 1: // Ground and Liquid Traversal
                // Raycast over ground + liquid
                //  Apply modifier for distance over liquid tiles

                // Get pathfinding over liquid distance, apply modifier
                //  Compare distance of raycast vs both pathfinds and choose lowest cost
                if (curTile.liquidPathDistance - curTile.totalLiquidDistance + curTile.totalLiquidDistance * liquidModifier < curTile.groundPathDistance)
                {
                    bestDirection = curTile.liquidPathDirection;
                    Debug.Log("Current tile liquid: " + curTile.liquidPathDirection);
                }
                break;
            case 2: // Flying Traversal
                // Raycast over anything other than walls
                //  Go if able
                // Get pathfinding over everything distance
                //  Go on everything pathfind

                // TODO: Switch for air version
                if (curTile.liquidPathDistance - curTile.totalLiquidDistance + curTile.totalLiquidDistance * liquidModifier < curTile.groundPathDistance) {
                    bestDirection = curTile.liquidPathDirection;
                    Debug.Log("Current tile air(liquid): " + curTile.liquidPathDirection);
                }

                break;
        }

        Debug.Log(bestDirection);
        return bestDirection;
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3Int visitedPos in visitedList)
        {
            ref World.TileData tile = ref World.UnsafeGetTileData(visitedPos);

            if (DrawWater && tile.liquidPathDirection != Vector2.zero)
            {
                Gizmos.color = Color.blue;
                Vector3 worldPos = cellToWorld(visitedPos);
                Gizmos.DrawRay(worldPos, (Vector3)tile.liquidPathDirection * TileSize.x * 0.4f);
            }
            if (DrawLand && tile.groundPathDirection != Vector2.zero)
            {
                Gizmos.color = Color.yellow;
                Vector3 worldPos = cellToWorld(visitedPos);
                Gizmos.DrawRay(worldPos, (Vector3)tile.groundPathDirection * TileSize.x * 0.4f);
            }
        }
    }

    #region HelperFunctions
    public static readonly Vector3 TileSize = new(2, 2, 0);
    private Vector3 cellToWorld(Vector3Int cellPos)
    {
        float x = cellPos.x * TileSize.x;
        float y = cellPos.y * TileSize.y;

        x += TileSize.x * 0.5f;
        y += TileSize.y * 0.5f;

        return new Vector3(x, y, 0);
    }

    private Vector3Int worldToCell(Vector3 worldPos)
    {
        float x = (worldPos.x) / TileSize.x;
        float y = (worldPos.y) / TileSize.y;

        return new Vector3Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0);
    }
    #endregion
}