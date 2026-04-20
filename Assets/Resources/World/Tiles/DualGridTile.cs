using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DualGridTile", menuName = "ScriptableObjects/DualGridTile", order = 1)]
public class DualGridTile : ScriptableObject
{
    #region Static Stuff
    public static Dictionary<Tuple<bool, bool, bool, bool>, int> NeighbourRelations = SetNeighborRelations(false); //top left, top right, bot left, bot right
    public static Dictionary<Tuple<bool, bool, bool, bool>, int> WallNeighbourRelations = SetNeighborRelations(true);
    public static Dictionary<Tuple<bool, bool, bool, bool>, int> SetNeighborRelations(bool wall)
    {
        if(wall)
            return new() {
            {new (true, true, false, true), 0},
            {new (true, true, true, true), 1},
            {new (true, true, true, false), 2},
            {new (false, true, false, true), 3},
            {new (true, false, true, false), 5},
            {new (false, true, false, false), 6},
            {new (true, true, false, false), 7},
            {new (true, false, false, false), 8},
            {new (false, false, false, false), -1}, //Empty
            };
        return new() {
            {new (true, true, true, true), 6}, //Inner tile
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
    public bool AdjacentTileSameType(Vector3Int coords, out bool ghostReturn)
    {
        ghostReturn = false;
        var adjacentTileType = World.RealTileMap.Map.GetTile(coords);
        if (adjacentTileType == null)
            return false;
        if (adjacentTileType == TileType(GeneratingBorder))
            return true;
        DualGridTile tile = TileID.GetTileIDFromTile(adjacentTileType);
        if (IsWall)
        {
            if (TileID.WallTileRelations[TypeIndex, tile.TypeIndex] && World.SolidTile(coords))
                return true;
        }
        else if (GeneratingBorder)
        {
            if (tile.LayerOffset < LayerOffset && World.SolidTile(coords) && !tile.IsWall)
                ghostReturn = true;
        }
        else if (tile.LayerOffset < LayerOffset || World.SolidTile(coords))
            ghostReturn = true;
        return false;
    }
    public int CalculateDisplayTile(Vector3Int coords)
    {
        bool topRight = AdjacentTileSameType(coords -NEIGHBOURS[0], out bool ghostTopRight);
        bool topLeft = AdjacentTileSameType(coords -NEIGHBOURS[1], out bool ghostTopLeft);
        bool botRight = AdjacentTileSameType(coords -NEIGHBOURS[2], out bool ghostBotRight);
        bool botLeft = AdjacentTileSameType(coords -NEIGHBOURS[3], out bool ghostBotLeft);
        Tuple<bool, bool, bool, bool> neighbourTuple = new(topLeft || ghostTopLeft, topRight || ghostTopRight, botLeft || ghostBotLeft, botRight || ghostBotRight);
        int i = NeighbourRelations[neighbourTuple];
        if(i == 4 || i == 13) //weird double corner tiles do not consider ghosts
        {
            i = NeighbourRelations[new(topLeft, topRight, botLeft, botRight)];
        }
        return i;
    }
    public int CalculateDisplayWall(Vector3Int coords)
    {
        bool topRight = AdjacentTileSameType(coords -NEIGHBOURS[0], out bool ghostTopRight);
        bool topLeft = AdjacentTileSameType(coords -NEIGHBOURS[1], out bool ghostTopLeft);
        bool botRight = AdjacentTileSameType(coords -NEIGHBOURS[2], out bool ghostBotRight);
        bool botLeft = AdjacentTileSameType(coords -NEIGHBOURS[3], out bool ghostBotLeft);
        //These statements might not make sense if we use a .5 offset for our walls, as then another variant will be needed
        if (botRight)
            topRight = true;
        if (botLeft)
            topLeft = true;
        if((!topRight && !topLeft) || (botRight && botLeft))
            topRight = topLeft = true; //throw new Exception("ERROR: Wall placed in area without top tiles");
        Tuple<bool, bool, bool, bool> neighbourTuple = new(topLeft || ghostTopLeft, topRight || ghostTopRight, botLeft || ghostBotLeft, botRight || ghostBotRight);
        int i = WallNeighbourRelations[neighbourTuple];
        return i;
    }
    private static bool GeneratingBorder { get; set; } = false;
    public void UpdateDisplayTile(Vector3Int pos, Tilemap map, bool isBorder = false)
    {
        GeneratingBorder = isBorder;
        for (int i = 0; i < NEIGHBOURS.Length; i++)
        {
            Vector3Int newPos = pos + NEIGHBOURS[i];
            int id = IsWall ? CalculateDisplayWall(newPos) : CalculateDisplayTile(newPos);
            //if (id == 6 && BonusCenterVariants.Length > 0 )
            //{
                //float chanceOfAlternateTexture = 1f / (1f + BonusTileVariations.Length);
                //if (Utils.RandFloat() > chanceOfAlternateTexture)
                    //map.SetTile(newPos, BonusCenterVariants[Utils.RandInt(BonusCenterVariants.Length)]);
            //} else 
            if (id != -1)
            {
                if (BonusTileTextures != null && BonusTileTextures.Length > 0)
                    id += Utils.RandInt(BonusTileTextures.Length + 1) * SpriteCount;
                Tile t = DisplayTileVariants[id];
                map.SetTile(newPos, t);
            }
        }
        GeneratingBorder = false;
    }
    #region Scriptable Object Stuff
    public Texture2D TileTexture;
    public Texture2D[] BonusTileTextures;
    public float LayerOffset { get; set; } = 0;
    [SerializeField]
    private Tile RealTileMapVariant;
    [SerializeField]
    private Tile BorderTileMapVariant;
    //public Tile[] TilesThatCountForBlending;
    //public Sprite[] BonusTileVariations;
    public Color ColorModifier = Color.white;
    [SerializeField]
    private bool IsWall = false;
    #endregion
    public bool CountsAsWall() => IsWall;
    public int SpriteCount => IsWall ? 9 : 15;
    public int TypeIndex { get; set; }
    public Tile TileType(bool border) => border ? BorderTileMapVariant : RealTileMapVariant;
    public Tile FloorTileType => RealTileMapVariant;
    public Tile BorderTileType => BorderTileMapVariant;
    public List<Tile> DisplayTileVariants { get; private set; }
    public Tile[] BonusCenterVariants { get; private set; }
    public void Init()
    {
        SetDisplayVariants();
    }
    public void SetDisplayVariants()
    {
        if(TileTexture != null)
        {
            string path = IsWall ? $"World/Walls/{TileTexture.name}/" : $"World/Tiles/{TileTexture.name}/";
            Sprite[] sprites = Resources.LoadAll<Sprite>(path + TileTexture.name);
            int len = sprites.Length;
            if (len < 3)
                throw new Exception($"Could not find tile texture at \"{path + TileTexture.name}\". Is it possible that the texture is not the same name as the directory?");
            DisplayTileVariants = new(16);
            for (int i = 0; i < len; ++i)
            {
                Tile tile = ScriptableObject.CreateInstance<Tile>();
                tile.colliderType = Tile.ColliderType.Grid;
                tile.color = ColorModifier;
                tile.sprite = sprites[i];
                DisplayTileVariants.Add(tile);
            }
            if(BonusTileTextures != null)
            {
                for (int j = 0; j < BonusTileTextures.Length; ++j)
                {
                    sprites = Resources.LoadAll<Sprite>(path + BonusTileTextures[j].name);
                    len = sprites.Length;
                    if (len < 3)
                        throw new Exception($"Could not find tile texture at \"{path + BonusTileTextures[j].name}\". Is it possible that the texture is not the same name as the directory?");
                    for (int i = 0; i < len; ++i)
                    {
                        Tile tile = ScriptableObject.CreateInstance<Tile>();
                        tile.colliderType = Tile.ColliderType.Grid;
                        tile.color = ColorModifier;
                        tile.sprite = sprites[i];
                        DisplayTileVariants.Add(tile);
                    }
                }
            }
            //len = BonusTileVariations.Length;
            //BonusCenterVariants = new Tile[len];
            //for (int i = 0; i < len; ++i)
            //{
            //    Tile tile = ScriptableObject.CreateInstance<Tile>();
            //    tile.colliderType = Tile.ColliderType.Grid;
            //    tile.color = ColorModifier;
            //    tile.sprite = BonusTileVariations[i];
            //    BonusCenterVariants[i] = tile;
            //}
        }
    }
}