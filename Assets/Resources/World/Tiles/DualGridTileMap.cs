using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class DualGridTilemap : MonoBehaviour
{
    public static GameObject TallGrass => Resources.Load<GameObject>("World/Decor/Nature/TallGrass");
    public static GameObject Mushroom => Resources.Load<GameObject>("World/Decor/Nature/Mushroom");
    public static GameObject VisualMapPrefab => Resources.Load<GameObject>("World/Tiles/VisualMap");
    public static OverlayMaterials OverlayMats => Resources.Load<OverlayMaterials>("Materials/OverlayShader/OverlayMaterials");
    public Transform Visual;
    public Transform DecorVisual;
    private Dictionary<int, Tilemap> DisplayMap;
    private Dictionary<int, Tilemap> BorderDisplayMap;
    private Dictionary<int, Tilemap> WallDisplayMap;
    public Tilemap Map;
    public void Init()
    {
        DisplayMap = new();
        BorderDisplayMap = new();
        WallDisplayMap = new();
        PrepareDisplayMap(Visual, DisplayMap, -50);
        AddDecor(Color.white, -20);
        //-49 is for occlusion for now
        PrepareDisplayMap(Visual, BorderDisplayMap, -48, border: true);
        AddDecor(new Color(0.5f, 0.5f, 0.5f), -30, true);
        PrepareDisplayMap(Visual, WallDisplayMap, -47, wall: true);
        RefreshDisplayTilemap(Map, DisplayMap, BorderDisplayMap, WallDisplayMap);
        //GetComponent<TilemapRenderer>().enabled = false;
    }
    public static void PrepareDisplayMap(Transform Visual, Dictionary<int, Tilemap> DisplayMap, int orderOffset, bool border = false, bool wall = false)
    {
        for (int k = 0; k < TileID.TileTypes.Count; ++k)
        {
            DualGridTile tile = TileID.TileTypes[k];
            Color c = border ? new Color(0.4f, 0.4f, 0.4f) : Color.white;
            DisplayMap.Add(k, null);
            if (tile.CountsAsWall() == wall)
            {
                Tilemap t = Instantiate(VisualMapPrefab, Visual).GetComponent<Tilemap>();
                DisplayMap[k] = t;
                TilemapRenderer r = DisplayMap[k].GetComponent<TilemapRenderer>();
                float layerOffset = tile.LayerOffset;
                float wallGridTransform = 0;
                int order = orderOffset;
                if(tile.CountsAsWall())
                {
                    wallGridTransform = -0.425f;
                    c = new(0.35f, 0.35f, 0.35f);
                }
                else if(border && tile.HasWallVariant())
                {
                    wallGridTransform = 0.25f;
                    c = new(0.75f, 0.75f, 0.75f);
                    order += 2; //TODO: make the player appear visually behind tiles if appropriate
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

                r.sortingOrder = order;
                t.gameObject.name = $"{(wall ? "WALL" : border ? "Solid" : "Floor")}[{k}]: {tile.name}";
                t.color = c;
            }
        }
    }
    //private static readonly Vector3Int[] Adjacencies = new Vector3Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(-1, -1), new(-1, 1), new(1, -1) };
    public static void RefreshDisplayTilemap(Tilemap Map, Dictionary<int, Tilemap> DisplayMap, Dictionary<int, Tilemap> BorderMap, Dictionary<int, Tilemap> WallMap)
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
                    DualGridTile tile = TileID.GetTileIDFromTile(t);
                    if(tile.CountsAsWall())
                        tile.UpdateDisplayTile(coords, WallMap[tile.TypeIndex]);
                    else if (World.SolidTile(coords))
                    {
                        tile.UpdateDisplayTile(coords, BorderMap[tile.TypeIndex], true);
                        if(tile.HasWallVariant())
                        {
                            DualGridTile wall = tile.MyWallVariant();
                            wall.UpdateDisplayTile(coords, WallMap[wall.TypeIndex]);
                        }
                    }
                    else
                        tile.UpdateDisplayTile(coords, DisplayMap[tile.TypeIndex]);
                }
            }
        }
    }
    public void AddDecor(Color c, int order, bool border = false)
    {
        Map.GetCorners(out int left, out int right, out int bottom, out int top);
        left += 10;
        right -= 10;
        bottom += 10;
        top -= 10;
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
                bool isGrassTile = t == TileID.Grass.TileType(border);
                bool isDirtTile = t == TileID.Dirt.TileType(border);
                if(i % 3 == 0 && j % 3 == 0)
                {
                    AddTrees(i + Utils.RandInt(2), j + Utils.RandInt(2));
                }
                if (isGrassTile && Utils.RandFloat() < 0.16f * mult)
                {
                    int type = Utils.RandInt(3);
                    var g = Instantiate(TallGrass, DecorVisual).GetComponent<SpriteRenderer>();
                    var pos = new Vector3(i + 1, j + 1, 0);
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