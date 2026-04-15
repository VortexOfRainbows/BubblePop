using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileID
{
    private static int LoadIndexCount = 0;
    public static readonly List<DualGridTile> TileTypes = new();
    public static readonly Dictionary<TileBase, DualGridTile> TileToParentTile = new();
    public static readonly DualGridTile Dirt = Load("Dirt/DirtTile", 6);
    public static readonly DualGridTile Grass = Load("Grass/GrassTile", 4);
    public static readonly DualGridTile Cobblestone = Load("Cobblestone/CobblestoneTile", 2);
    public static readonly DualGridTile Plank = Load("Wood/WoodTile", 1);
    public static readonly DualGridTile DarkGrass = Load("DarkGrass/DarkGrassTile", 3);
    public static readonly DualGridTile TestTile = Load("TestTile/TestTile", 5);
    public static readonly DualGridTile WallTest = LoadWall("WallTest/WallTest", 0);
    public static DualGridTile Load(string path, float tileOrder = 0)
    {
        var tile = Resources.Load<DualGridTile>($"World/Tiles/{path}");
        tile.TypeIndex = LoadIndexCount++;
        tile.LayerOffset = tileOrder;

        TileTypes.Add(tile);
        TileToParentTile.Add(tile.FloorTileType, tile);
        TileToParentTile.Add(tile.BorderTileType, tile);
        return tile;
    }
    public static DualGridTile LoadWall(string path, float tileOrder = 0)
    {
        var tile = Resources.Load<DualGridTile>($"World/Walls/{path}");
        tile.TypeIndex = LoadIndexCount++;
        tile.LayerOffset = tileOrder;

        TileTypes.Add(tile);
        TileToParentTile.Add(tile.FloorTileType, tile);
        TileToParentTile.Add(tile.BorderTileType, tile);
        return tile;
    }
    public static DualGridTile GetTileIDFromTile(TileBase tile)
    {
        return TileToParentTile[tile];
    }
}
