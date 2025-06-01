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
    public void Start()
    {
        m_Instance = this;
        if (RealTileMap != null)
        {
            CurrentGeneratingMap = RealTileMap.Map;
            RealTileMap.Init();
        }
        if (ColliderTileMap != null)
        {
            CurrentGeneratingMap = ColliderTileMap.Map;
            ColliderTileMap.Init();
        }
        CurrentGeneratingMap = null;
    }
    public void Update()
    {
        m_Instance = this;
    }
}