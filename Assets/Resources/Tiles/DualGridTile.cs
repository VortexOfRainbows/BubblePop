using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(fileName = "DualGridTile", menuName = "ScriptableObjects/DualGridTile", order = 1)]
public class DualGridTile : ScriptableObject
{
    #region static/global stuff
    public static Tilemap RealTileMap => DualGridTilemap.RealTileMap;
    public static Dictionary<Tuple<bool, bool, bool, bool>, int> NeighbourRelations = SetNeighborRelations();
    public static Dictionary<Tuple<bool, bool, bool, bool>, int> SetNeighborRelations()
    {
        return new() {
            {new (true, true, true, true), 6},
            {new (false, false, false, true), 12}, // OUTER_BOTTOM_RIGHT
            {new (false, false, true, false), 0}, // OUTER_BOTTOM_LEFT
            {new (false, true, false, false), 8}, // OUTER_TOP_RIGHT
            {new (true, false, false, false), 14}, // OUTER_TOP_LEFT
            {new (false, true, false, true), 1}, // EDGE_RIGHT
            {new (true, false, true, false), 11}, // EDGE_LEFT
            {new (false, false, true, true), 3}, // EDGE_BOTTOM
            {new (true, true, false, false), 9}, // EDGE_TOP
            {new (false, true, true, true), 5}, // INNER_BOTTOM_RIGHT
            {new (true, false, true, true), 2}, // INNER_BOTTOM_LEFT
            {new (true, true, false, true), 10}, // INNER_TOP_RIGHT
            {new (true, true, true, false), 7}, // INNER_TOP_LEFT
            {new (false, true, true, false), 13}, // DUAL_UP_RIGHT
            {new (true, false, false, true), 4}, // DUAL_DOWN_RIGHT
            {new (false, false, false, false), -1}, //Empty
        };
    }
    public static Vector3Int[] NEIGHBOURS = new Vector3Int[] {
        new(0, 0, 0),
        new(1, 0, 0),
        new(0, 1, 0),
        new(1, 1, 0)
    };
    #endregion
    public bool AdjacentTileSameType(Vector3Int coords, Vector3Int offset)
    {
        var adjacentTileType = RealTileMap.GetTile(coords + offset);
        if(TilesThatCountForBlending != null && TilesThatCountForBlending.Contains(adjacentTileType))
        {
            return true;
        }
        return adjacentTileType == RealTileMapVariant;
    }
    public int CalculateDisplayTile(Vector3Int coords)
    {
        bool topRight = AdjacentTileSameType(coords, -NEIGHBOURS[0]);
        bool botRight = AdjacentTileSameType(coords, -NEIGHBOURS[2]);
        bool topLeft = AdjacentTileSameType(coords, -NEIGHBOURS[1]);
        bool botLeft = AdjacentTileSameType(coords, -NEIGHBOURS[3]);
        Tuple<bool, bool, bool, bool> neighbourTuple = new(topLeft, topRight, botLeft, botRight);
        return NeighbourRelations[neighbourTuple];
    }
    public void UpdateDisplayTile(Vector3Int pos, Tilemap map)
    {
        for (int i = 0; i < NEIGHBOURS.Length; i++)
        {
            Vector3Int newPos = pos + NEIGHBOURS[i];
            int id = CalculateDisplayTile(newPos);
            map.SetTile(newPos, id != -1 ? DisplayTileVariants[CalculateDisplayTile(newPos)] : null);
        }
    }
    public Tile RealTileMapVariant;
    public Tile[] TilesThatCountForBlending;
    public Tile[] DisplayTileVariants;
    public void SetDisplayVariants()
    {
    }
}