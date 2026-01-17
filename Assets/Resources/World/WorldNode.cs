using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

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
    public Tilemap TileMap;
    public bool IsSubNode = false;
    public Tilemap GetTileMap()
    {
        TileMap = TileMap != null ? TileMap : GetComponentInChildren<Tilemap>();
        return TileMap;
    }
    public void Generate(World world, WorldNode PreviousNode = null)
    {
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
                    world.Tilemap.Map.SetTile(v, tile);
            }
        }
        GeneratePaths(world, PreviousNode);
        FeatureParent.DetachChildren();
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
    public readonly List<NodeConnector> Connectors = new();
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
    public void GeneratePaths(World world, WorldNode prev)
    {
        if (world == null || prev == null)
            return;
        List<NodeConnector> starts = prev.Connectors;
        List<NodeConnector> ends = Connectors;
        if (Connectors.Count <= 0 || ends.Count <= 0)
            return;
        GetClosestConnectors(starts, ends, out NodeConnector bestStart, out NodeConnector bestEnd, out float bestDist);
        if (bestStart == null || bestEnd == null || bestDist <= 0 || bestDist > 2000)
            return;
        GeneratePath(world, bestStart.Position, bestEnd.Position);
    }
    public void GeneratePath(World world, Vector2 start, Vector2 end)
    {
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
            GenerateLine(world, prev, pointBetween);
            prev = pointBetween;
            if(i == 2)
            {
                subNodeConPos = pointBetween;
                subNodePos = pointBetween + rNorm * (pathVariance * 1.5f + 10);
            }
            ++i;
        }
        GenerateLine(world, prev, end);
        if(!IsSubNode)
        {
            WorldNode sub = Instantiate(Main.PrefabAssets.CrucibleNode, subNodePos, Quaternion.identity).GetComponent<WorldNode>();
            sub.RoundPosition();
            sub.Generate(world, null);
            sub.GetClosestSingleNode(subNodeConPos, sub.Connectors, out NodeConnector best, out float _);
            sub.GeneratePath(world, subNodeConPos, best.Position);
        }
    }
    public void GenerateLine(World world, Vector2 start, Vector2 end)
    {
        Vector2 startToEnd = end - start;
        float dist = startToEnd.magnitude;
        Vector2 norm = startToEnd / dist;
        for (float i = 0; i < dist; ++i)
        {
            //Vector3Int v = new(Mathf.FloorToInt(start.x / 2), Mathf.FloorToInt(start.y / 2));
            DiamondBrush(world, start, 2);
            start += norm;
        }
    }
    public void DiamondBrush(World world, Vector2 center, int radias)
    {
        var tile = TileID.Grass.TileType;
        float percent = 0;
        float iter = 0.25f / radias;
        for (float j = -radias; j <= radias; j += 0.5f)
        {
            int sin = Mathf.FloorToInt(Mathf.Sin(percent * Mathf.PI) * radias + 0.5f);
            for(int i = -sin; i <= sin; ++i)
            {
                Vector3Int v = new(Mathf.FloorToInt(center.x / 2 + i), Mathf.FloorToInt(center.y / 2 + j));
                if(world.Tilemap.Map.GetTile(v) == null || world.SolidTile(v))
                    world.Tilemap.Map.SetTile(v, tile);
            }
            percent += iter;
        }
    }
}
