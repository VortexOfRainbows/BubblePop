using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTilemap : MonoBehaviour
{
    //public static OverlayMaterials OverlayMats => Resources.Load<OverlayMaterials>("Materials/OverlayShader/OverlayMaterials");
    //private static readonly Vector3Int[] Adjacencies = new Vector3Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(-1, -1), new(-1, 1), new(1, -1) };
    public static GameObject SnowPile;
    public static GameObject TallGrass;
    public static GameObject Mushroom;
    public static GameObject BubbleMushroom, BubblePlantObj;  
    public static GameObject VisualMapPrefab;
    public static GameObject CratePrefab, BarrelPrefab, UrnPrefab;
    public Transform FloorMapParent;
    public Transform WallMapParent;
    public Transform BorderMapParent;
    private Dictionary<int, Tilemap> DisplayMap;
    private Dictionary<int, Tilemap> BorderDisplayMap;
    private Dictionary<int, Tilemap> WallDisplayMap;
    public Tilemap Map;
    public void ClearDict(Dictionary<int, Tilemap> dict)
    {
        if (dict != null)
        {
            foreach (var kvp in dict)
                if (kvp.Value != null)
                    Destroy(kvp.Value.gameObject);
        }
    }
    public void Init()
    {
        ClearDict(DisplayMap);
        ClearDict(BorderDisplayMap);
        ClearDict(WallDisplayMap);
        VisualMapPrefab = VisualMapPrefab != null ? VisualMapPrefab : Resources.Load<GameObject>("World/Tiles/VisualMap");
        BubbleMushroom = BubbleMushroom != null ? BubbleMushroom : Resources.Load<GameObject>("World/Decor/Nature/BubbleMushroom");
        Mushroom = Mushroom != null ? Mushroom : Resources.Load<GameObject>("World/Decor/Nature/Mushroom");
        SnowPile = SnowPile != null ? SnowPile : Resources.Load<GameObject>("World/Decor/Snow/SnowClump");
        TallGrass = TallGrass != null ? TallGrass : Resources.Load<GameObject>("World/Decor/Nature/TallGrass");
        CratePrefab = CratePrefab != null ? CratePrefab : Resources.Load<GameObject>("World/Breakable/BreakableCrate");
        BarrelPrefab = BarrelPrefab != null ? BarrelPrefab : Resources.Load<GameObject>("World/Breakable/BreakableBarrel");
        UrnPrefab = UrnPrefab != null ? UrnPrefab : Resources.Load<GameObject>("World/Breakable/BreakableUrn");
        BubblePlantObj = BubblePlantObj != null ? BubblePlantObj : Resources.Load<GameObject>("World/Decor/Nature/BubblePlant");
        DisplayMap = new();
        BorderDisplayMap = new();
        WallDisplayMap = new();
        PrepareDisplayMap(FloorMapParent, DisplayMap);
        PrepareDisplayMap(BorderMapParent, BorderDisplayMap, border: true);
        PrepareDisplayMap(WallMapParent, WallDisplayMap, wall: true);
        AddDecor();
        NewFasterRefresh(DisplayMap, BorderDisplayMap, WallDisplayMap);
    }
    public static void PrepareDisplayMap(Transform Visual, Dictionary<int, Tilemap> DisplayMap, bool border = false, bool wall = false)
    {
        for (int k = 0; k < TileID.TileTypes.Count; ++k)
        {
            DualGridTile tile = TileID.TileTypes[k];
            Color c = border ? tile.BorderColor : Color.white;
            DisplayMap.Add(k, null);
            if (tile.CountsAsWall() == wall)
            {
                Tilemap t = Instantiate(VisualMapPrefab, Visual).GetComponent<Tilemap>();
                DisplayMap[k] = t;
                TilemapRenderer r = DisplayMap[k].GetComponent<TilemapRenderer>();
                float layerOffset = tile.LayerOffset;
                float wallGridTransform = 0;
                if(tile.CountsAsWall())
                {
                    wallGridTransform = -0.425f;
                    c = tile.BorderColor;
                }
                else if(border && tile.HasWallVariant())
                {
                    wallGridTransform = 0.25f;
                }
                else if(!border)
                {
                    r.sortingLayerID = World.FloorSortingLayer;
                }
                DisplayMap[k].transform.localPosition = new Vector3(0, wallGridTransform, layerOffset);

                //TEMPORARILY DISABLING GRASS SHADER FOR LIGHT TEST

                //TODO: Change this to not use string.Contains(string) this as the check system :sob:
                //if (tile.name.Contains("Grass")) // Applies overlay to tiles based on their names
                //{
                //    r.material = OverlayMats.Overlays[0];
                //}

                r.sortingOrder = -(int)layerOffset;
                if (wall)
                    t.gameObject.layer = 16; //Wall layer
                else if (border)
                    t.gameObject.layer = 14; //Border Layer
                t.gameObject.name = $"{(wall ? "WALL" : border ? "Solid" : "Floor")}[{k}]: {tile.name}";
                t.color = c;
            }
        }
    }
    public static bool TileIsNotSolidOrRendersBelow(int i, int j, float myLayerOffset)
    {
        ref var UnsafeData = ref World.UnsafeGetTileData(i, j);
        if (!UnsafeData.IsSolid)
            return true;
        var otherTile = UnsafeData.TileType;
        return otherTile.LayerOffset > myLayerOffset && !otherTile.HasWallVariant();
    }
    public static bool TileIsNotBlendableWall(int i, int j, float myLayerOffset)
    {
        return TileIsNotSolidOrRendersBelow(i + 1, j - 1, myLayerOffset) || TileIsNotSolidOrRendersBelow(i, j + 1, myLayerOffset) ||
               TileIsNotSolidOrRendersBelow(i + 1, j + 1, myLayerOffset) || TileIsNotSolidOrRendersBelow(i, j - 1, myLayerOffset) ||
               TileIsNotSolidOrRendersBelow(i - 1, j + 1, myLayerOffset) || TileIsNotSolidOrRendersBelow(i + 1, j, myLayerOffset) ||
               TileIsNotSolidOrRendersBelow(i - 1, j - 1, myLayerOffset) || TileIsNotSolidOrRendersBelow(i - 1, j, myLayerOffset);
    }
    public static void NewFasterRefresh(Dictionary<int, Tilemap> DisplayMap, Dictionary<int, Tilemap> BorderMap, Dictionary<int, Tilemap> WallMap)
    {
        World.GetCorners(out int left, out int right, out int bottom, out int top, 7);
        DualGridTile[] tileBuffer = new DualGridTile[4];
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                for(int k = 0; k < 4; ++k)
                {
                    int i2 = i - DualGridTile.NEIGHBOURS[k].x;
                    int j2 = j - DualGridTile.NEIGHBOURS[k].y;
                    ref World.TileData unsafeData = ref World.UnsafeGetTileData(i2, j2);
                    DualGridTile tile = tileBuffer[k] = unsafeData.TileType;
                    if (tile == null)
                        continue;
                    if (tile.CountsAsWall())
                        tile.MarkForWallUpdate = true;
                    else if (unsafeData.IsSolid)
                    {
                        tile.MarkForBorderUpdate = true;
                        if (!tile.MarkForSpecialBorderUpdate)
                            if (tile.HasWallVariant() && TileIsNotBlendableWall(i2, j2, tile.LayerOffset))
                                tile.MarkForSpecialBorderUpdate = true;
                    }
                    else
                        tile.MarkForUpdate = true;
                }
                for (int k = 0; k < 4; ++k)
                {
                    DualGridTile tile = tileBuffer[k];
                    if(tile != null)
                    {
                        if (tile.MarkForWallUpdate)
                        {
                            tile.UpdateDisplayTileSingular(i, j, tile.QueuedWallChangeData);
                            tile.MarkForUpdate = false;
                        }
                        if (tile.MarkForBorderUpdate)
                        {
                            tile.UpdateDisplayTileSingular(i, j, tile.QueuedBorderChangeData, true);
                            if(tile.MarkForSpecialBorderUpdate)
                            {
                                DualGridTile wall = tile.MyWallVariant();
                                wall.UpdateDisplayTileSingular(i, j, wall.QueuedWallChangeData);
                                tile.MarkForSpecialBorderUpdate = false;
                            }
                            tile.MarkForBorderUpdate = false;
                        }
                        if (tile.MarkForUpdate)
                        {
                            tile.UpdateDisplayTileSingular(i, j, tile.QueuedTileChangeData);
                            tile.MarkForUpdate = false;
                        }
                    }
                }
            }
        }

        foreach (DualGridTile tile in TileID.TileTypes)
        {
            if(tile.CountsAsWall())
            {
                WallMap[tile.TypeIndex].SetTiles(tile.QueuedWallChangeData.ToArray(), true);
            }
            else
            {
                BorderMap[tile.TypeIndex].SetTiles(tile.QueuedBorderChangeData.ToArray(), true);
                DisplayMap[tile.TypeIndex].SetTiles(tile.QueuedTileChangeData.ToArray(), true);
            }
            tile.QueuedWallChangeData.Clear();
            tile.QueuedBorderChangeData.Clear();
            tile.QueuedTileChangeData.Clear();
        }
    }
    public void AddDecor()
    {
        World.GetCorners(out int left, out int right, out int bottom, out int top, 15);
        Color borderColor = new(0.5f, 0.5f, 0.5f);
        Color[] StandardColors = new Color[] { Color.white, borderColor };
        Transform[] Parents = new Transform[] { World.Instance.FloorDecorParent, World.Instance.BorderDecorParent };
        int[] StandardOrders = new int[] { LayerHelper.FloorObjAndFloraSortingLayer, LayerHelper.SolidTileSortingOrder };
        bool mushroom;
        float mult;
        Color c;
        Transform parent;
        int order;
        bool border;
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                ref var UnsafeData = ref World.UnsafeGetTileData(i, j);
                border = UnsafeData.IsSolid;
                if (border)
                {
                    c = StandardColors[1];
                    parent = Parents[1];
                    order = StandardOrders[1];
                    mushroom = true;
                    mult = 0.5f;
                }
                else
                {
                    c = StandardColors[0];
                    parent = Parents[0];
                    order = StandardOrders[0];
                    mushroom = false;
                    mult = 1.0f;
                }
                DualGridTile t = UnsafeData.TileType;
                bool isGrassTile = t == TileID.Grass;
                bool isDirtTile = t == TileID.Dirt;
                bool isDarkGrass = t == TileID.DarkGrass;
                bool isSnowTile = t == TileID.Snow;
                Vector2 pos = new Vector3(i + 1, j + 1, 0);
                //if (border)
                //    pos.y += 0.25f;
                if(i % 3 == 0 && j % 3 == 0)
                {
                    AddSparseDecor(i + Utils.RandInt(2), j + Utils.RandInt(2));
                }
                if ((isGrassTile && Utils.RandFloat() < 0.16f * mult) || (isDarkGrass && Utils.RandFloat() < 0.04f))
                {
                    int type = Utils.RandInt(3);
                    pos.y += type == 0 ? Utils.RandFloat(0.1f, 0.3f) : type == 1 ? Utils.RandFloat(0.1f) : type == 2 ? Utils.RandFloat(0.05f, 0.25f) : 0;
                    var g = SpawnSmallDecor(TallGrass, parent, pos, Utils.RandFloat(0.9f, 1.0f), order, isDarkGrass ? borderColor : c);
                    if (type == 0)
                        g.sprite = Main.TextureAssets.TallGrass[Utils.RandInt(Main.TextureAssets.TallGrass.Length)];
                    if (type == 1)
                        g.sprite = Main.TextureAssets.Flowers[Utils.RandInt(Main.TextureAssets.Flowers.Length)];
                    if (type == 2)
                        g.sprite = Main.TextureAssets.ShortGrass[Utils.RandInt(Main.TextureAssets.ShortGrass.Length)];
                    g.flipX = Utils.rand.NextBool();
                    continue;
                }
                else if (isSnowTile && Utils.RandFloat() < 0.16f * mult)
                { 
                    bool edgeTile = (!border && World.SolidTile(i, j + 1)) || (border && (!World.SolidTile(i, j + 1) || !World.SolidTile(i, j - 1)));
                    if(!edgeTile)
                    {
                        pos += new Vector2(Utils.RandFloat(-0.05f, 0.05f), Utils.RandFloat(-0.05f, 0.05f));
                        var g = SpawnSmallDecor(SnowPile, parent, pos, Utils.RandFloat(0.9f, 1.0f), order, border ? TileID.Snow.BorderColor : c);
                        g.sprite = Main.TextureAssets.SnowPiles[Utils.RandInt(Main.TextureAssets.SnowPiles.Length)];
                        g.flipX = Utils.rand.NextBool();
                    }
                    continue;
                }
                if ((mushroom && (isGrassTile || isDirtTile)) || isDarkGrass)
                {
                    float chance = isDirtTile ? 0.1f : 0.05f;
                    if (Utils.RandFloat() < chance)
                    {
                        SpawnSmallDecor(Mushroom, parent, pos + Utils.RandCircle(0.2f), 1f, order, borderColor);
                        continue;
                    }
                }
                bool randomOccurence = Utils.RandFloat() < 0.05f && (isGrassTile || isDirtTile);
                if ((isDirtTile && i % 3 == 0 && j % 3 == 0) || randomOccurence)
                {
                    bool edgeTile = (!border && World.SolidTile(i, j + 1)) || (border && (!World.SolidTile(i, j + 1) || !World.SolidTile(i, j - 1)));
                    float chance = edgeTile ? 0.5f : randomOccurence ? 0 : 0.1f;
                    if (edgeTile)
                        pos.y += border ? 0.25f : -0.45f;
                    if (Utils.RandFloat() < chance)
                    {
                        Color c2 = border ? new Color(0.825f, 0.825f, 0.825f) : c;
                        var childR = SpawnSmallDecor(BubbleMushroom, parent, pos + Utils.RandCircle(0.2f), edgeTile ? Utils.RandFloat(0.9f, 1.0f) : Utils.RandFloat(0.7f, 0.9f), order, c2).transform.GetChild(0).GetComponent<SpriteRenderer>();
                        childR.color = c2.WithAlpha(0.8f);
                        childR.sortingOrder = order;
                        continue;
                    }
                }    
                if(((isGrassTile && border) || isDarkGrass) && i % 2 == 0 && j % 2 == 0 && Utils.RandFloat() < 0.08f)
                {
                    bool tileHasOppositeABitAway = (!border && (World.SolidTile(i, j + 2) || World.SolidTile(i, j - 2) || World.SolidTile(i + 2, j) || World.SolidTile(i - 2, j))) ||
                        (border && (!World.SolidTile(i, j + 2) || !World.SolidTile(i, j - 2) || !World.SolidTile(i - 2, j ) || !World.SolidTile(i + 2, j)));
                    if (tileHasOppositeABitAway)
                    {
                        var g = SpawnSmallDecor(BubblePlantObj, parent, pos + Utils.RandCircle(0.2f), Utils.rand.NextBool() ? 0.75f : 1.0f, order, border ? new Color(0.825f, 0.825f, 0.825f, 0.8f) : null);
                        g.flipX = Utils.rand.NextBool();
                        continue;
                    }
                }
            }
        }
    }
    public SpriteRenderer SpawnSmallDecor(GameObject prefab, Transform parent, Vector2 localPosition, float scaler, int sortingOrder, Color? color = null)
    {
        SpriteRenderer r = Instantiate(prefab, parent).GetComponent<SpriteRenderer>();
        r.transform.localPosition = localPosition;
        r.transform.localScale *= scaler;
        r.sortingOrder = sortingOrder;
        if (color.HasValue)
            r.color = color.Value;
        return r;
    }
    public void AddSparseDecor(int i, int j)
    {
        ref var data = ref World.UnsafeGetTileData(i, j);
        int order = 20;
        Color c = Color.white;
        bool border = data.IsSolid;
        if (border && data.TileType == TileID.Grass && Utils.RandFloat() < 0.55f)
        {
            int nonSolidTiles = 0;
            for(int x = -1; x <= 1; ++x)
            {
                for(int y = -1; y <= 3; ++y)
                {
                    if (!World.SolidTile(new Vector3Int(i + x, j + y)))
                        nonSolidTiles++;
                }
            }
            if (nonSolidTiles > 3)
                return;
            var list = Utils.RandFloat() < 0.3f ? Main.PrefabAssets.Stumps : Main.PrefabAssets.Trees;
            var g = Instantiate(list[Utils.RandInt(list.Count)], World.Instance.NatureParent.transform, true).GetComponent<SpriteRenderer>();
            g.transform.localPosition = new Vector2(i * 2 + 1, j * 2 + 1) + Utils.RandCircle(.5f);
            g.transform.localScale = new Vector3(g.transform.localScale.x * Utils.RandFloat(0.95f, 1.0f), g.transform.localScale.y * Utils.RandFloat(0.95f, 1.0f));
            g.color = c;
            g.sortingOrder = order;
            g.flipX = Utils.rand.NextBool();
        }
        else if(!border)
        {
            bool isBarrel = Utils.RandBool(5);
            bool isUrn = false;
            bool isStoneFloor = data.TileType == TileID.Cobblestone;
            bool isWoodFloor = data.TileType == TileID.Plank;
            int solidTiles = 1;
            int minimumSolidTiles = 1;
            if(isStoneFloor)
            {
                isUrn = !Utils.RandBool(3) && !isBarrel;
                if (isUrn)
                {
                    solidTiles += Utils.RandInt(1, 3);
                    minimumSolidTiles += Utils.RandInt(3);
                }
            }
            else if (!isWoodFloor)
            {
                if (Utils.RandBool(5))
                    return;
                bool isDarkGrassFloor = data.TileType == TileID.DarkGrass;
                isBarrel = true;
                if(!isDarkGrassFloor) //more likely to spawn on dark grass
                    solidTiles -= 1;
                if (data.IsRoadblock) //more likely to spawn in cooridors between rooms
                {
                    minimumSolidTiles++;
                    solidTiles += Utils.RandInt(1, 3);
                }
            }
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    if (World.SolidTile(new Vector3Int(i + x, j + y)))
                        solidTiles++;
                }
            }
            float chanceOfCrate = solidTiles / 9f;
            if (solidTiles <= minimumSolidTiles)
                return;
            else if(solidTiles <= 4)
                chanceOfCrate *= chanceOfCrate * 0.625f;
            else if(solidTiles < 6)
                chanceOfCrate *= chanceOfCrate * 1.125f;
            if (Utils.RandFloat() < chanceOfCrate)
            {
                GameObject prefab;
                Vector2 pos = new(i * 2 + 1, j * 2 + (isBarrel ? 0.5f : 1.1f));
                Vector2 scaleModifier = new(1, 1);
                if (isBarrel) 
                {
                    prefab = BarrelPrefab;
                    pos.y += 0.5f;
                    scaleModifier *= Utils.RandFloat(0.9f, 1.0f);
                }
                else if (isUrn)
                {
                    prefab = UrnPrefab;
                    pos.y += 0.5f;
                    scaleModifier *= Utils.RandFloat(0.8f, 1.0f);
                }
                else
                {
                    prefab = CratePrefab;
                    pos.y += 1.1f;
                    scaleModifier.x *= Utils.RandFloat(0.9f, 1.0f);
                    scaleModifier.y *= Utils.RandFloat(0.9f, 1.0f);
                }
                var g = Instantiate(prefab, World.Instance.NatureParent.transform, true).GetComponent<SpriteRenderer>();
                g.transform.localPosition = new Vector2(i * 2 + 1, j * 2 + (isBarrel ? 0.5f : 1.1f)) + Utils.RandCircle(.2f);
                g.transform.localScale = new Vector3(g.transform.localScale.x * scaleModifier.x, g.transform.localScale.y * scaleModifier.y, 1);
            }
        }
    }
}