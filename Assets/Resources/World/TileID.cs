using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileID
{
    private static int LoadIndexCount = 0;
    public static readonly List<DualGridTile> TileTypes = new();
    public static readonly Dictionary<TileBase, DualGridTile> TileToParentTile = new();
    public static readonly DualGridTile Dirt = Load("Dirt/DirtTile", 0);
    public static readonly DualGridTile Grass = Load("Grass/GrassTile", 2);
    public static readonly DualGridTile Cobblestone = Load("Cobblestone/CobblestoneTile", 4);
    public static readonly DualGridTile Plank = Load("Wood/WoodTile", 5);
    public static readonly DualGridTile DarkGrass = Load("DarkGrass/DarkGrassTile", 3);
    public static readonly DualGridTile TestTile = Load("TestTile/TestTile", 1);
    public static DualGridTile Load(string path, int tileOrder = 0)
    {
        var tile = Resources.Load<DualGridTile>($"World/Tiles/{path}");
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
