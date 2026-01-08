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
    public List<WorldNode> nodes;
    public static bool WithinBorders(Vector3 position)
    {
        return RealTileMap.Map.GetColliderType(RealTileMap.Map.WorldToCell(position)) == Tile.ColliderType.None;
    }
    public void Start()
    {
        m_Instance = this;
        foreach (DualGridTile tile in TileID.TileTypes)
            tile.Init();
        LoadNodesOntoWorld();

        CreateWorldOuterFill();

        RealTileMap.Init();
    }
    public void LoadNodesOntoWorld()
    {
        for(int i = 0; i < nodes.Count; ++i)
        {
            WorldNode node = nodes[i];
            node.Generate(this);
        }
    }
    public void CreateWorldOuterFill()
    {
        GeneratingBorder = true;
        var Map = RealTileMap.Map;
        int padding = 15;

        FastNoiseLite Noise = new();
        Noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        Noise.SetFractalType(FastNoiseLite.FractalType.PingPong);
        Noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.EuclideanSq);
        Noise.SetSeed(1337);
        Noise.SetFrequency(0.04f);

        Map.GetCorners(out int left, out int right, out int bottom, out int top);
        left -= padding;
        right += padding;
        bottom -= padding;
        top += padding;
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                Vector3Int pos = new(i, j);
                if (!Map.HasTile(pos))
                {
                    float f = Noise.GetNoise(i, j);
                    if (f < 0.2f && f > -0.2f)
                        Map.SetTile(pos, TileID.Dirt.TileType);
                    else
                        Map.SetTile(pos, TileID.Grass.TileType);
                }
            }
        }
        GeneratingBorder = false;
    }
    public void Update()
    {
        m_Instance = this;
        #if UNITY_EDITOR
        //if(Input.GetKey(KeyCode.R) && true)
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        #endif
    }
}