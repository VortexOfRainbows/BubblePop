using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TilePathfinding : MonoBehaviour
{
    [Header("Pathfinding Settings")]
    public float maxDistance = 160;
    [SerializeField] Tilemap nodeTilemap;

    [Header("Debugging")]
    public bool DrawGroundPath = true;
    public bool DrawLiquidPath = false;

    // Used for gizmos to draw
    private List<Vector3Int> visitedList = new List<Vector3Int>();

    // Adds nodes neighbors to check
    private Queue<Vector3Int> openSet = new Queue<Vector3Int>();

    // Allows for a quick check if node has been visited
    private int CurrentRunID = 0;

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
            Vector3Int plrTilePos = World.RealPosToTilePos(Player.AllPlayers[i].Position);

            if (plrTilePos != plrPositions[i])
            {
                pathfind();
                plrPositions[i] = plrTilePos;
                return;
            }
        }
    }

    private void pathfind()
    {
        // Sets data sets for use
        openSet.Clear();
        visitedList.Clear();
        ++CurrentRunID;

        // Loops through each player for start tiles
        foreach (Player plr in Player.AllPlayers)
        {
            Vector3Int plrTilePos = World.RealPosToTilePos(plr.Position);
            ref World.TileData plrTile = ref World.UnsafeGetTileData(plrTilePos);

            // Check if node is visited, only matters if multiple players are on the same tile
            if (plrTile.RunID != CurrentRunID)
            {
                plrTile.RunID = CurrentRunID;
                plrTile.GroundDistance = 0f;
                plrTile.GroundDirection = Vector2.zero;
                plrTile.LiquidDistance = 0f;
                plrTile.LiquidDirection = Vector2.zero;
                plrTile.LiquidTiles = 0f;

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
                // Finds position of neighbor and gets tile
                int neighborX = curTilePos.x + offsetX[i];
                int neighborY = curTilePos.y + offsetY[i];
                Vector3Int neighborTilePos = new Vector3Int(neighborX, neighborY, 0);
                ref World.TileData neighborTile = ref World.UnsafeGetTileData(neighborTilePos);

                // Skips if not a valid tile
                if (neighborTile.IsSolid) continue;

                // Sets distance of tile if it is straight or diagonal
                float stepCost = (i < 4) ? 1.0f : 1.414214f;
                float newGroundDistance = curTile.GroundDistance + stepCost;
                float newLiquidDistance = curTile.LiquidDistance + stepCost;

                bool isUnvisited = neighborTile.RunID != CurrentRunID;
                bool passed = false;

                // Check if isn't liquid, distance is less than max, and if tile has not yet been visited or if new path is better
                if ((!neighborTile.IsLiquid) && ((newGroundDistance < maxDistance && (isUnvisited || newGroundDistance < neighborTile.GroundDistance)) || (isUnvisited)))
                {
                    neighborTile.GroundDistance = newGroundDistance;
                    neighborTile.GroundDirection = -neighborDirection[i];
                    passed = true;
                }
                // Check if distance is less than max, and if tile has not yet been visited or if new path is better
                if ((newLiquidDistance < maxDistance && (isUnvisited || newLiquidDistance < neighborTile.LiquidDistance)) || isUnvisited)
                {
                    neighborTile.LiquidDistance = newLiquidDistance;
                    neighborTile.LiquidDirection = -neighborDirection[i];
                    // Add to liquid tiles if tile is liquid
                    if (neighborTile.IsLiquid)
                    {
                        neighborTile.LiquidTiles = curTile.LiquidTiles + stepCost;
                        neighborTile.GroundDistance = float.MaxValue; // TODO: Might be able to remove once fixing ground pathing calculation
                    }
                    passed = true;
                }

                if (passed)
                {
                    neighborTile.RunID = CurrentRunID;
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

    public static Vector2 BestPathToPlayer(Vector3 entityPos, PathfindingType pathingType, float liquidModifier)
    {
        // Raycast to player over ground
        //  Go if able

        // Get tile to compare distances
        Vector3Int tilePos = World.RealPosToTilePos(entityPos);
        ref World.TileData curTile = ref World.UnsafeGetTileData(tilePos.x, tilePos.y);
        Vector2 bestDirection = curTile.GroundDirection;

        switch ((int)pathingType)
        {
            case 0: // Ground only Traversal
                break;

            case 1: // Ground and Liquid Traversal
                // Raycast over ground + liquid
                //  Apply modifier for distance over liquid tiles

                // Get pathfinding over liquid distance, apply modifier
                //  Compare distance of raycast vs both pathfinds and choose lowest cost
                if (curTile.LiquidDistance - curTile.LiquidTiles + curTile.LiquidTiles * liquidModifier < curTile.GroundDistance)
                {
                    bestDirection = curTile.LiquidDirection;
                }
                break;

            case 2: // Flying Traversal
                // Raycast over anything other than walls
                //  Go if able
                // Get pathfinding over everything distance
                //  Go on everything pathfind

                // TODO: Switch for air version
                if ((bestDirection == Vector2.zero) || (curTile.LiquidDistance - curTile.LiquidTiles + curTile.LiquidTiles * liquidModifier < curTile.GroundDistance))
                {
                    bestDirection = curTile.LiquidDirection;
                }
                
                break;
        }

        return bestDirection;
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3Int visitedPos in visitedList)
        {
            ref World.TileData tile = ref World.UnsafeGetTileData(visitedPos);
            Vector3 worldPos = World.CenterOfTile(visitedPos);

            if (DrawGroundPath && tile.GroundDirection != Vector2.zero)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(worldPos, (Vector3)tile.GroundDirection * World.TileSize.x * 0.4f);
            }
            if (DrawLiquidPath && tile.LiquidDirection != Vector2.zero)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(worldPos, (Vector3)tile.LiquidDirection * World.TileSize.x * 0.4f);
            }
        }
    }
}