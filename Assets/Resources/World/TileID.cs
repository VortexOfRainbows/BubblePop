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
    public static readonly DualGridTile WoodWall = LoadWall("WoodWall/WoodWall", 0);
    public static readonly DualGridTile GrassWall = LoadWall("GrassWall/GrassWall", 2);
    public static readonly DualGridTile StoneWall = LoadWall("StoneWall/StoneWall", 1);
    public static readonly DualGridTile DirtWall = LoadWall("DirtWall/DirtWall", 3);
    public static bool[,] WallTileRelations { get; private set; } = LoadWallTileRelations();
    private static bool[] HasWallTile { get; set; }
    private static DualGridTile[] MyWallTile { get; set; }
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
    public static bool[,] LoadWallTileRelations()
    {
        WallTileRelations = new bool[LoadIndexCount, LoadIndexCount];
        HasWallTile = new bool[LoadIndexCount];
        MyWallTile = new DualGridTile[LoadIndexCount];
        AddWallRelation(Plank, WoodWall);
        AddWallRelation(Grass, GrassWall);
        AddWallRelation(Cobblestone, StoneWall);
        AddWallRelation(Dirt, DirtWall);
        return WallTileRelations;
    }
    public static void AddWallRelation(DualGridTile tile, DualGridTile wallTile)
    {
        WallTileRelations[tile.TypeIndex, wallTile.TypeIndex] = true;
        WallTileRelations[wallTile.TypeIndex, tile.TypeIndex] = true;
        HasWallTile[tile.TypeIndex] = true;
        MyWallTile[tile.TypeIndex] = wallTile;
    }
    public static bool HasWallVariant(this DualGridTile tile)
    {
        return HasWallTile[tile.TypeIndex];
    }
    public static DualGridTile MyWallVariant(this DualGridTile tile)
    {
        return MyWallTile[tile.TypeIndex];
    }
    public static DualGridTile GetTileIDFromTile(TileBase tile)
    {
        return TileToParentTile[tile];
    }
}
