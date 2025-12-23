using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DualGridTilemap : MonoBehaviour
{
    public static GameObject Flower => Resources.Load<GameObject>("World/Decor/Nature/WhiteFlower");
    public static GameObject Mushroom => Resources.Load<GameObject>("World/Decor/Nature/Mushroom");
    public static GameObject VisualMapPrefab => Resources.Load<GameObject>("World/Tiles/VisualMap");
    public GameObject Visual;
    private List<Tilemap> FloorMaps;
    private List<Tilemap> BorderMaps;
    public Tilemap Map;
    public void Init(Color c, int orderOffset = -50)
    {
        BorderMaps = new List<Tilemap>();
        for (int k = 0; k < World.Instance.TileTypes.Length; ++k) {
            Tilemap t = Instantiate(VisualMapPrefab, Visual.transform).GetComponent<Tilemap>();
            BorderMaps.Add(t);
            t.color = c;
            BorderMaps[k].GetComponent<TilemapRenderer>().sortingOrder = orderOffset + World.Instance.TileTypes[k].LayerOffset;
        };
        RefreshDisplayTilemap();

        if(orderOffset == -50)
            AddDecor(c, -20);
        //else if (orderOffset == -49)
        //    AddDecor(new Color(0.8f, 0.8f, 0.8f), -30);
    }
    public void RefreshDisplayTilemap()
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
                    if (World.Instance.TileTypes[k].RealTileMapVariant == t)
                    {
                        World.Instance.TileTypes[k].UpdateDisplayTile(coords, BorderMaps[k]);
                        if (k != 1) //THIS IS THE DIRT LAYER
                            World.Instance.TileTypes[1].UpdateDisplayTile(coords, BorderMaps[1]); //MAKE IT SO DIRT IS UNDER EVERYTHING TO PREVENT CERTAIN VISUAL ISSUES. MAYBE TEMPORARY
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
        Tile grassTile = World.Instance.TileTypes[grassLayer].RealTileMapVariant; //This should be replaced with a more robust system later, like indexing all tiles in an array kept in world
        Tile dirtTile = World.Instance.TileTypes[dirtLayer].RealTileMapVariant; //This should be replaced with a more robust system later, like indexing all tiles in an array kept in world
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                bool isGrassTile = Map.GetTile(i, j) == grassTile;
                if (isGrassTile && Utils.RandFloat() < 0.05f * mult)
                {
                    var g = Instantiate(Flower, Visual.transform).GetComponent<SpriteRenderer>();
                    g.transform.localPosition = new Vector3(i + 1, j + 1, 0);
                    g.color = c;
                    g.sortingOrder = order;
                    continue;
                }
                if (mushroom && (isGrassTile || Map.GetTile(i, j) == dirtTile))
                {
                    if (Utils.RandFloat() < 0.05f)
                    {
                        var g = Instantiate(Mushroom, Visual.transform).GetComponent<SpriteRenderer>();
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