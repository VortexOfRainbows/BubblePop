using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using static TileType;

public class DualGridTilemap : MonoBehaviour
{
    public static DualGridTilemap Instance => m_Instance == null ? (m_Instance = FindFirstObjectByType<DualGridTilemap>()): m_Instance;
    private static DualGridTilemap m_Instance;
    public static Vector3Int[] NEIGHBOURS => DualGridTile.NEIGHBOURS;
    public static Tilemap RealTileMap => Instance.m_RealTileMap;
    public Tilemap[] VisualMaps;
    public Tilemap m_RealTileMap;
    // Provide the 16 tiles in the inspector
    public DualGridTile[] Tiles;
    // The tiles on the display tilemap will recalculate themselves based on the placeholder tilemap
    public void Start()
    {
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
        for (int i = -50; i < 50; i++)
        {
            for (int j = -50; j < 50; j++)
            {
                Vector3Int coords = new Vector3Int(i, j);
                for (int k = 0; k < Tiles.Length; ++k)
                {
                    if (Tiles[k].RealTileMapVariant == RealTileMap.GetTile(coords))
                    {
                        Tiles[k].UpdateDisplayTile(coords, VisualMaps[k]);
                        break;
                    }
                }
            }
        }
    }
}
public enum TileType
{
    None,
    Grass,
    Dirt
}