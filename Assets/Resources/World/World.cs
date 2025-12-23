using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    public static World Instance => m_Instance == null ? (m_Instance = FindFirstObjectByType<World>()) : m_Instance;
    private static World m_Instance;
    public static DualGridTilemap RealTileMap => Instance.Tilemap;
    public static bool GeneratingBorder { get; set; } = false;
    public DualGridTilemap Tilemap;
    public DualGridTile[] TileTypes;
    public List<WorldNode> nodes;
    public static bool WithinBorders(Vector3 position)
    {
        return RealTileMap.Map.GetColliderType(RealTileMap.Map.WorldToCell(position)) == Tile.ColliderType.None;
    }
    public void Start()
    {
        m_Instance = this;
        foreach (DualGridTile tile in TileTypes)
        {
            tile.Init();
        }
        LoadNodesOntoWorld();
        if (RealTileMap != null)
        {
            RealTileMap.Init();
        }
    }
    public void LoadNodesOntoWorld()
    {
        for(int i = 0; i < nodes.Count; ++i)
        {
            WorldNode node = nodes[i];
            node.Generate(this);
        }
    }
    public void Update()
    {
        m_Instance = this;
    }
}