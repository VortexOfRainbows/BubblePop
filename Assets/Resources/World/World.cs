using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    //TODO: Update this so it doesn't use Resources.Load every time, maybe use a static constructor or something
    public static GameObject OcclusionShadowPrefab => Resources.Load<GameObject>("Lighting/OcclusionTileShadow");
    public static GameObject tileShadowPrefab => Resources.Load<GameObject>("Lighting/TileShadow");
    public static GameObject shadowPT => Resources.Load<GameObject>("Lighting/TileShadowPT");
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
    public static readonly int Padding = 20;
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
    public DualGridTilemap Tilemap;
    [SerializeField] private Tilemap DepthTilemap, RoadblockTilemap;
    public NatureOrderer NatureParent;
    public Transform PylonParent;
    public Transform RoadblockParent;
    public Transform ProceduralGenParent;
    public List<Transform> PlayerSpawnPosition = new();
    public int NodesToGenerate = 7;
    public Rect ApproximateSize { get; set; }
    [SerializeField]
    public List<Transform> nodes;
    public static bool ValidEnemySpawnTile(Vector3 pos)
    {
        Vector3Int posi = RealTileMap.Map.WorldToCell(pos);
        bool validSpawnTile = RealTileMap.Map.GetTile(posi) != TileID.DarkGrass.FloorTileType && !GetTileData(posi).IsRoadblock;
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
        var data = GetTileData(RealTileMap.Map.WorldToCell(position));
        bool currentlyOnThisProgressionTier = data.ProgressionNumber > Main.PylonProgressionNumber;
        bool roadblock = IncludeProgressionBounds && (currentlyOnThisProgressionTier || (data.IsRoadblock && Main.PylonActive));
        return WithinBorders(position) && !roadblock;
    }
    public static readonly List<Pylon> Pylons = new();
    public static readonly List<Roadblock> Roadblocks = new();
    public void Start()
    {
        GachaponShop.AllShops.Clear();
        GachaponShop.TotalPowersPurchased = 0;
        NodeID.LoadAllNodes();
        NextToGenerate.Clear();
        Pylons.Clear();
        Roadblocks.Clear();
        m_Instance = this;
        foreach (DualGridTile tile in TileID.TileTypes)
        {
            tile.Init();
        }

        PlaceNodeLocations();
        ApproximateWorldBounds();
        LoadNodesOntoWorld();

        CreateWorldOuterFill();
        RealTileMap.Init();
        if (NatureParent != null)
            NatureParent.Init();

        Player.AllPlayers.Clear();

        foreach(Transform spawnPos in PlayerSpawnPosition)
        {
            var p = Instantiate(Main.PrefabAssets.PlayerPrefab, spawnPos.position, Quaternion.identity).GetComponent<Player>();
            p.InstanceID = Player.AllPlayers.Count;
            Player.AllPlayers.Add(p);
            Camera.main.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, Camera.main.transform.position.z);
            spawnPos.gameObject.SetActive(false);
        }
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
        NodeID.ResetNodePositions(); //This is mostly for editor stuff
    }
    /// <summary>
    /// Generates the positions for nodes on the map before they are fully loaded. Uses approximations to find the best spot to place the nodes.
    /// </summary>
    public void PlaceNodeLocations()
    {
        int nodeCount = NodesToGenerate + nodes.Count;
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
            float att = (current.width + current.height + prev.width + prev.height) * 0.4f;
            float start = att;
            while (IntersectionCount(current) > 0)
            {
                float r = Utils.RandFloat(-att, att);
                r += Mathf.Sign(r) * 10;
                arbitraryGameObject.transform.position = prevPosition - (Vector3)((toPrev * (att + Utils.RandFloat(25))).RotatedBy(r * Mathf.Deg2Rad));
                current = node.TileMap.GetRect(arbitraryGameObject.transform.position.ToTilePosition(), false, 5);
                if(att > 360)
                    throw new Exception("BUBBLE: FAILED TO PLACE PROCEDURAL NODE");
                att += 2;
            }
            Debug.Log($"Placing Node [{i}] took {att - start} attempts".WithColor("#5500FF"));
            toPrev = prevPosition - arbitraryGameObject.transform.position;
            toPrev = toPrev.normalized;
            prevNode = node;
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
        bool shop = i == 2 || i == 4 || i == 6 || i == totalNodes - 1;
        bool largo = i == totalNodes - 1;
        bool? crucible = i % 3 == 1 && !largo ? Utils.RollWithLuck(0.5f) : null;
        if (i <= 1) //Don't want crucible on first room
            crucible = false;
        WorldNode node = NodeID.GetRandomNodeWithParameters(NodeID.Nodes, 0,
            shop,
        crucible,
            largo);
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

        WorldNode node = NodeID.GetRandomNodeWithParameters(NodeID.SecretNodes, 0, null, null, null);
        node.Generate(n.transform.position, this, bestGenOwner, null, false);

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
                        Map.SetTile(pos, TileID.Dirt.BorderTileType);
                    }
                    else
                        Map.SetTile(pos, TileID.Grass.BorderTileType);
                }
            }
        }
    }
    public void Update()
    {
        m_Instance = this;
        #if UNITY_EDITOR
        //if(Input.GetKey(KeyCode.R) && Main.DebugCheats)
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        #endif
    }
    private int FrameDelay = 0;
    public void FixedUpdate()
    {
        //This is to account for the time it takes unity to set up the tilecollider physics and other tilemap stuff (ran on the first fixed update)
        if(++FrameDelay == 2)
        {
            CreateLightingVertices();
        }
    }
    public class TileLightSegment
    {
        public List<Vector3> Vertices;
        public TileLightSegment(List<Vector3> Vertices)
        {
            this.Vertices = Vertices;
        }
        public void Extend(float x, float y) => Extend(new Vector2(x, y));
        public void Extend(Vector2 direction)
        {
            for(int i = Vertices.Count - 1; i >= 0; --i)
            {
                Vector3 extend = Vertices[i];
                extend.x += direction.x;
                extend.y += direction.y;
                Vertices.Add(extend);
            }
            Vertices.Reverse();
        }
        public void Unleash()
        {
            Light2D L = GameObject.Instantiate(tileShadowPrefab, Vector2.zero, Quaternion.identity).GetComponent<Light2D>();
            L.SetShapePath(Vertices.ToArray());
            L.transform.parent = ShadowParent.transform;
            L.name = $"Shadow {Vertices.Count}";
        }
    }
    public void PossibleEdgesAtIntersectionPoint(Vector3 point, out bool possibleBotEdge, out bool possibleTopEdge, out bool possibleRightEdge, out bool possibleLeftEdge)
    {
        Vector3Int forSpeed = RealTileMap.Map.WorldToCell(point + new Vector3(0.5f, 0.5f));
        bool tileTopRight = World.SolidTile(forSpeed);
        forSpeed.x -= 1;
        bool tileTopLeft = World.SolidTile(forSpeed);
        forSpeed.y -= 1;
        bool tileBotLeft = World.SolidTile(forSpeed);
        forSpeed.x += 1;
        bool tileBotRight = World.SolidTile(forSpeed);
        int count = 0;
        if (tileTopRight) ++count;
        if (tileTopLeft) ++count;
        if (tileBotLeft) ++count;
        if (tileBotRight) ++count;
        //Debug.Log($"AdjacentTileCount[{point}] = {count}");
        possibleBotEdge = possibleTopEdge = possibleRightEdge = possibleLeftEdge = false;
        if(count == 3)
        {
            possibleBotEdge = tileBotRight && tileBotLeft;
            possibleTopEdge = tileTopLeft && tileTopRight;
            possibleRightEdge = tileTopRight && tileBotRight;
            possibleLeftEdge = tileTopLeft && tileBotLeft;
        }
        else if(count == 1)
        {
            if (tileBotLeft)
                possibleBotEdge = possibleLeftEdge = true;
            if (tileTopRight)
                possibleRightEdge = possibleTopEdge = true;
            if (tileTopLeft)
                possibleTopEdge = possibleLeftEdge = true;
            if(tileBotRight)
                possibleBotEdge = possibleRightEdge = true;
        }
    }
    public static GameObject ShadowParent;
    public void CreateLightingVertices()
    {
        ShadowParent = new GameObject("Shadow Parent");
        TilemapCollider2D collider = RealTileMap.Map.GetComponent<TilemapCollider2D>(); //temp
        CompositeCollider2D composite = collider.composite;
        List<TileLightSegment> Lights = new();
        int paths = composite.pathCount;
        for(int i = 1; i < paths; ++i) //i = 1 to skip the first path, which is the exterior (does not need a shadow)
        {
            int count = composite.GetPathPointCount(i);
            Vector2[] points = new Vector2[count];
            List<Vector3> lightPoints = new(); //Unity why I need to do this? are you stupid? why is a light 2D use vector3 for its path
            composite.GetPath(i, points);
            bool continuityBroken = false;
            bool? prevValid = null;
            int start = -1;
            int end = count;
            for (int j = 0; j < end; ++j)
            {
                Vector3 point = points[j % count];
                Vector3 next = points[(j + 1) % count]; 
                //int k = (j - 1) % count;
                //if (k < 0)
                //    k = k + count;
                //Vector3 prev = points[k];
                bool valid = false;

                Vector3 travelDirection = next - point;
                bool movingHorizontal = Mathf.Abs(travelDirection.x) > 0.2f;
                bool movingVertical = Mathf.Abs(travelDirection.y) > 0.2f;
                PossibleEdgesAtIntersectionPoint(point, out bool posBotEdge, out bool posTopEdge, out bool posRightEdge, out bool posLeftEdge);
                PossibleEdgesAtIntersectionPoint(next, out bool posBotEdgeN, out bool posTopEdgeN, out bool posRightEdgeN, out bool posLeftEdgeN);
                bool top = posTopEdge && posTopEdgeN && movingHorizontal;
                bool bot = posBotEdge && posBotEdgeN && movingHorizontal;
                bool left = posLeftEdge && posLeftEdgeN && movingVertical;
                bool right = posRightEdge && posRightEdgeN && movingVertical;
                //bool nextTileTopRight = World.SolidTile(RealTileMap.Map.WorldToCell(next + new Vector3(0.5f, 0.5f)));
                //bool nextTileTopLeft = World.SolidTile(RealTileMap.Map.WorldToCell(next + new Vector3(-0.5f, 0.5f)));
                //bool nextTileBotRight = World.SolidTile(RealTileMap.Map.WorldToCell(next + new Vector3(0.5f, -0.5f)));
                //bool nextTileBotLeft = World.SolidTile(RealTileMap.Map.WorldToCell(next + new Vector3(-0.5f, -0.5f)));

                ////bool prevTileTopRight = World.SolidTile(RealTileMap.Map.WorldToCell(prev + new Vector3(0.5f, 0.5f)));
                ////bool prevTileTopLeft = World.SolidTile(RealTileMap.Map.WorldToCell(prev + new Vector3(-0.5f, 0.5f)));
                ////bool prevTileBotRight = World.SolidTile(RealTileMap.Map.WorldToCell(prev + new Vector3(0.5f, -0.5f)));
                ////bool prevTileBotLeft = World.SolidTile(RealTileMap.Map.WorldToCell(prev + new Vector3(-0.5f, -0.5f)));

                //Vector3 travelDirectionToPrev = prev - point;

                //bool solidRightEdge = travelDirection.y != 0 && (travelDirection.y < 0 ? tileBotRight && nextTileTopRight : tileTopRight && nextTileBotRight);
                //bool solidTopEdge = travelDirection .x != 0 && (travelDirection.x > 0 ? tileTopRight && nextTileTopLeft : tileTopLeft && nextTileTopRight);
                //bool previousRightEdge = travelDirectionToPrev.y != 0 && (travelDirectionToPrev.y > 0 ? tileBotRight && prevTileTopRight : tileTopRight && prevTileBotRight);
                //bool previousTopEdge = travelDirectionToPrev.x != 0 && (travelDirectionToPrev.x < 0 ? tileTopRight && prevTileTopLeft : tileTopLeft && prevTileTopRight);
                //For this shadow scenario (tentative), the light source is in the top right

                //if (!tileBotLeft) //Casting a shadow to the botLeft means if there is no tile there, we want a shadow
                //    valid = true;
                //else if(tileTopRight)
                //    valid = true;
                //if (bot || left)
                //    valid = false;
                //else 
                if (right || top) // || previousRightEdge || previousTopEdge)
                    valid = true;
                else
                    valid = false;
                //previousRightEdge = solidRightEdge;
                //previousTopEdge = solidTopEdge;
                if(prevValid == null)
                    prevValid = valid;
                if(start == -1)
                {
                    if(prevValid != valid && valid) //ON A TRANSITIONPT
                    {
                        var obj = GameObject.Instantiate(shadowPT, point, Quaternion.identity);
                        obj.name = $"START[{i}:{j}]";
                        obj.transform.parent = ShadowParent.transform;
                        start = j;
                        end += start;
                    }
                }
                if(start != -1)
                {
                    if (continuityBroken)
                    {
                        if (lightPoints.Count >= 2)
                            Lights.Add(new(lightPoints));
                        lightPoints = new();
                        continuityBroken = false;
                    }
                    if(!continuityBroken)
                    {
                        if (prevValid.Value || valid)
                            lightPoints.Add(point);
                        continuityBroken = !valid;
                    }
                }
                prevValid = valid;
            }
            if (lightPoints.Count >= 2)
                Lights.Add(new(lightPoints));
        }
        foreach (TileLightSegment light in Lights)
        {
            light.Extend(-4, -2);
            light.Unleash();
        }
        CreateOcclusionSpriteLighting();
    }
    public void CreateOcclusionSpriteLighting()
    {
        Tilemap.Map.GetCorners(out int left, out int right, out int bot, out int top);
        int w = right - left;
        int h = top - bot;
        Texture2D lightTexture = new(w * 2, h * 2);
        Color c1 = Color.clear;
        Color c2 = Color.white;
        for (int i = 0; i < w; ++i)
        {
            for (int j = 0; j < h; ++j)
            {
                Vector3Int pos = new Vector3Int(i + left, j + bot);
                Color c = World.SolidTile(pos) ? c2 : c1;
                int i2 = i * 2;
                int j2 = j * 2;
                lightTexture.SetPixel(i2, j2, c);
                lightTexture.SetPixel(i2 + 1, j2, c);
                lightTexture.SetPixel(i2, j2 + 1, c);
                lightTexture.SetPixel(i2 + 1, j2 + 1, c);
            }
        }
        lightTexture.Apply();
        Sprite lightSprite = Sprite.Create(lightTexture, new Rect(0, 0, lightTexture.width, lightTexture.height), Vector2.zero, 1f, 0, SpriteMeshType.FullRect);
        Light2D L = GameObject.Instantiate(OcclusionShadowPrefab, new Vector3(left * 2, bot * 2), Quaternion.identity).GetComponent<Light2D>();
        L.lightCookieSprite = lightSprite;
    }
}