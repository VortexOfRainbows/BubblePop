using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    public static World Instance => m_Instance == null ? (m_Instance = FindFirstObjectByType<World>()) : m_Instance;
    private static World m_Instance;
    public static DualGridTilemap RealTileMap => Instance.Tilemap;
    public static Tilemap CurrentGeneratingMap { get; private set; }
    public DualGridTilemap Tilemap;
    public DualGridTile[] TileTypes;
    public List<WorldNode> nodes;
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
            CurrentGeneratingMap = RealTileMap.Map;
            RealTileMap.Init(Color.white);
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