using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.GraphicsBuffer;
public class WaveCard 
{
    public override string ToString()
    {
        return $"[P: {Patterns.Length}, C: {Cost}, MS: {mulliganDelay}]";
    }
    public WaveCard(float cost, float startUpDelay, float endDelay, List<EnemyPattern> patterns)
    {
        this.mulliganDelay = fullDelay = startUpDelay;
        this.postPlayDelay = endDelay;
        this.Patterns = patterns.ToArray();
        this.Cost = cost;
    }
    public WaveCard(float cost, float startUpDelay, float endDelay, params EnemyPattern[] patterns)
    {
        this.mulliganDelay = fullDelay = startUpDelay;
        this.postPlayDelay = endDelay;
        this.Patterns = patterns;
        this.Cost = cost;
    }
    public EnemyPattern[] Patterns;
    public float fullDelay = 1;
    public float mulliganDelay = 1;
    /// <summary>
    /// Additional delay put on the next card drawn.
    /// Currently unimplemented
    /// </summary>
    public float postPlayDelay = 1;
    private int patternNum = 0;
    public bool Resolved = false;
    public float Cost = 100;
    public float resolveCounter = 0;
    public void Resolve()
    {
        resolveCounter += Time.fixedDeltaTime;
        if (resolveCounter > 0)
        {
            if (patternNum < Patterns.Length)
            {
                Patterns[patternNum].Release();
                resolveCounter -= Patterns[patternNum].EndDelay;
                ++patternNum;
            }
            else
            {
                if(resolveCounter > postPlayDelay)
                {
                    Finish();
                }
            }
        }
    }
    public void Finish()
    {
        Resolved = true;
    }
}
public abstract class EnemySpawnPattern
{
    public EnemySpawnPattern(Player target)
    {
        Target = target;
    }
    public virtual float MinimumDistanceToPlayer => 12;
    public Player Target { get; set; } = null;
    public virtual Vector2 GenerateLocation()
    {
        return Target.transform.position;
    }
    /// <summary>
    /// How regenerating the location should be handled if the location was invalid, such as it was in a tile
    /// </summary>
    /// <returns></returns>
    public virtual Vector2 RegenerateLocation(int attemptNumber, bool finalGeneration = false)
    {
        return GenerateLocation();
    }
}
public class ArbitrarySpawnPattern : EnemySpawnPattern
{
    public Func<Vector2> locationFunction = null;
    public Vector2? Location = null;
    private readonly bool OnlyGenOnce;
    public ArbitrarySpawnPattern(Player target, Func<Vector2> location, bool onlyGenerateOnce = false) : base(target)
    {
        locationFunction = location;
        OnlyGenOnce = onlyGenerateOnce;
    }
    public ArbitrarySpawnPattern(Player target, Vector2 location) : base(target)
    {
        Location = location;
    }
    public override Vector2 GenerateLocation()
    {
        if(OnlyGenOnce)
        {
            if(Location == null)
                Location = locationFunction.Invoke();
            return Location.Value;
        }
        return locationFunction != null ? (Location = locationFunction.Invoke()).Value : Location.Value;
    }
    public override Vector2 RegenerateLocation(int attemptNumber, bool finalGeneration = false)
    {
        if(locationFunction == null)
        {
            Vector2 toPlayer = (Target.Position - Location.Value).normalized;
            Location += toPlayer * 1.25f + Utils.RandCircle(2f);
            if (finalGeneration)
                return Location.Value;
        }
        Location = null; //Since the location was invalid, we regenerate
        return GenerateLocation();
    }
}
public class SpawnWithOffset : EnemySpawnPattern
{
    public override float MinimumDistanceToPlayer => 0;
    public Vector2 Offset;
    public EnemySpawnPattern Center;
    public Vector2 Location;
    public SpawnWithOffset(Player target, EnemySpawnPattern center, Vector2 offset) : base(target)
    {
        Center = center;
        Offset = offset;
    }
    public override Vector2 GenerateLocation()
    {
        return Location = Center.GenerateLocation() + Offset;
    }
    public override Vector2 RegenerateLocation(int attemptNumber, bool finalGeneration = false)
    {
        Vector2 toPlayer = (Target.Position - Location).normalized;
        Location += toPlayer * 1.25f + Utils.RandCircle(2f);
        if (finalGeneration)
            return Location;
        return Location;
    }
}
public class EnemyPattern
{
    public EnemySpawnPattern SpawnPattern;
    public EnemyPattern(EnemySpawnPattern location, float endDelay, float betweenEnemyDelay, bool isSkull, params GameObject[] prefabs)
    {
        SpawnPattern = location;
        EndDelay = endDelay;
        BetweenEnemyDelay = betweenEnemyDelay;
        EnemyPrefabs = prefabs;
        Skull = isSkull;
    }
    public bool RelativeLocationToPlayer = true;
    public GameObject[] EnemyPrefabs;
    public float EndDelay = 0.50f;
    public float BetweenEnemyDelay = 0.2f;
    public bool Skull { get; set; } = false;
    public void Release()
    {
        Vector2 spawnPos = ShiftLocationIfOutOfBounds(SpawnPattern.GenerateLocation());
        Wormhole.Spawn(spawnPos, EnemyPrefabs, Skull, BetweenEnemyDelay / Time.fixedDeltaTime);
    }
    public Vector2 ShiftLocationIfOutOfBounds(Vector2 location)
    {
        Player closest = Player.FindClosest(location, out Vector2 toPlayer, out float dist);
        float minDist = SpawnPattern.MinimumDistanceToPlayer;
        if (dist < minDist) //If I'm too close to the player, shift myself away from the player.
        {
            location = closest.Position - toPlayer * minDist;
        }
        for (int att = 0; !ValidLocation(location) && att <= 100; ++att)
        {
            SpawnPattern.Target = closest;
            location = SpawnPattern.RegenerateLocation(att, att == 100);
        }
        for (int deg = 0; deg <= 180; deg += 5)
        {
            closest = Player.FindClosest(location, out toPlayer, out dist);
            if (dist < minDist)
            {
                Vector2 testLocation = closest.Position - (toPlayer * minDist).RotatedBy(Utils.RandFloat(-deg, deg));
                if(ValidLocation(testLocation))
                {
                    Debug.Log($"Shifted enemy spawn postion that was too close to Player[{closest.InstanceID}]".WithColor("#FF5511"));
                    location = testLocation;
                    break;
                }
            }
            else
                break;
        }
        return location;
    }
    public bool ValidLocation(Vector2 location)
    {
        bool hasWorldMap = World.RealTileMap != null && World.RealTileMap.Map != null;
        if(hasWorldMap)
            for(int i = -1; i <= 1; ++i)
                for(int j = -1; j <= 1; ++j)
                    if (!World.ValidEnemySpawnTile(location + new Vector2(i, j) * 2))
                        return false;
        return true;
    }
}