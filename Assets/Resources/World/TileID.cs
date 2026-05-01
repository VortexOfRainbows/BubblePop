using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileID
{
    private static int LoadIndexCount = 0;
    private static float BottomTileOrder = 0;
    public static Queue<Tuple<DualGridTile, DualGridTile>> WallTileRelationQueue = new();
    public static readonly List<DualGridTile> TileTypes = new();
    public static readonly Dictionary<TileBase, DualGridTile> TileToParentTile = new();
    public static readonly DualGridTile Dirt = LoadTile("Dirt/DirtTile", 6);
    public static readonly DualGridTile Grass = LoadTile("Grass/GrassTile", 4);
    public static readonly DualGridTile Cobblestone = LoadTile("Cobblestone/CobblestoneTile", 2);
    public static readonly DualGridTile Plank = LoadTile("Wood/WoodTile", 1);
    public static readonly DualGridTile DarkGrass = LoadTile("DarkGrass/DarkGrassTile", 3);
    public static readonly DualGridTile TestTile = LoadTile("TestTile/TestTile", 5);


    public static readonly DualGridTile WoodWall = LoadWall("WoodWall/WoodWall", Plank);
    public static readonly DualGridTile GrassWall = LoadWall("GrassWall/GrassWall", Grass);
    public static readonly DualGridTile StoneWall = LoadWall("StoneWall/StoneWall", Cobblestone);
    public static readonly DualGridTile DirtWall = LoadWall("DirtWall/DirtWall", Dirt);
    public static bool[,] WallTileRelations { get; private set; } = LoadWallTileRelations();
    private static bool[] HasWallTile { get; set; }
    private static DualGridTile[] MyWallTile { get; set; }
    public static DualGridTile LoadTile(string path, float tileOrder = 0)
    {
        var tile = Resources.Load<DualGridTile>($"World/Tiles/{path}");
        tile.TypeIndex = LoadIndexCount++;
        tile.LayerOffset = tileOrder;
        BottomTileOrder = Math.Max(BottomTileOrder, tileOrder);

        TileTypes.Add(tile);
        TileToParentTile.Add(tile.FloorTileType, tile);
        TileToParentTile.Add(tile.BorderTileType, tile);
        return tile;
    }
    public static DualGridTile LoadWall(string path, DualGridTile partnerTile)
    {
        var tile = Resources.Load<DualGridTile>($"World/Walls/{path}");
        tile.TypeIndex = LoadIndexCount++;
        tile.LayerOffset = BottomTileOrder + partnerTile.LayerOffset;

        TileTypes.Add(tile);
        TileToParentTile.Add(tile.FloorTileType, tile);
        TileToParentTile.Add(tile.BorderTileType, tile);
        WallTileRelationQueue.Enqueue(new(partnerTile, tile));
        return tile;
    }
    public static bool[,] LoadWallTileRelations()
    {
        WallTileRelations = new bool[LoadIndexCount, LoadIndexCount];
        HasWallTile = new bool[LoadIndexCount];
        MyWallTile = new DualGridTile[LoadIndexCount];
        while(WallTileRelationQueue.TryDequeue(out Tuple<DualGridTile, DualGridTile> r))
            AddWallRelation(r.Item1, r.Item2);
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
