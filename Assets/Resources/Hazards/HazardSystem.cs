using System.Collections.Generic;
using UnityEngine;

public static class HazardSystem
{
    public static GameObject OilObject => Resources.Load<GameObject>("Hazards/OilSplat");
    public class Hazard
    {
        public Vector2Int Position;
        public int Duration { get; set; } = 0;
        public HazardType Type { get; private set; }
        public GameObject PairedObject { get; private set; }
        public bool Dead { get; set; } = false;
        public Hazard(Vector2Int pos, HazardType type, int DurationInTicks = 100)
        {
            Position = pos;
            Type = type;
            Duration = DurationInTicks;
            AttachGameObject();
        }
        public void AttachGameObject()
        {
            if(Type == HazardType.Oil)
                PairedObject = GameObject.Instantiate(OilObject, World.RealTileMap.Map.CellToWorld((Vector3Int)Position), Quaternion.identity, Main.GenericSuperParent);
        }
        public void Update()
        {
            --Duration;
            if (Duration <= 0)
                Dead = true;
        }
        public void Kill()
        {
            if (PairedObject != null)
                GameObject.Destroy(PairedObject);
        }
    }
    public enum HazardType
    {
        None = 0,
        Oil = 1,
    }
    public static Dictionary<Vector2Int, Hazard> HazardTilemap = new(1000);
    public static bool GetHazard(Vector2Int position, out Hazard hazard)
    {
        if (HazardTilemap.TryGetValue(position, out hazard))
            return true;
        hazard = null;
        return false;
    }
    public static bool GetHazard(Vector2 worldPos, out Hazard hazard) => GetHazard(ToHazardPosition(worldPos), out hazard);
    public static bool GetHazard(int x, int y, out Hazard hazard) => GetHazard(new Vector2Int(x, y), out hazard);
    private static readonly List<Vector2Int> ExpiredKeys = new();
    public static void FixedUpdate()
    {
        ExpiredKeys.Clear();

        // 1. Tick down durations and mark expired ones
        foreach (KeyValuePair<Vector2Int, Hazard> pair in HazardTilemap)
        {
            pair.Value.Update();
            if(pair.Value.Dead)
                ExpiredKeys.Add(pair.Key);
        }

        for (int i = 0; i < ExpiredKeys.Count; i++)
            RemoveHazard(ExpiredKeys[i]);
    }
    private static void RemoveHazard(Vector2Int pos)
    {
        HazardTilemap[pos].Kill();
        HazardTilemap.Remove(pos);
    }
    public static void ClearAll()
    {
        foreach (KeyValuePair<Vector2Int, Hazard> pair in HazardTilemap)
            pair.Value.Kill();
        HazardTilemap.Clear();
    }
    public static Vector2Int ToHazardPosition(Vector2 worldPosition)
    {
        return (Vector2Int)World.WorldPosition(worldPosition);
    }
    public static Hazard AddHazard (Vector2 worldPosition, HazardType type, int duration, bool overrideOld = true) => AddHazard(ToHazardPosition(worldPosition), type, duration, overrideOld);
    public static Hazard AddHazard(Vector2Int position, HazardType type, int duration, bool overrideOld = true)
    {
        if(GetHazard(position, out Hazard existing))
        {
            if (existing.Type == type)
            {
                if (existing.Duration < duration)
                    existing.Duration = duration;
                return existing;
            }
            else if (!overrideOld)
                return existing;
            else
                existing.Kill();
        }
        return HazardTilemap[position] = new Hazard(position, type, duration);
    }
}
