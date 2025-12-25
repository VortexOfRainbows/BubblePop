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
        PrepareDisplayMap(Visual, DisplayMap, Color.white, -50);
        RefreshDisplayTilemap(Map, DisplayMap, false);
        AddDecor(Color.white, -20);
        World.GeneratingBorder = true;
        PrepareDisplayMap(Visual, BorderDisplayMap, new Color(0.4f, 0.4f, 0.4f), -49);
        RefreshDisplayTilemap(Map, BorderDisplayMap, true);
        AddDecor(new Color(0.8f, 0.8f, 0.8f), -30);
        World.GeneratingBorder = false;
        //GetComponent<TilemapRenderer>().enabled = false;
    }
    public static void PrepareDisplayMap(Transform Visual, List<Tilemap> DisplayMap, Color c, int orderOffset)
    {
        for (int k = 0; k < World.Instance.TileTypes.Length; ++k)
        {
            Tilemap t = Instantiate(VisualMapPrefab, Visual).GetComponent<Tilemap>();
            t.gameObject.name = $"{(World.GeneratingBorder ? "Solid" : "Floor")}[{k}]: {World.Instance.TileTypes[k].name}";
            t.color = c;
            DisplayMap.Add(t);
            DisplayMap[k].GetComponent<TilemapRenderer>().sortingOrder = orderOffset + World.Instance.TileTypes[k].LayerOffset;
        };
    }
    public static void RefreshDisplayTilemap(Tilemap Map, List<Tilemap> DisplayMap, bool border)
    {
        Map.GetCorners(out int left, out int right, out int bottom, out int top);
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                Vector3Int coords = new(i, j);
                for (int k = 0; k < World.Instance.TileTypes.Length; ++k) //TODO: Replace this lookup with a dictionary for better efficiency 
                {
                    var t = Map.GetTile(coords);
                    if (World.Instance.TileTypes[k].TileType == t)
                    {
                        World.Instance.TileTypes[k].UpdateDisplayTile(coords, DisplayMap[k]);
                        break;
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
        int grassLayer = 2;
        int dirtLayer = 1;
        Tile grassTile = World.Instance.TileTypes[grassLayer].TileType; //This should be replaced with a more robust system later, like indexing all tiles in an array kept in world
        Tile dirtTile = World.Instance.TileTypes[dirtLayer].TileType; //This should be replaced with a more robust system later, like indexing all tiles in an array kept in world
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                bool isGrassTile = Map.GetTile(i, j) == grassTile;
                if (isGrassTile && Utils.RandFloat() < 0.1f * mult)
                {
                    var g = Instantiate(Utils.RandInt(3) != 0 ? Flower3 : Utils.RandInt(2) == 0 ? Flower2 : Flower, DecorVisual).GetComponent<SpriteRenderer>();
                    g.transform.localPosition = new Vector3(i + 1, j + 1, 0);
                    g.color = c;
                    g.sortingOrder = order;
                    g.flipX = Utils.rand.NextBool();
                    continue;
                }
                if (mushroom && (isGrassTile || Map.GetTile(i, j) == dirtTile))
                {
                    if (Utils.RandFloat() < 0.05f)
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
}