using System.Collections.Generic;
using UnityEngine;

public static class HazardSystem
{
    public static GameObject OilObject => Resources.Load<GameObject>("Hazards/OilSplat");
    public class Hazard
    {
        public Vector2Int Position;
        public Vector2 WorldPosition { get; private set; }
        public int Duration { get; set; } = 0;
        public HazardType Type { get; private set; }
        public List<FloorHazard> PairedObjects { get; private set; }
        public bool Dead { get; set; } = false;
        public float SizeMultiplier { get; set; }
        public Hazard(Vector2Int pos, Vector2 worldPosition, HazardType type, int DurationInTicks, float sizeMultiplier)
        {
            Position = pos;
            Type = type;
            Duration = DurationInTicks;
            SizeMultiplier = sizeMultiplier;
            WorldPosition = worldPosition; 
            PairedObjects = new();
            AttachGameObject(worldPosition);
        }
        public void AttachGameObject(Vector2 worldPos)
        {
            if(Type == HazardType.Oil)
            {
                //worldPos.y -= 0.7f;
                var newObj = GameObject.Instantiate(OilObject, worldPos, Quaternion.identity, Main.GenericSuperParent).GetComponent<FloorHazard>();
                newObj.Init(Type, Duration, SizeMultiplier);
                PairedObjects.Add(newObj);
            }
        }
        public void Update()
        {
            for(int i = PairedObjects.Count - 1; i >= 0; --i)
            {
                var pairedObject = PairedObjects[i];
                bool alive = pairedObject.TickUpdate();
                if(!alive)
                {
                    GameObject.Destroy(pairedObject.gameObject);
                    PairedObjects.RemoveAt(i);
                }
            }
            --Duration;
            if (Duration <= 0)
                Dead = true;
        }
        public void Kill()
        {
            foreach (FloorHazard h in PairedObjects)
                GameObject.Destroy(h.gameObject);
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
        //We don't actually need to kill, as the splatters are part of generic superparent which autoclears!
        //foreach (KeyValuePair<Vector2Int, Hazard> pair in HazardTilemap)
        //    pair.Value.Kill();
        HazardTilemap.Clear();
    }
    public static Vector2Int ToHazardPosition(Vector2 worldPosition)
    {
        return (Vector2Int)World.WorldPosition(worldPosition);
    }
    public static Hazard AddHazard(Vector2 worldPosition, HazardType type, int duration, float size, bool overrideOld = true) => AddHazard(ToHazardPosition(worldPosition), worldPosition, type, duration, size, overrideOld);
    public static Hazard AddHazard(Vector2Int position, Vector2 worldPosition, HazardType type, int duration, float size, bool overrideOld = true)
    {
        if(GetHazard(position, out Hazard existing))
        {
            if (existing.Type == type)
            {
                if (existing.Duration < duration)
                    existing.Duration = duration;
                if (existing.SizeMultiplier < size)
                    existing.SizeMultiplier = size;
                if(overrideOld || existing.WorldPosition.Distance(worldPosition) > 1.1f)
                    existing.AttachGameObject(worldPosition);
                return existing;
            }
            else if (!overrideOld)
                return existing;
            else
                existing.Kill();
        }
        return HazardTilemap[position] = new Hazard(position, worldPosition, type, duration, size);
    }
}
