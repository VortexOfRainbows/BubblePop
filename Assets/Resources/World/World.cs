using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    public static World Instance => m_Instance == null ? (m_Instance = FindFirstObjectByType<World>()) : m_Instance;
    private static World m_Instance;
    public static DualGridTilemap RealTileMap => Instance.m_RealTileMap;
    public static DualGridTilemap ColliderTileMap => Instance.m_ColliderTileMap;
    public static Tilemap CurrentGeneratingMap { get; private set; }
    public DualGridTilemap m_RealTileMap;
    public DualGridTilemap m_ColliderTileMap;
    public DualGridTile[] Tiles;
    public List<WorldNode> nodes;
    public void Start()
    {
        m_Instance = this;
        LoadNodesOntoWorld();
        if (RealTileMap != null)
        {
            CurrentGeneratingMap = RealTileMap.Map;
            RealTileMap.Init(Color.white);
        }
        if (ColliderTileMap != null)
        {
            CurrentGeneratingMap = ColliderTileMap.Map;
            ColliderTileMap.Init(new Color(0.4056604f, 0.4056604f, 0.4056604f), -49);
        }
        CurrentGeneratingMap = null;
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