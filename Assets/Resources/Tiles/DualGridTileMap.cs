using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    public static DualGridTilemap RealTileMap;
    public static DualGridTilemap ColliderTileMap;
}
public class DualGridTilemap : MonoBehaviour
{
    public static DualGridTilemap Instance => m_Instance == null ? (m_Instance = FindFirstObjectByType<DualGridTilemap>()): m_Instance;
    private static DualGridTilemap m_Instance;
    public static Vector3Int[] NEIGHBOURS => DualGridTile.NEIGHBOURS;
    public static Tilemap RealTileMap => Instance.m_RealTileMap;
    public GameObject VisualMapPrefab;
    public GameObject Visual;
    private List<Tilemap> VisualMaps;
    public Tilemap m_RealTileMap;
    // Provide the 16 tiles in the inspector
    public DualGridTile[] Tiles;
    // The tiles on the display tilemap will recalculate themselves based on the placeholder tilemap
    public void Start()
    {
        VisualMaps = new List<Tilemap>();
        for (int k = 0; k < Tiles.Length; ++k) {
            VisualMaps.Add(Instantiate(VisualMapPrefab, Visual.transform).GetComponent<Tilemap>());
            VisualMaps[k].GetComponent<TilemapRenderer>().sortingOrder = -50 + Tiles[k].LayerOffset;
        }
        m_Instance = this;
        RefreshDisplayTilemap();
        RealTileMap.gameObject.SetActive(false);
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            RefreshDisplayTilemap();
        }
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
                for (int k = 0; k < Tiles.Length; ++k)
                {
                    if (Tiles[k].RealTileMapVariant == RealTileMap.GetTile(coords))
                    {
                        Tiles[k].UpdateDisplayTile(coords, VisualMaps[k]);
                        if(k == 0) //if the tile is grass, the tile is also dirt (this is temporary logic that will be abstracted later)
                        {
                            Tiles[1].UpdateDisplayTile(coords, VisualMaps[1]);
                        }
                        break;
                    }
                }
            }
        }
    }
}