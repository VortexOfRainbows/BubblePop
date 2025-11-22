using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DualGridTilemap : MonoBehaviour
{
    public static GameObject Flower => Resources.Load<GameObject>("World/Decor/Nature/WhiteFlower");
    public static GameObject Mushroom => Resources.Load<GameObject>("World/Decor/Nature/Mushroom");
    public static readonly int WorldSize = 100;
    public static Vector3Int[] NEIGHBOURS => DualGridTile.NEIGHBOURS;
    public Tilemap Map => m_RealTileMap;
    public GameObject VisualMapPrefab;
    public GameObject Visual;
    private List<Tilemap> VisualMaps;
    public Tilemap m_RealTileMap;
    // Provide the 16 tiles in the inspector
    // The tiles on the display tilemap will recalculate themselves based on the placeholder tilemap
    public void Init(Color c, int orderOffset = -50)
    {
        VisualMaps = new List<Tilemap>();
        for (int k = 0; k < World.Instance.Tiles.Length; ++k) {
            Tilemap t = Instantiate(VisualMapPrefab, Visual.transform).GetComponent<Tilemap>();
            VisualMaps.Add(t);
            t.color = c;
            VisualMaps[k].GetComponent<TilemapRenderer>().sortingOrder = orderOffset + World.Instance.Tiles[k].LayerOffset;
        };
        RefreshDisplayTilemap();
        if(orderOffset == -50)
            AddDecor(c, -20);
        if (orderOffset == -49)
            AddDecor(new Color(0.8f, 0.8f, 0.8f), -30);
        m_RealTileMap.GetComponent<TilemapRenderer>().enabled = false;
    }
    //public void Update()
    //{
    //    //if(Input.GetKeyDown(KeyCode.R))
    //    //{
    //        //RefreshDisplayTilemap();
    //    //}
    //}
    //public void SetCell(Vector3Int coords, DualGridTile tile)
    //{
    //    RealTileMap.SetTile(coords, tile.RealTileMapVariant);
    //    tile.UpdateDisplayTile(coords);
    //}
    public void RefreshDisplayTilemap()
    {
        for (int i = -WorldSize; i <= WorldSize; i++)
        {
            for (int j = -WorldSize; j <= WorldSize; j++)
            {
                Vector3Int coords = new Vector3Int(i, j);
                for (int k = 0; k < World.Instance.Tiles.Length; ++k)
                {
                    if (World.Instance.Tiles[k].RealTileMapVariant == m_RealTileMap.GetTile(coords))
                    {
                        World.Instance.Tiles[k].UpdateDisplayTile(coords, VisualMaps[k]);
                        if (k != 1) //THIS IS THE DIRT LAYER
                            World.Instance.Tiles[1].UpdateDisplayTile(coords, VisualMaps[1]); //MAKE IT SO DIRT IS UNDER EVERYTHING TO PREVENT CERTAIN VISUAL ISSUES. MAYBE TEMPORARY
                        break;
                    }
                }
            }
        }
    }
    public void AddDecor(Color c, int order)
    {
        bool mushroom = false;
        float mult = 1.0f;
        if (order == -30)
        {
            mushroom = true;
            mult = 0.5f;
        }
        int grassLayer = 2;
        int dirtLayer = 1;
        Tile grassTile = World.Instance.Tiles[grassLayer].RealTileMapVariant; //This should be replaced with a more robust system later, like indexing all tiles in an array kept in world
        Tile dirtTile = World.Instance.Tiles[dirtLayer].RealTileMapVariant; //This should be replaced with a more robust system later, like indexing all tiles in an array kept in world
        for (int i = -WorldSize; i <= WorldSize; i++)
        {
            for (int j = -WorldSize; j <= WorldSize; j++)
            {
                bool isGrassTile = m_RealTileMap.GetTile(i, j) == grassTile;
                if (isGrassTile && Utils.RandFloat() < 0.05f * mult)
                {
                    var g = Instantiate(Flower, Visual.transform).GetComponent<SpriteRenderer>();
                    g.transform.localPosition = new Vector3(i + 1, j + 1, 0);
                    g.color = c;
                    g.sortingOrder = order;
                    continue;
                }
                if (mushroom && (isGrassTile || m_RealTileMap.GetTile(i, j) == dirtTile))
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