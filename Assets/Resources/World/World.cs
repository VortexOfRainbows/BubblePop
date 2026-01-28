using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    public static Tile DepthTile => Resources.Load<Tile>("World/Tiles/DepthTile");
    public struct TileData
    {
        public byte ProgressionNumber;
        public bool IsRoadblock;
        public TileData(byte progressionNum = byte.MaxValue, bool roadBlock = false)
        {
            ProgressionNumber = progressionNum;
            IsRoadblock = roadBlock;
        }
    }
    private static Vector2Int tileDataOffset;
    private static TileData[,] tileData;
    private static readonly TileData NoTileData = new(byte.MaxValue);
    public static readonly int Padding = 15;
    public static TileData GetTileData(Vector3Int pos)
    {
        Vector2Int pointPos = (Vector2Int)pos - tileDataOffset;
        if (pointPos.x < 0 || pointPos.y < 0 || pointPos.x >= tileData.Length || pointPos.y >= tileData.GetLength(1))
        {
            Debug.Log($"Tile QUERY out of BOUNDS: [{pointPos.x},{pointPos.y}]".WithColor("#FF0000"));
            return NoTileData;
        }
        return tileData[pointPos.x, pointPos.y];
    }
    public static void SetTileData(Vector3Int pos, TileData newData)
    {
        Vector2Int pointPos = (Vector2Int)pos - tileDataOffset;
        if (pointPos.x < 0 || pointPos.y < 0 || pointPos.x >= tileData.Length || pointPos.y >= tileData.GetLength(1))
        {
            Debug.Log($"Tile SET out of BOUNDS: [{pointPos.x},{pointPos.y}]".WithColor("#FF0000"));
            return;
        }
        bool drawMaps1 = Instance.DepthTilemap.isActiveAndEnabled;
        bool drawMaps2 = Instance.DepthTilemap.isActiveAndEnabled;
        #if UNITY_EDITOR
        drawMaps1 = drawMaps2 = true;
        #endif
        if (Instance.DepthTilemap != null && drawMaps1)
        {
            byte progNumber = newData.ProgressionNumber;
            Color c = Color.Lerp(Color.Lerp(Color.red, Color.blue, progNumber / 20f % 1), Color.green, (progNumber / 5f) % 1).WithAlpha(0.5f);
            Instance.DepthTilemap.SetTile(new (pos, DepthTile, c, Matrix4x4.identity), true);
        }
        if (Instance.RoadblockTilemap != null && drawMaps2 && newData.IsRoadblock)
        {
            Instance.RoadblockTilemap.SetTile(new(pos, DepthTile, Color.white, Matrix4x4.identity), true);
        }
        tileData[pointPos.x, pointPos.y] = newData;
    }
    public static World Instance => m_Instance == null ? (m_Instance = FindFirstObjectByType<World>()) : m_Instance;
    private static World m_Instance;
    public static DualGridTilemap RealTileMap => Instance.Tilemap;
    public static bool GeneratingBorder { get; set; } = false;
    public DualGridTilemap Tilemap;
    [SerializeField] private Tilemap DepthTilemap, RoadblockTilemap;
    public NatureOrderer NatureParent;
    public Transform PylonParent;
    public Transform RoadblockParent;
    public Transform PlayerSpawnPosition;
    [SerializeField]
    private List<Transform> nodes;
    public static bool ValidEnemySpawnTile(Vector3 pos)
    {
        bool validSpawnTile = RealTileMap.Map.GetTile(RealTileMap.Map.WorldToCell(pos)) != TileID.DarkGrass.TileType;
        //if(!validSpawnTile)
        //    Debug.Log($"Valid Spawn Tile: {validSpawnTile}");
        return WithinBorders(pos) && validSpawnTile;
    }
    public static bool WithinBorders(Vector3 position)
    {
        return RealTileMap.Map.GetColliderType(RealTileMap.Map.WorldToCell(position)) == Tile.ColliderType.None;
    }
    public static bool SolidTile(Vector3Int pos)
    {
        return RealTileMap.Map.GetColliderType(pos) == Tile.ColliderType.Grid;
    }
    public static bool WithinBorders(Vector3 position, bool IncludeProgressionBounds)
    {
        var data = GetTileData(RealTileMap.Map.WorldToCell(position));
        bool currentlyOnThisProgressionTier = data.ProgressionNumber > Main.PylonProgressionNumber;
        bool roadblock = IncludeProgressionBounds && (currentlyOnThisProgressionTier || (data.IsRoadblock && Main.PylonActive));
        return WithinBorders(position) && !roadblock;
    }
    public static readonly List<Pylon> Pylons = new();
    public static readonly List<Roadblock> Roadblocks = new();
    public void Start()
    {
        NodeID.LoadAllNodes();
        NextToGenerate.Clear();
        Pylons.Clear();
        Roadblocks.Clear();
        m_Instance = this;
        foreach (DualGridTile tile in TileID.TileTypes)
            tile.Init();
        ApproximateWorldBounds();
        LoadNodesOntoWorld();

        CreateWorldOuterFill();
        RealTileMap.Init();
        if (NatureParent != null)
            NatureParent.Init();

        var p = Instantiate(Main.PrefabAssets.PlayerPrefab, PlayerSpawnPosition.position, Quaternion.identity).GetComponent<Player>();
        Player.Instance = p;
        Camera.main.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, Camera.main.transform.position.z);
        Destroy(PlayerSpawnPosition.gameObject);
        Tilemap.GetComponent<TilemapRenderer>().enabled = false;

        int i = 0;
        foreach(Pylon pylon in PylonParent.GetComponentsInChildren<Pylon>())
        {
            pylon.name = $"Pylon:{i}";
            pylon.ProgressionNumber = (byte)i++;
            Pylons.Add(pylon);
        }
        foreach (Roadblock rb in RoadblockParent.GetComponentsInChildren<Roadblock>())
        {
            rb.name = $"{(rb.IsEndRoadblock ? "EndRoadblock" : "Roadblock")}:{rb.ProgressionLevel}";
            Roadblocks.Add(rb);
        }
        Pylons.Last().EndlessPylon = true; //temporary endless pylon
    }
    public Queue<WorldNode> NextToGenerate { get; private set; } = new();
    public void ApproximateWorldBounds()
    {
        int left = int.MaxValue;
        int right = int.MinValue;
        int bottom = int.MaxValue;
        int top = int.MinValue;
        for (int i = 0; i < nodes.Count; ++i)
        {
            Transform tr = nodes[i];
            if (!nodes[i].TryGetComponent(out WorldNode node))
            {
                tr.gameObject.SetActive(false);
                node = NodeID.GetRandomNodeWithParameters(NodeID.Nodes, 0);
                NextToGenerate.Enqueue(node);
            }
            Vector3Int transformPos = new(Mathf.FloorToInt(tr.position.x / 2), Mathf.FloorToInt(tr.position.y / 2));
            node.TileMap.GetCorners(out int l, out int r, out int b, out int t);
            left = Mathf.Min(l + transformPos.x, left);
            right = Mathf.Max(r + transformPos.x, right);
            bottom = Mathf.Min(b + transformPos.y, bottom);
            top = Mathf.Max(t + transformPos.y, top);
        }
        if (nodes.Count <= 0)
            left = right = bottom = top = 0;
        left -= Padding;
        right += Padding;
        bottom -= Padding;
        top += Padding;
        Vector2Int size = new(right - left, top - bottom);
        Debug.Log($"World Border: L: {left}, R: {right}, B: {bottom}, T: {top}".WithColor("#CC77FF"));
        Debug.Log($"World size: X: {size.x}, Y: {size.y}".WithColor("#CC77FF"));
        if(size.x < 1000 && size.y < 1000 && size.x > 0 && size.y > 0)
        {
            tileDataOffset = new Vector2Int(left, bottom);
            tileData = new TileData[size.x, size.y];
        }
        else
        {
            throw new System.Exception("BUBBLE: WORLD GENERATED TOO LARGE OR NEGATIVE SIZE. CANCELLING FOR SAFETY");
        }
    }
    public void LoadNodesOntoWorld()
    {
        WorldNode prevNode = null;
        for(int i = 0; i < nodes.Count; ++i)
        {
            Transform t = nodes[i];
            if (!nodes[i].TryGetComponent(out WorldNode node))
                node = NextToGenerate.Dequeue();
            node.Generate(t.position, this, (byte)i, prevNode);
            prevNode = node;
        }
    }
    public void CreateWorldOuterFill()
    {
        GeneratingBorder = true;
        var Map = RealTileMap.Map;

        FastNoiseLite Noise = new();
        Noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        Noise.SetFractalType(FastNoiseLite.FractalType.PingPong);
        Noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.EuclideanSq);
        Noise.SetSeed(1337);
        Noise.SetFrequency(0.04f);

        Map.GetCorners(out int left, out int right, out int bottom, out int top);
        left -= Padding;
        right += Padding;
        bottom -= Padding;
        top += Padding;
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                Vector3Int pos = new(i, j);
                if (!Map.HasTile(pos))
                {
                    float f = Noise.GetNoise(i, j);
                    if (f < 0.2f && f > -0.2f)
                    {
                        Map.SetTile(pos, TileID.Dirt.TileType);
                    }
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
        if(Input.GetKey(KeyCode.R) && true)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        #endif
    }
}