using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class NodeID
{
    public static readonly List<WorldNode> Nodes = new();
    public static readonly List<WorldNode> SubNodes = new();
    public static readonly List<WorldNode> MeadowNodes = new();
    public static bool LoadAllNodes()
    {
        Nodes.Clear();
        SubNodes.Clear();
        MeadowNodes.Clear();
        var nodes = Resources.LoadAll<GameObject>("World/Nodes");
        //var subNodes = Resources.LoadAll<GameObject>("World/Nodes/SubNodes");
        Debug.Log("Loading Nodes...".WithColor("FFFF00"));
        foreach (GameObject obj in nodes)
        {
            if(obj.TryGetComponent(out WorldNode n))
            {
                if(n.IsSubNode)
                {
                    SubNodes.Add(n);
                    Debug.Log($"Loaded SubNode: {obj.name.WithColor("#FFFF00")}");
                }
                else
                {
                    Nodes.Add(n);
                    Debug.Log($"Loaded Node: {obj.name.WithColor("#FF77AA")}");
                }
            }
            else
            {
                Debug.Log($"Skipped loading {obj.name.WithColor("#FF0000")} as it was not a node");
            }
        }
        return true;
    }
    public static WorldNode GetRandomNodeWithParameters(List<WorldNode> pool, int zoneID = 0, bool? shop = null, bool? crucible =  null, bool? needsLarge = null)
    {
        List<WorldNode> viableNodes = new();
        foreach (WorldNode n in pool)
        {
            if(n.Zone == zoneID && 
                (shop == null || shop.Value == n.HasShop) && 
                (crucible == null || crucible.Value == n.HasCrucible) && 
                (needsLarge == null || needsLarge.Value == n.CountsAsLargeNode))
            {
                viableNodes.Add(n);
            }
        }
        WorldNode pickedNode = null;
        while (viableNodes.Count > 0)
        {
            int i = Utils.RandInt(viableNodes.Count);
            pickedNode = viableNodes[i];
            if (pickedNode.Weighting >= Utils.RandFloat())
                return pickedNode;
            else
                viableNodes.RemoveAt(i);
        }
        return pickedNode;
    }
}

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class WorldNode : MonoBehaviour
{
    #if UNITY_EDITOR
    public void Update() => RoundPosition();
    #endif
    public void Start()
    {
        RoundPosition();
    }
    public void RoundPosition()
    {
        Vector3Int transformPos = new((int)transform.position.x / 2, (int)transform.position.y / 2);
        transform.position = transformPos * 2;
    }
    public World World { get; set; }
    public byte GenerationNumber { get; set; } = 0;
    public Tilemap TileMap;
    public bool IsSubNode = false;
    public Tilemap GetTileMap()
    {
        TileMap = TileMap != null ? TileMap : GetComponentInChildren<Tilemap>();
        return TileMap;
    }
    public void Generate(World world, byte GenNumber, WorldNode PreviousNode = null)
    {
        GenerationNumber = GenNumber;
        this.World = world;
        Vector3Int transformPos = new(Mathf.FloorToInt(transform.position.x / 2), Mathf.FloorToInt(transform.position.y / 2));
        GatherConnectors();
        GetTileMap();
        TileMap.GetCorners(out int left, out int right, out int bottom, out int top);
        for(int i = left; i < right; ++i)
        {
            for(int j = bottom; j < top; ++j)
            {
                Vector3Int v = new(i, j);
                var tile = TileMap.GetTile(v);
                v += transformPos;
                if (tile != null && !world.Tilemap.Map.HasTile(v))
                {
                    world.Tilemap.Map.SetTile(v, tile);
                    World.SetTileData(v, new World.TileData(GenerationNumber));
                }
            }
        }
        GeneratePaths(PreviousNode);
        if(FeatureParent != null)
        {
            for (int i = FeatureParent.childCount - 1; i >= 0; --i)
                FeatureParent.GetChild(i).parent = world.NatureParent.transform;
        }
        if(PylonParent != null)
        {
            for (int i = PylonParent.childCount - 1; i >= 0; --i)
                PylonParent.GetChild(i).parent = world.PylonParent;
        }
        //FeatureParent.DetachChildren();
        gameObject.SetActive(false);
    }
    public void GatherConnectors()
    {
        Connectors.Clear();
        foreach (NodeConnector con in ConnectorParent.GetComponentsInChildren<NodeConnector>())
            Connectors.Add(con);
    }
    public Transform ConnectorParent;
    public Transform FeatureParent;
    public Transform PylonParent;
    public int Zone = -1;
    public float Weighting = 1.0f;
    public readonly List<NodeConnector> Connectors = new();
    public static bool OverrideTiles = true;
    public bool HasShop = false;
    public bool HasCrucible = false;
    public bool CountsAsLargeNode = false;
    public void GetClosestConnectors(List<NodeConnector> starts, List<NodeConnector> ends, 
        out NodeConnector bestStart, out NodeConnector bestEnd, out float bestDist)
    {
        bestStart = bestEnd = null;
        bestDist = float.MaxValue;
        foreach (NodeConnector start in starts)
        {
            if (!start.ExitNode)
                continue;
            foreach (NodeConnector end in ends)
            {
                if (!end.EntranceNode)
                    continue;
                float dist = start.Distance(end);
                if (dist < bestDist)
                {
                    bestStart = start;
                    bestEnd = end;
                    bestDist = dist;
                }
            }
        }
    }
    public void GetClosestSingleNode(Vector2 pos, List<NodeConnector> nodes,
        out NodeConnector bestNode, out float bestDist)
    {
        bestNode = null;
        bestDist = float.MaxValue;
        foreach (NodeConnector node in nodes)
        {
            if (!node.EntranceNode)
                continue;
            float dist = pos.Distance(node.Position);
            if (dist < bestDist)
            {
                bestNode = node;
                bestDist = dist;
            }
        }
    }
    public void GeneratePaths(WorldNode prev)
    {
        if (World == null || prev == null)
            return;
        List<NodeConnector> starts = prev.Connectors;
        List<NodeConnector> ends = Connectors;
        if (Connectors.Count <= 0 || ends.Count <= 0)
            return;
        GetClosestConnectors(starts, ends, out NodeConnector bestStart, out NodeConnector bestEnd, out float bestDist);
        if (bestStart == null || bestEnd == null || bestDist <= 0 || bestDist > 2000)
            return;
        OverrideTiles = bestStart.OverrideTiles && bestEnd.OverrideTiles;
        CanGenerateRoadBlock = true;
        GeneratePath(bestStart.Position, bestEnd.Position);
    }
    public void GeneratePath(Vector2 start, Vector2 end)
    {
        bool roadBlock = CanGenerateRoadBlock;
        Vector2 startToEnd = end - start;
        Vector2 norm = startToEnd.normalized;
        Vector2 prev = start;
        float pathVariance = Mathf.Max(4, startToEnd.magnitude / 4f);
        int totalPoints = 5;
        float iterAmt = 1f / (totalPoints + 1);
        int i = 0;
        Vector2 subNodeConPos = Vector2.zero;
        Vector2 subNodePos = Vector2.zero;
        for(float per = iterAmt; per < 1; per += iterAmt)
        {
            float sin = Mathf.Sin(Mathf.PI * per);
            Vector2 pointBetween = Vector2.Lerp(start, end, per);
            Vector2 rNorm = norm.RotatedBy(Mathf.PI / 2f * Utils.Rand1OrMinus1());
            pointBetween += Utils.RandFloat(pathVariance * 0.25f, pathVariance) * sin * rNorm;
            GenerateLine(prev, pointBetween);
            prev = pointBetween;
            if(i == 2)
            {
                subNodeConPos = pointBetween;
                subNodePos = pointBetween + rNorm * (pathVariance * 1.5f + 10);
            }
            ++i;
        }
        if (roadBlock)
            TryGeneratingEndRoadBlock = true;
        GenerateLine(prev, end);
        if(!IsSubNode)
        {
            WorldNode sub = Instantiate(NodeID.GetRandomNodeWithParameters(NodeID.SubNodes, 0), subNodePos, Quaternion.identity);
            sub.RoundPosition();
            sub.Generate(World, GenerationNumber, null);
            sub.GetClosestSingleNode(subNodeConPos, sub.Connectors, out NodeConnector best, out float _);
            OverrideTiles = best.OverrideTiles;
            sub.GeneratePath(subNodeConPos, best.Position);
        }
    }
    public void GenerateLine(Vector2 start, Vector2 end)
    {
        Vector2 bestEnd = end;
        Vector2 startToEnd = end - start;
        float dist = startToEnd.magnitude;
        Vector2 norm = startToEnd / dist;
        for (float i = 0; i < dist; ++i)
        {
            //Vector3Int v = new(Mathf.FloorToInt(start.x / 2), Mathf.FloorToInt(start.y / 2));
            bool isRoadblock = DiamondBrush(start, 2);
            start += norm;
            if (isRoadblock)
            {
                if (CanGenerateRoadBlock)
                {
                    Roadblock r = Instantiate(Main.PrefabAssets.Roadblock, start, Quaternion.identity).GetComponent<Roadblock>();
                    r.ProgressionLevel = GenerationNumber;
                    r.transform.SetParent(World.RoadblockParent);
                    CanGenerateRoadBlock = false;
                }
                bestEnd = start;
            }
        }
        if (TryGeneratingEndRoadBlock)
        {
            Roadblock r = Instantiate(Main.PrefabAssets.Roadblock, bestEnd, Quaternion.identity).GetComponent<Roadblock>();
            r.ProgressionLevel = GenerationNumber;
            r.transform.SetParent(World.RoadblockParent);
            r.IsEndRoadblock = true;
            r.portal.gameObject.SetActive(false);
            TryGeneratingEndRoadBlock = false;
        }
    }
    private bool CanGenerateRoadBlock { get; set; } = false;
    private bool TryGeneratingEndRoadBlock { get; set; } = false;
    public bool DiamondBrush(Vector2 center, int radias)
    {
        bool isValidForRoadblock = false;
        var tile = TileID.Grass.TileType;
        float percent = 0;
        float iter = 0.25f / radias;
        for (float j = -radias; j <= radias; j += 0.5f)
        {
            int sin = Mathf.FloorToInt(Mathf.Sin(percent * Mathf.PI) * radias + 0.5f);
            for(int i = -sin; i <= sin; ++i)
            {
                Vector3Int v = new(Mathf.FloorToInt(center.x / 2 + i), Mathf.FloorToInt(center.y / 2 + j));
                bool canGenerate = World.GetTileData(v).IsRoadblock;
                if(World.Tilemap.Map.GetTile(v) == null || (World.SolidTile(v) && OverrideTiles))
                {
                    World.Tilemap.Map.SetTile(v, tile);
                    //Instantiate(Main.PrefabAssets.Roadblock, center + new Vector2(i * 2, j * 2), Quaternion.identity);
                    //for(int x = -2; x <= 2; ++x)
                    //{
                    //    for(int y = -2; y <= 2; ++y)
                    //    {
                    //        var v2 = v + new Vector3Int(x, y);
                    //        if (World.Tilemap.Map.GetTile(v2) == null)
                    //            World.SetTileData(v2, new World.TileData(GenerationNumber, true));
                    //    }
                    //}
                    World.SetTileData(v, new World.TileData(GenerationNumber, true));
                    canGenerate = true;
                }
                if (i == 0 && j == 0 && canGenerate)
                {
                    isValidForRoadblock = true;
                }
            }
            percent += iter;
        }
        return isValidForRoadblock;
    }
}
