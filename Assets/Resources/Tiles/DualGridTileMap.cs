using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DualGridTilemap : MonoBehaviour
{
    public static Vector3Int[] NEIGHBOURS => DualGridTile.NEIGHBOURS;
    public Tilemap Map => m_RealTileMap;
    public GameObject VisualMapPrefab;
    public GameObject Visual;
    private List<Tilemap> VisualMaps;
    public Tilemap m_RealTileMap;
    // Provide the 16 tiles in the inspector
    // The tiles on the display tilemap will recalculate themselves based on the placeholder tilemap
    public void Init()
    {
        VisualMaps = new List<Tilemap>();
        for (int k = 0; k < World.Instance.Tiles.Length; ++k) {
            VisualMaps.Add(Instantiate(VisualMapPrefab, Visual.transform).GetComponent<Tilemap>());
            VisualMaps[k].GetComponent<TilemapRenderer>().sortingOrder = -50 + World.Instance.Tiles[k].LayerOffset;
        };
        RefreshDisplayTilemap();
        m_RealTileMap.GetComponent<TilemapRenderer>().enabled = false;
    }
    public void Update()
    {
        //if(Input.GetKeyDown(KeyCode.R))
        //{
            //RefreshDisplayTilemap();
        //}
    }
    //public void SetCell(Vector3Int coords, DualGridTile tile)
    //{
    //    RealTileMap.SetTile(coords, tile.RealTileMapVariant);
    //    tile.UpdateDisplayTile(coords);
    //}
    public void RefreshDisplayTilemap()
    {
        int worldSize = 100;
        for (int i = -worldSize; i <= worldSize; i++)
        {
            for (int j = -worldSize; j <= worldSize; j++)
            {
                Vector3Int coords = new Vector3Int(i, j);
                for (int k = 0; k < World.Instance.Tiles.Length; ++k)
                {
                    if (World.Instance.Tiles[k].RealTileMapVariant == m_RealTileMap.GetTile(coords))
                    {
                        World.Instance.Tiles[k].UpdateDisplayTile(coords, VisualMaps[k]);
                        if(k == 0) //if the tile is grass, the tile is also dirt (this is temporary logic that will be abstracted later)
                        {
                            World.Instance.Tiles[1].UpdateDisplayTile(coords, VisualMaps[1]);
                        }
                        break;
                    }
                }
            }
        }
    }
}