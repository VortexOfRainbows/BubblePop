using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DualGridTilemap : MonoBehaviour
{
    public static GameObject SnowPile;
    public static GameObject TallGrass;
    public static GameObject Mushroom;
    public static GameObject BubbleMushroom;  
    public static GameObject VisualMapPrefab;
    public static GameObject CratePrefab;
    //public static OverlayMaterials OverlayMats => Resources.Load<OverlayMaterials>("Materials/OverlayShader/OverlayMaterials");
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

        DisplayMap = new();
        BorderDisplayMap = new();
        WallDisplayMap = new();
        PrepareDisplayMap(FloorMapParent, DisplayMap);
        AddDecor(false);
        //-49 is for occlusion for now
        PrepareDisplayMap(BorderMapParent, BorderDisplayMap, border: true);
        AddDecor(true);
        PrepareDisplayMap(WallMapParent, WallDisplayMap, wall: true);
        NewFasterRefresh(Map, DisplayMap, BorderDisplayMap, WallDisplayMap);
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
    //private static readonly Vector3Int[] Adjacencies = new Vector3Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(-1, -1), new(-1, 1), new(1, -1) };
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
    public static void NewFasterRefresh(Tilemap Map, Dictionary<int, Tilemap> DisplayMap, Dictionary<int, Tilemap> BorderMap, Dictionary<int, Tilemap> WallMap)
    {
        World.GetCorners(out int left, out int right, out int bottom, out int top, 7);
        DualGridTile[] tileBuffer = new DualGridTile[4];
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                Vector3Int coords = new(i, j);
                for(int k = 0; k < 4; ++k)
                {
                    Vector3Int trueC = coords - DualGridTile.NEIGHBOURS[k];
                    ref World.TileData unsafeData = ref World.UnsafeGetTileData(trueC.x, trueC.y);
                    DualGridTile tile = tileBuffer[k] = unsafeData.TileType;
                    if (tile == null)
                        continue;
                    if (tile.CountsAsWall())
                        tile.MarkForWallUpdate = true;
                    else if (unsafeData.IsSolid)
                    {
                        tile.MarkForBorderUpdate = true;
                        if (!tile.MarkForSpecialBorderUpdate)
                            if (tile.HasWallVariant() && TileIsNotBlendableWall(trueC.x, trueC.y, tile.LayerOffset))
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
                            tile.UpdateDisplayTileSingular(coords, tile.QueuedWallChangeData);
                            tile.MarkForUpdate = false;
                        }
                        if (tile.MarkForBorderUpdate)
                        {
                            tile.UpdateDisplayTileSingular(coords, tile.QueuedBorderChangeData, true);
                            if(tile.MarkForSpecialBorderUpdate)
                            {
                                DualGridTile wall = tile.MyWallVariant();
                                wall.UpdateDisplayTileSingular(coords, wall.QueuedWallChangeData);
                                tile.MarkForSpecialBorderUpdate = false;
                            }
                            tile.MarkForBorderUpdate = false;
                        }
                        if (tile.MarkForUpdate)
                        {
                            tile.UpdateDisplayTileSingular(coords, tile.QueuedTileChangeData);
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
    public void AddDecor(bool border)
    {
        Color borderColor = new(0.5f, 0.5f, 0.5f);
        Color c = border ? borderColor : Color.white;
        Transform Parent = border ? World.Instance.BorderDecorParent : World.Instance.FloorDecorParent;
        World.GetCorners(out int left, out int right, out int bottom, out int top, 15);
        int order = border ? LayerHelper.SolidTileSortingOrder : LayerHelper.FloorObjAndFloraSortingLayer;
        bool mushroom = false;
        float mult = 1.0f;
        if (border)
        {
            mushroom = true;
            mult = 0.5f;
        }
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                TileBase t = Map.GetTile(i, j);
                bool isGrassTile = t == TileID.Grass.TileType(border);
                bool isDirtTile = t == TileID.Dirt.TileType(border);
                bool isDarkGrass = t == TileID.DarkGrass.TileType(border);
                bool isSnowTile = t == TileID.Snow.TileType(border);
                var pos = new Vector3(i + 1, j + 1, 0);
                if(i % 3 == 0 && j % 3 == 0)
                {
                    AddSparseDecor(i + Utils.RandInt(2), j + Utils.RandInt(2));
                }
                if ((isGrassTile && Utils.RandFloat() < 0.16f * mult) || (isDarkGrass && Utils.RandFloat() < 0.04f))
                {
                    int type = Utils.RandInt(3);
                    var g = Instantiate(TallGrass, Parent).GetComponent<SpriteRenderer>();
                    if (type == 0)
                    {
                        g.sprite = Main.TextureAssets.TallGrass[Utils.RandInt(Main.TextureAssets.TallGrass.Length)];
                        pos.y += Utils.RandFloat(0.1f, 0.3f);
                    }
                    if (type == 1)
                    {
                        g.sprite = Main.TextureAssets.Flowers[Utils.RandInt(Main.TextureAssets.Flowers.Length)];
                        pos.y += Utils.RandFloat(0.1f);
                    }
                    if (type == 2)
                    {
                        g.sprite = Main.TextureAssets.ShortGrass[Utils.RandInt(Main.TextureAssets.ShortGrass.Length)];
                        pos.y += Utils.RandFloat(0.05f, 0.25f);
                    }
                    g.transform.localPosition = pos;
                    g.color = isDarkGrass ? borderColor : c;
                    g.sortingOrder = order;
                    g.flipX = Utils.rand.NextBool();
                    continue;
                }
                else if (isSnowTile && Utils.RandFloat() < 0.16f * mult)
                { 
                    bool edgeTile = (!border && World.SolidTile(i, j + 1)) || (border && (!World.SolidTile(i, j + 1) || !World.SolidTile(i, j - 1)));
                    if(!edgeTile)
                    {
                        var g = Instantiate(SnowPile, Parent).GetComponent<SpriteRenderer>();
                        pos.y += Utils.RandFloat(-0.05f, 0.05f);
                        pos.x += Utils.RandFloat(-0.05f, 0.05f);
                        g.sprite = Main.TextureAssets.SnowPiles[Utils.RandInt(Main.TextureAssets.SnowPiles.Length)];
                        g.transform.localPosition = pos;
                        g.color = border ? TileID.Snow.BorderColor : c;
                        g.sortingOrder = order;
                        g.flipX = Utils.rand.NextBool();
                        g.transform.localScale *= Utils.RandFloat(0.9f, 1.0f);
                    }
                    continue;
                }
                if ((mushroom && (isGrassTile || isDirtTile)) || isDarkGrass)
                {
                    float chance = isDirtTile ? 0.1f : 0.05f;
                    if (Utils.RandFloat() < chance)
                    {
                        var g = Instantiate(Mushroom, Parent).GetComponent<SpriteRenderer>();
                        g.transform.localPosition = pos + (Vector3)Utils.RandCircle(0.2f);
                        g.color = borderColor;
                        g.sortingOrder = order;
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
                        var g = Instantiate(BubbleMushroom, Parent).GetComponent<SpriteRenderer>();
                        var childR = g.transform.GetChild(0).GetComponent<SpriteRenderer>();
                        g.transform.localPosition = pos + (Vector3)Utils.RandCircle(0.2f);
                        g.transform.localScale *= edgeTile ? Utils.RandFloat(0.9f, 1.0f) : Utils.RandFloat(0.7f, 0.9f);
                        g.color = c2;
                        childR.color = c2.WithAlpha(0.8f);
                        g.sortingOrder = childR.sortingOrder = order;
                        continue;
                    }
                }    
            }
        }
    }
    public void AddSparseDecor(int i, int j)
    {
        TileBase t = Map.GetTile(i, j);
        int order = 20;
        Color c = Color.white;
        bool isGrassTile = t == TileID.Grass.BorderTileType;
        if (isGrassTile && Utils.RandFloat() < 0.55f)
        {
            int nonSolidTiles = 0;
            for(int x = -1; x <= 1; ++x)
            {
                for(int y = -1; y <= 3; ++y)
                {
                    if (!World.SolidTile(new Vector3Int(i + x, j + y)) && World.GetTile(i, j) != TileID.DarkGrass.FloorTileType)
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
        else if(t == TileID.Plank.FloorTileType || (t == TileID.Cobblestone.FloorTileType && Utils.RandBool(2)))
        {
            int solidTiles = 1;
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    if (World.SolidTile(new Vector3Int(i + x, j + y)))
                        solidTiles++;
                }
            }
            float chanceOfCrate = solidTiles / 9f;
            if (solidTiles <= 1)
                return;
            else if(solidTiles <= 4)
                chanceOfCrate *= chanceOfCrate * 0.6f;
            else if(solidTiles < 6)
                chanceOfCrate *= chanceOfCrate * 1.1f;
            if (Utils.RandFloat() < chanceOfCrate)
            {
                var g = Instantiate(CratePrefab, World.Instance.NatureParent.transform, true).GetComponent<SpriteRenderer>();
                g.transform.localPosition = new Vector2(i * 2 + 1, j * 2 + 1.1f) + Utils.RandCircle(.2f);
                g.transform.localScale = new Vector3(g.transform.localScale.x * Utils.RandFloat(0.9f, 1.0f), g.transform.localScale.y * Utils.RandFloat(0.9f, 1.0f));
            }
        }
    }
}