using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileID
{
    private static int LoadIndexCount = 0;
    public static readonly List<DualGridTile> TileTypes = new();
    public static readonly Dictionary<TileBase, DualGridTile> TileToParentTile = new();
    public static readonly DualGridTile Dirt = Load("Dirt/DirtTile");
    public static readonly DualGridTile Grass = Load("Grass/GrassTile");
    public static readonly DualGridTile Cobblestone = Load("Cobblestone/CobblestoneTile");
    public static readonly DualGridTile Plank = Load("Wood/WoodTile");
    public static readonly DualGridTile DarkGrass = Load("DarkGrass/DarkGrassTile");
    public static DualGridTile Load(string path)
    {
        var tile = Resources.Load<DualGridTile>($"World/Tiles/{path}");
        tile.TypeIndex = LoadIndexCount++;

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
