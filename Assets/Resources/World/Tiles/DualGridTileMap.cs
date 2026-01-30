using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DualGridTilemap : MonoBehaviour
{
    public static GameObject Flower => Resources.Load<GameObject>("World/Decor/Nature/WhiteFlower");
    public static GameObject Flower2 => Resources.Load<GameObject>("World/Decor/Nature/WhiteFlower2");
    public static GameObject Flower3 => Resources.Load<GameObject>("World/Decor/Nature/TallGrass");
    public static GameObject Mushroom => Resources.Load<GameObject>("World/Decor/Nature/Mushroom");
    public static GameObject VisualMapPrefab => Resources.Load<GameObject>("World/Tiles/VisualMap");
    public Transform Visual;
    public Transform DecorVisual;
    private List<Tilemap> DisplayMap;
    private List<Tilemap> BorderDisplayMap;
    public Tilemap Map;
    public void Init()
    {
        DisplayMap = new List<Tilemap>();
        BorderDisplayMap = new List<Tilemap>();
        World.GeneratingBorder = false;
        PrepareDisplayMap(Visual, DisplayMap, Color.white, -60);
        RefreshDisplayTilemap(Map, DisplayMap, false);
        AddDecor(Color.white, -20);
        World.GeneratingBorder = true;
        PrepareDisplayMap(Visual, BorderDisplayMap, new Color(0.4f, 0.4f, 0.4f), -55);
        RefreshDisplayTilemap(Map, BorderDisplayMap, true);
        AddDecor(new Color(0.4f, 0.4f, 0.4f), -30);
        World.GeneratingBorder = false;
        //GetComponent<TilemapRenderer>().enabled = false;
    }
    public static void PrepareDisplayMap(Transform Visual, List<Tilemap> DisplayMap, Color c, int orderOffset)
    {
        for (int k = 0; k < TileID.TileTypes.Count; ++k)
        {
            Tilemap t = Instantiate(VisualMapPrefab, Visual).GetComponent<Tilemap>();
            t.gameObject.name = $"{(World.GeneratingBorder ? "Solid" : "Floor")}[{k}]: {TileID.TileTypes[k].name}";
            t.color = c;
            DisplayMap.Add(t);
            DisplayMap[k].GetComponent<TilemapRenderer>().sortingOrder = orderOffset + TileID.TileTypes[k].LayerOffset;
        };
    }
    private static readonly Vector3Int[] Adjacencies = new Vector3Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(-1, -1), new(-1, 1), new(1, -1) };
    public static void RefreshDisplayTilemap(Tilemap Map, List<Tilemap> DisplayMap, bool border)
    {
        Map.GetCorners(out int left, out int right, out int bottom, out int top);
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                Vector3Int coords = new(i, j);
                var t = Map.GetTile(coords);
                if (t != null)
                {
                    if(border == World.SolidTile(coords))
                    {
                        DualGridTile tile = TileID.GetTileIDFromTile(t);
                        tile.UpdateDisplayTile(coords, DisplayMap[tile.TypeIndex]);
                        //for(int k = 0; k < 4; ++k)
                        //{
                        //    Vector3Int offset = coords + Adjacencies[k];
                        //    var t2 = Map.GetTile(offset);
                        //    if(t2 != t)
                        //        tile.UpdateDisplayTile(offset, DisplayMap[tile.TypeIndex], true);
                        //}
                    }
                }
            }
        }
    }
    public void AddDecor(Color c, int order)
    {
        Map.GetCorners(out int left, out int right, out int bottom, out int top);
        bool mushroom = false;
        float mult = 1.0f;
        if (order == -30)
        {
            mushroom = true;
            mult = 0.5f;
        }
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                TileBase t = Map.GetTile(i, j);
                bool isGrassTile = t == TileID.Grass.TileType;
                bool isDirtTile = t == TileID.Dirt.TileType;
                if(i % 3 == 0 && j % 3 == 0)
                {
                    AddTrees(i + Utils.RandInt(2), j + Utils.RandInt(2));
                }
                if (isGrassTile && Utils.RandFloat() < 0.1f * mult)
                {
                    var g = Instantiate(Utils.RandInt(3) != 0 ? Flower3 : Utils.RandInt(2) == 0 ? Flower2 : Flower, DecorVisual).GetComponent<SpriteRenderer>();
                    g.transform.localPosition = new Vector3(i + 1, j + 1, 0);
                    g.color = c;
                    g.sortingOrder = order;
                    g.flipX = Utils.rand.NextBool();
                    continue;
                }
                if (mushroom && (isGrassTile || isDirtTile))
                {
                    float chance = isDirtTile ? 0.1f : 0.05f;
                    if (Utils.RandFloat() < chance)
                    {
                        var g = Instantiate(Mushroom, DecorVisual).GetComponent<SpriteRenderer>();
                        g.transform.localPosition = new Vector3(i + 1, j + 1, 0) + (Vector3)Utils.RandCircle(0.2f);
                        g.color = c;
                        g.sortingOrder = order;
                    }
                    continue;
                }
            }
        }
    }
    public void AddTrees(int i, int j)
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
                    if (!World.SolidTile(new Vector3Int(i + x, j + y)) && World.RealTileMap.Map.GetTile(i, j) != TileID.DarkGrass.FloorTileType)
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
    }
}