using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
public class World : MonoBehaviour
{
    public static Tile DepthTile;
    public static int FloorSortingLayer { get; private set; }
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
    public static readonly int Padding = 20;
    public static TileData GetTileData(Vector3Int pos)
    {
        Vector2Int pointPos = (Vector2Int)pos - tileDataOffset;
        if (pointPos.x < 0 || pointPos.y < 0 || pointPos.x >= tileData.GetLength(0) || pointPos.y >= tileData.GetLength(1))
        {
            //Debug.Log($"Tile QUERY out of BOUNDS: [{pointPos.x},{pointPos.y}]".WithColor("#FF0000"));
            return NoTileData;
        }
        return tileData[pointPos.x, pointPos.y];
    }
    public static void SetTileData(Vector3Int pos, TileData newData)
    {
        Vector2Int pointPos = (Vector2Int)pos - tileDataOffset;
        if (pointPos.x < 0 || pointPos.y < 0 || pointPos.x >= tileData.GetLength(0) || pointPos.y >= tileData.GetLength(1))
        {
            Debug.Log($"Tile SET out of BOUNDS: [{pointPos.x},{pointPos.y}]".WithColor("#FF0000"));
            return;
        }
        bool drawMaps1 = Instance.DepthTilemap.isActiveAndEnabled;
        #if UNITY_EDITOR
        drawMaps1 = true;
        #endif
        if (Instance.DepthTilemap != null && drawMaps1)
        {
            byte progNumber = newData.ProgressionNumber;
            Color c = Color.Lerp(Color.Lerp(Color.red, Color.blue, progNumber / 20f % 1), Color.green, (progNumber / 5f) % 1).WithAlpha(0.5f);
            Instance.DepthTilemap.SetTile(new (pos, DepthTile, c, Matrix4x4.identity), true);
        }
        if (newData.IsRoadblock)
            Instance.RoadblockTilemap.SetTile(new(pos, DepthTile, RoadblockColor(newData.ProgressionNumber), Matrix4x4.identity), true);
        tileData[pointPos.x, pointPos.y] = newData;
    }
    private static readonly Dictionary<byte, Color> RoadblockColorStorage = CreateRoadblockColors();
    private static Dictionary<byte, Color> CreateRoadblockColors()
    {
        Dictionary<byte, Color> storage = new(byte.MaxValue + 1);
        for (byte i = 0; i < 255; ++i)
        {
            int r = i % 8;
            int g = i / 8 % 8;
            int b = i / 64 % 4;
            storage[i] = new Color(r * 0.125f, g * 0.125f, b * 0.25f);
        }
        return storage;
    }
    public static Color RoadblockColor(byte progNum) => RoadblockColorStorage[progNum];
    private static byte ConvertColorToProgNum(Color c)
    {
        return (byte)(c.r * 8 + c.g * 64 + c.b * 256);
    }
    private static void TestColorProgRelations()
    {
        Debug.Log("START COLOR PROG TEST (REMOVE THESE TESTS LATER PLEASE)".WithColor("#FF11AA"));
        for(byte b = 0; b < 255; ++b)
            Assert.AreEqual(ConvertColorToProgNum(RoadblockColor(b)), b);
        Debug.Log("PASSED COLOR PROG TEST".WithColor("#FF11AA"));
    }
    public static World Instance => m_Instance == null ? (m_Instance = FindFirstObjectByType<World>()) : m_Instance;
    private static World m_Instance;
    public static DualGridTilemap RealTileMap => Instance.Tilemap;
    public DualGridTilemap Tilemap;
    [SerializeField] private Tilemap DepthTilemap, RoadblockTilemap, InverseRoadblockMap, OcclusionMap;
    public Tilemap LightingTilemapFront;
    public Tilemap LightingTilemapBack;
    public NatureOrderer NatureParent;
    public Transform PylonParent;
    public Transform RoadblockParent;
    public Transform ProceduralGenParent;
    public Transform FloorDecorParent;
    public Transform BorderDecorParent;
    public List<Transform> PlayerSpawnPosition = new();
    public int NodesToGenerate = 7;
    private int OriginalNodeCount { get; set; } = 0;
    public static byte FinalProgNumber { get; set; }
    public Rect ApproximateSize { get; set; }
    public List<Transform> nodes;
    public static bool ValidEnemySpawnTile(Vector3 pos)
    {
        Vector3Int posi = RealTileMap.Map.WorldToCell(pos);
        var data = GetTileData(posi);
        bool currentlyOnThisProgressionTier = data.ProgressionNumber == Main.PylonProgressionNumber;
        bool validSpawnTile = RealTileMap.Map.GetTile(posi) != TileID.DarkGrass.FloorTileType && !GetTileData(posi).IsRoadblock;
        return WithinBorders(pos) && validSpawnTile && currentlyOnThisProgressionTier;
    }
    public static Vector3Int WorldPosition(Vector3 position)
    {
        return RealTileMap.Map.WorldToCell(position);
    }
    public static bool WithinBorders(Vector3 position)
    {
        return RealTileMap.Map.GetColliderType(RealTileMap.Map.WorldToCell(position)) == Tile.ColliderType.None;
    }
    public static bool SolidTile(Vector3 position)
    {
        return RealTileMap.Map.GetColliderType(RealTileMap.Map.WorldToCell(position)) != Tile.ColliderType.None;
    }
    public static bool SolidTile(Vector3Int pos)
    {
        return RealTileMap.Map.GetColliderType(pos) != Tile.ColliderType.None;
    }
    public static bool SolidTile(int x, int y) => SolidTile(new Vector3Int(x, y));
    public static bool HasTile(Vector3Int pos)
    {
        return RealTileMap.Map.HasTile(pos);
    }
    public static bool AreaIsClear(Vector3Int area, int squareRadius = 0)
    {
        for(int i = -squareRadius; i <= squareRadius; ++i)
            for(int j = -squareRadius; j <= squareRadius; ++j)
                if (RealTileMap.Map.HasTile(area + new Vector3Int(i, j)))
                    return false;
        return true;
    }
    public static bool WithinBorders(Vector3 position, bool IncludeProgressionBounds)
    {
        bool roadblock = IncludeProgressionBounds && IsRoadblocked(position);
        return WithinBorders(position) && !roadblock;
    }
    public static bool IsRoadblocked(Vector3 position)
    {
        var data = GetTileData(RealTileMap.Map.WorldToCell(position));
        bool currentlyOnThisProgressionTier = data.ProgressionNumber > Main.PylonProgressionNumber;
        bool roadblock = currentlyOnThisProgressionTier || (data.IsRoadblock && Main.PylonActive);
        return roadblock;
    }
    public static readonly List<WavePylon> Pylons = new();
    public static WarpPylon FinalPylon { get; private set; }
    public static readonly List<Roadblock> Roadblocks = new();
    public void FirstInitialization()
    {
        OriginalNodeCount = nodes.Count;
        //TestColorProgRelations();
        m_Instance = this;
        DepthTile = Resources.Load<Tile>("World/Tiles/DepthTile");
        FloorSortingLayer = RealTileMap.GetComponent<TilemapRenderer>().sortingLayerID;
        NodeID.LoadAllNodes();
        Lighting.LoadTextures();

        ResetWorld(true);

        GachaponShop.TotalPowersPurchased = 0;
        GachaponShop.GlobalRestockCost = GachaponShop.SetDefaultGemRestockCost();
    }
    public void ResetWorld(bool firstInit)
    {
        Main.PylonProgressionNumber = 0;
        if(!firstInit)
        {
            foreach(Player p in Player.AllPlayers)
                p.OnWorldReset();
            Main.ResetContainers();
            ResetTransformParents();
        }
        ResetAllTilemaps();
        Player.ObjectsConsideredForUIInteraction.Clear();
        GachaponShop.AllShops.Clear();
        NextToGenerate.Clear();
        Pylons.Clear();
        Roadblocks.Clear();
        foreach (DualGridTile tile in TileID.TileTypes)
            tile.Init();
        PlaceNodeLocations();
        ApproximateWorldBounds();
        LoadNodesOntoWorld();

        CreateWorldOuterFill();
        RealTileMap.Init();
        if (NatureParent != null)
            NatureParent.Init();

        if (firstInit)
        {
            Player.AllPlayers.Clear();
            foreach (Transform spawnPos in PlayerSpawnPosition)
            {
                var p = Instantiate(Main.PrefabAssets.PlayerPrefab, spawnPos.position, Quaternion.identity).GetComponent<Player>();
                p.InstanceID = Player.AllPlayers.Count;
                Player.AllPlayers.Add(p);
                Camera.main.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, Camera.main.transform.position.z);
                spawnPos.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach(Player p in Player.AllPlayers)
                p.transform.position = PlayerSpawnPosition[p.InstanceID].position;
        }

        Tilemap.GetComponent<TilemapRenderer>().enabled = false;
        int i = 0;
        foreach (WavePylon pylon in PylonParent.GetComponentsInChildren<WavePylon>())
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
        Pylons.Last().WavesRequired = 1;
        FinalPylon = PylonParent.GetChild(PylonParent.childCount - 1).GetComponent<WarpPylon>();
        NodeID.ResetNodePositions();
        Lighting.Setup(RealTileMap.Map, LightingTilemapFront, LightingTilemapBack, OcclusionMap);
    }
    public void Start()
    {
        FirstInitialization();
    }
    private void ReplaceTransform(ref Transform original, Transform replacement)
    {
        Vector3 originalScale = original.localScale;
        Vector3 originalPos = original.localPosition;
        Destroy(original.gameObject);
        original = replacement;
        original.parent = Tilemap.transform;
        original.localScale = originalScale;
        original.localPosition = originalPos;
    }
    private int ReloadNumber { get; set; } = 0;
    public void ResetTransformParents()
    {
        ReloadNumber++;
        string bonus = ReloadNumber.ToString();
        nodes = new List<Transform>(nodes.GetRange(0, OriginalNodeCount));
        NatureOrderer newOrderer = new GameObject(NatureParent.gameObject.name + bonus).AddComponent<NatureOrderer>();
        GameObject newPylonParent = new(PylonParent.gameObject.name + bonus);
        GameObject newRoadblockParent = new(RoadblockParent.gameObject.name + bonus);
        GameObject newProceduralGenParent = new(ProceduralGenParent.gameObject.name + bonus);
        GameObject newFloorDecorParent = new(FloorDecorParent.gameObject.name + bonus);
        GameObject newBorderDecorParent = new(BorderDecorParent.gameObject.name + bonus);
        ReplaceTransform(ref PylonParent, newPylonParent.transform);
        ReplaceTransform(ref RoadblockParent, newRoadblockParent.transform);
        ReplaceTransform(ref ProceduralGenParent, newProceduralGenParent.transform);
        ReplaceTransform(ref FloorDecorParent, newFloorDecorParent.transform);
        ReplaceTransform(ref BorderDecorParent, newBorderDecorParent.transform);
        Destroy(NatureParent.gameObject);
        NatureParent = newOrderer;
        NatureParent.transform.parent = Tilemap.transform;
    }
    public void ResetAllTilemaps()
    {
        Tilemap.Map.ClearAllTiles();
        DepthTilemap.ClearAllTiles();
        RoadblockTilemap.ClearAllTiles();
        InverseRoadblockMap.ClearAllTiles();
        OcclusionMap.ClearAllTiles();
        LightingTilemapFront.ClearAllTiles();
        LightingTilemapBack.ClearAllTiles();
    }
    /// <summary>
    /// Generates the positions for nodes on the map before they are fully loaded. Uses approximations to find the best spot to place the nodes.
    /// </summary>
    public void PlaceNodeLocations()
    {
        int nodeCount = NodesToGenerate + nodes.Count + 1; //+1 for the end node
        WorldNode prevNode = null;
        for (int i = 0; i < nodes.Count; ++i) //Assign all nodes 
        {
            if (!nodes[i].TryGetComponent(out WorldNode node))
                prevNode = AssignNodeToTransform(nodes[i], i, nodeCount);
            else
            {
                NextToGenerate.Enqueue(node);
                prevNode = node;
            }
        }
        FinalProgNumber = (NodesToGenerate < 1 ? (byte)0 : (byte)(NodesToGenerate - 1)); //Minus 1 as the final node is never cleared. the first node starts cleared
        if (prevNode == null)
            throw new System.Exception("BUBBLE: FAILED TO FIND STARTING PROCEDURAL NODE");
        Vector2 toPrev = Utils.RandCircleEdge();
        for(int i = nodes.Count; i < nodeCount; ++i)
        {
            Transform t = nodes.Last();
            Vector3 prevPosition = t.position;
            GameObject arbitraryGameObject = new($"ProceduralNode[{i}]");
            arbitraryGameObject.transform.position = prevPosition;
            arbitraryGameObject.transform.parent = ProceduralGenParent;
            WorldNode node = AssignNodeToTransform(arbitraryGameObject.transform, i, nodeCount);
            nodes.Add(arbitraryGameObject.transform);
            Rect prev = node.TileMap.GetRect(prevPosition.ToTilePosition(), true, 4);
            Rect current = node.TileMap.GetRect(arbitraryGameObject.transform.position.ToTilePosition(), true, 4);
            float att = (current.width + current.height + prev.width + prev.height) * 0.45f;
            float start = att;
            while (IntersectionCount(current) > 0)
            {
                float r = Utils.RandFloat(-att, att);
                r += Mathf.Sign(r) * 10;
                arbitraryGameObject.transform.position = prevPosition - (Vector3)((toPrev * (att + Utils.RandFloat(30))).RotatedBy(r * Mathf.Deg2Rad));
                current = node.TileMap.GetRect(arbitraryGameObject.transform.position.ToTilePosition(), false, 5);
                if(att > 360)
                    throw new Exception("BUBBLE: FAILED TO PLACE PROCEDURAL NODE");
                att += 2;
            }
            Debug.Log($"Placing Node [{i}] took {att - start} attempts".WithColor("#5500FF"));
            toPrev = prevPosition - arbitraryGameObject.transform.position;
            toPrev = toPrev.normalized;
            //prevNode = node;
        }
    }
    public int IntersectionCount(Rect r)
    {
        int c = 0;
        int i = 0;
        foreach(WorldNode node in NextToGenerate)
        {
            if (i >= NextToGenerate.Count - 1)
                return c;
            Transform t = nodes[i++];
            Rect prev = node.TileMap.GetRect(t.position.ToTilePosition(), true, 5);
            if(prev.Intersects(r))
                ++c;
        }
        return c;
    }
    public WorldNode AssignNodeToTransform(Transform t, int i, int totalNodes)
    {
        t.gameObject.SetActive(false);
        bool shop = i == 2 || i == 4 || i == 6 || i == totalNodes - 2;
        bool largo = i == totalNodes - 2;
        bool? crucible = i % 3 == 1 && !largo ? Utils.RollWithLuck(0.5f) : null;
        bool? forge = i > 4 && Utils.RollWithLuck(0.75f) ? true : (i > 2 ? null : false);
        if (i <= 1) //Don't want crucible on first room
            crucible = false;
        WorldNode node;
        if(i == totalNodes - 1)
            node = NodeID.GetRandomNodeWithParameters(NodeID.EndNodes, 0, null, null, null, null);
        else
            node = NodeID.GetRandomNodeWithParameters(NodeID.Nodes, 0, shop, crucible, largo, forge);
        NodeID.PreviousNode = node;
        NextToGenerate.Enqueue(node);
        return node;
    }
    public Queue<WorldNode> NextToGenerate { get; private set; } = new();
    public void ApproximateWorldBounds()
    {
        int left = int.MaxValue;
        int right = int.MinValue;
        int bottom = int.MaxValue;
        int top = int.MinValue;
        int i = 0;
        foreach(WorldNode n in NextToGenerate)
        {
            Transform tr = nodes[i];
            Vector3Int transformPos = tr.position.ToTilePosition(); // new(Mathf.FloorToInt(tr.position.x / 2), Mathf.FloorToInt(tr.position.y / 2));
            n.TileMap.GetCorners(out int l, out int r, out int b, out int t);
            left = Mathf.Min(l + transformPos.x, left);
            right = Mathf.Max(r + transformPos.x, right);
            bottom = Mathf.Min(b + transformPos.y, bottom);
            top = Mathf.Max(t + transformPos.y, top);
            Debug.Log($"Node[{i}]: L: {left}, R: {right}, B: {bottom}, T: {top}".WithColor("#66FF11"));
            ++i;
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
        ApproximateSize = new(left, bottom, right - left, top - bottom);
        if (size.x < 1000 && size.y < 1000 && size.x > 0 && size.y > 0)
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
        byte genNum = 0;
        for(int i = 0; i < nodes.Count; ++i)
        {
            Transform t = nodes[i];
            bool disable = nodes[i].TryGetComponent(out WorldNode _);
            WorldNode node = NextToGenerate.Dequeue();
            node.Generate(t.position, this, genNum, prevNode, disable);
            if (!node.IsSubNode)
                genNum++;
            prevNode = node;
        }
        PlaceBonusNodes();
        GenerateBonusNodes();
    }
    public void PlaceBonusNodes()
    {
        Vector3Int[] directions = new Vector3Int[] { new (1, 0), new (-1, 0), new (0, 1), new (0, -1) };
        Vector3Int bestLocation = Vector3Int.zero;
        Vector3Int bestEndLocation = Vector3Int.zero;
        byte bestGenOwner = 0;
        int bestValue = 0;
        for(int i = 0; i < 20; ++i)
        {
            Vector3Int randomLocation = new (Utils.RandInt((int)ApproximateSize.xMin + Padding, (int)ApproximateSize.xMax - Padding), Utils.RandInt((int)ApproximateSize.yMin + Padding, (int)ApproximateSize.yMax - Padding));
            int TotalSolidTilesSeen = 0;
            int ReachedEndPoint = 0;
            byte closestGenOwner = 0;
            int exit = int.MaxValue;
            Vector3Int closeEndLocation = Vector3Int.zero;
            for(int j = 0; j < 4; ++j)
            {
                bool hasReachedEndHere = false;
                int solids = 0;
                Vector3Int dir2 = directions[j];
                Vector3Int endLocation = Vector3Int.zero;
                byte genOwner = 0;
                for(int k = 1; k < 80; k += 4)
                {
                    Vector3Int locationToCheck = randomLocation + dir2 * k;
                    if(!HasTile(locationToCheck) || World.SolidTile(locationToCheck))
                    {
                        solids++;
                    }
                    else
                    {
                        if (solids < 6)
                        {
                            --ReachedEndPoint;
                            break;
                        }
                        genOwner = World.GetTileData(locationToCheck).ProgressionNumber;
                        hasReachedEndHere = true;
                        ReachedEndPoint++;
                        solids += 20;
                        endLocation = locationToCheck;
                        break;
                    }
                }
                if (hasReachedEndHere)
                {
                    if(solids < exit)
                    {
                        exit = solids;
                        closestGenOwner = genOwner;
                        closeEndLocation = endLocation;
                    }
                    TotalSolidTilesSeen += solids;
                }
            }
            int value = TotalSolidTilesSeen * ReachedEndPoint;
            if(bestValue < value)
            {
                bestValue = value;
                bestLocation = randomLocation;
                bestGenOwner = closestGenOwner;
                bestEndLocation = closeEndLocation;
            }
            //GameObject n2 = new($"SecretRoom[{value},{ReachedEndPoint}]");
            //n2.transform.position = randomLocation * 2;
            //n2.transform.parent = ProceduralGenParent;
        }
        GameObject n = new($"TrueSecretRoom[{bestValue}]");
        n.transform.position = bestLocation * 2;
        n.transform.parent = ProceduralGenParent;

        WorldNode node = NodeID.GetRandomNodeWithParameters(NodeID.SecretNodes, 0, null, null, null, null);
        node.Generate(n.transform.position, this, (byte)(MathF.Min(FinalProgNumber, bestGenOwner + 1)), null, false);

        Vector2 dir = (Vector2)n.transform.position - new Vector2(bestEndLocation.x, bestEndLocation.y);
        node.GeneratePath(new Vector2(bestEndLocation.x, bestEndLocation.y) * 2, (Vector2)n.transform.position - dir.normalized * 10, false, -1);
    }
    public void GenerateBonusNodes()
    {

    }
    public void CreateWorldOuterFill()
    {
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
        for(int passNum = 0; passNum < 2; ++passNum)
        {
            for (int i = left; i < right; i++)
            {
                for (int j = bottom; j < top; j++)
                {
                    Vector3Int pos = new(i, j);
                    if (passNum == 0)
                    {
                        if (!Map.HasTile(pos))
                        {
                            float f = Noise.GetNoise(i, j);
                            if (f < 0.2f && f > -0.2f)
                            {
                                Map.SetTile(pos, TileID.Dirt.BorderTileType);
                            }
                            else
                                Map.SetTile(pos, TileID.Grass.BorderTileType);
                        }
                        if (SolidTile(pos))
                        {
                            if (Instance.LightingTilemapFront != null && Instance.LightingTilemapBack != null) //This is also used for occlusion so it is obtained when typically setting up the tile maps... Additionally, it could be used to check for solid tiles quicker, but im not certain if it is faster (NEEDS TESTING)
                            {
                                Instance.LightingTilemapFront.SetTile(pos, DepthTile);
                                Instance.LightingTilemapBack.SetTile(pos, DepthTile);
                            }
                        }
                    }
                    else
                    {
                        var data = GetTileData(pos);
                        if (passNum == 1 && data.IsRoadblock)
                        {
                            Vector3Int tleft = new(pos.x - 1, pos.y);
                            Vector3Int tright = new(pos.x + 1, pos.y);
                            Vector3Int ttop = new(pos.x, pos.y + 1);
                            Vector3Int tbot = new(pos.x, pos.y - 1);
                            Color c = RoadblockColor(data.ProgressionNumber);
                            if (SolidTile(tleft))
                                Instance.RoadblockTilemap.SetTile(new(tleft, DepthTile, c, Matrix4x4.identity), true);
                                //SetTileData(tleft, new(data.ProgressionNumber, true));
                            if (SolidTile(tright))
                                Instance.RoadblockTilemap.SetTile(new(tright, DepthTile, c, Matrix4x4.identity), true);
                                //SetTileData(tright, new(data.ProgressionNumber, true));
                            if (SolidTile(ttop))
                                Instance.RoadblockTilemap.SetTile(new(ttop, DepthTile, c, Matrix4x4.identity), true);
                                //SetTileData(ttop, new(data.ProgressionNumber, true));
                            if (SolidTile(tbot))
                                Instance.RoadblockTilemap.SetTile(new(tbot, DepthTile, c, Matrix4x4.identity), true);
                                //SetTileData(tbot, new(data.ProgressionNumber, true));
                        }
                        //else if(passNum == 2 && !data.IsRoadblock)
                        //{
                        //    if(!SolidTile(pos))
                        //        Instance.InverseRoadblockMap.SetTile(new(pos, DepthTile, Color.white, Matrix4x4.identity), true);
                        //}
                        //Instance.RoadblockTilemap.SetTile(new(pos, DepthTile, Color.white, Matrix4x4.identity), true);
                    }
                }
            }
        }
    }
    /// <summary>
    /// This keeps track of elapsed time for the purpose of visual effects. Do not use for stuff that requires more precise logic
    /// </summary>
    public static float GlobalTimeElapsedCounter = 0.0f;
    public void Update()
    {
        m_Instance = this;
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R) && Main.DebugCheats)
            ResetWorld(false);
#endif
        Lighting.Update();
        GlobalTimeElapsedCounter += Time.deltaTime;
    }
    public void LateUpdate()
    {
        Lighting.LateUpdate();
    }
    //public void OnDisable()
    //{
    //    Lighting.OnDisable();
    //}
    //public void OnEnable()
    //{
    //    Lighting.OnEnable();
    //}
}