using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HazardSystem
{
    public static GameObject OilObject = Resources.Load<GameObject>("Hazards/OilSplat");
    public static Sprite[] SplatterSprites = new Sprite[] { 
        Resources.Load<Sprite>("Hazards/Splatter1"),
        Resources.Load<Sprite>("Hazards/Splatter2"), 
        Resources.Load<Sprite>("Hazards/Splatter3"), 
        Resources.Load<Sprite>("Hazards/Splatter4"), 
        Resources.Load<Sprite>("Hazards/Splatter5") };
    public class Hazard
    {
        public Player PlayerOwner;
        public Vector2Int Position;
        public Vector2 WorldPosition { get; set; }
        public int Duration { get; set; } = 0;
        public HazardType Type { get; private set; }
        public LinkedList<FloorHazard> PairedObjects { get; private set; }
        public bool Dead { get; set; } = false;
        public float SizeMultiplier { get; set; }
        public int SpecialInformationNumber { get; set; }
        public int TimePassed { get; set; } = 0;
        public Hazard(Vector2Int pos, Vector2 worldPosition, HazardType type, int DurationInTicks, float sizeMultiplier, int infoNum)
        {
            Position = pos;
            Type = type;
            Duration = DurationInTicks;
            SizeMultiplier = sizeMultiplier;
            WorldPosition = worldPosition; 
            PairedObjects = new();
            SpecialInformationNumber = infoNum;
            AttachGameObject(worldPosition);
        }
        public void AttachGameObject(Vector2 worldPos)
        {
            if (PairedObjects.Count > 5)
            {
                var objectToReplace = PairedObjects.First();
                float timeLeft = objectToReplace.InitDuration - objectToReplace.Counter;
                if(Duration - timeLeft > 20) //If the new hazard would refresh at least 20 (.2s) ticks on the visual, then replace it
                {
                    GameObject.Destroy(objectToReplace.gameObject);
                    PairedObjects.RemoveFirst();
                }
                else
                    return;
            }
            FloorHazard newObj = null;
            if (Type == HazardType.Oil)
            {
                //worldPos.y -= 0.7f;
                newObj = GameObject.Instantiate(OilObject, worldPos, Quaternion.identity, Main.GenericSuperParent).GetComponent<FloorHazard>();
                newObj.Init(Type, Duration, SizeMultiplier);
            }
            else if (Type == HazardType.FireOil)
            {
                //worldPos.y -= 0.7f;
                newObj = GameObject.Instantiate(OilObject, worldPos, Quaternion.identity, Main.GenericSuperParent).GetComponent<FloorHazard>();
                newObj.Init(Type, Duration, SizeMultiplier);
            }
            if (newObj == null)
                return;
            PairedObjects.AddLast(newObj);
        }
        public void Update()
        {
            LinkedListNode<FloorHazard> currentNode = PairedObjects.First;

            while (currentNode != null)
            {
                // Save the NEXT node before deleting the current one
                LinkedListNode<FloorHazard> nextNode = currentNode.Next;
                
                var pairedObject = currentNode.Value;
                bool alive = pairedObject.TickUpdate();
                if (!alive)
                {
                    GameObject.Destroy(pairedObject.gameObject);
                    PairedObjects.Remove(currentNode);
                }

                // Move to the next node
                currentNode = nextNode;
            }
            ++TimePassed;
            if (TimePassed > Duration)
                Dead = true;
            else if(Type == HazardType.FireOil && SpecialInformationNumber > 0 && PlayerOwner != null)
            {
                if(TimePassed == 10)
                {
                    TryDetonatingOil(Position + new Vector2Int(1, 0), PlayerOwner, SpecialInformationNumber - 1);
                    TryDetonatingOil(Position + new Vector2Int(0, 1), PlayerOwner, SpecialInformationNumber - 1);
                    TryDetonatingOil(Position + new Vector2Int(-1, 0), PlayerOwner, SpecialInformationNumber - 1);
                    TryDetonatingOil(Position + new Vector2Int(0, -1), PlayerOwner, SpecialInformationNumber - 1);
                }
                else if (TimePassed == 20 && SpecialInformationNumber > 1)
                {
                    TryDetonatingOil(Position + new Vector2Int(1, 1), PlayerOwner, SpecialInformationNumber - 2);
                    TryDetonatingOil(Position + new Vector2Int(-1, 1), PlayerOwner, SpecialInformationNumber - 2);
                    TryDetonatingOil(Position + new Vector2Int(-1, -1), PlayerOwner, SpecialInformationNumber - 2);
                    TryDetonatingOil(Position + new Vector2Int(1, -1), PlayerOwner, SpecialInformationNumber - 2);
                    SpecialInformationNumber = -1;
                }
            }
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
        FireOil = 2,
    }
    public static Dictionary<Vector2Int, Hazard> HazardTilemap = new(1000);
    public static readonly List<Vector2Int> ExpiredKeys = new();
    public static readonly Dictionary<Vector2Int, BurnSpreadData> KeysRequestingFireSpread = new();
    public static bool GetHazard(Vector2Int position, out Hazard hazard)
    {
        if (HazardTilemap.TryGetValue(position, out hazard))
            return true;
        hazard = null;
        return false;
    }
    public static bool GetHazard(Vector2 worldPos, out Hazard hazard) => GetHazard(ToHazardPosition(worldPos), out hazard);
    public static bool GetHazard(int x, int y, out Hazard hazard) => GetHazard(new Vector2Int(x, y), out hazard);
    public static void FixedUpdate()
    {
        foreach (KeyValuePair<Vector2Int, BurnSpreadData> pair in KeysRequestingFireSpread)
            DetonateOil(pair.Key, pair.Value);

        KeysRequestingFireSpread.Clear();
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
        return (Vector2Int)World.RealPosToTilePos(worldPosition);
    }
    public static Hazard AddHazard(Vector2 worldPosition, HazardType type, int duration, float size, int infoNum = 0, bool overrideOld = true) => AddHazard(ToHazardPosition(worldPosition), worldPosition, type, duration, size, infoNum, overrideOld);
    public static Hazard AddHazard(Vector2Int position, Vector2 worldPosition, HazardType type, int duration, float size, int infoNum = 0, bool overrideOld = true)
    {
        if(GetHazard(position, out Hazard existing))
        {
            if (existing.Type == type)
            {
                existing.TimePassed = 0;
                if (existing.Duration < duration)
                    existing.Duration = duration;
                if (existing.SizeMultiplier < size)
                    existing.SizeMultiplier = size;
                if (overrideOld || existing.WorldPosition.Distance(worldPosition) > 0.75f)
                {
                    existing.WorldPosition = worldPosition;
                    existing.AttachGameObject(worldPosition);
                }
                return existing;
            }
            else if (!overrideOld)
                return existing;
            else if (existing.Type != HazardType.FireOil || type != HazardType.Oil) //Cannot place oil on fireoil
                existing.Kill();
            else
                return existing;
        }
        return HazardTilemap[position] = new Hazard(position, worldPosition, type, duration, size, infoNum);
    }
    public struct BurnSpreadData
    {
        public Player player;
        public int recursionLevel;
        public BurnSpreadData(Player player, int recursionLevel)
        {
            this.player = player;
            this.recursionLevel = recursionLevel;
        }
    }
    public static void TryDetonatingOil(Vector2 worldPosition, Player PlausibleOwner, int recursionLevel = 0) => TryDetonatingOil(ToHazardPosition(worldPosition), PlausibleOwner, recursionLevel);
    public static void TryDetonatingOil(Vector2Int position, Player PlausibleOwner, int recursionLevel = 0)
    {
        if (GetHazard(position, out Hazard existing) && existing.Type == HazardType.Oil && (!KeysRequestingFireSpread.TryGetValue(position, out BurnSpreadData existingData) || existingData.recursionLevel < recursionLevel))
            KeysRequestingFireSpread[position] = new BurnSpreadData(PlausibleOwner, recursionLevel);
    }
    public static void DetonateOil(Vector2Int pos, BurnSpreadData data)
    {
        if (GetHazard(pos, out Hazard existing) && existing.Type == HazardType.Oil)
        {
            float scaleMult = Mathf.Sqrt(Mathf.Max(0.1f, (1 - existing.TimePassed / (float)existing.Duration)));
            Hazard newH = AddHazard(pos, existing.WorldPosition, HazardType.FireOil, 150, existing.SizeMultiplier, data.recursionLevel, true);
            foreach(FloorHazard hazard in existing.PairedObjects)
            {
                Vector2 pos2 = hazard.transform.position;
                newH.AttachGameObject(pos2);
                ParticleManager.NewParticle(pos2, hazard.Visual.transform.localScale.x + 0.2f, Utils.RandCircle(0.5f), 0.5f, Utils.RandFloat(0.5f, 1.2f), ParticleManager.ID.Fire, Color.white.WithAlpha(0.4f));
            }
            newH.PlayerOwner = data.player;
            Vector2 truePosition = new(pos.x * 2 + 1f, pos.y * 2 + 1f);
            Vector2 visualPosition = existing.WorldPosition;
            Projectile.NewProjectile<OilFire>(Vector2.Lerp(truePosition, visualPosition, Utils.RandFloat(1)), Utils.RandCircle(0.5f), 1, data.player, scaleMult);
        }
    }
    public static void SpreadCircle(Vector2 position, int duration, float area, HazardType t)
    {
        float radia = Mathf.Sqrt(area);
        float halfRadia = radia + 0.5f; //add a bit of padding for calculations to be consistent
        area += 0.5f;
        for(float i = -halfRadia; i <= halfRadia; ++i)
        {
            for(float j = -halfRadia; j <= halfRadia; ++j)
            {
                float lenSq = i * i + j * j;
                if(lenSq <= area)
                {
                    HazardSystem.AddHazard(position + new Vector2(i, j) + Utils.RandCircle(0.5f), t, duration, 1);
                }
            }
        }
    }
}
